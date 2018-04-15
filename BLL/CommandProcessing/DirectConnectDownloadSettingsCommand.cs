using TcpExcange;
using DeviceBuilding;
using BaseDevice;
using CommandProcessing.DTO;
using Newtonsoft.Json;
using ServiceInterfaces;
using System;
using System.Timers;

namespace CommandProcessing
{
    public class DirectConnectDownloadSettingsCommand : Command<Device, TcpAgent>
    {
        private readonly DeviceFactory _deviceFactory;
        private readonly int _timeout;
        private Timer _timer;

        public override string Name => "Запрос конфигурации";

        public event EventHandler<Device> OnConfigurationDownloaded;

        public DirectConnectDownloadSettingsCommand(Device device,
            DeviceFactory deviceFactory, ILogger logger, int timeout = 10000) : base(device, logger)
        {
            _deviceFactory = deviceFactory;
            _timeout = timeout;
        }

        public override void Execute()
        {
            var requestString = JsonConvert.SerializeObject(new Request
            {
                GetConfig = new object()
            });
            _networkAgent.Send(_device.Network.IpAddress, _device.Network.Port, requestString);
            _logger.Debug(this, $"Жду ответ {_timeout} мс");
            _networkAgent.Listen(_device.Network.Port, (recieveString) =>
            {
                var d = _deviceFactory.DeserializeDevice(recieveString);
                _device = _deviceFactory.Customize(d);
                _timer.Stop();
                EndCommand();
            });

            _timer = new Timer()
            {
                AutoReset = false,
                Interval = _timeout
            };
            _timer.Elapsed += (o, e) =>
            {
                EndCommand();
            };
            _timer.Start();
        }

        private void EndCommand()
        {
            _networkAgent.Close();
            _logger.Debug(this, $"Завершение запроса конфигурации");
            OnConfigurationDownloaded?.Invoke(this,_device);
        }

        public override void Finally()
        {
            _networkAgent.Close();
        }
    }
}
