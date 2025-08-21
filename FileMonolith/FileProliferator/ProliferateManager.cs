using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProliferator
{
    public class ProliferateManager
    {
        public static string[] TppFileList = File.ReadAllLines("TppMasterFileList.txt");
        public Exception errorMsg = null;

        public event EventHandler<FeedbackEventArgs> SendFeedback;

        protected virtual void OnSendFeedback(string feedback)
        {
            SendFeedback?.Invoke(this, new FeedbackEventArgs() { Feedback = feedback });
        }

        public void DoProliferate(string[] filesToProlif, string outputPath)
        {
            try
            {
                foreach (string fileToProlif in filesToProlif)
                {
                    string fileName = Path.GetFileName(fileToProlif);
                    List<string> foundDirectories = SearchDirectoriesEndsWith(fileName);

                    if (foundDirectories.Count() == 0)
                        OnSendFeedback(string.Format("No results for {0}", fileName));
                    else
                    {
                        OnSendFeedback(fileName);
                        CopyFilesToDirectories(outputPath, foundDirectories, fileToProlif);
                    }
                }
                ProliferateExtraFtexs(filesToProlif, outputPath);
            } 
            catch (Exception e)
            {
                errorMsg = e;
            }
        }

        public void DoProliferateFromReference(string[] filesToProlif, string outputPath, bool setInRoot, string referenceFile)
        {
            try
            {
                List<string> foundDirectories = SearchDirectoriesEndsWith(referenceFile);

                if (foundDirectories.Count() == 0)
                    OnSendFeedback(string.Format("No results for {0}", referenceFile));
                else
                {
                    OnSendFeedback(referenceFile);
                    if (setInRoot)
                        RootRefDirectories(foundDirectories);
                    CopyFilesToDirectories(outputPath, foundDirectories, filesToProlif);
                }
            }
            catch (Exception e)
            {
                errorMsg = e;
            }
        }

        private void RootRefDirectories(List<string> foundDirectories)
        {
            for (int i = 0; i < foundDirectories.Count; i++)
            {
                string foundDirectory = foundDirectories[i];
                if (foundDirectory.Contains("_pftxs\\"))
                {
                    foundDirectory = foundDirectory.Remove(foundDirectory.IndexOf("_pftxs") + 6);
                }
                else if (foundDirectory.Contains("_fpk\\"))
                {
                    foundDirectory = foundDirectory.Remove(foundDirectory.IndexOf("_fpk") + 4);
                }
                else if (foundDirectory.Contains("_fpkd\\"))
                {
                    foundDirectory = foundDirectory.Remove(foundDirectory.IndexOf("_fpkd") + 5);
                }
                else if (foundDirectory.Contains("_sbp\\"))
                {
                    foundDirectory = foundDirectory.Remove(foundDirectory.IndexOf("_sbp") + 4);
                }
                foundDirectories[i] = foundDirectory;
            }
        }

        private void ProliferateExtraFtexs(string[] filesToProlif, string outputPath) 
        {
            foreach (string fileToProlif in filesToProlif)
            {
                if (fileToProlif.EndsWith(".ftex"))
                {
                    OnSendFeedback("Confirming .ftexs count...");
                    string TppCondensedDir = FindCondensedDir(Path.GetFileName(fileToProlif));
                    if (TppCondensedDir == null)
                        continue;
                    string textureName = Path.GetFileNameWithoutExtension(fileToProlif);
                    string textureDir = Path.GetDirectoryName(fileToProlif);

                    for (int i = 2; i <= 6; i++)
                    {
                        string largerftexs = string.Format("{0}\\{1}.{2}.ftexs", textureDir, textureName, i);
                        if (filesToProlif.Contains(largerftexs))
                        {
                            CopyFilesToDirectories(outputPath, TppCondensedDir, largerftexs);
                        } 
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private List<string> SearchDirectoriesEndsWith(string fileName)
        {
            List<string> foundDirectories = new List<string>();

            foreach (string TppFile in TppFileList)
                if (TppFile.EndsWith(fileName))
                    foundDirectories.Add(Path.GetDirectoryName(TppFile));
            return foundDirectories;
        }

        private string FindCondensedDir(string fileName)
        {

            foreach (string TppFile in TppFileList)
                if (TppFile.EndsWith(fileName))
                {
                    if (TppFile.Contains("_pftxs"))
                    {
                        int pftxsPathLength = TppFile.IndexOf("_pftxs") + 6;
                        return Path.GetDirectoryName(TppFile.Remove(0, pftxsPathLength + 1));
                    }
                    else
                    {
                        return Path.GetDirectoryName(TppFile);
                    }
                }
            return null;
        }

        private void CopyFilesToDirectories(string outputPath, List<string> tppPaths, params string[] filepaths)
        {
            foreach (string tppPath in tppPaths)
            {
                string fullPath = Path.Combine(outputPath, tppPath);
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);
                foreach (string filePath in filepaths)
                {
                    string newFilePath = Path.Combine(fullPath, Path.GetFileName(filePath));
                    if (Path.GetFullPath(filePath) != Path.GetFullPath(newFilePath))
                        File.Copy(filePath, newFilePath, true);
                }
            }
        }

        private void CopyFilesToDirectories(string outputPath, string tppPath, params string[] filepaths)
        {
            string fullPath = Path.Combine(outputPath, tppPath);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
            foreach (string filePath in filepaths)
            {
                string newFilePath = Path.Combine(fullPath, Path.GetFileName(filePath));
                File.Copy(filePath, newFilePath, true);
            }
        }
    }
}
