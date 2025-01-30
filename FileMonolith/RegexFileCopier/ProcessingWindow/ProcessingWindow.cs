using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ProcessWindow
{
    public static class ProcessingWindow
    {
        public static void Show(Form processWindow, Action WorkerFunction)
        {
            BackgroundWorker processWorker = new BackgroundWorker();

            processWorker.DoWork += (obj, var) => {
                WorkerFunction();
            };
            processWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate (object sender, RunWorkerCompletedEventArgs e) 
            {
                processWindow.Invoke((MethodInvoker)delegate 
                {
                    processWindow.Close();
                });
                processWorker.Dispose();
            });

            processWorker.RunWorkerAsync();
            processWindow.ShowDialog();
        }
    }
}
