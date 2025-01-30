namespace RegexFileCopier
{
    partial class FormProcessing
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcessing));
            labelProcessing = new Label();
            labelCurrentFile = new Label();
            pictureSpiral = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureSpiral).BeginInit();
            SuspendLayout();
            // 
            // labelProcessing
            // 
            labelProcessing.Location = new Point(14, 9);
            labelProcessing.Margin = new Padding(4, 0, 4, 0);
            labelProcessing.Name = "labelProcessing";
            labelProcessing.Size = new Size(357, 27);
            labelProcessing.TabIndex = 0;
            labelProcessing.Text = "Processing, please wait...";
            labelProcessing.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelCurrentFile
            // 
            labelCurrentFile.Location = new Point(14, 53);
            labelCurrentFile.Margin = new Padding(4, 0, 4, 0);
            labelCurrentFile.Name = "labelCurrentFile";
            labelCurrentFile.Size = new Size(357, 50);
            labelCurrentFile.TabIndex = 1;
            labelCurrentFile.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureSpiral
            // 
            pictureSpiral.Image = (Image)resources.GetObject("pictureSpiral.Image");
            pictureSpiral.Location = new Point(12, 8);
            pictureSpiral.Margin = new Padding(4, 3, 4, 3);
            pictureSpiral.Name = "pictureSpiral";
            pictureSpiral.Size = new Size(36, 35);
            pictureSpiral.TabIndex = 2;
            pictureSpiral.TabStop = false;
            // 
            // FormProcessing
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(376, 104);
            ControlBox = false;
            Controls.Add(pictureSpiral);
            Controls.Add(labelCurrentFile);
            Controls.Add(labelProcessing);
            Cursor = Cursors.WaitCursor;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormProcessing";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Processing...";
            ((System.ComponentModel.ISupportInitialize)pictureSpiral).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label labelProcessing;
        private System.Windows.Forms.Label labelCurrentFile;
        private System.Windows.Forms.PictureBox pictureSpiral;
    }
}