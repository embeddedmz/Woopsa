using System;
using System.Diagnostics;
using System.Threading;
using Woopsa;

namespace WoopsaStressTestClient
{
    class Program
    {
        private static Stopwatch stopWatch = new Stopwatch();

        static void Main(string[] args)
        {
            ManualResetEvent quit = new ManualResetEvent(false);

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                // Tell .NET to not terminate the process
                e.Cancel = true;

                quit.Set();
            };

            string serverUrl = (args.Length > 0) ? args[0] : "http://localhost:80/woopsa/";

            WoopsaClient client = new WoopsaClient(serverUrl);
            Console.WriteLine("Woopsa client created on URL: {0}", serverUrl);

            client.SubscriptionChannel.Subscribe("Temperature", Temperature_Changed);

            // Leave the command prompt open
            /*Console.WriteLine("Press any key to exit...");
            Console.ReadLine();*/

            quit.WaitOne();

            client.Dispose();
        }

        private static void Temperature_Changed(object sender, WoopsaNotificationEventArgs e)
        {
            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            Console.WriteLine(String.Format("Delta T = {0}s - {1}ms", ts.Seconds, ts.Milliseconds));

            //Console.WriteLine("Temperature value : {0}", e.Notification.Value.ToString());

            stopWatch.Restart();
        }
    }
}
