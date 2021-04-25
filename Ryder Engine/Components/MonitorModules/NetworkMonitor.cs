using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Ryder_Engine.Components.MonitorModules
{
    public class NetworkMonitor
    {
        public struct Status
        {
            public float downloadSpeed;
            public float uploadSpeed;
        }

        public Status status;

        private Stopwatch stopwatch = new Stopwatch();
        private NetworkInterface[] interfaces;
        private long[] interfaces_received_bytes, interfaces_sent_bytes;
        private long last_interface_update = -1;

        public NetworkMonitor()
        {
            stopwatch.Start();
            interfaces = NetworkInterface.GetAllNetworkInterfaces();
            interfaces_sent_bytes = new long[interfaces.Length];
            interfaces_received_bytes = new long[interfaces.Length];
        }

        public void update()
        {
            status.downloadSpeed = 0;
            status.uploadSpeed = 0;
            for (int i = 0; i < interfaces.Length; i++)
            {
                long val_d, val_u;
                try { val_d = interfaces[i].GetIPv4Statistics().BytesReceived; } catch { val_d = 0; }
                try { val_u = interfaces[i].GetIPv4Statistics().BytesSent; } catch { val_u = 0; }

                status.downloadSpeed += val_d - interfaces_received_bytes[i];
                status.uploadSpeed += val_u - interfaces_sent_bytes[i];

                interfaces_received_bytes[i] = val_d;
                interfaces_sent_bytes[i] = val_u;
            }
            if (last_interface_update != -1)
            {
                status.downloadSpeed /= 1024f;
                status.uploadSpeed /= 1024f;
                status.downloadSpeed *= 1000f / (stopwatch.ElapsedMilliseconds - last_interface_update);
            }
            else
            {
                status.downloadSpeed = 0;
                status.uploadSpeed = 0;
            }
            last_interface_update = stopwatch.ElapsedMilliseconds;
        }
    }
}
