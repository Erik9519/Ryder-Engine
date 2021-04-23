using System;
using System.IO;
using System.Windows.Forms;
using static Ryder_Engine.Components.Server;

namespace Ryder_Engine.Forms
{
    public partial class Steam_2FA : Form
    {
        public EventHandler<object[]> sendSteam2FA;
        private Listener listener;

        public Steam_2FA(Listener listener)
        {
            InitializeComponent();
            this.listener = listener;
            this.TopLevel = true;
            this.Focus();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            try
            {
                listener.m.WaitOne();
                listener.writer.WriteLine("[\"steamLogin2FA\",\"" + codeTextBox.Text + "\"]");
                listener.writer.Flush();
                listener.m.ReleaseMutex();
            }
            catch { }
            this.Close();
        }
    }
}
