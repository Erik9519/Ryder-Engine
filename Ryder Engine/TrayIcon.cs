﻿using Ryder_Engine.Components;
using Ryder_Engine.Properties;
using Ryder_Engine.Components.Tools;
using System;
using System.IO;
using System.Windows.Forms;
using Ryder_Engine.Forms;

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
        MonitorManager monitorManager;
        Server server;

        // Forms
        Forms.JSONViewer jsonViewer;
        Forms.App_Launcher appLauncher;
        Forms.Settings settings;

        public TrayIcon()
        {
            // Initialize custom components
            powerPlanManager = new PowerPlanManager();
            monitorManager = new MonitorManager();
            systemMonitor = new SystemMonitor();
            server = new Server(systemMonitor, powerPlanManager, monitorManager);
            systemMonitor.newForegroundProcess = server.sendForegroundProcessToListener;
            server.start();
            systemMonitor.applySettings();
            // Retrieve Application Icon
            MemoryStream ms = new MemoryStream(Resources.Icon);
            System.Drawing.Icon icon = new System.Drawing.Icon(ms);
            ms.Dispose();
            // Create and Initialize NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = icon;
            notifyIcon.Text = "Ryder Engine";
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Data Viewer");
            notifyIcon.ContextMenuStrip.Items[0].Click += openDataViewer;
            notifyIcon.ContextMenuStrip.Items.Add("App Launcher");
            notifyIcon.ContextMenuStrip.Items[1].Click += openAppLauncher;
            notifyIcon.ContextMenuStrip.Items.Add("Settings");
            notifyIcon.ContextMenuStrip.Items[2].Click += openSettings;
            notifyIcon.ContextMenuStrip.Items.Add("Close");
            notifyIcon.ContextMenuStrip.Items[3].Click += close;
            notifyIcon.Visible = true;
            // Initialize interrupt timer
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += systemMonitorUpdate;
            timer.Start();
        }

        private void openDataViewer(object sender, EventArgs e)
        {
            if (jsonViewer == null || jsonViewer.IsDisposed)
            {
                jsonViewer = new Forms.JSONViewer(systemMonitor);
                jsonViewer.Show();
            }
            else
            {
                jsonViewer.BringToFront();
            }
        }

        private void openAppLauncher(object sender, EventArgs e)
        {
            if (appLauncher == null || appLauncher.IsDisposed)
            {
                appLauncher = new Forms.App_Launcher();
                appLauncher.appLauncherUpdated = server.notifyListenersAppLauncherUpdate;
                appLauncher.Show();
            }
            else
            {
                appLauncher.BringToFront();
            }
        }

        private void openSettings(object sender, EventArgs e)
        {
            if (settings == null || settings.IsDisposed)
            {
                settings = new Forms.Settings(systemMonitor);
                settings.Show();
            } else
            {
                settings.BringToFront();
            }
        }

        private void systemMonitorUpdate(object sender, EventArgs e)
        {
            systemMonitor.update();
            server.sendDataToListeners();
            if (!(jsonViewer == null || jsonViewer.IsDisposed))
            {
                jsonViewer.updateData();
            }
        }

        private void close(object sender, EventArgs e)
        {
            this.server.stop();
            this.timer.Stop();
            this.notifyIcon.Dispose();
            this.Dispose();
            Application.Exit();
        }
    }
}
