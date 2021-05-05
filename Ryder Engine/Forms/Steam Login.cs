using System;
using System.IO;
using System.Windows.Forms;
using static Ryder_Engine.Components.Server;

namespace Ryder_Engine
{
    public partial class Steam_Login : Form
    {
        public EventHandler<object[]> sendSteamLogin;
        private Listener listener;

        public Steam_Login(Listener listener)
        {
            InitializeComponent();
            this.listener = listener;
            this.TopLevel = true;
            this.Focus();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            listener.sendMsg("[\"steamLogin\",\"" + usernameTextBox.Text + "\",\"" + passwordTextBox.Text + "\"]");
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
