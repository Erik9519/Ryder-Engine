using MSIAfterburner;
using System;
using System.Diagnostics;
using System.Management;

namespace Ryder_Engine.Components.MonitorModules
{
    class MSIMonitor
    {
        public struct Info
        {
            public struct CPU
            {
                public string name;
                public UInt16 threadCount;
                public uint maxPower;
            }
            public struct GPU
            {
                public string name;
                public uint vram;
                public uint maxPower;
                public uint refreshRate;
            }
            public struct RAM
            {
                public long capacity;
            }
            public CPU cpu;
            public GPU gpu;
            public RAM ram;
        }
        public struct Status
        {
            public struct CPU
            {
                public uint clock;
                public short temperature;
                public ushort load;
                public ushort maxThreadLoad;
                public uint power;
            };
            public struct GPU
            {
                public uint coreClock;
                public short temperature;
                public ushort coreLoad;
                public uint memClock;
                public uint power;
                public uint memUsage;
                public bool tempLimit;
                public bool powerLimit;
                public bool voltageLimit;
                public bool noLoadLimit;
            };
            public struct RAM
            {
                public long usage;
            };

            public CPU cpu;
            public GPU gpu;
            public RAM ram;
            public short framerate;
        };

        public Info info;
        public Status status;

        private AfterBurner msi;
        private int[] metricsIndeces;
        private int numberOfMetrics = -1;

        public MSIMonitor()
        {
            msi = new AfterBurner();

            info.cpu.threadCount = UInt16.Parse(getManagementObject("root\\CIMV2", "SELECT * FROM Win32_Processor", "ThreadCount"));
            metricsIndeces = new int[16 + info.cpu.threadCount];
        }

        public void update()
        {
            float[] values = msi.getSensorsReadings();

            if (numberOfMetrics == -1 || values.Length != numberOfMetrics)
            {
                Sensor[] sensors = msi.getSensors();
                for (int i = 0; i < sensors.Length; i++)
                {
                    switch (sensors[i].name)
                    {
                        case "CPU clock": metricsIndeces[0] = i; break;
                        case "CPU temperature": metricsIndeces[1] = i; break;
                        case "CPU usage": metricsIndeces[2] = i; break;
                        case "CPU power": metricsIndeces[3] = i; break;
                        case "Core clock": metricsIndeces[4] = i; break;
                        case "GPU temperature": metricsIndeces[5] = i; break;
                        case "GPU usage": metricsIndeces[6] = i; break;
                        case "Memory clock": metricsIndeces[7] = i; break;
                        case "Power": metricsIndeces[8] = i; break;
                        case "Memory usage": metricsIndeces[9] = i; break;
                        case "RAM usage": metricsIndeces[10] = i; break;
                        case "Framerate": metricsIndeces[11] = i; break;
                        case "Temp limit": metricsIndeces[12] = i; break;
                        case "Power limit": metricsIndeces[13] = i; break;
                        case "Voltage limit": metricsIndeces[14] = i; break;
                        case "No load limit": metricsIndeces[15] = i; break;
                        default:
                            string[] split = sensors[i].name.Split(' ');
                            if (split.Length == 2)
                            {
                                if (split[0].Contains("CPU"))
                                {
                                    split[0] = split[0].Substring(3);
                                    metricsIndeces[15 + Int32.Parse(split[0])] = i;
                                }
                            }
                            break;
                    }
                }
                numberOfMetrics = sensors.Length;
            }

            if (numberOfMetrics >= 16)
            {
                status.cpu.clock = (uint)values[metricsIndeces[0]];
                status.cpu.temperature = (short)values[metricsIndeces[1]];
                status.cpu.load = (ushort)values[metricsIndeces[2]];
                ushort maxThreadLoad = 0;
                for (short i = 0; i < info.cpu.threadCount; i++)
                {
                    maxThreadLoad = (ushort)Math.Max(maxThreadLoad, values[metricsIndeces[12 + i]]);
                }
                status.cpu.maxThreadLoad = maxThreadLoad;
                status.cpu.power = (uint)values[metricsIndeces[3]];
                status.gpu.coreClock = (uint)values[metricsIndeces[4]];
                status.gpu.temperature = (short)values[metricsIndeces[5]];
                status.gpu.coreLoad = (ushort)values[metricsIndeces[6]];
                status.gpu.memClock = (uint)values[metricsIndeces[7]];
                status.gpu.power = (uint)values[metricsIndeces[8]];
                status.gpu.memUsage = (uint)values[metricsIndeces[9]];
                status.gpu.tempLimit = values[metricsIndeces[12]] > 0 ? true : false;
                status.gpu.powerLimit = values[metricsIndeces[13]] > 0 ? true : false;
                status.gpu.voltageLimit = values[metricsIndeces[14]] > 0 ? true : false;
                status.gpu.noLoadLimit = values[metricsIndeces[15]] > 0 ? true : false;
                status.ram.usage = (long)values[metricsIndeces[10]];
                status.framerate = (short)values[metricsIndeces[11]];
            }
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
