using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Ryder_Engine.Components.Tools
{
    static class AudioManager
    {
        static public void switchDeviceTo(string device, int type)
        {
            string nircmd_location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Resources\nircmd\nircmd.exe";
            string args = String.Format(
                "setdefaultsounddevice \"{0}\" {1}",
                device,
                type
            );
            Debug.WriteLine(nircmd_location);
            Debug.WriteLine(args);
            Process nircmd = new Process();
            nircmd.StartInfo.FileName = nircmd_location;
            nircmd.StartInfo.Arguments = args;
            nircmd.StartInfo.CreateNoWindow = true;
            nircmd.Start();
        }
    }
}
