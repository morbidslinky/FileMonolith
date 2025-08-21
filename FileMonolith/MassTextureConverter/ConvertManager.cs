using System;
using System.Collections.Generic;
using System.IO;

namespace MassTextureConverter
{
    public class FeedbackEventArgs : EventArgs { public string Feedback { get; set; } }

    public class ConvertManager
    {
        public event EventHandler<FeedbackEventArgs> SendFeedback;
        public Dictionary<string, Exception>FailedMapping = new Dictionary<string, Exception>();

        private int conversionFailedCount = 0;
        private int conversionTryCount = 0;

        protected virtual void OnSendFeedback(string feedback)
        {
            SendFeedback?.Invoke(this, new FeedbackEventArgs() { Feedback = feedback });
        }

        public void DoMassConversion(string inputDirectory, string outputDirectory, bool subFolders)
        {
            ConvertTextures(inputDirectory, subFolders, outputDirectory);
        }

        private void ConvertTextures(string inputRootDir, bool searchSubFolders, string outputRootDir)
        {
            int numRemoveRootDir = inputRootDir.Length + 1;
            SearchOption searchOpt = new SearchOption();
            if (searchSubFolders)
                searchOpt = SearchOption.AllDirectories;
            else
                searchOpt = SearchOption.TopDirectoryOnly;

            foreach (var ftexFileInfo in new DirectoryInfo(inputRootDir).EnumerateFiles("*.ftex", searchOpt))
            {
                conversionTryCount++;
                string ddsOutputDir = ftexFileInfo.FullName.Remove(0, numRemoveRootDir);

                OnSendFeedback(ddsOutputDir);

                ddsOutputDir = Path.GetDirectoryName(ddsOutputDir);
                ddsOutputDir = Path.Combine(outputRootDir, ddsOutputDir);

                if (!Directory.Exists(ddsOutputDir))
                    Directory.CreateDirectory(ddsOutputDir);

                try
                {
                    //string[] ftexArgs = { ftexFileInfo.FullName, ddsOutputDir }; Same outcome, but using UnpackFtexFile directly saves on processing. UnpackFtexFile is private by default, so I made it public in the dll I'm using.
                    //FtexTool.Program.Main(ftexArgs);
                    FtexTool.Program.UnpackFtexFile(ftexFileInfo.FullName, ddsOutputDir);
                }
                catch (Exception e)
                {
                    FailedMapping.Add(ftexFileInfo.FullName, e);
                    conversionFailedCount++;
                }
            }
        }

        public int GetFailureCount()
        {
            return conversionFailedCount;
        }

        public int GetTryCount()
        {
            return conversionTryCount;
        }
    }
}
