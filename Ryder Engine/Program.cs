using Microsoft.Toolkit.Uwp.Notifications;
using Ryder_Engine.Utils;
using System;
using System.Windows.Forms;
using Windows.UI.Notifications;

namespace Ryder_Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            DesktopNotificationManagerCompat.RegisterAumidAndComServer<CustomNotificationActivator>("RyderEngine");
            DesktopNotificationManagerCompat.RegisterActivator<CustomNotificationActivator>();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TrayIcon());
        }

        static public void showToast(string txt)
        {
            ToastContent toastContent = new ToastContentBuilder()
                .AddText(txt)
                .SetToastDuration(ToastDuration.Short)
                .GetToastContent();
            DesktopNotificationManagerCompat.CreateToastNotifier().Show(new ToastNotification(toastContent.GetXml()));
        }
    }
}
