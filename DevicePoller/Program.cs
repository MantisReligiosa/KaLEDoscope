using BaseDevice;
using CommandProcessing;
using Configuration;
using DeviceBuilding;
using DevicePoller.Properties;
using PixelBoardDevice;
using ServiceInterfaces;
using SevenSegmentBoardDevice;
using System;
using TcpExcange;

namespace DevicePoller
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = new Logger();
            INetworkAgent networkAgent = new TcpAgent();
            IConfig _config = Config.GetConfig();

            var deviceFactory = new DeviceFactory(logger);
            deviceFactory.AddBuilder(new PixelDeviceBuilder());
            deviceFactory.AddBuilder(new SevenSegmentDeviceBuilder());
            var service = new ConfigurationService(networkAgent, deviceFactory, logger, _config);
            var device = new Device
            {
                Model = Settings.Default.Model,
                Id = Settings.Default.DeviceId,
                Network = new Network
                {
                    ActualPort = Settings.Default.Port,
                    ActualIpAddress = Settings.Default.IP
                }
            };
            service.DownloadSettings(device);
        }
    }

    class Logger : ILogger
    {
        public event EventHandler<string> DebugRaised;
        public event EventHandler<string> InfoRaised;
        public event EventHandler<string> WarnRaised;
        public event EventHandler<string> ErrorRaised;

        public void Debug(object sender, string message)
        {
            Console.WriteLine($"[DEBUG]: {message}");
        }

        public void Error(object sender, string message)
        {
            Console.WriteLine($"[ERROR]: {message}");
        }

        public void Error(object sender, string message, Exception exception)
        {
            Console.WriteLine($"[DEBUG]: {message}. Exception: {exception.Message}");
        }

        public void Info(object sender, string message)
        {
            Console.WriteLine($"[INFO]: {message}");
        }

        public void Warn(object sender, string message)
        {
            Console.WriteLine($"[WARN]: {message}");
        }
    }
}
