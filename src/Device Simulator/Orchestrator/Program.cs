using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Orchestrator
{
    class Program
    {
        static RegistryManager registryManager;
        //static string connectionString = "HostName=hub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=";

        static void Main(string[] args)
        {
            int deviceCount = Convert.ToInt16(args[0]);
            registryManager = RegistryManager.CreateFromConnectionString(args[1]);

            //string str_directory = Environment.CurrentDirectory.ToString();
            for (int i = 0; i < deviceCount; i++)
            {
                string myDevice = String.Format("device00{0}", i);

                Task<string> callTask = Task.Run(() => AddDeviceAsync(myDevice));
                callTask.Wait();
                string deviceKey = callTask.Result;

                var proc = new Process();
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.FileName = "..\\..\\..\\SimulatedDevice\\bin\\Debug\\DeviceSimulatorStd.exe";
                proc.StartInfo.Arguments = String.Format("syncHub.azure-devices.net {0} {1}", myDevice, deviceKey);
                proc.Start();
            }
            Console.WriteLine("All set");
            Console.ReadLine();
        }

        private static async Task<string> AddDeviceAsync(string deviceName)
        {
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceName));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceName);
            }
            Console.WriteLine("Generated new device named: {0}", deviceName);

            return device.Authentication.SymmetricKey.PrimaryKey;
        }
    }
}
