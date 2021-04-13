using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Toolkit.Uwp.Notifications.NotificationActivator;

namespace Ryder_Engine.Utils
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(INotificationActivationCallback))]
    [Guid("513b273e-9446-492e-81f1-12b1da5566ab"), ComVisible(true)]
    class CustomNotificationActivator : NotificationActivator
    {
        public override void OnActivated(string arguments, NotificationUserInput userInput, string appUserModelId)
        {
            throw new NotImplementedException();
        }
    }
}
