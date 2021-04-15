using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ryder_Engine.Forms
{
    public partial class Steam_2FA : Form
    {
        public EventHandler<string[]> sendSteam2FA;
        private string ip;

        public Steam_2FA(string ip)
        {
            InitializeComponent();
            this.ip = ip;
            this.TopLevel = true;
            this.Focus();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string[] vals = { codeTextBox.Text, ip };
            sendSteam2FA(this, vals);
            this.Close();
        }
    }
}
