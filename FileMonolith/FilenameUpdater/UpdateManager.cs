using FilenameUpdater.GzsTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilenameUpdater
{
    class UpdateManager
    {
        private int successfulUpdateCount = 0;
        private int totalCount = 0;

        public event EventHandler<FeedbackEventArgs> SendFeedback;

        protected virtual void OnSendFeedback(string feedback)
        {
            SendFeedback?.Invoke(this, new FeedbackEventArgs() { Feedback = feedback });
        }

        public void DoUpdates(string[] inputFilePaths, string outputDir, bool includeDirs)
        {
            try
            {
                ReadDictionary();
                UpdateFilePaths(inputFilePaths, outputDir, includeDirs);
            }
            catch (Exception e)
            {
                MessageBox.Show("The update process failed due to an error: \n" + e, "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateFilePaths(string[] inputFilePaths, string outputDir, bool includeDirs)
        {
            foreach (string inputFilePath in inputFilePaths)
            {
                totalCount++;
                string filename = Path.GetFileNameWithoutExtension(inputFilePath);
                string ext = Path.GetExtension(inputFilePath);
                string extInner = "";
                if (filename.Contains(".")) // Ex: .1.ftexs, .eng.lng
                {
                    extInner = Path.GetExtension(filename);
                    filename = Path.GetFileNameWithoutExtension(filename);
                }
                bool isUpdated = false;
                ulong fileNameHash;

                OnSendFeedback(filename);

                if (TryGetFileNameHash(filename, out fileNameHash))
                {
                    string foundFilePathNoExt;
                    string outputFilePath;

                    if (Hashing.TryGetFilePathFromHash(fileNameHash, out foundFilePathNoExt))
                    {
                        foundFilePathNoExt = foundFilePathNoExt.Remove(0, 1);
                        if (includeDirs)
                        {
                            outputFilePath = Path.Combine(outputDir, foundFilePathNoExt + extInner + ext);
                        }
                        else
                        {
                            outputFilePath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(foundFilePathNoExt) + extInner + ext);
                        }

                        string outputFilePathDir = Path.GetDirectoryName(outputFilePath);
                        if (!Directory.Exists(outputFilePathDir))
                        {
                            Directory.CreateDirectory(outputFilePathDir);
                        }
                        File.Copy(inputFilePath, outputFilePath, true);

                        isUpdated = true;
                    }
                }

                if (isUpdated)
                {
                    successfulUpdateCount++;
                } 
                else
                {
                    OnSendFeedback(filename + " update not found");
                }

            }
        }

        private bool TryGetFileNameHash(string filename, out ulong fileNameHash)
        {
            bool isConverted = true;
            try
            {
                fileNameHash = Convert.ToUInt64(filename, 16);
            }
            catch (FormatException)
            {
                isConverted = false;
                fileNameHash = 0;
            }
            return isConverted;
        }

        private static void ReadDictionary()
        {
            string executingAssemblyLocation = AppContext.BaseDirectory;
            const string qarDictionaryName = "qar_dictionary.txt";
            try
            {
                Hashing.ReadDictionary(Path.Combine(executingAssemblyLocation, qarDictionaryName));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading {0}: {1}", qarDictionaryName, e.Message);
            }
        }

        internal int getSuccessCount()
        {
            return successfulUpdateCount;
        }

        internal int getTotalCount()
        {
            return totalCount;
        }
        
    }
}
