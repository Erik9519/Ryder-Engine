using Ryder_Engine.Components;
using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace Ryder_Engine.Forms
{
    public partial class Settings : Form
    {
        SystemMonitor systemMonitor;
        public Settings(SystemMonitor systemMonitor)
        {
            InitializeComponent();
            this.systemMonitor = systemMonitor;
            // Initialize UI
            msiCheckBox.Checked = Properties.Settings.Default.MSI_module;
            foregroundMonitorCheckBox.Checked = Properties.Settings.Default.ForegroundAppMonitor_module;
            networkMonitorCheckBox.Checked = Properties.Settings.Default.NetworkMonitor_module;
            storageMonitorCheckBox.Checked = Properties.Settings.Default.StorageMonitor_module;
            comPortMonitorCheckBox.Checked = Properties.Settings.Default.COMPortMonitor_module;
            pswTextBox.Text = Properties.Settings.Default.Password;

            string[] ports = SerialPort.GetPortNames();
            short index = -1;
            for (short i = 0; i < ports.Length; i++) {
                comPort_comboBox.Items.Add(ports[i]);
                if (ports[i] == Properties.Settings.Default.COMPort)
                {
                    index = i;
                }
            }
            if (index != -1)
                comPort_comboBox.SelectedIndex = index;
            else
                comPort_comboBox.SelectedIndex = 0;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            // Apply new settings
            Properties.Settings.Default.MSI_module = msiCheckBox.Checked;
            Properties.Settings.Default.ForegroundAppMonitor_module = foregroundMonitorCheckBox.Checked;
            Properties.Settings.Default.NetworkMonitor_module = networkMonitorCheckBox.Checked;
            Properties.Settings.Default.StorageMonitor_module = storageMonitorCheckBox.Checked;
            Properties.Settings.Default.COMPortMonitor_module = comPortMonitorCheckBox.Checked;
            Properties.Settings.Default.COMPort = comPort_comboBox.Text;
            Properties.Settings.Default.Password = pswTextBox.Text;
            Properties.Settings.Default.Save();
            systemMonitor.applySettings();
            this.Close();
            this.Dispose();
        }
    }
}
