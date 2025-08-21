using System;
using System.Collections.Generic;
using System.IO;

namespace TextureAggregator
{
    public class ConvertManager
    {
        public event EventHandler<FeedbackEventArgs> SendFeedback;
        public List<string> errorList = new List<string>();

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
                string ddsOutputDir = ftexFileInfo.FullName.Remove(0, numRemoveRootDir);
                string texturename = Path.GetFileName(ftexFileInfo.FullName);
                OnSendFeedback("Converting to .dds...\n" + texturename);

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
                catch (FtexTool.Exceptions.MissingFtexsFileException)
                {
                    errorList.Add("[Convert .dds]: Failed to convert " + texturename + "\nMissing .ftexs files, maybe due to a custom (new) texture?");
                }
                catch (ArgumentOutOfRangeException)
                {
                    errorList.Add("[Convert .dds]: Failed to convert " + texturename + "\nMaybe mismatching .ftexs due to a modded (existing) texture?");
                }
                catch (Exception e)
                {
                    errorList.Add("[Convert .dds]: Mysteriously failed to convert " + texturename + "\nError message: " + e);
                }
            }
        }
    }
}
