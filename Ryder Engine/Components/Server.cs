using Newtonsoft.Json;
using Ryder_Engine.Components.Tools;
using Ryder_Engine.Forms;
using Ryder_Engine.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ryder_Engine.Components
{
    public class Server
    {
        public struct Listener
        {
            public Mutex m;
            public StreamWriter writer;
        }

        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        private TcpListener listener;

        private List<Listener> listeners = new List<Listener>();
        private bool _stop = false;
        private SystemMonitor systemMonitor;
        private PowerPlanManager powerPlanManager;
        // netsh http add urlacl url=http://+:9519/ user=administrator listen=yes
        public Server(SystemMonitor systemMonitor, PowerPlanManager powerPlanManager)
        {
            this.systemMonitor = systemMonitor;
            this.powerPlanManager = powerPlanManager;

            this.listener = new TcpListener(IPAddress.Any, 9519);
        }

        public void start()
        {
            _stop = false;
            listener.Start();
            new Thread(() =>
            {
                TcpClient client;
                while (!_stop)
                {
                    client = listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(handleClient, client);
                }
            }).Start();
        }

        public void stop()
        {
            _stop = true;
            listener.Stop();
        }

        public void handleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            Listener listener = new Listener();
            listener.m = new Mutex();

            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            listener.writer = new StreamWriter(stream);
            listener.writer.NewLine = "\n";
            listeners.Add(listener);

            while (!_stop && client.Connected)
            {
                try
                {
                    string data = reader.ReadLine();
                    Debug.WriteLine(data);
                    string[] json_request = JsonConvert.DeserializeObject<string[]>(data);

                    switch (json_request[0])
                    {
                        case "status":
                            {
                                try
                                {
                                    listener.m.WaitOne();
                                    listener.writer.WriteLine("[\"status\"," + systemMonitor.getStatusJSON() + "]");
                                    listener.writer.Flush();
                                    listener.m.ReleaseMutex();
                                }
                                catch { }
                                break;
                            }
                        case "foregroundProcessName":
                            {
                                try
                                {
                                    Process process = systemMonitor.foregroundProcessMonitor.foregroundProcess;
                                    string name = "null";
                                    name = process != null ? ("\"" + process.ProcessName + "\"") : null;

                                    listener.m.WaitOne();
                                    listener.writer.WriteLine("[\"foregroundProcessName\"," + name + "]");
                                    listener.writer.Flush();
                                    listener.m.ReleaseMutex();
                                }
                                catch { }
                                break;
                            }
                        case "foregroundProcessIcon":
                            {
                                // Retrieve process name and icon
                                Process process = systemMonitor.foregroundProcessMonitor.foregroundProcess;
                                string name = "null";
                                string icon = "null";
                                try
                                {
                                    name = process != null ? ("\"" + process.ProcessName + "\"") : null;
                                    string icon_t = convertExeIconToBase64(process);
                                    if (icon_t != null) icon = "\"" + icon_t + "\"";
                                }
                                catch { }
                                // Attempt to send data back to requester
                                try
                                {
                                    Debug.WriteLine("Foreground Process Icon: " + name);
                                    listener.m.WaitOne();
                                    listener.writer.WriteLine("[\"foregroundProcessIcon\"," + name + "," + icon + "]");
                                    listener.writer.Flush();
                                    listener.m.ReleaseMutex();
                                }
                                catch { }
                                break;
                            }
                        case "steamLoginUP":
                            {
                                new Task(() =>
                                {
                                    Steam_Login steamLoginForm = new Steam_Login(listener);
                                    Application.Run(steamLoginForm);
                                    steamLoginForm.Dispose();
                                }).Start();
                                Debug.WriteLine("Steam login data request");
                                break;
                            }
                        case "steamLogin2FA":
                            {
                                new Task(() =>
                                {
                                    Steam_2FA steam2faForm = new Steam_2FA(listener);
                                    Application.Run(steam2faForm);
                                    steam2faForm.Dispose();
                                }).Start();
                                Debug.WriteLine("Steam 2FA data request");
                                break;
                            }
                        case "powerPlan":
                            {
                                powerPlanManager.applyPowerPlan(json_request[1]);
                                break;
                            }
                        case "audioProfile":
                            {
                                AudioManager.switchDeviceTo(json_request[1], 1);
                                AudioManager.switchDeviceTo(json_request[2], 2);
                                AudioManager.switchDeviceTo(json_request[3], 1);
                                AudioManager.switchDeviceTo(json_request[3], 2);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                catch { }
            }

            listeners.Remove(listener);
            stream.Close();
            client.Close();
        }

        public void sendDataToListeners()
        {
            foreach (Listener listener in listeners)
            {
                try
                {
                    listener.m.WaitOne();
                    listener.writer.WriteLine("[\"status\"," + systemMonitor.getStatusJSON() + "]");
                    listener.writer.Flush();
                    listener.m.ReleaseMutex();
                }
                catch { }
            }
        }

        public void sendForegroundProcessToListener(object sender, string name)
        {
            Debug.WriteLine("Foreground Process: " + name);
            foreach (Listener listener in listeners)
            {
                try
                {
                    listener.m.WaitOne();
                    listener.writer.WriteLine("[\"foregroundProcessName\"," + name + "]");
                    listener.writer.Flush();
                    listener.m.ReleaseMutex();
                }
                catch { }
            }
        }

        private string convertExeIconToBase64(Process p)
        {
            try
            {
                if (p != null)
                {
                    string filename = p.MainModule.FileName;
                    Debug.WriteLine("Application: " + p.MainModule.FileName);
                    IntPtr hIcon = IconExtractor.GetJumboIcon(IconExtractor.GetIconIndex(filename));
                    // Extract Icon
                    string result;
                    ImageConverter converter = new ImageConverter();
                    using (Bitmap ico = ((Icon)Icon.FromHandle(hIcon).Clone()).ToBitmap())
                    {
                        Bitmap bitmap = IconExtractor.ClipToCircle(ico);
                        result = Convert.ToBase64String((byte[])converter.ConvertTo(bitmap, typeof(byte[])));
                        bitmap.Dispose();
                    }
                    IconExtractor.Shell32.DestroyIcon(hIcon); // Cleanup
                    GC.Collect();
                    EmptyWorkingSet(Process.GetCurrentProcess().Handle);
                    return result;
                }
            }
            catch (Exception e) {
                Debug.WriteLine("Exception: " + e.Message);
            }
            return null;
        }
    }
}
