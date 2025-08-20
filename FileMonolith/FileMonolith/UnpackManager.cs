using GzsTool.Core.Common;
using GzsTool.Core.Common.Interfaces;
using GzsTool.Core.Fpk;
using GzsTool.Core.Pftxs;
using GzsTool.Core.Qar;
using GzsTool.Core.Sbp;
using GzsTool.Core.Utility;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ArchiveUnpacker
{
    public class FeedbackEventArgs : EventArgs { public object Feedback { get; set; } }

    public class UnpackManager
    {
        public event EventHandler<FeedbackEventArgs> SendFeedback;

        protected virtual void OnSendFeedback(object feedback)
        {
            SendFeedback?.Invoke(this, new FeedbackEventArgs() { Feedback = feedback });
        }

        public void DoUnpack(string[] archiveFilePaths, string outputDir, bool isCondensed)
        {
            try
            {
                ReadDictionaries();
                UnpackQarArchives(archiveFilePaths, outputDir);
                UnpackModArchives(archiveFilePaths, outputDir);
                UnpackChildArchives(outputDir, isCondensed);
                //UnpackChildArchivesCatchEdgeCases(outputDir, isCondensed);

            }
            catch (Exception e)
            {
                OnSendFeedback(e);
            }
        }

        private void ReadDictionaries()
        {
            string executingAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            const string qarDictionaryName = "qar_dictionary.txt";
            try
            {
                Hashing.ReadDictionary(Path.Combine(executingAssemblyLocation, qarDictionaryName));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading {0}: {1}", qarDictionaryName, e.Message);
            }

            const string fpkDictionaryName = "fpk_dictionary.txt";
            try
            {
                Hashing.ReadMd5Dictionary(Path.Combine(executingAssemblyLocation, fpkDictionaryName));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading {0}: {1}", fpkDictionaryName, e.Message);
            }
        }

        public void UnpackQarArchives(string[] qarFilePaths, string outputDir)
        {
            foreach (string filePath in qarFilePaths)
            {
                if (filePath.EndsWith(".mgsv")) continue;
                try
                {
                    OnSendFeedback(Path.GetFileName(filePath));
                    ReadQARArchive(filePath, outputDir);
                }
                catch (ArgumentOutOfRangeException)
                {
                    string filename = Path.GetFileName(filePath);
                    MessageBox.Show(filename + " could not be unpacked.","Unpack Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public void UnpackModArchives(string[] qarFilePaths, string outputDir)
        {
            foreach (string filePath in qarFilePaths)
            {
                if (!filePath.EndsWith(".mgsv")) continue;
                try
                {
                    OnSendFeedback(Path.GetFileName(filePath));
                    ExtractModArchive(filePath, outputDir);
                }
                catch (ArgumentOutOfRangeException)
                {
                    string filename = Path.GetFileName(filePath);
                    MessageBox.Show(filename + " could not be unpacked.", "Unpack Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public void UnpackChildArchives(string rootDir, bool moveToRoot) // note: archives preexisting in the rootDir will be read. This could potentially lead to the unhashed and the hashed filenames both getting listed to TppGeneratedFileList
        {
            File.Delete("TppGeneratedFileList.txt"); // TppGeneratedFileList.txt should live with the .exe's and a TppMasterFileList.txt, so that the user can swap out the master file list
            List<string> ChildPaths;

            using (var writer = File.CreateText("TppGeneratedFileList.txt"))
            {
                foreach (var fileInfo in new DirectoryInfo(rootDir).EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    string filePath = fileInfo.FullName;
                    string relativePath = filePath.Substring(rootDir.Length + 1);
                    writer.WriteLine(relativePath);

                    string extension = Path.GetExtension(filePath).TrimStart('.');

                    string unpackPath = Path.Combine(
                        Path.GetDirectoryName(filePath) ?? string.Empty,
                        $"{Path.GetFileNameWithoutExtension(filePath)}_{extension}"
                    );

                    string relativeUnpackPath = unpackPath.Substring(rootDir.Length + 1);

                    if (moveToRoot)
                        OnSendFeedback(relativeUnpackPath);

                    IEnumerable<string>? childPaths = extension switch
                    {
                        "fpk" or "fpkd" => ReadArchive<FpkFile>(filePath, moveToRoot ? rootDir : unpackPath),
                        "pftxs" => ReadArchive<PftxsFile>(filePath, moveToRoot ? rootDir : unpackPath),
                        "sbp" => ReadArchive<SbpFile>(filePath, moveToRoot ? rootDir : unpackPath),
                        _ => null
                    };

                    if (childPaths != null)
                    {
                        foreach (string childPath in childPaths)
                            writer.WriteLine(Path.Combine(relativeUnpackPath, childPath));

                        if (!moveToRoot)
                            OnSendFeedback(relativeUnpackPath);
                    }
                }
            }
        }
        public List<string> ReadArchive<T>(string filePath, string outputDir) where T : ArchiveFile, new()
        {

            IDirectory iDir = new FileSystemDirectory(outputDir);
            List<string> fileNames = new List<string>();

            using (FileStream input = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                T file = new T();
                file.Read(input);
                foreach (var exportedFile in file.ExportFiles(input))
                {
                    if (typeof(T) == typeof(SbpFile))
                    {
                        exportedFile.FileName = Path.GetFileNameWithoutExtension(filePath) + exportedFile.FileName;
                    }

                    fileNames.Add(exportedFile.FileName);

                    string exportedFileFullName = Path.Combine(outputDir, exportedFile.FileName);
                    if (!File.Exists(exportedFileFullName)) // saved roughly 1:10 when unpacking texture3 and chunk3
                    {
                        iDir.WriteFile(exportedFile.FileName, exportedFile.DataStream);
                    }
                }
            }
            return fileNames;
        }

        public void ReadQARArchive(string filePath, string outputDir)
        {
            IDirectory iDir = new FileSystemDirectory(outputDir);

            using (FileStream input = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                QarFile file = new QarFile();
                file.Read(input);
                foreach (var exportedFile in file.ExportFiles(input))
                {
                    iDir.WriteFile(exportedFile.FileName, exportedFile.DataStream); // doesn't bother checking if the file already exists, since dats don't often have pre-existing stuff to overwrite. saved roughly 30 seconds when unpacking texture3
                }
            }
        }

        public void ExtractModArchive(string filePath, string outputDir)
        {
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(filePath, outputDir, null);
        }

        /*
        public void UnpackChildArchivesCatchEdgeCases(string rootDir, bool moveToRoot) // Searches for oddball files that share a filename but have different contents
        {
            File.Delete("TppGeneratedFileList.txt");
            List<string> ChildPaths;

            using (StreamWriter TppFileList = File.CreateText("TppGeneratedFileList.txt"))
            {
                foreach (var fileInfo in new DirectoryInfo(rootDir).EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    string filePath = fileInfo.FullName;
                    TppFileList.WriteLine(filePath.Remove(0, rootDir.Length + 1));

                    string extension = Path.GetExtension(filePath).Replace(".", "");

                    string unpackPath = string.Format("{0}\\{1}_{2}", Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath), extension);
                    string unpackPathWithoutRootDir = unpackPath.Remove(0, rootDir.Length + 1);

                    switch (extension)
                    {
                        case "fpk":
                        case "fpkd":
                            ChildPaths = ReadArchiveCatchEdgeCases<FpkFile>(filePath, rootDir);
                            foreach (string childPath in ChildPaths)
                                TppFileList.WriteLine(Path.Combine(unpackPathWithoutRootDir, childPath));
                            OnSendFeedback(unpackPathWithoutRootDir);
                            break;
                        case "pftxs":
                            ChildPaths = ReadArchiveCatchEdgeCases<PftxsFile>(filePath, rootDir);
                            foreach (string childPath in ChildPaths)
                                TppFileList.WriteLine(Path.Combine(unpackPathWithoutRootDir, childPath));
                            OnSendFeedback(unpackPathWithoutRootDir);
                            break;
                        case "sbp":
                            ChildPaths = ReadArchiveCatchEdgeCases<SbpFile>(filePath, rootDir);
                            foreach (string childPath in ChildPaths)
                                TppFileList.WriteLine(Path.Combine(unpackPathWithoutRootDir, childPath));
                            OnSendFeedback(unpackPathWithoutRootDir);
                            break;
                    }
                }
            }
        }

        public List<string> ReadArchiveCatchEdgeCases<T>(string filePath, string outputDir) where T : ArchiveFile, new() // Searches for oddball files that share a filename but have different contents
        {

            IDirectory iDir = new FileSystemDirectory(outputDir);
            List<string> fileNames = new List<string>();

            using (FileStream input = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                T file = new T();
                file.Read(input);
                foreach (var exportedFile in file.ExportFiles(input))
                {
                    string exportedFileFullName = Path.Combine(outputDir, exportedFile.FileName);
                    if (File.Exists(exportedFileFullName))
                    {
                        string outfilename = Path.GetFileName(exportedFile.FileName);
                        string outfileDir = Path.GetDirectoryName(exportedFile.FileName);
                        string parentFilename = Path.GetFileNameWithoutExtension(filePath);

                        exportedFile.FileName = Path.Combine(outfileDir, parentFilename + "_oddball_" + outfilename);

                        fileNames.Add(exportedFile.FileName);
                        iDir.WriteFile(exportedFile.FileName, exportedFile.DataStream);

                        string specialFileFullName = Path.Combine(outputDir, exportedFile.FileName);
                        if (new FileInfo(specialFileFullName).Length == new FileInfo(exportedFileFullName).Length)
                        {
                            File.Delete(specialFileFullName);
                        }
                        else
                        {
                            Console.WriteLine("Special file found: " + specialFileFullName);
                        }
                    }
                    else
                    {
                        fileNames.Add(exportedFile.FileName);
                        iDir.WriteFile(exportedFile.FileName, exportedFile.DataStream);
                    }

                }

            }
            return fileNames;
        }
        */
    }
}
