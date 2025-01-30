using System.Text.RegularExpressions;

namespace RegexFileCopier
{
    public class FeedbackEventArgs : EventArgs { public object Feedback { get; set; } }

    public class CopyManager
    {
        public event EventHandler<FeedbackEventArgs> SendFeedback;

        protected virtual void OnSendFeedback(object feedback)
        {
            SendFeedback?.Invoke(this, new FeedbackEventArgs() { Feedback = feedback });
        }

        public void DoCopy(string inputDir, string outputDir, string strRegex)
        {
            try
            {
                if (!Directory.Exists(inputDir))
                {
                    throw new DirectoryNotFoundException($"The directory '{inputDir}' does not exist.");
                }

                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir); // Ensure outputDir exists
                }

                var regex = new Regex(strRegex, RegexOptions.Compiled);

                OnSendFeedback("Scanning files...");
                var fileMatches = Directory.EnumerateFiles(inputDir, "*", SearchOption.AllDirectories)
                    .Select(file => Path.GetRelativePath(inputDir, file))
                    .Where(file => regex.IsMatch(Path.GetFileName(file)))
                    .ToList();

                foreach (var file in fileMatches)
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
