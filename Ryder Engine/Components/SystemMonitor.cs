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
            string res = "{";
            bool hasPrevious = false;
            // MSI
            if (this.msiMonitor != null) {
                res += "\"msi\":{";
                for (ushort i = 0; i < msiMonitor.sensors.Length; i++)
                {
                    res += "\"" + msiMonitor.sensors[i].name + "\":" + ((int)msiMonitor.sensors[i].value).ToString();
                    if (i < msiMonitor.sensors.Length - 1) res += ",";
                }
                res += "}";
                hasPrevious = true;
            }
            // Network
            if (this.networkMonitor != null)
            {
                if (hasPrevious) res += ",";
                res += "\"network\":{";
                res += "\"downloadSpeed\":" + networkMonitor.status.downloadSpeed.ToString("0.##") + ",";
                res += "\"uploadSpeed\":" + networkMonitor.status.uploadSpeed.ToString("0.##");
                res += "}";
                hasPrevious = true;
            }
            // Storage
            if (this.storageMonitor != null)
            {
                if (hasPrevious) res += ",";
                res += "\"storage\":{";
                for (ushort i = 0; i < storageMonitor.drives.Length; i++)
                {
                    res += "\"" + storageMonitor.drives[i].letter + "\":{";
                    res += "\"activity\":" + ((int)storageMonitor.drives[i].activity).ToString() + ",";
                    res += "\"readSpeed\":" + storageMonitor.drives[i].readSpeed.ToString("0.##") + ",";
                    res += "\"writeSpeed\":" + storageMonitor.drives[i].writeSpeed.ToString("0.##");
                    res += "}";
                    if (i < storageMonitor.drives.Length - 1) res += ",";
                }
                res += "}";
                hasPrevious = true;
            }
            // Fan Controller
            if (this.fanController != null)
            {
                if (hasPrevious) res += ",";
                res += "\"fanController\":{";
                res += "\"ambient\":" + fanController.ambient.ToString() + ",";
                res += "\"liquid\":" + fanController.liquid.ToString();
                res += "}";
                hasPrevious = true;
            }
            // End
            res += "}";
            return res;
        }
    }
}
