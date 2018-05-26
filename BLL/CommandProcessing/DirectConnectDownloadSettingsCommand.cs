using DeviceBuilding;
using BaseDevice;
using CommandProcessing.DTO;
using ServiceInterfaces;
using System;
using System.Timers;

namespace CommandProcessing
{
    public class DirectConnectDownloadSettingsCommand : DeviceCommand<Device>
    {
        private readonly DeviceFactory _deviceFactory;
        private readonly int _timeout;
        private Timer _timer;

        public override string Name => "Запрос конфигурации";

        public event EventHandler<Device> OnConfigurationDownloaded;

        public DirectConnectDownloadSettingsCommand(
            Device device,
            DeviceFactory deviceFactory,
            INetworkAgent networkAgent,
            IRequestBuilder requestBuilder,
            IResponceProcessor responceProcessor,
            ILogger logger,
            int timeout = 10000)
            : base(device, networkAgent, requestBuilder, responceProcessor, logger)
        {
            _deviceFactory = deviceFactory;
            _timeout = timeout;
        }

        public override void Execute()
        {
#warning перейти к запросам блоками
            var request = new BaseRequest
            {
                GetConfig = new object()
            };
            _requestBuilder.SetRequest(request);
            _networkAgent.Send(_device.Network.IpAddress, _device.Network.Port, _requestBuilder);
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
            OnConfigurationDownloaded?.Invoke(this, _device);
        }

        public override void Finally()
        {
            _networkAgent.Close();
        }
    }
}
