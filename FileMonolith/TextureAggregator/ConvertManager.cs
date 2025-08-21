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

        public void DoMassConversion(string[] unpackedFiles, string outputDirectory, bool subFolders)
        {
            ConvertTextures(unpackedFiles, subFolders, outputDirectory);
        }

        private void ConvertTextures(string[] unpackedFiles, bool searchSubFolders, string outputRootDir)
        {
            foreach (var ftexFile in unpackedFiles)
            {
                string filePath = searchSubFolders ? 
                    Path.Combine(outputRootDir, ftexFile) :
                    Path.Combine(outputRootDir, Path.GetFileName(ftexFile));

                string texturename = Path.GetFileName(ftexFile);
                OnSendFeedback("Converting to .dds...\n" + texturename);

                string dirPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                try
                {
                    //string[] ftexArgs = { ftexFileInfo.FullName, ddsOutputDir }; Same outcome, but using UnpackFtexFile directly saves on processing. UnpackFtexFile is private by default, so I made it public in the dll I'm using.
                    //FtexTool.Program.Main(ftexArgs);
                    FtexTool.Program.UnpackFtexFile(filePath, dirPath);
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
