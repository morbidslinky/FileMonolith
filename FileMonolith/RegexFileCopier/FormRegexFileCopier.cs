using ProcessWindow;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Text;

namespace RegexFileCopier
{
    public partial class FormRegexFileCopier : Form
    {
        public FormRegexFileCopier()
        {
            InitializeComponent();

            string defaultInputDir = Settings.InputDirectory;
            if (!string.IsNullOrEmpty(defaultInputDir))
            {
                if (Directory.Exists(defaultInputDir))
                {
                    inputDir = defaultInputDir;
                    textInputDir.Text = defaultInputDir;
                }
            }

            string defaultOutputDir = Settings.OutputDirectory;
            if (!string.IsNullOrEmpty(defaultOutputDir))
            {
                if (Directory.Exists(defaultOutputDir))
                {
                    outputDir = defaultOutputDir;
                    textOutputDir.Text = defaultOutputDir;
                }
            }

            string defaultRegex = Settings.RegexText;
            if (!string.IsNullOrEmpty(defaultRegex))
            {
                regexText = defaultRegex;
                textFilename.Text = defaultRegex;
            }
        }

        private string inputDir { get; set; } = "";

        private string outputDir { get; set; } = "";

        private string regexText { get; set; } = "";

        [STAThread]
        private void buttonInputDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose a source folder where the files will be copied from.";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string inputDirectoryPath = dialog.SelectedPath;

                    inputDir = inputDirectoryPath;
                    textInputDir.Text = inputDirectoryPath;
                    Settings.InputDirectory = inputDirectoryPath;
                    Settings.Save();
                }
            }
        }

        [STAThread]
        private void buttonOutDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose a folder where the files will be copied into. Making a new folder is recommended.";
                dialog.UseDescriptionForTitle = true;

                if (textOutputDir.Text != "")
                    dialog.InitialDirectory = textOutputDir.Text;
                else if (textInputDir.Text != "")
                    dialog.InitialDirectory = Path.GetDirectoryName(inputDir);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string outputDirectoryPath = dialog.SelectedPath;

                    outputDir = outputDirectoryPath;
                    textOutputDir.Text = outputDirectoryPath;
                    Settings.OutputDirectory = outputDirectoryPath;
                    Settings.Save();
                }
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (inputDir == "")
            {
                MessageBox.Show("Please choose which archives to unpack.", "No Archives Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (outputDir == "")
            {
                MessageBox.Show("Please select an output folder.", "Missing Output Directory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            regexText = textFilename.Text;
            if (regexText == "")
            {
                MessageBox.Show("Please add a regular expression.", "Missing Regex", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Settings.RegexText = regexText;
            Settings.Save();

            if (inputDir == outputDir)
            {
                MessageBox.Show("The input folder is the same as the output folder.", "Incorrect Output Directory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            CopyManager copyManager = new CopyManager();
            FormProcessing processWindow = new FormProcessing();
            copyManager.SendFeedback += processWindow.OnSendFeedback;

            List<string > files = new List<string>();
            ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { files = copyManager.DoScan(inputDir, regexText); }));

            if (files.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"The directory contains {files.Count} matching result(s), including:\n");
                for (int i = 0; i < 3; i++)
                {
                    if (i >= files.Count)
                    {
                        break;
                    }

                    stringBuilder.AppendLine(files[i]);
                    if (i == 2 && i < files.Count - 1)
                    {
                        stringBuilder.AppendLine("...");
                        break;
                    }
                }
                stringBuilder.AppendLine("\nPress OK to copy.");

                var dialog = MessageBox.Show(stringBuilder.ToString(), $"{files.Count} result(s) found", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (dialog == DialogResult.OK)
                {
                    ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { copyManager.DoCopy(inputDir, outputDir); }));
                    MessageBox.Show("Done!", "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } else
            {
                MessageBox.Show("The directory contains no matching results.", "No matching results", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
