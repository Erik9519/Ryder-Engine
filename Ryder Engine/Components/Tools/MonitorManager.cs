using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Ryder_Engine.Components.Tools
{
    public class MonitorManager
    {
        public int brightness = 100;
        private string path;

        public MonitorManager()
        {
            path = getEXE();
            getBrightness();
        }

        private void getBrightness()
        {
            if (path != null)
            {
                try
                {
                    string bat_location =
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                            @"\Resources\ControlMyMonitor\GetBrightness.bat";
                    Process p = new Process();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.FileName = bat_location;
                    p.Start();
                    string[] txt = p.StandardOutput.ReadToEnd().Split("\n");
                    brightness = int.Parse(txt[txt.Length - 2]);
                    p.WaitForExit();
                } catch
                {
                    brightness = 100;
                }
            }
        }

        public void setBrightness(int value)
        {
            if (path != null)
            {
                Process p = getProcess(String.Format("/SetValueIfNeeded Primary 10 {0}", value.ToString()));
                p.Start();
                brightness = value;
            }
        }

        private string getEXE()
        {
            string exe_location = 
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                @"\Resources\ControlMyMonitor\ControlMyMonitor.exe";
            return (File.Exists(exe_location) ? exe_location : null);
        }

        private Process getProcess(string args)
        {
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = args;
            p.StartInfo.CreateNoWindow = true;
            return p;
        }
    }
}
