using System;
using System.Threading;
using System.Windows.Forms;

namespace Ryder_Engine.Utils
{
    class MTAOpenFileDialog
    {
        public class MTAOpenFileDialogEventArgs : EventArgs
        {
            public string FileName { get; set; }
            public MTAOpenFileDialogEventArgs(string fname)
            {
                FileName = fname;
            }
        }

        public EventHandler OnComplete;

        public void ShowDialog()
        {
            try
            {
                Thread t = new Thread(() => GetFile(OnComplete));
                t.TrySetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error: " + exc.Message);
            }
        }

        private void GetFile(EventHandler OnComplete)
        {
            using (OpenFileDialog fileOpen = new OpenFileDialog())
            {
                if (fileOpen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (OnComplete != null)
                    {
                        // Call event on new thread
                        new Thread(() =>
                        {
                            OnComplete.Invoke(this, new MTAOpenFileDialogEventArgs(fileOpen.FileName));
                        }).Start();
                    }
                }
                //Thread.CurrentThread.Abort();
            }
        }
    }
}
