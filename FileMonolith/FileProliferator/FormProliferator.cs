using ProcessWindow;
using System;
using System.IO;
using System.Windows.Forms;

namespace FileProliferator
{
    public partial class FormProliferator : Form
    {
        public FormProliferator()
        {
            InitializeComponent();
            buttonRefFile.Enabled = false;
            checkRefFile.Checked = false;

            buttonTextureDir.Enabled = false;
            checkPullTextures.Checked = false;

            checkSetRefRoot.Checked = false;
            checkSetRefRoot.Enabled = false;

            checkConvertDds.Checked = true;


            if (!File.Exists("TppMasterFileList.txt"))
            {
                MessageBox.Show("TppMasterFileList.txt is missing from the application folder. This tool cannot build directory structures without TppMasterFileList.txt.", "Missing TppMasterFileList.txt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string defaultOutputDir = Settings.OutputDirectory;
            if (!string.IsNullOrEmpty(defaultOutputDir))
            {
                if (Directory.Exists(defaultOutputDir))
                {
                    outputDirectory = defaultOutputDir;
                    textOutDir.Text = defaultOutputDir;
                }
            }
            string defaultTextureDir = Settings.TextureDirectory;
            if (!string.IsNullOrEmpty(defaultTextureDir))
            {
                if (Directory.Exists(defaultTextureDir))
                {
                    VanillaTexturesPath = defaultTextureDir;
                    textTextureDir.Text = defaultTextureDir;
                }
            }
        }

        private string[] selectedFilePaths { get; set; }

        private string referenceFileName { get; set; }

        private string outputDirectory { get; set; }

        private string VanillaTexturesPath { get; set; }

        private void buttonFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog inputFileDialog = new OpenFileDialog();
            inputFileDialog.Filter = "All files (*.*)|*.*";
            inputFileDialog.Multiselect = true;

            string inputDirectory = Settings.InputDirectory;
            if (!string.IsNullOrEmpty(inputDirectory))
            {
                if (Directory.Exists(inputDirectory))
                {
                    inputFileDialog.InitialDirectory = inputDirectory;
                }
            }

            DialogResult selectionResult = inputFileDialog.ShowDialog();
            if (selectionResult != DialogResult.OK) return;

            selectedFilePaths = inputFileDialog.FileNames;
            if (selectedFilePaths.Length > 0)
            {
                Settings.InputDirectory = Path.GetDirectoryName(selectedFilePaths[0]);
                Settings.Save();
            }

            string filesText = "";
            foreach (string filePath in selectedFilePaths)
            {
                filesText += string.Format("\"{0}\" ", Path.GetFileName(filePath));
            }
            textFiles.Text = filesText;
        }
        
        private void checkRefFile_CheckedChanged(object sender, EventArgs e)
        {
            buttonRefFile.Enabled = checkRefFile.Checked;
            checkSetRefRoot.Enabled = checkRefFile.Checked;
        }

        private void buttonRefFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog inputFileDialog = new OpenFileDialog();
            inputFileDialog.Filter = "All files (*.*)|*.*";

            string defaultRefDir = Settings.ReferenceDirectory;
            if (!string.IsNullOrEmpty(defaultRefDir))
            {
                if (Directory.Exists(defaultRefDir))
                {
                    inputFileDialog.InitialDirectory = defaultRefDir;
                }
            }

            DialogResult selectionResult = inputFileDialog.ShowDialog();
            if (selectionResult != DialogResult.OK) return;

            string refFilePath = inputFileDialog.FileName;
            referenceFileName = Path.GetFileName(refFilePath);

            if (!string.IsNullOrEmpty(refFilePath))
            {
                Settings.ReferenceDirectory = Path.GetDirectoryName(refFilePath);
                Settings.Save();
            }

            textRefFile.Text = string.Format("\"{0}\" ", referenceFileName);
        }

        private void buttonOutDir_Click(object sender, EventArgs e)
        {
            string defaultOutputDir = Settings.OutputDirectory;

            using (FolderBrowserDialog selectionDialog = new FolderBrowserDialog())
            {
                selectionDialog.Description = "Choose a folder where the MakeBite structure will be built. Making a new folder is highly recommended.";
                selectionDialog.UseDescriptionForTitle = true;

                if (textOutDir.Text != "")
                    selectionDialog.InitialDirectory = textOutDir.Text;
                else if (!string.IsNullOrEmpty(defaultOutputDir))
                    selectionDialog.InitialDirectory = defaultOutputDir;

                if (selectionDialog.ShowDialog() != DialogResult.OK) return;

                string directoryPath = selectionDialog.SelectedPath;

                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Settings.OutputDirectory = directoryPath;
                    Settings.Save();
                }

                outputDirectory = directoryPath;
                textOutDir.Text = directoryPath;
            }
        }

        private void checkPullTextures_CheckedChanged(object sender, EventArgs e)
        {
            buttonTextureDir.Enabled = checkPullTextures.Checked;
        }

        private void buttonTextureDir_Click(object sender, EventArgs e)
        {
            string defaultTextureDir = Settings.TextureDirectory;

            using (FolderBrowserDialog selectionDialog = new FolderBrowserDialog())
            {
                selectionDialog.Description = "Choose a folder where vanilla .ftex and .ftexs files can be copied from. The tool will search subfolders as well.";
                selectionDialog.UseDescriptionForTitle = true;

                if (textTextureDir.Text != "")
                    selectionDialog.InitialDirectory = textTextureDir.Text;
                else if (!string.IsNullOrEmpty(defaultTextureDir))
                    selectionDialog.InitialDirectory = defaultTextureDir;

                if (selectionDialog.ShowDialog() != DialogResult.OK) return;

                string directoryPath = selectionDialog.SelectedPath;

                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Settings.TextureDirectory = directoryPath;
                    Settings.Save();
                }

                VanillaTexturesPath = directoryPath;
                textTextureDir.Text = directoryPath;
            }
        }

        private void buttonProliferate_Click(object sender, EventArgs e)
        {

            if (!File.Exists("TppMasterFileList.txt"))
            {
                MessageBox.Show("TppMasterFileList.txt is missing from the File Proliferator.exe folder.", "Missing TppMasterFileList.txt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedFilePaths == null)
            {
                MessageBox.Show("Please choose file(s) to build directories for.", "No Files Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (outputDirectory == null)
            {
                MessageBox.Show("Please select an output folder.", "Missing Output Directory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (checkRefFile.Checked && referenceFileName == null)
            {
                MessageBox.Show("Please select a Reference File.", "No Reference File Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (checkPullTextures.Checked && VanillaTexturesPath == null)
            {
                MessageBox.Show("Please set a Texture Directory.", "No Reference File Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            UpdateManager updateManager = new UpdateManager();
            TextureManager textureManager = new TextureManager();
            ProliferateManager proliferator = new ProliferateManager();
            FormProcessingProliferation processWindow = new FormProcessingProliferation();
            updateManager.SendFeedback += processWindow.OnSendFeedback;
            proliferator.SendFeedback += processWindow.OnSendFeedback;
            textureManager.SendFeedback += processWindow.OnSendFeedback;
            Exception err = null;

            if (checkConvertDds.Checked)
            {
                ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { selectedFilePaths = textureManager.convertDdsToFtex(selectedFilePaths); }));
                err = textureManager.errorMsg;
                if (err != null)
                {
                    DisplayError(err);
                    return;
                }
            }
            Console.WriteLine("Ftex Conversion Complete");
            int ddsConversionFailedCount = textureManager.getConversionFailedCount();
            if (ddsConversionFailedCount > 0)
            {
                DialogResult dialogResult = MessageBox.Show(string.Format("There were {0} .dds file(s) that failed to convert to .ftex formatting. Unconverted .dds files will not be included in the Directory Structure.\n\nWould you still like to build the Directory Structure?", ddsConversionFailedCount), "Missing _pftxs Textures", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult != DialogResult.Yes)
                    return;
            }

            if(checkNameUpdates.Checked)
            {
                ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { selectedFilePaths = updateManager.DoUpdates(selectedFilePaths); }));
                err = updateManager.errorMsg;
                if (err != null)
                {
                    DisplayError(err);
                    return;
                }
            }

            if (!checkRefFile.Checked)
            {
                ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { proliferator.DoProliferate(selectedFilePaths, outputDirectory); }));                
            }
            else
            {
                ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { proliferator.DoProliferateFromReference(selectedFilePaths, outputDirectory, checkSetRefRoot.Checked, referenceFileName); }));
            }
            err = proliferator.errorMsg;
            if (err != null)
            {
                DisplayError(err);
                return;
            }
            Console.WriteLine("DoProliferate Complete");

            if (checkPullTextures.Checked)
            {
                if (checkRefFile.Checked)
                {
                    ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { textureManager.PullVanillaTextures(outputDirectory, VanillaTexturesPath, referenceFileName); }));
                }
                else
                {
                    ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { textureManager.PullVanillaTextures(outputDirectory, VanillaTexturesPath, selectedFilePaths); }));
                    //textureManager.PullVanillaTextures(outputDirectory, VanillaTexturesPath, selectedFilePaths);
                }
                err = textureManager.errorMsg;
                if (err != null)
                {
                    DisplayError(err);
                    return;
                }
                Console.WriteLine("PullVanillaTextures Complete");
            }

            int texturePullsFailed = textureManager.getTextureNotFoundCount();
            if (checkPackPftxs.Checked && textureManager.getPftxsDirCount(outputDirectory) > 0)
            {
                bool doPftxsPack = true;

                if(texturePullsFailed > 0)
                {
                    DialogResult dialogResult = MessageBox.Show(string.Format("There are {0} texture file(s) that could not be found in {1}. These textures were not pulled into the _pftxs folder(s).\n\nWould you still like to pack the _pftxs folders into .pftxs files?", texturePullsFailed, Path.GetFileName(VanillaTexturesPath)), "Missing _pftxs Textures", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    doPftxsPack = (dialogResult == DialogResult.Yes);
                }

                if (doPftxsPack)
                {
                    ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { textureManager.PackPftxsFolders(outputDirectory); }));
                    err = textureManager.errorMsg;
                    if (err != null)
                    {
                        DisplayError(err);
                        return;
                    }

                    DialogResult dialogResult = MessageBox.Show("The _pftxs folders have been packed into .pftxs files.\n\nWould you like to delete the leftover _pftxs folders?", "Delete _pftxs Folders?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.Yes)
                    {
                        ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { textureManager.DeletePftxsFolders(outputDirectory); }));
                        err = textureManager.errorMsg;
                        if (err != null)
                        {
                            DisplayError(err);
                            return;
                        }
                    }
                }
                Console.WriteLine("Pack _PFTXS Complete");
            }

            if (texturePullsFailed > 0)
            {
                MessageBox.Show(string.Format("Process Complete. There were {0} texture file(s) that could not be found in {1}.\n\nThese textures were not pulled into the _pftxs folder(s).", texturePullsFailed, Path.GetFileName(VanillaTexturesPath)), "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Done!", "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void checkNameUpdates_CheckedChanged(object sender, EventArgs e)
        {
            if (checkNameUpdates.Checked)
            {
                if (!File.Exists("qar_dictionary.txt"))
                {
                    MessageBox.Show("qar_dictionary.txt is missing from the application folder. This tool cannot update filenames without qar_dictionary.txt", "Missing qar_dictionary.txt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    checkNameUpdates.Checked = false;
                }
            }
        }

        public void DisplayError(Exception errorMsg)
        {
            MessageBox.Show("An error occurred while attempting to proliferate data:\n" + errorMsg);
        }
    }
}
