using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Ryder_Engine.Components.Tools
{
    static public class AudioManager
    {
        static public void switchDeviceTo(string device, int type)
        {
            // Locate NirCmd exe and setup arguments
            string exe_location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + 
                @"\Resources\NirCmd\nircmd.exe";
            if (File.Exists(exe_location)) {
                string args = String.Format(
                    "setdefaultsounddevice \"{0}\" {1}",
                    device,
                    type
                );
                // Run NirCmd
                Process p = new Process();
                p.StartInfo.FileName = exe_location;
                p.StartInfo.Arguments = args;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
            }
        }
    }
}
