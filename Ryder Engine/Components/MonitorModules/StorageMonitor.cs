using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Ryder_Engine.Components.MonitorModules
{
    class StorageMonitor
    {
        public struct Drive
        {
            public char letter;
            public float readSpeed;
            public float writeSpeed;
        }

        public Drive[] drives;

        private PerformanceCounter[] diskRead;
        private PerformanceCounter[] diskWrite;

        public StorageMonitor()
        {
            // Disk Activity
            string category = "PhysicalDisk";
            string readCounterName = "Disk Read Bytes/sec";
            string writeCounterName = "Disk Write Bytes/sec";
            // Check Drive types
            HashSet<char> fixedDrives = new HashSet<char>();
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (var drive in drives)
                {
                    if (drive.DriveType == DriveType.Fixed)
                    {
                        fixedDrives.Add(drive.Name[0]);
                    }
                }
            }
            //// Instances (Filter out non fixed drives)
            var cat = new System.Diagnostics.PerformanceCounterCategory(category);
            List<string> instNames = new List<string>();
            {
                List<string> instNames_tmp = new List<string>(cat.GetInstanceNames());
                instNames_tmp.Remove("_Total");
                instNames_tmp.Sort((x, y) => { if (x[x.Length - 2] > y[y.Length - 2]) return 1; else return -1; });
                for (int i = 0; i < instNames_tmp.Count; i++)
                {
                    if (fixedDrives.Contains(instNames_tmp[i][instNames_tmp[i].Length - 2]))
                    {
                        instNames.Add(instNames_tmp[i]);
                    }
                }
            }
            //// Setup
            drives = new Drive[instNames.Count];
            diskRead = new PerformanceCounter[instNames.Count];
            diskWrite = new PerformanceCounter[instNames.Count];
            for (int i = 0; i < instNames.Count; i++)
            {
                drives[i].letter = instNames[i].Substring(instNames[i].Length - 2, 1).ToCharArray()[0];
                diskRead[i] = new PerformanceCounter(category, readCounterName, instNames[i]);
                diskWrite[i] = new PerformanceCounter(category, writeCounterName, instNames[i]);
            }
            // Release Memory
            fixedDrives.Clear(); cat = null;
            GC.Collect();
        }

        public void update()
        {
            for (short i = 0; i < diskRead.Length; i++)
            {
                drives[i].readSpeed = diskRead[i].NextValue() / 1024f / 1024f;
                drives[i].writeSpeed = diskWrite[i].NextValue() / 1024f / 1024f;
            }
        }
    }
}
