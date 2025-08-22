using FileProliferator.GzsTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileProliferator
{
    class UpdateManager
    {
        private int successfulUpdateCount = 0;
        public Exception errorMsg = null;
        private int totalCount = 0;

        public event EventHandler<FeedbackEventArgs> SendFeedback;

        protected virtual void OnSendFeedback(string feedback)
        {
            SendFeedback?.Invoke(this, new FeedbackEventArgs() { Feedback = feedback });
        }

        public string[] DoUpdates(string[] inputFilePaths)
        {
            try
            {
                ReadDictionary();
                return UpdateFilePaths(inputFilePaths);
            }
            catch (Exception e)
            {
                errorMsg = e;
                return new string[0];
            }
        }

        private string[] UpdateFilePaths(string[] inputFilePaths)
        {

            totalCount = inputFilePaths.Length;
            for (int i = 0; i < inputFilePaths.Length; i++)
            {
                string inputFilePath = inputFilePaths[i];
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
                    string dirPath = Path.GetDirectoryName(inputFilePath);
                    string outputFilePath;

                    if (Hashing.TryGetFilePathFromHash(fileNameHash, out foundFilePathNoExt))
                    {
                        foundFilePathNoExt = foundFilePathNoExt.Remove(0, 1);
                        outputFilePath = Path.Combine(dirPath, Path.GetFileNameWithoutExtension(foundFilePathNoExt) + extInner + ext);
                        

                        string outputFilePathDir = Path.GetDirectoryName(outputFilePath);
                        if (!Directory.Exists(outputFilePathDir))
                        {
                            Directory.CreateDirectory(outputFilePathDir);
                        }

                        File.Copy(inputFilePath, outputFilePath, true);
                        inputFilePaths[i] = outputFilePath;

                        isUpdated = true;
                    }
                }

                if (isUpdated)
                {
                    successfulUpdateCount++;
                }
                else
                {
                    OnSendFeedback(filename + " Update Failed");
                }

            }
            return inputFilePaths;
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
