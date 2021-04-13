using Ryder_Engine.Components.MonitorModules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace Ryder_Engine.Components
{
    class SystemMonitor
    {
        public NotificationMonitor notificationMonitor;
        public ForegroundProcessMonitor foregroundProcessMonitor;
        public MSIMonitor msiMonitor;
        public NetworkMonitor networkMonitor;
        public StorageMonitor storageMonitor;
        public FanController fanController;

        public SystemMonitor()
        {
            notificationMonitor = new NotificationMonitor();
            foregroundProcessMonitor = new ForegroundProcessMonitor();
            msiMonitor = new MSIMonitor();
            networkMonitor = new NetworkMonitor();
            storageMonitor = new StorageMonitor();
            fanController = new FanController("COM4");
        }

        public void update()
        {
            msiMonitor.update();
            networkMonitor.update();
            storageMonitor.update();
            fanController.update();
        }

        public SystemStatus getStatus()
        {
            SystemStatus status = new SystemStatus();
            status.cpu = msiMonitor.status.cpu;
            status.gpu = msiMonitor.status.gpu;
            status.ram = msiMonitor.status.ram;
            status.network = networkMonitor.status;
            status.storage = storageMonitor.status;
            status.total_power = status.cpu.power + status.gpu.power;
            status.framerate = msiMonitor.status.framerate;
            status.fanController.ambient = fanController.ambient;
            status.fanController.liquid = fanController.liquid;
            return status;
        }
    }
}
