using ProcessWindow;
using System;
using System.IO;
using System.Windows.Forms;

namespace ArchiveUnpacker
{
    public partial class FormUnpacker : Form
    {
        public FormUnpacker()
        {
            InitializeComponent();
            checkCondenseDir.Checked = true;
        }

        private string[] archivePaths { get; set; }

        private string outputDir { get; set; }

        private void buttonArchives_Click(object sender, EventArgs e)
        {
            OpenFileDialog inputDialog = new OpenFileDialog();
            inputDialog.Filter = "Archive Files (*.dat)|*.dat|Mod Files (*.mgsv)|*.mgsv";
            inputDialog.Multiselect = true;

            DialogResult selectionResult = inputDialog.ShowDialog();
            if (selectionResult != DialogResult.OK) return;

            archivePaths = inputDialog.FileNames;
            Array.Sort(archivePaths); Array.Reverse(archivePaths);

            string archiveText = "";
            foreach (string archivePath in archivePaths)
            {
                string archiveName = Path.GetFileName(archivePath);
                string archiveDir = Path.GetFileName(Path.GetDirectoryName(archivePath));
                archiveText += string.Format("\"{0}/{1}\" ", archiveDir, archiveName);
            }
            textArchives.Text = archiveText;
        }

        private void buttonOutDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog  folderBrowserDialog = new FolderBrowserDialog()) {
                folderBrowserDialog.Description = "Choose a folder where the .dat files will unpack into. Making a new folder is highly recommended.";
                folderBrowserDialog.UseDescriptionForTitle = true;

                if (textOutDir.Text != "")
                    folderBrowserDialog.InitialDirectory = textOutDir.Text;
                else if (textArchives.Text != "")
                    folderBrowserDialog.InitialDirectory = Path.GetDirectoryName(archivePaths[0]);

                if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;
                string directoryPath = folderBrowserDialog.SelectedPath;

                outputDir = directoryPath;
                textOutDir.Text = directoryPath;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            UnpackManager archiveUnpacker = new UnpackManager();
            FormProcessesingUnpack processWindow = new FormProcessesingUnpack();
            archiveUnpacker.SendFeedback += processWindow.OnSendFeedback;

            if (archivePaths != null)
                if (outputDir != null)
                {
                    ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { archiveUnpacker.DoUnpack(archivePaths, outputDir, checkCondenseDir.Checked); }));
                    MessageBox.Show("Done!","Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Please select an output folder.", "Missing Output Directory", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Please choose which archives to unpack.", "No Archives Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
