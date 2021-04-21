using Ryder_Engine.Components.MonitorModules;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ryder_Engine.Components
{
    public class SystemMonitor
    {
        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        public ForegroundProcessMonitor foregroundProcessMonitor = null;
        public EventHandler<string> newForegroundProcess = null;

        public MSIMonitor msiMonitor = null;
        public NetworkMonitor networkMonitor = null;
        public StorageMonitor storageMonitor = null;
        public FanController fanController = null;

        public SystemMonitor() { }

        public void applySettings()
        {
            if (Properties.Settings.Default.MSI_module)
            {
                if (msiMonitor == null) msiMonitor = new MSIMonitor();
            } 
            else
            {
                if (msiMonitor != null) msiMonitor = null;
            }
            if (Properties.Settings.Default.ForegroundAppMonitor_module)
            {
                if (foregroundProcessMonitor == null)
                {
                    foregroundProcessMonitor = new ForegroundProcessMonitor();
                    foregroundProcessMonitor.newForegroundProcess = newForegroundProcess;
                }
            }
            else
            {
                if (foregroundProcessMonitor != null)
                {
                    foregroundProcessMonitor.Dispose();
                    foregroundProcessMonitor = null;
                }
            }
            if (Properties.Settings.Default.NetworkMonitor_module)
            {
                if (networkMonitor == null) networkMonitor = new NetworkMonitor();
            }
            else
            {
                if (networkMonitor != null) networkMonitor = null;
            }
            if (Properties.Settings.Default.StorageMonitor_module)
            {
                if (storageMonitor == null) storageMonitor = new StorageMonitor();
            }
            else
            {
                if (storageMonitor == null)
                {
                    storageMonitor.Dispose();
                    storageMonitor = null;
                }
            }
            if (Properties.Settings.Default.COMPortMonitor_module)
            {
                if (fanController == null)
                {
                    fanController = new FanController(Properties.Settings.Default.COMPort);
                } else
                {
                    fanController.connect(Properties.Settings.Default.COMPort);
                }
            }
            else
            {
                if (fanController != null)
                {
                    fanController.disconnect();
                    fanController = null;
                }
            }

            GC.Collect();
            EmptyWorkingSet(Process.GetCurrentProcess().Handle);
        }

        public void update()
        {
            if (msiMonitor != null) msiMonitor.update();
            if (networkMonitor != null) networkMonitor.update();
            if (storageMonitor != null) storageMonitor.update();
            if (fanController != null) fanController.update();
        }

        public string getStatusJSON()
        {
            string res = "{\n";
            bool hasPrevious = false;
            // MSI
            if (this.msiMonitor != null) {
                res += "\t\"msi\": {\n";
                for (ushort i = 0; i < msiMonitor.sensors.Length; i++)
                {
                    res += "\t\t\"" + msiMonitor.sensors[i].name + "\": " + ((int)msiMonitor.sensors[i].value).ToString();
                    if (i < msiMonitor.sensors.Length - 1) res += ",";
                    res += "\n";
                }
                res += "\t}";
                hasPrevious = true;
            }
            // Network
            if (this.networkMonitor != null)
            {
                if (hasPrevious) res += ",\n";
                res += "\t\"network\": {\n";
                res += "\t\t\"downloadSpeed\": " + networkMonitor.status.downloadSpeed.ToString() + ",\n";
                res += "\t\t\"uploadSpeed\": " + networkMonitor.status.uploadSpeed.ToString() + "\n";
                res += "\t}";
                hasPrevious = true;
            }
            // Storage
            if (this.storageMonitor != null)
            {
                if (hasPrevious) res += ",\n";
                res += "\t\"storage\": {\n";
                for (ushort i = 0; i < storageMonitor.drives.Length; i++)
                {
                    res += "\t\t\"" + storageMonitor.drives[i].letter + "\": {\n";
                    res += "\t\t\t\"readSpeed\": " + storageMonitor.drives[i].readSpeed + ",\n";
                    res += "\t\t\t\"writeSpeed\": " + storageMonitor.drives[i].writeSpeed + "\n";
                    res += "\t\t}";
                    if (i < storageMonitor.drives.Length - 1) res += ",";
                    res += "\n";
                }
                res += "\t}";
                hasPrevious = true;
            }
            // Fan Controller
            if (this.fanController != null)
            {
                if (hasPrevious) res += ",\n";
                res += "\t\"fanController\": {\n";
                res += "\t\t\"ambient\": " + fanController.ambient.ToString() + ",\n";
                res += "\t\t\"liquid\": " + fanController.liquid.ToString() + "\n";
                res += "\t}";
                hasPrevious = true;
            }
            // End
            res += "\n}";
            return res;
        }
    }
}
