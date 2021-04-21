using Newtonsoft.Json;
using Ryder_Engine.Components.MonitorModules;
using Ryder_Engine.Components.Tools;
using Ryder_Engine.Forms;
using Ryder_Engine.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Ryder_Engine.Components
{
    class Server
    {
        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        public static HttpListener listener;
        public static HttpClient client;
        public static string url = "http://+:9519/";

        private List<string> listeners = new List<string>();
        private bool _stop = false;
        private SystemMonitor systemMonitor;
        private PowerPlanManager powerPlanManager;
        // netsh http add urlacl url=http://+:9519/ user=administrator listen=yes
        public Server(SystemMonitor systemMonitor, PowerPlanManager powerPlanManager)
        {
            this.systemMonitor = systemMonitor;
            this.powerPlanManager = powerPlanManager;

            listener = new HttpListener();
            listener.Prefixes.Add(url);
            client = new HttpClient();
        }

        public void start()
        {
            _stop = false;
            listener.Start();
            ListenAsync();
        }

        public void stop()
        {
            _stop = true;
            listener.Stop();
        }

        public void sendDataToListeners()
        {
            if (listeners.Count > 0)
            {
                string data = systemMonitor.getStatusJSON();
                foreach (var ip in listeners)
                {
                    try
                    {
                        HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
                        client.PostAsync(ip + "/status", content);
                    }
                    catch { }
                }
            }
        }

        public async void ListenAsync()
        {
            while (!_stop)
            {
                HttpListenerContext ctx = null;
                try
                {
                    ctx = await listener.GetContextAsync();
                }
                catch (HttpListenerException ex)
                {
                    Debug.Print("Error: " + ex.Message);
                }

                if (ctx == null) continue;

                // Process Request
                HttpListenerRequest request = ctx.Request;
                HttpListenerResponse response = ctx.Response;

                string txt;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    txt = reader.ReadToEnd();
                }
                Debug.WriteLine(txt);
                dynamic json_request = JsonConvert.DeserializeObject(txt);
                string txt_request = json_request["request"];

                response.ContentType = "application/json";
                string rsp_txt = "";
                byte[] rsp_buff;
                switch (txt_request)
                {
                    case "status":
                        {
                            rsp_txt = systemMonitor.getStatusJSON();
                            break;
                        }
                    case "subscribe":
                        {
                            string ip = getIPfromContext(ctx);
                            if (!listeners.Contains(ip))
                            {
                                listeners.Add(ip);
                                Debug.WriteLine(ip);
                                rsp_txt = JsonConvert.SerializeObject(ip + " registered");
                            }
                            else
                            {
                                rsp_txt = JsonConvert.SerializeObject(ip + " is already registered");
                            }
                            new Thread(() =>
                            {
                                if (systemMonitor.foregroundProcessMonitor != null)
                                {
                                    HttpContent content = new StringContent(JsonConvert.SerializeObject(systemMonitor.foregroundProcessMonitor.foregroundProcessName), Encoding.UTF8, "application/json");
                                    Debug.WriteLine(systemMonitor.foregroundProcessMonitor.foregroundProcessName);
                                    client.PostAsync(ip + "/foregroundProcessName", content);
                                }
                            }).Start();
                            break;
                        }
                    case "foregroundProcessIcon":
                        {
                            string ip = getIPfromContext(ctx);
                            if (systemMonitor.foregroundProcessMonitor != null)
                            {
                                new Thread(() =>
                                {
                                    // Retrieve process name and icon
                                    Process process = systemMonitor.foregroundProcessMonitor.foregroundProcess;
                                    string name = null;
                                    string icon = convertExeIconToBase64(process);
                                    try
                                    {
                                        name = process != null ? process.ProcessName : null;
                                    }
                                    catch { }
                                    // Attempt to send data back to requester
                                    try
                                    {
                                        HttpContent content = new StringContent(JsonConvert.SerializeObject(new string[] { name, icon }), Encoding.UTF8, "application/json");
                                        client.PostAsync(ip + "/foregroundProcessIcon", content);
                                    }
                                    catch { }
                                }).Start();
                            }
                            rsp_txt = JsonConvert.SerializeObject("OK");
                            break;
                        }
                    case "steamLoginUP":
                        {
                            string ip = getIPfromContext(ctx);
                            new Thread(() =>
                            {
                                Steam_Login steamLoginForm = new Steam_Login(ip);
                                steamLoginForm.sendSteamLogin = this.sendSteamLoginUsernameAndPassword;
                                Application.Run(steamLoginForm);
                                steamLoginForm.Dispose();
                            }).Start();
                            rsp_txt = JsonConvert.SerializeObject("OK");
                            Debug.WriteLine("Steam login data request");
                            break;
                        }
                    case "steamLogin2FA":
                        {
                            string ip = getIPfromContext(ctx);
                            new Thread(() =>
                            {
                                Steam_2FA steam2faForm = new Steam_2FA(ip);
                                steam2faForm.sendSteam2FA = this.sendSteam2FA;
                                Application.Run(steam2faForm);
                                steam2faForm.Dispose();
                            }).Start();
                            rsp_txt = JsonConvert.SerializeObject("OK");
                            Debug.WriteLine("Steam 2FA data request");
                            break;
                        }
                    case "powerPlan":
                        {
                            string plan = json_request["name"];
                            Debug.WriteLine("Power Plan switch: " + plan);
                            powerPlanManager.applyPowerPlan(plan);
                            rsp_txt = JsonConvert.SerializeObject("OK");
                            break;
                        }
                    case "audioProfile":
                        {
                            string playbackDevice = json_request["devices"]["playbackDevice"];
                            string playbackCommunicationDevice = json_request["devices"]["playbackDeviceCommunication"];
                            string recordingDevice = json_request["devices"]["recordingDevice"];
                            AudioManager.switchDeviceTo(playbackDevice, 1);
                            AudioManager.switchDeviceTo(playbackCommunicationDevice, 2);
                            AudioManager.switchDeviceTo(recordingDevice, 1);
                            AudioManager.switchDeviceTo(recordingDevice, 2);
                            rsp_txt = JsonConvert.SerializeObject("OK");
                            break;
                        }
                    default:
                        {
                            rsp_txt = JsonConvert.SerializeObject("Unknown");
                            break;
                        }
                }
                rsp_buff = System.Text.Encoding.UTF8.GetBytes(rsp_txt);

                try
                {
                    response.Headers.Add(HttpResponseHeader.CacheControl, "private, no-store");
                    response.ContentLength64 = rsp_buff.Length;
                    response.OutputStream.Write(rsp_buff, 0, rsp_buff.Length);
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.OutputStream.Close();
                    response.Close();
                }
                catch { }
            }
        }

        public void sendForegroundProcessToListener(object sender, string name)
        {
            foreach (var ip in listeners)
            {
                try
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(name), Encoding.UTF8, "application/json");
                    client.PostAsync(ip + "/foregroundProcessName", content);
                }
                catch { }
            }
        }

        public void sendSteamLoginUsernameAndPassword(object sender, string[] formData)
        {
            try
            {
                string[] data = { formData[0], formData[1] };
                HttpContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                client.PostAsync(formData[2] + "/steamLogin", content);
            }
            catch { }
        }

        public void sendSteam2FA(object sender, string[] formData)
        {
            try
            {
                string data = formData[0];
                HttpContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                client.PostAsync(formData[1] + "/steam2fa", content);
            }
            catch { }
        }

        private string getIPfromContext(HttpListenerContext ctx)
        {
            string ip = ctx.Request.RemoteEndPoint.ToString();
            ip = ip.Substring(0, ip.LastIndexOf(":"));
            ip = "http://" + ip + ":9520";
            return ip;
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
