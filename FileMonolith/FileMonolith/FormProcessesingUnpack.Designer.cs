namespace ArchiveUnpacker
{
    partial class FormProcessesingUnpack
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcessesingUnpack));
            labelUnpack = new System.Windows.Forms.Label();
            labelCurrentWork = new System.Windows.Forms.Label();
            pictureSpiral = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureSpiral).BeginInit();
            SuspendLayout();
            // 
            // labelUnpack
            // 
            labelUnpack.Location = new System.Drawing.Point(14, 9);
            labelUnpack.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelUnpack.Name = "labelUnpack";
            labelUnpack.Size = new System.Drawing.Size(357, 27);
            labelUnpack.TabIndex = 0;
            labelUnpack.Text = "Unpacking, please wait...";
            labelUnpack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelCurrentFile
            // 
            labelCurrentWork.Location = new System.Drawing.Point(14, 53);
            labelCurrentWork.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelCurrentWork.Name = "labelCurrentFile";
            labelCurrentWork.Size = new System.Drawing.Size(357, 50);
            labelCurrentWork.TabIndex = 1;
            labelCurrentWork.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureSpiral
            // 
            pictureSpiral.Image = (System.Drawing.Image)resources.GetObject("pictureSpiral.Image");
            pictureSpiral.Location = new System.Drawing.Point(14, 9);
            pictureSpiral.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureSpiral.Name = "pictureSpiral";
            pictureSpiral.Size = new System.Drawing.Size(42, 40);
            pictureSpiral.TabIndex = 2;
            pictureSpiral.TabStop = false;
            // 
            // FormProcessesingUnpack
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(372, 101);
            ControlBox = false;
            Controls.Add(pictureSpiral);
            Controls.Add(labelCurrentWork);
            Controls.Add(labelUnpack);
            Cursor = System.Windows.Forms.Cursors.WaitCursor;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormProcessesingUnpack";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Processing...";
            ((System.ComponentModel.ISupportInitialize)pictureSpiral).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelUnpack;
        private System.Windows.Forms.Label labelCurrentWork;
        private System.Windows.Forms.PictureBox pictureSpiral;
    }
}