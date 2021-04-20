using Ryder_Engine.Components.MonitorModules;
using System;

namespace Ryder_Engine.Components
{
    class SystemMonitor
    {
        public ForegroundProcessMonitor foregroundProcessMonitor;
        public MSIMonitor msiMonitor;
        public NetworkMonitor networkMonitor;
        public StorageMonitor storageMonitor;
        public FanController fanController;

        public SystemMonitor()
        {
            foregroundProcessMonitor = new ForegroundProcessMonitor();
            msiMonitor = new MSIMonitor();
            networkMonitor = new NetworkMonitor();
            storageMonitor = new StorageMonitor();
            fanController = new FanController(Environment.MachineName == "DESKTOP-ERIK" ? "COM4" : "null");
        }

        public void update()
        {
            msiMonitor.update();
            networkMonitor.update();
            storageMonitor.update();
            fanController.update();
        }

        public string getStatusJSON()
        {
            string res = "{\n";
            // MSI
            res += "\t\"msi\": {\n";
            for (ushort i = 0; i < msiMonitor.sensors.Length; i++)
            {
                res += "\t\t\""+ msiMonitor.sensors[i].name + "\": " + ((int)msiMonitor.sensors[i].value).ToString();
                if (i < msiMonitor.sensors.Length - 1) res += ",";
                res += "\n";
            }
            res += "\t},\n";
            // Network
            res += "\t\"network\": {\n";
            res += "\t\t\"downloadSpeed\": " + networkMonitor.status.downloadSpeed.ToString() +",\n";
            res += "\t\t\"uploadSpeed\": " + networkMonitor.status.uploadSpeed.ToString() +"\n";
            res += "\t},\n";
            // Storage
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
            // Fan Controller
            if (fanController.connected)
            {
                res += ",\n";
                res += "\t\"fanController\": {\n";
                res += "\t\t\"ambient\": " + fanController.ambient.ToString() + ",\n";
                res += "\t\t\"liquid\": " + fanController.liquid.ToString() + "\n";
                res += "\t}\n";
            } else
            {
                res += "\n";
            }
            // End
            res += "}";
            return res;
        }
    }
}
