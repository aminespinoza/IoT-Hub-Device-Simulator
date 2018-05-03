using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;

namespace SimulatedDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Sorry, no parameters");
            }
            else
            {
                SendTelemetryData(args[0], args[1], args[2]);
            }
            Console.ReadLine();
        }

        private static async void SendTelemetryData(string iotHubUri, string deviceId, string deviceKey)
        {
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));

            double avgTemperature = 21;
            double avgHumidity = 35;
            double avgPower = 50;
            Random rand = new Random();

            while (true)
            {
                double humidity = Math.Round((avgHumidity + rand.NextDouble() * 4 - 2), 2);
                double temperature = Math.Round((avgTemperature + rand.NextDouble() * 4 - 2), 2);
                double power = Math.Round((avgPower + rand.NextDouble()), 2);

                var telemetryDataPoint = new
                {
                    deviceId = deviceId,
                    humidity = humidity,
                    temperature = temperature,
                    power = power,
                    date = DateTime.Now
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Thread.Sleep(1000);
                Console.WriteLine(messageString);
            }
        }
    }
}
