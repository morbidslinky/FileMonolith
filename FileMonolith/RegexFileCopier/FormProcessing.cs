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
            try
            {
                labelCurrentFile.Invoke(new Action(() => labelCurrentFile.Text = (string)e.Feedback));
            }
            catch
            {
                MessageBox.Show("Exception occurred during processing: \n" + (Exception)e.Feedback);
            }
        }
    }
}
