using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Hangfire;
using Hangfire.SqlServer;
using Windows.UI.Notifications;

using System.Windows.Threading;

namespace ConsoleHangFireusing
{
    class Program
    {
        static void Main()
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

            using (var server = new BackgroundJobServer())
            {
                BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget"));
                RecurringJob.AddOrUpdate(() => new Program().ShowTemplate(), Cron.Minutely);

                Console.WriteLine("Hangfire Server started. Press any key to exit...");
                Console.ReadKey();
            }
        }

        public void ShowTemplate()
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);

            // Fill in the text elements
            var stringElements = toastXml.GetElementsByTagName("text");
            for (int i = 0; i < stringElements.Length; i++)
            {
                stringElements[i].AppendChild(toastXml.CreateTextNode("Line " + i));
            }

            // Specify the absolute path to an image
            String imagePath = "file:///C:\\Temp\\team.png";
            var imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);
            toast.Activated += ToastActivated;
            toast.Dismissed += ToastDismissed;
            toast.Failed += ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier("TestApplication").Show(toast);
        }
        public void ToastActivated(ToastNotification sender, object e)
        {
            Console.WriteLine("The user activated the toast.");
        }

        private void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs e)
        {
            String outputText = "";
            switch (e.Reason)
            {
                case ToastDismissalReason.ApplicationHidden:
                    outputText = "The app hid the toast using ToastNotifier.Hide";
                    break;
                case ToastDismissalReason.UserCanceled:
                    outputText = "The user dismissed the toast";
                    break;
                case ToastDismissalReason.TimedOut:
                    outputText = "The toast has timed out";
                    break;
            }

            Console.WriteLine(outputText);
        }

        private void ToastFailed(ToastNotification sender, ToastFailedEventArgs e)
        {

            Console.WriteLine("The toast encountered an error.");
        }        
    }
}