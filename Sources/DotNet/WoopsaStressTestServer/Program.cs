using System;
using System.Net.Sockets;
using System.Threading;
using Woopsa;

namespace WoopsaStressTestServer
{
    public class WeatherStation
    {
        public double Temperature { get; set; }

        public double Sensitivity { get; set; }

        public string GetWeatherAtDate(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "cloudy";
                default:
                    return "sunny";
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int updateFrequencyMs = (args.Length > 0) ? Convert.ToInt32(args[0]) : 250;
                Console.WriteLine("Update frequency = {0}", updateFrequencyMs);

                CancellationTokenSource cts = new CancellationTokenSource();

                Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
                {
                    // Tell .NET to not terminate the process
                    e.Cancel = true;

                    cts.Cancel();
                };

                WeatherStation station = new WeatherStation();
                using (WoopsaServer woopsaServer = new WoopsaServer(station, 5000))
                {
                    Random randomTemperature = new Random();

                    while (!cts.Token.IsCancellationRequested)
                    {
                        station.Temperature = randomTemperature.Next(0, 1000);
                        Thread.Sleep(updateFrequencyMs);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e?.Message ?? "");
            }
        }
    }
}
