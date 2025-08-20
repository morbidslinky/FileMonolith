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

namespace ArchiveTransferrer
{
    public partial class FormTransferrer : Form
    {
        string gzTextureG0s;
        string mgoTextureDat;
        string tppMasterDir;

        string GZExe;
        string TPPExe;

        public FormTransferrer()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {

            TransferManager transferrer = new TransferManager();
            FormProcessingTransfer processWindow = new FormProcessingTransfer();
            transferrer.SendFeedback += processWindow.OnSendFeedback;

            ProcessingWindow.Show(processWindow, new Action((MethodInvoker)delegate { transferrer.Transfer(gzTextureG0s, mgoTextureDat, tppMasterDir); }));

            if (transferrer.errorOccurred != "")
            {
                MessageBox.Show("An error occurred while attempting to transfer data:\n" + transferrer.errorOccurred);
            } 
            if (transferrer.successfulTransfers.Count > 0)
            {
                MessageBox.Show("The following archives were transferred successfully: " + transferrer.GetSuccessfulTransfers());
            }
            else
            {
                MessageBox.Show("No archives were transferred.");
            }
        }

        private void buttonGZEXE_Click(object sender, EventArgs e)
        {
            OpenFileDialog inputDialog = new OpenFileDialog();
            inputDialog.Filter = "GZ Executable|MgsGroundZeroes.exe";
            inputDialog.Multiselect = false;

            DialogResult selectionResult = inputDialog.ShowDialog();
            if (selectionResult != DialogResult.OK) return;
            textGZEXE.Text = inputDialog.FileName;

            string filename = "data_01.g0s";
            gzTextureG0s = Path.Combine(Path.GetDirectoryName(textGZEXE.Text), filename);
            checkForArchive(gzTextureG0s, filename, labelGZFOUND);

            updateTransferButton();
        }

        private void buttonTPPEXE_Click(object sender, EventArgs e)
        {
            OpenFileDialog inputDialog = new OpenFileDialog();
            inputDialog.Filter = "TPP Executable|mgsvtpp.exe";
            inputDialog.Multiselect = false;

            DialogResult selectionResult = inputDialog.ShowDialog();
            if (selectionResult != DialogResult.OK) return;
            textTPPEXE.Text = inputDialog.FileName;
            
            string filename = "mgo\\texture0.dat";
            mgoTextureDat = Path.Combine(Path.GetDirectoryName(textTPPEXE.Text), filename);
            checkForArchive(mgoTextureDat, filename, labelMGOFOUND);
            
            tppMasterDir = Path.Combine(Path.GetDirectoryName(textTPPEXE.Text), "master");
            updateTransferButton();
        }

        private void checkForArchive(string archivePath, string filename, Label statusLabel)
        {
            if (File.Exists(archivePath))
            {
                statusLabel.Text = string.Format("'{0}' Found!", filename);
                statusLabel.ForeColor = Color.ForestGreen;
            }
            else
            {
                statusLabel.Text = string.Format("'{0}' Not Found!", filename);
                statusLabel.ForeColor = Color.Red;
            }
        }

        private void updateTransferButton()
        {
            bool g0sReady = textGZEXE.Text != "" && File.Exists(gzTextureG0s);
            bool mgoReady = textTPPEXE.Text != "" && File.Exists(mgoTextureDat);
            bool masterExists = Directory.Exists(tppMasterDir);

            buttonStart.Enabled = g0sReady && mgoReady && masterExists;
            Console.WriteLine(gzTextureG0s + " | " + mgoTextureDat + " | " + tppMasterDir);
        }
        
    }
}
