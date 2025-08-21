using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureAggregator
{
    class AggregateManager
    {
        public event EventHandler<FeedbackEventArgs> SendFeedback;
        public List<string> errorList = new List<string>();

        protected virtual void OnSendFeedback(string feedback)
        {
            SendFeedback?.Invoke(this, new FeedbackEventArgs() { Feedback = feedback });
        }

        public string[] DoAggregate(string[] ftexpaths, string archivePath, string outputDir, bool condense)
        {
            IEnumerable<string> textures = ftexpaths.Select(file => file.Substring(0, file.Length - 5));

            OnSendFeedback("Reading TppMasterFileList...");
            string[] TppFileList = File.ReadAllLines("TppMasterFileList.txt");
            IEnumerable<string> ftexsEntries = TppFileList.Where(entry => entry.EndsWith(".ftexs") && !entry.EndsWith(".1.ftexs"));

            OnSendFeedback("Scanning for .pftxs Textures...");
            List<string> pullFiles = new List<string>();
            foreach (string ftexsEntry in ftexsEntries)
            {
                if (textures.Any(texture => ftexsEntry.StartsWith(texture)))
                {
                    pullFiles.Add(ftexsEntry);
                }
            }

            foreach (string textureFile in pullFiles)
            {
                PullFile(Path.Combine(archivePath, textureFile), Path.Combine(outputDir, condense ? Path.GetFileName(textureFile) : textureFile));
            }

            return pullFiles.ToArray();
        }


        internal void PullFile(string srcPath, string dstPath)
        {
            if (!File.Exists(dstPath)) // all original .2+.ftexs live outside packs, so the monolith can be condensed or uncondensed 
            {
                if (File.Exists(srcPath))
                {
                    OnSendFeedback("Copying...\n" + Path.GetFileName(srcPath));
                    File.Copy(srcPath, dstPath);
                }
                else
                {
                    errorList.Add("[Pull file]: Could not find " + srcPath + "\nPerhaps the archives are incomplete?");
                }
            }
        }

        internal void DeleteFoxTextures(string[] ftexPaths, string[] pulledFtexsPaths, string outputDir, bool condense)
        {
            IEnumerable<string> ftexRelativePath = condense ? ftexPaths.Select(entry => Path.GetFileNameWithoutExtension(entry)) : ftexPaths.Select(file => file.Substring(0, file.Length - 5));
            OnSendFeedback("Deleting Leftover Fox Textures...");
            foreach (string ftexFile in ftexRelativePath)
            {
                try
                {
                    string outFile = Path.Combine(outputDir, ftexFile);

                    string outdds = outFile + ".dds";
                    if (File.Exists(outdds))
                    {
                        string outftex = outFile + ".ftex";
                        if (File.Exists(outftex))
                            File.Delete(outftex);

                        string out1ftexs = outFile + ".1.ftexs";
                        if (File.Exists(out1ftexs))
                            File.Delete(out1ftexs);
                    }
                }
                catch { }
            }

            IEnumerable<string> ftexsRelativePath = condense ? pulledFtexsPaths.Select(entry => Path.GetFileName(entry)) : pulledFtexsPaths;
            foreach (string ftexsFile in ftexsRelativePath)
            {
                try
                {
                    string outfile = Path.Combine(outputDir, ftexsFile);
                    if (File.Exists(outfile))
                        File.Delete(outfile);
                }
                catch { }
            }
        }
    }
}
