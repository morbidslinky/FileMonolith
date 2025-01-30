namespace RegexFileCopier
{
    partial class FormRegexFileCopier
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
            textInputDir = new TextBox();
            buttonOutDir = new Button();
            textOutputDir = new TextBox();
            buttonStart = new Button();
            buttonArchives = new Button();
            labelArchives = new Label();
            labelOutDir = new Label();
            labelFilenameRegex = new Label();
            textFilename = new TextBox();
            SuspendLayout();
            // 
            // textArchives
            // 
            textInputDir.BackColor = SystemColors.MenuBar;
            textInputDir.Enabled = false;
            textInputDir.Location = new Point(14, 29);
            textInputDir.Margin = new Padding(4, 3, 4, 3);
            textInputDir.Name = "textArchives";
            textInputDir.ReadOnly = true;
            textInputDir.Size = new Size(518, 23);
            textInputDir.TabIndex = 1;
            // 
            // buttonOutDir
            // 
            buttonOutDir.Location = new Point(540, 83);
            buttonOutDir.Margin = new Padding(4, 3, 4, 3);
            buttonOutDir.Name = "buttonOutDir";
            buttonOutDir.Size = new Size(34, 27);
            buttonOutDir.TabIndex = 4;
            buttonOutDir.Text = "...";
            buttonOutDir.UseVisualStyleBackColor = true;
            buttonOutDir.Click += buttonOutDir_Click;
            // 
            // textOutDir
            // 
            textOutputDir.BackColor = SystemColors.MenuBar;
            textOutputDir.Enabled = false;
            textOutputDir.Location = new Point(14, 85);
            textOutputDir.Margin = new Padding(4, 3, 4, 3);
            textOutputDir.Name = "textOutDir";
            textOutputDir.ReadOnly = true;
            textOutputDir.Size = new Size(518, 23);
            textOutputDir.TabIndex = 3;
            // 
            // buttonStart
            // 
            buttonStart.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonStart.Location = new Point(434, 135);
            buttonStart.Margin = new Padding(4, 3, 4, 3);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(140, 38);
            buttonStart.TabIndex = 5;
            buttonStart.Text = "Copy Files";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += buttonStart_Click;
            // 
            // buttonArchives
            // 
            buttonArchives.Location = new Point(540, 27);
            buttonArchives.Margin = new Padding(4, 3, 4, 3);
            buttonArchives.Name = "buttonArchives";
            buttonArchives.Size = new Size(34, 27);
            buttonArchives.TabIndex = 2;
            buttonArchives.Text = "...";
            buttonArchives.UseVisualStyleBackColor = true;
            buttonArchives.Click += buttonInputDir_Click;
            // 
            // labelArchives
            // 
            labelArchives.AutoSize = true;
            labelArchives.Location = new Point(14, 10);
            labelArchives.Margin = new Padding(4, 0, 4, 0);
            labelArchives.Name = "labelArchives";
            labelArchives.Size = new Size(81, 15);
            labelArchives.TabIndex = 7;
            labelArchives.Text = "Search Folder:";
            // 
            // labelOutDir
            // 
            labelOutDir.AutoSize = true;
            labelOutDir.Location = new Point(14, 67);
            labelOutDir.Margin = new Padding(4, 0, 4, 0);
            labelOutDir.Name = "labelOutDir";
            labelOutDir.Size = new Size(84, 15);
            labelOutDir.TabIndex = 8;
            labelOutDir.Text = "Output Folder:";
            // 
            // labelFilenameRegex
            // 
            labelFilenameRegex.AutoSize = true;
            labelFilenameRegex.Location = new Point(14, 132);
            labelFilenameRegex.Margin = new Padding(4, 0, 4, 0);
            labelFilenameRegex.Name = "labelFilenameRegex";
            labelFilenameRegex.Size = new Size(93, 15);
            labelFilenameRegex.TabIndex = 10;
            labelFilenameRegex.Text = "Filename Regex:";
            // 
            // textFilename
            // 
            textFilename.Location = new Point(14, 150);
            textFilename.Name = "textFilename";
            textFilename.Size = new Size(413, 23);
            textFilename.TabIndex = 11;
            // 
            // FormExtractor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(588, 187);
            Controls.Add(textFilename);
            Controls.Add(labelFilenameRegex);
            Controls.Add(labelOutDir);
            Controls.Add(labelArchives);
            Controls.Add(buttonArchives);
            Controls.Add(buttonStart);
            Controls.Add(buttonOutDir);
            Controls.Add(textOutputDir);
            Controls.Add(textInputDir);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "FormExtractor";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Regex File Copier";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textInputDir;
        private System.Windows.Forms.Button buttonOutDir;
        private System.Windows.Forms.TextBox textOutputDir;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonArchives;
        private System.Windows.Forms.Label labelArchives;
        private System.Windows.Forms.Label labelOutDir;
        private Label labelFilenameRegex;
        private TextBox textFilename;
    }
}
