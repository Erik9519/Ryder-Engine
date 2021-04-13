using MSIAfterburner;
using System;
using System.Diagnostics;
using System.Management;

namespace Ryder_Engine.Components.MonitorModules
{
    class MSIMonitor
    {
        private AfterBurner msi;

        public Sensor[] sensors;

        public MSIMonitor()
        {
            msi = new AfterBurner();
        }

        public void update()
        {
            sensors = msi.getSensors();
        }

        public string getStatusJSON()
        {
            string res = "\"msi\"{\n";

            return res;
        }

        private uint getVRAMsize()
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = @"C:\Windows\System32\nvidia-smi.exe";
            p.StartInfo.Arguments = "--query-gpu=memory.total --format=csv,noheader,nounits";
            p.Start();
            // Get VRAM size
            string txt = p.StandardOutput.ReadToEnd();
            Debug.WriteLine(txt);
            return uint.Parse(txt);
        }
        private string getManagementObject(string root, string entry, string property)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(root, entry);
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    searcher.Dispose();
                    return queryObj[property].ToString();
                }
            }
            catch (ManagementException e) { }
            return null;
        }
    }
}
