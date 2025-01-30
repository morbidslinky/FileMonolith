using System;
using System.Windows.Forms;

namespace ArchiveUnpacker
{
    public partial class FormProcessesingUnpack : Form
    {
        public FormProcessesingUnpack()
        {
            InitializeComponent();
        }
        public void OnSendFeedback(object source, FeedbackEventArgs e)
        {
            if (e.Feedback is string)
            {
                if (labelCurrentFile.InvokeRequired)
                {
                    labelCurrentFile.Invoke(new Action(() => labelCurrentFile.Text = (string)e.Feedback));
                }
                else
                {
                    labelCurrentFile.Text = (string)e.Feedback;
                }
            }
            else if (e.Feedback is Exception)
            {
                MessageBox.Show("Exception occurred during unpack: \n" + (Exception)e.Feedback);
            }
        }
    }
}
