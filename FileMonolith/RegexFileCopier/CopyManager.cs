using System.Text.RegularExpressions;

namespace RegexFileCopier
{
    public class FeedbackEventArgs : EventArgs { public string Feedback { get; set; } }

    public class CopyManager
    {
        public event EventHandler<FeedbackEventArgs> SendFeedback;

        private List<string> files = new List<string>();

        protected virtual void OnSendFeedback(string feedback)
        {
            SendFeedback?.Invoke(this, new FeedbackEventArgs() { Feedback = feedback });
        }

        public List<string> DoScan(string inputDir, string strRegex)
        {
            OnSendFeedback("Scanning files...");
            if (!Directory.Exists(inputDir))
            {
                return new List<string>();
            }

            var regex = new Regex(strRegex, RegexOptions.Compiled);

            files = Directory.EnumerateFiles(inputDir, "*", SearchOption.AllDirectories)
                    .Select(file => Path.GetRelativePath(inputDir, file))
                    .Where(file => regex.IsMatch(Path.GetFileName(file)))
                    .ToList();

            return files;
        }

        public void DoCopy(string inputDir, string outputDir)
        {
            try
            {
                if (!Directory.Exists(inputDir))
                {
                    throw new DirectoryNotFoundException($"The directory '{inputDir}' does not exist.");
                }

                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                foreach (var file in files)
                {
                    var sourcePath = Path.Combine(inputDir, file);
                    var destinationPath = Path.Combine(outputDir, file);

                    try
                    {
                        var destinationDir = Path.GetDirectoryName(destinationPath);
                        if (!string.IsNullOrEmpty(destinationDir) && !Directory.Exists(destinationDir))
                        {
                            Directory.CreateDirectory(destinationDir);
                        }

                        OnSendFeedback($"Copying {sourcePath} to {destinationPath}");
                        File.Copy(sourcePath, destinationPath, true);
                    }
                    catch (Exception fileEx)
                    {
                        OnSendFeedback($"Error copying {sourcePath} -> {destinationPath}: {fileEx.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                OnSendFeedback($"Critical Error: {e.Message}");
            }
        }
    }
}
