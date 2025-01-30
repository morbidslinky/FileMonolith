using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegexFileCopier
{
    public partial class FormProcessing : Form
    {
        public FormProcessing()
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
                MessageBox.Show("Exception occurred during processing: \n" + (Exception)e.Feedback);
            }
        }
    }
}
