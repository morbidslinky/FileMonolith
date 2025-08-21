using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace FileProliferator
{
    class TextureManager
    {

        public event EventHandler<FeedbackEventArgs> SendFeedback;

        public static string[] TppFileList = File.ReadAllLines("TppMasterFileList.txt");

        public Exception errorMsg = null;

        private int conversionFailedCount = 0;

        private int textureNotFoundCount = 0;

        protected virtual void OnSendFeedback(string feedback)
        {
            SendFeedback?.Invoke(this, new FeedbackEventArgs() { Feedback = feedback });
        }

        public string[] convertDdsToFtex(string[] filePaths, out string[] temporaryFiles)
        {
            List<string> newFilePaths = new List<string>();
            List<string> temporaryFilePaths = new List<string>();

            try
            {
                foreach (string filePath in filePaths)
                {
                    if (filePath.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
                    {
                        string fileNameNoExtension = Path.GetFileNameWithoutExtension(filePath);
                        string fileDir = Path.GetDirectoryName(filePath);
                        try
                        {
                            string[] ftexArgs = { filePath };
                            FtexTool.Program.Main(ftexArgs);

                            OnSendFeedback("Converting: " + Path.GetFileName(filePath));
                            string[] convertedFiles = Directory.GetFiles(fileDir, String.Format("{0}*.ftex?", fileNameNoExtension), SearchOption.TopDirectoryOnly);
                            newFilePaths.AddRange(convertedFiles);
                            temporaryFilePaths.AddRange(convertedFiles);
                        }
                        catch (FtexTool.Exceptions.FtexToolException)
                        {
                            conversionFailedCount++;
                        }

                    }
                    else
                    {
                        newFilePaths.Add(filePath);
                    }
                }
            }
            catch (Exception e)
            {
                errorMsg = e;
            }

            temporaryFiles = temporaryFilePaths.ToArray();
            return newFilePaths.ToArray();
        }

        internal void PullVanillaTextures(string outputDirectory, string vanillaTexturesPath, params string[] inputFilePaths)
        {
            try
            {
                //List<string> pftxsPaths = new List<string>();
                List<string> inputTextureNames = new List<string>();
                foreach (string inputFilePath in inputFilePaths) // every selected file from the user
                {
                    string fileName = Path.GetFileName(inputFilePath);
                    if (fileName.EndsWith(".ftex") || fileName.EndsWith(".ftexs"))
                    {
                        inputTextureNames.Add(GetTextureName(fileName));
                    }
                    /*
                    else
                    {
                        continue;
                    }
                    foreach (string ProliferatePath in SearchDirectoriesEndsWith(fileName)) // every tpplist line that mentions selected file from the user
                    {
                        if (ProliferatePath.Contains("_pftxs"))
                        {
                            int pftxsPathLength = ProliferatePath.IndexOf("_pftxs") + 6;
                            string pftxsPath = ProliferatePath.Substring(0, pftxsPathLength);
                            if (!pftxsPaths.Contains(pftxsPath))
                            {
                                pftxsPaths.Add(pftxsPath); // every pftxs folder of every selected file from the user... weren't these made when the user did proliferatefile? 
                            }
                        }
                    }
                    */
                }
                string[] pftxsPaths = Directory.GetDirectories(outputDirectory, "*_pftxs", SearchOption.AllDirectories); //much more efficient I think
                foreach (string pftxsPath in pftxsPaths)
                {
                    string pftxsName = Path.GetFileName(pftxsPath);
                    List<string> TppListedTexturePaths = SearchPathsContaining(pftxsName);
                    foreach (string TppListedTexturePath in TppListedTexturePaths)
                    {
                        string textureFilename = Path.GetFileName(TppListedTexturePath);
                        if (inputTextureNames.Contains(GetTextureName(textureFilename)))
                        {
                            continue;
                        }
                        string textureTppListedDirPath = Path.GetDirectoryName(TppListedTexturePath);
                        OnSendFeedback(string.Format("Pulling {0}", Path.GetFileName(TppListedTexturePath)));
                        string foundTexturePath = findTexturePath(vanillaTexturesPath, pftxsName, TppListedTexturePath);
                        if (foundTexturePath != null)
                        {
                            string fullOutputPath = Path.Combine(outputDirectory, textureTppListedDirPath);

                            if (!Directory.Exists(fullOutputPath))
                            {
                                Directory.CreateDirectory(fullOutputPath);
                            }

                            string newFilePath = Path.Combine(outputDirectory, TppListedTexturePath);
                            if (!File.Exists(newFilePath))
                            {
                                File.Copy(foundTexturePath, newFilePath);
                            }
                        }
                        else
                        {
                            textureNotFoundCount++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorMsg = e;
            }
        }

        private string findTexturePath(string vanillaTexturesPath, string pftxsName, string TppListedTexturePath)
        {
            int nameIndex = TppListedTexturePath.IndexOf(pftxsName);
            string condensedTexturePath = (TppListedTexturePath.Substring(nameIndex + pftxsName.Length + 1)); //Expected path if textures were unpacked with Archive Unpacker's Condensed Directory Structure
            //Console.WriteLine("condensedTexturePath = " + condensedTexturePath);
            condensedTexturePath = Path.Combine(vanillaTexturesPath, condensedTexturePath);
            if (File.Exists(condensedTexturePath))
            {
                return condensedTexturePath;
            }

            string LongTexturePath = Path.Combine(vanillaTexturesPath, TppListedTexturePath); //Expected path if textures were unpacked with Archive Unpacker
            if (File.Exists(LongTexturePath))
            {
                return LongTexturePath;
            }

            string textureFileName = Path.GetFileName(TppListedTexturePath);
            var directoryInfo = new DirectoryInfo(vanillaTexturesPath).EnumerateFiles(textureFileName, SearchOption.AllDirectories).FirstOrDefault(); //Desperate search
            if (directoryInfo != null)
            {
                return directoryInfo.FullName;
            }
            else
            {
                return null;
            }
        }
        
        public void PackPftxsFolders(string directoryPath)
        {
            try
            {
                string[] allPftxsDirPaths = Directory.GetDirectories(directoryPath, "*_pftxs", SearchOption.AllDirectories);
                foreach (string pftxsDirPath in allPftxsDirPaths)
                {
                    string pftxsFileName = Path.GetFileName(pftxsDirPath).Replace("_pftxs", ".pftxs");
                    string pftxsFilePath = Path.Combine(Path.GetDirectoryName(pftxsDirPath), pftxsFileName);

                    OnSendFeedback("Packing: " + pftxsFileName);
                    AutoPftxsTool.ArchiveHandler.WritePftxsArchive(pftxsFilePath, pftxsDirPath);
                }
            }
            catch (Exception e)
            {
                errorMsg = e;
            }
        }

        public void DeletePftxsFolders(string directoryPath)
        {
            try
            {
                string[] allPftxsDirPaths = Directory.GetDirectories(directoryPath, "*_pftxs", SearchOption.AllDirectories);
                foreach (string pftxsDirPath in allPftxsDirPaths)
                {
                    string pftxsFileName = Path.GetFileName(pftxsDirPath).Replace("_pftxs", ".pftxs");
                    string DirName = Path.GetDirectoryName(pftxsDirPath);
                    string pftxsFilePath = Path.Combine(DirName, pftxsFileName);

                    if (File.Exists(pftxsFilePath))
                    {
                        OnSendFeedback("Deleting: " + DirName);
                        DeleteDirectory(pftxsDirPath);
                    }
                }
            }
            catch (Exception e)
            {
                errorMsg = e;
            }

        }

        private void DeleteDirectory(string dir)
        {
            foreach (string directory in Directory.GetDirectories(dir))
            {
               DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(dir, true);
            }
            catch (IOException)
            {
                Directory.Delete(dir, true);
            }
            catch (System.UnauthorizedAccessException)
            {
                Directory.Delete(dir, true);
            }
        }

        public void DeleteTemporaryFiles(string[] tempFiles)
        {
            foreach (string tempFile in tempFiles)
            {
                try
                {
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                }
                catch { }
            }
        }

        private List<string> SearchDirectoriesEndsWith(string fileName)
        {
            List<string> foundDirectories = new List<string>();

            foreach (string TppFile in TppFileList)
            {
                if (TppFile.EndsWith(fileName))
                {
                    foundDirectories.Add(Path.GetDirectoryName(TppFile));
                }
            }
            return foundDirectories;
        }

        private List<string> SearchPathsContaining(string pathSnippet)
        {
            List<string> foundFiles = new List<string>();

            foreach (string TppFile in TppFileList)
            {
                if (TppFile.Contains(pathSnippet))
                {
                    foundFiles.Add(TppFile);
                }
            }
            return foundFiles;
        }

        private string GetTextureName(string FileNameWithExtension)
        {
            if (FileNameWithExtension.EndsWith(".ftex"))
            {
                return Path.GetFileNameWithoutExtension(FileNameWithExtension);
            } else
            {
                string ftexNameDotNumber = Path.GetFileNameWithoutExtension(FileNameWithExtension);
                return ftexNameDotNumber.Remove(ftexNameDotNumber.Length - 2);
            }
        }

        public int getPftxsDirCount(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath, "*_pftxs", SearchOption.AllDirectories).Length;
        }

        public int getConversionFailedCount()
        {
            return conversionFailedCount;
        }

        public int getTextureNotFoundCount()
        {
            return textureNotFoundCount;
        }
    }
}
