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
            try
            {
                if (labelCurrentWork.IsHandleCreated)
                {
                    labelCurrentWork.Invoke(new Action(() => labelCurrentWork.Text = e.Feedback));
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception occurred during feedback: \n" + exception);
            }
        }
    }
}
