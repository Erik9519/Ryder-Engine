using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace Ryder_Engine.Components.MonitorModules
{
    class NotificationMonitor
    {
        public struct Notification
        {
            public string app;
            public string title;
            public string message;
        }
        UserNotificationListener listener;
        public EventHandler<Notification> newNotification;

        public NotificationMonitor()
        {
            initializeNotificationsListenerAsync();
        }

        private async System.Threading.Tasks.Task initializeNotificationsListenerAsync()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                listener = UserNotificationListener.Current;
                UserNotificationListenerAccessStatus accessStatus = await listener.RequestAccessAsync();
                switch (accessStatus)
                {
                    case UserNotificationListenerAccessStatus.Allowed:
                        break;
                    case UserNotificationListenerAccessStatus.Denied:
                        await initializeNotificationsListenerAsync();
                        return;
                    case UserNotificationListenerAccessStatus.Unspecified:
                        await initializeNotificationsListenerAsync();
                        return;
                }
                try
                {
                    listener.NotificationChanged += notifications_changedAsync;
                    Program.showToast("Initialized");
                }
                catch (Exception e)
                {
                    Program.showToast("Notifications Listener could not be hooked");
                }
            }
            else
            {
                Program.showToast("Notifications Listener unsupported");
            }
        }

        public void notifications_changedAsync(UserNotificationListener sender, Windows.UI.Notifications.UserNotificationChangedEventArgs args)
        {
            try
            {
                if (args.ChangeKind == UserNotificationChangedKind.Added)
                {
                    uint id = args.UserNotificationId;
                    UserNotification not = sender.GetNotification(id);
                    if (not.AppInfo.AppUserModelId == "com.squirrel.Discord.Discord")
                    {
                        var elements = not.Notification.Visual.Bindings[0].GetTextElements();
                        Notification notification = new Notification();
                        notification.app = "Discord";
                        notification.title = elements[0].Text;
                        notification.message = elements[1].Text;
                        newNotification.Invoke(this, notification);
                    }
                }
            }
            catch (Exception e) { }
        }
    }
}
