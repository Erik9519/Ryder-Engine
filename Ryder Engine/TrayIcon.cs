using Microsoft.Toolkit.Uwp.Notifications;
using Ryder_Engine.Components;
using Ryder_Engine.Properties;
using Ryder_Engine.Components.Tools;
using Ryder_Engine.Utils;
using System;
using System.IO;
using System.Windows.Forms;

namespace Ryder_Engine
{ 
    class TrayIcon : ApplicationContext
    {
        // The TrayIcon
        NotifyIcon notifyIcon;
        // The interrupt timer
        Timer timer;
        // Custom Components
        SystemMonitor systemMonitor;
        PowerPlanManager powerPlanManager;
        Server server;

        public TrayIcon()
        {
            // Initialize custom components
            powerPlanManager = new PowerPlanManager();
            systemMonitor = new SystemMonitor();
            server = new Server(systemMonitor, powerPlanManager);
            systemMonitor.notificationMonitor.newNotification = server.sendNotificationToListener;
            systemMonitor.foregroundProcessMonitor.newForegroundProcess = server.sendForegroundProcessToListener;
            server.start();
            // Retrieve Application Icon
            MemoryStream ms = new MemoryStream(Resources.Icon);
            System.Drawing.Icon icon = new System.Drawing.Icon(ms);
            ms.Dispose();
            // Create and Initialize NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = icon;
            notifyIcon.Text = "Ryder Engine";
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Close");
            notifyIcon.ContextMenuStrip.Items[0].Click += close;
            notifyIcon.Visible = true;
            // Initialize interrupt timer
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += systemMonitorUpdate;
            timer.Start();
        }

        private void systemMonitorUpdate(object sender, EventArgs e)
        {
            systemMonitor.update();
            server.sendDataToListeners();
        }

        private void close(object sender, EventArgs e)
        {
            this.server.stop();
            this.timer.Stop();
            this.Dispose();
            Application.Exit();
        }
    }
}
