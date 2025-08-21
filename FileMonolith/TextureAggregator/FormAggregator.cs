using ProcessWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Resources.ResXFileRef;

namespace TextureAggregator
{
    public partial class FormAggregator : Form
    {
        private string pftxsFullPath = "";
        private bool condense = true;
        public List<string> errorList;

        public FormAggregator()
        {
            InitializeComponent();
            checkConvertDDS.Checked = true;
            checkDirStruc.Checked = false;
            buttonAggregate.Enabled = false;

            if (!checkTPPMasterFileListExists())
                return;

            string defaultOutputDir = Settings.OutputDirectory;
            if (!string.IsNullOrEmpty(defaultOutputDir))
            {
                if (Directory.Exists(defaultOutputDir))
                {
                    textOutDir.Text = defaultOutputDir;
                }
            }
            string defaultTextureDir = Settings.TextureDirectory;
            if (!string.IsNullOrEmpty(defaultTextureDir))
            {
                if (Directory.Exists(defaultTextureDir))
                {
                    textArchiveDir.Text = defaultTextureDir;
                }
            }
            string defaultpftxs = Settings.InputPftxs;
            if (!string.IsNullOrEmpty(defaultpftxs))
            {
                if (Directory.Exists(Path.GetDirectoryName(defaultpftxs)))
                {
                    pftxsFullPath = defaultpftxs;
                    if (File.Exists(pftxsFullPath))
                        textPftxs.Text = Path.GetFileName(pftxsFullPath);
                }
            }
        }

        private bool checkTPPMasterFileListExists()
        {
            if (!File.Exists("TppMasterFileList.txt"))
            {
                MessageBox.Show("TppMasterFileList.txt is missing from the application folder. This tool cannot build directory structures without TppMasterFileList.txt.", "Missing TppMasterFileList.txt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void checkConvertDDS_CheckedChanged(object sender, EventArgs e)
        {
            labelOutDir.Text = checkConvertDDS.Checked ? "Output Folder [.ftex/.ftexs/.dds]:" : "Output Folder [.ftex/.ftexs]:";
        }

        private bool updateProcessButton()
        {
            bool archiveReady = textArchiveDir.Text != "";
            bool pftxsReady = textPftxs.Text != "";
            bool outputReady = textOutDir.Text != "";

            buttonAggregate.Enabled = archiveReady && pftxsReady && outputReady;
            return buttonAggregate.Enabled;
        }

        private void textArchiveDir_TextChanged(object sender, EventArgs e)
        {
            updateProcessButton();
        }

        private void textOutDir_TextChanged(object sender, EventArgs e)
        {
            updateProcessButton();
        }

        private void textPftxs_TextChanged(object sender, EventArgs e)
        {
            updateProcessButton();
        }

        private void buttonPftxs_Click(object sender, EventArgs e)
        {
            OpenFileDialog inputFileDialog = new OpenFileDialog();
            inputFileDialog.Filter = "Packed Fox Textures (*.pftxs)|*.pftxs";
            
            if (!string.IsNullOrEmpty(pftxsFullPath))
            {
                string initialD = Path.GetDirectoryName(pftxsFullPath);
                if (Directory.Exists(initialD))
                {
                    inputFileDialog.InitialDirectory = initialD;
                }
            }

            DialogResult selectionResult = inputFileDialog.ShowDialog();
            if (selectionResult != DialogResult.OK) return;

            pftxsFullPath = inputFileDialog.FileName;

            Settings.InputPftxs = pftxsFullPath;
            Settings.Save();
            
            textPftxs.Text = Path.GetFileName(pftxsFullPath);
        }

        private void buttonOutDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog selectionDialog = new FolderBrowserDialog())
            {
                selectionDialog.Description = "Choose a folder where the files will be copied to. Making a new folder is highly recommended.";
                selectionDialog.UseDescriptionForTitle = true;

                string defaultOutputDir = textOutDir.Text;
                if (!string.IsNullOrEmpty(defaultOutputDir))
                {
                    if (Directory.Exists(defaultOutputDir))
                    {
                        selectionDialog.InitialDirectory = defaultOutputDir;
                    }
                }

                if (selectionDialog.ShowDialog() != DialogResult.OK) return;
                string directoryPath = selectionDialog.SelectedPath;

                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Settings.OutputDirectory = directoryPath;
                    Settings.Save();
                }

                textOutDir.Text = directoryPath;
            }
        }

        private void buttonArchiveDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog selectionDialog = new FolderBrowserDialog())
            {
                selectionDialog.Description = "Choose a folder where vanilla .ftex and .ftexs files can be copied from. The tool will search subfolders as well.";
                selectionDialog.UseDescriptionForTitle = true;

                string defaultTextureDir = textArchiveDir.Text;
                if (!string.IsNullOrEmpty(defaultTextureDir))
                {
                    if (Directory.Exists(defaultTextureDir))
                    {
                        selectionDialog.InitialDirectory = defaultTextureDir;
                    }
                }

                if (selectionDialog.ShowDialog() != DialogResult.OK) return;
                string directoryPath = selectionDialog.SelectedPath;

                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Settings.TextureDirectory = directoryPath;
                    Settings.Save();
                }

                textArchiveDir.Text = directoryPath;
            }
        }

        private void buttonAggregate_Click(object sender, EventArgs e)
        {
            if (!checkTPPMasterFileListExists())
                return;

            errorList = new List<string>();
            UnpackManager unpacker = new UnpackManager();
            AggregateManager aggregator = new AggregateManager();
            ConvertManager converter = new ConvertManager();
            FormProcessing processWindow = new FormProcessing();

            unpacker.SendFeedback += processWindow.OnSendFeedback;
            aggregator.SendFeedback += processWindow.OnSendFeedback;
            converter.SendFeedback += processWindow.OnSendFeedback;

            string[] unpackedPaths = { };
            ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { unpackedPaths = unpacker.DoUnpack(pftxsFullPath, textOutDir.Text, condense); }));
            if (unpacker.err != "")
            {
                DisplayError(unpacker.err);
                return;
            }

            string[] pulledFtexsPaths = { };
            ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { pulledFtexsPaths = aggregator.DoAggregate(unpackedPaths, textArchiveDir.Text, textOutDir.Text, condense); }));

            if (checkConvertDDS.Checked)
            {
                ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { converter.DoMassConversion(textOutDir.Text, textOutDir.Text, !condense); }));

                DialogResult dialogResult = MessageBox.Show("The .ftex files have been converted to .dds files.\n\nWould you like to delete the leftover .ftex and .ftexs files?", "Delete Fox Files?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                {
                    ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { aggregator.DeleteFoxTextures(unpackedPaths, pulledFtexsPaths, textOutDir.Text, condense); }));
                }
            }

            errorList.AddRange(aggregator.errorList);
            errorList.AddRange(converter.errorList);
            if (errorList.Count() > 0)
            {
                DisplayErrorList();
                return;
            }
            MessageBox.Show("Done!", "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkDirStruc_CheckedChanged(object sender, EventArgs e)
        {
            condense = !checkDirStruc.Checked;
        }

        public void DisplayErrorList()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("FailedTextureConversions.txt"))
                {
                    foreach (string errors in errorList)
                    {
                        writer.WriteLine($"{errors}\n");
                    }
                    MessageBox.Show(string.Format("Process Complete.\n\n{0} texture(s) failed to be converted (missing .ftexs or something else).\nSee FailedTextureConversions.txt for more info.", errorList.Count), "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception fileError)
            {
                MessageBox.Show(string.Format("Process Complete.\n\n{0} texture(s) failed to be converted (missing .ftexs or something else).\nFailed to write FailedTextureConversions.txt:\n{1}.", errorList.Count, fileError.Message), "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void DisplayError(string errorMsg)
        {
            MessageBox.Show("An error occurred while attempting to proliferate data:\n" + errorMsg);
        }
    }
}
