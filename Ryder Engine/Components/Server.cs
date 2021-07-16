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
        public class Listener
        {
            private Mutex m;
            private StreamWriter writer;
            public bool authenticated = false;

            public Listener(NetworkStream stream)
            {
                m = new Mutex();
                writer = new StreamWriter(stream);
                writer.NewLine = "\n";
            }

            public bool sendMsg(string msg)
            {
                if (authenticated)
                {
                    try
                    {
                        m.WaitOne();
                        string length = String.Format("{0,8}", (msg.Length + 1));
                        writer.WriteLine(length);
                        writer.Flush();
                        writer.WriteLine(msg);
                        writer.Flush();
                        m.ReleaseMutex();
                        return true;
                    }
                    catch
                    {
                        m.ReleaseMutex();
                    }
                }
                return false;
            }
        }

        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        private TcpListener listener;

        private List<Listener> listeners = new List<Listener>();
        private Mutex listeners_m = new Mutex();
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
                    try
                    {
                        client = listener.AcceptTcpClient();
                        ThreadPool.QueueUserWorkItem(handleClient, client);
                    }
                    catch { }
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
            NetworkStream stream = client.GetStream();
            Listener listener = new Listener(stream);
            StreamReader reader = new StreamReader(stream);

            // Add Listener to List
            listeners_m.WaitOne();
            listeners.Add(listener);
            listeners_m.ReleaseMutex();

            // Process Listener commands
            while (!_stop && client.Connected)
            {
                try
                {
                    string data = reader.ReadLine();
                    Debug.WriteLine(data);
                    string[] json_request = JsonConvert.DeserializeObject<string[]>(data);

                    switch (json_request[0])
                    {
                        case "authCode":
                            {
                                try
                                {
                                    string psw = json_request[1];
                                    if (Properties.Settings.Default.Password == psw)
                                    {
                                        listener.authenticated = true;
                                        // Notify Client
                                        listener.sendMsg("[\"authenticated\"]");
                                    }
                                }
                                catch { }
                                break;
                            }
                        case "status":
                            {
                                try
                                {
                                    listener.sendMsg("[\"status\"," + systemMonitor.getStatusJSON() + "]");
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

                                    listener.sendMsg("[\"foregroundProcessName\"," + name + "]");
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
                                    listener.sendMsg("[\"foregroundProcessIcon\"," + name + "," + icon + "]");
                                }
                                catch { }
                                break;
                            }
                        case "steamLogin":
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
                        case "steam2fa":
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
            listeners_m.WaitOne();
            listeners.Remove(listener);
            listeners_m.ReleaseMutex();
            stream.Close();
            client.Close();
        }

        public void sendDataToListeners()
        {
            listeners_m.WaitOne();
            foreach (Listener listener in listeners)
            {
                try
                {
                    listener.sendMsg("[\"status\"," + systemMonitor.getStatusJSON() + "]");
                }
                catch { }
            }
            listeners_m.ReleaseMutex();
        }

        public void sendForegroundProcessToListener(object sender, string name)
        {
            Debug.WriteLine("Foreground Process: " + name);
            foreach (Listener listener in listeners)
            {
                try
                {
                    listener.sendMsg("[\"foregroundProcessName\"," + name + "]");
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
