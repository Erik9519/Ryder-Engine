using System;
using System.Windows.Forms;

namespace Ryder_Engine
{
    public partial class Steam_Login : Form
    {
        public EventHandler<string[]> sendSteamLogin;
        private string ip;

        public Steam_Login(string ip)
        {
            InitializeComponent();
            this.ip = ip;
            this.Focus();
            this.TopLevel = true;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string[] vals = { usernameTextBox.Text, passwordTextBox.Text, ip };
            sendSteamLogin(this, vals);
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
