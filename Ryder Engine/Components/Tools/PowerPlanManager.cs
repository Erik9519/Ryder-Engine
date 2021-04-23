using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Ryder_Engine.Components.Tools
{
    public class PowerPlanManager
    {
        Dictionary<string, string> powerPlan = new Dictionary<string, string>();
        public string activePowerPlan;

        public PowerPlanManager()
        {
            readPowerPlans();
        }

        public void applyPowerPlan(string name)
        {
            string guid;
            bool exists = powerPlan.TryGetValue(name, out guid);
            if (exists)
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "powercfg";
                p.StartInfo.Arguments = "-setactive " + guid;
                p.Start();
                activePowerPlan = name;
            }
        }

        private void readPowerPlans()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "powercfg";
            p.StartInfo.Arguments = "-l";
            p.Start();
            while (!p.StandardOutput.EndOfStream)
            {
                string line = p.StandardOutput.ReadLine();
                if (line.StartsWith("Power Scheme GUID"))
                {
                    int s = line.LastIndexOf("(") + 1;
                    string name = line.Substring(s, line.LastIndexOf(")") - s);
                    string guid = line.Split(" ")[3];
                    powerPlan.Add(name, guid);
                    if (line.EndsWith("*")) { activePowerPlan = name; }
                }
            }
            p.WaitForExit();
        }
    }
}
