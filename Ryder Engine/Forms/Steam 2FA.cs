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
            listener.sendMsg("[\"steam2fa\",\"" + codeTextBox.Text + "\"]");
            this.Close();
        }
    }
}
