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
using ProcessWindow;

namespace FilenameUpdater
{
    public partial class FormUpdater : Form
    {
        public FormUpdater()
        {
            InitializeComponent();

            if (!File.Exists("qar_dictionary.txt"))
            {
                MessageBox.Show("qar_dictionary.txt is missing from the application folder. This tool cannot update filenames without qar_dictionary.txt", "Missing qar_dictionary.txt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string[] selectedFilePaths { get; set; }

        private string outputDirectory { get; set; }

        private void buttonInFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog inputFileDialog = new OpenFileDialog();
            inputFileDialog.Filter = "All files (*.*)|*.*";
            inputFileDialog.Multiselect = true;

            DialogResult selectionResult = inputFileDialog.ShowDialog();
            if (selectionResult != DialogResult.OK) return;

            selectedFilePaths = inputFileDialog.FileNames;

            string filesText = "";
            foreach (string filePath in selectedFilePaths)
            {
                filesText += string.Format("\"{0}\" ", Path.GetFileName(filePath));
            }
            textInFiles.Text = filesText;
        }

        private void buttonOutDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog selectionDialog = new FolderBrowserDialog())
            {
                selectionDialog.Description = "Choose a folder where the updated files will be sent. Making a new folder is highly recommended.";
                selectionDialog.UseDescriptionForTitle = true;

                if (textOutDir.Text != "")
                    selectionDialog.InitialDirectory = textOutDir.Text;
                else if (textInFiles.Text != "")
                    selectionDialog.InitialDirectory = Path.GetDirectoryName(selectedFilePaths[0]);

                if (selectionDialog.ShowDialog() != DialogResult.OK) return;
                string directoryPath = selectionDialog.SelectedPath;

                outputDirectory = directoryPath;
                textOutDir.Text = directoryPath;
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (selectedFilePaths == null)
            {
                MessageBox.Show("Please choose file(s) to update their names.", "No Files Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (outputDirectory == null)
            {
                MessageBox.Show("Please select an output folder.", "No Directory Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            UpdateManager updater = new UpdateManager();
            FormProcessingUpdate processWindow = new FormProcessingUpdate();
            updater.SendFeedback += processWindow.OnSendFeedback;

            ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { updater.DoUpdates(selectedFilePaths, outputDirectory, checkIncludeDirs.Checked); }));

            int successfulUpdatesCount = updater.getSuccessCount();
            int totalFileCount = updater.getTotalCount();
            if (successfulUpdatesCount > 0)
            {
                MessageBox.Show(string.Format("{0} of {1} Filename(s) successfully updated.", successfulUpdatesCount, totalFileCount), "Update Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No filename updates were found.", "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
