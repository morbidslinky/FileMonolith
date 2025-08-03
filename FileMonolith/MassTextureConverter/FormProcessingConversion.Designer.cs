namespace MassTextureConverter
{
    partial class FormProcessingConversion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcessingConversion));
            pictureSpiral = new System.Windows.Forms.PictureBox();
            labelCurrentFile = new System.Windows.Forms.Label();
            labelUnpack = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)pictureSpiral).BeginInit();
            SuspendLayout();
            // 
            // pictureSpiral
            // 
            pictureSpiral.Image = (System.Drawing.Image)resources.GetObject("pictureSpiral.Image");
            pictureSpiral.Location = new System.Drawing.Point(14, 9);
            pictureSpiral.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureSpiral.Name = "pictureSpiral";
            pictureSpiral.Size = new System.Drawing.Size(42, 40);
            pictureSpiral.TabIndex = 5;
            pictureSpiral.TabStop = false;
            // 
            // labelCurrentFile
            // 
            labelCurrentFile.Location = new System.Drawing.Point(14, 53);
            labelCurrentFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelCurrentFile.Name = "labelCurrentFile";
            labelCurrentFile.Size = new System.Drawing.Size(357, 50);
            labelCurrentFile.TabIndex = 4;
            labelCurrentFile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelUnpack
            // 
            labelUnpack.Location = new System.Drawing.Point(14, 9);
            labelUnpack.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelUnpack.Name = "labelUnpack";
            labelUnpack.Size = new System.Drawing.Size(357, 27);
            labelUnpack.TabIndex = 3;
            labelUnpack.Text = "Unpacking, please wait...";
            labelUnpack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormProcessingConversion
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(376, 105);
            ControlBox = false;
            Controls.Add(pictureSpiral);
            Controls.Add(labelCurrentFile);
            Controls.Add(labelUnpack);
            Cursor = System.Windows.Forms.Cursors.WaitCursor;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormProcessingConversion";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Processing...";
            ((System.ComponentModel.ISupportInitialize)pictureSpiral).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureSpiral;
        private System.Windows.Forms.Label labelCurrentFile;
        private System.Windows.Forms.Label labelUnpack;
    }
}