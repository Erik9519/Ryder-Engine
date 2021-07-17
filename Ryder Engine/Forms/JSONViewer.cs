using Ryder_Engine.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace Ryder_Engine.Forms
{
    public partial class JSONViewer : Form
    {
        SystemMonitor systemMonitor;

        public JSONViewer(SystemMonitor systemMonitor)
        {
            InitializeComponent();
            this.systemMonitor = systemMonitor;
        }

        public void updateData()
        {
            string prettyJson = JsonSerializer.Serialize(
                JsonSerializer.Deserialize<JsonElement>(systemMonitor.getStatusJSON()),
                new JsonSerializerOptions(){WriteIndented = true}
            );
            jsonTextBox.Text = prettyJson;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
