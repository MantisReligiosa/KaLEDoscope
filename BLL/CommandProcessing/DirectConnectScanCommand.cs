using BaseDevice;
using CommandProcessing.DTO;
using Newtonsoft.Json;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Timers;

namespace CommandProcessing
{
    public class DirectConnectScanCommand : DeviceCommand<Device>
    {
        private readonly int _port;
        private readonly int _timeout;
        private readonly List<Device> _devices;

        public event Action<List<Device>> OnScanCompleted;
        public override string Name => "Распознавание устройств";

        public DirectConnectScanCommand(INetworkAgent networkAgent, ILogger logger, int port = 30000, int timeout = 10000)
            : base(null, networkAgent, logger)
        {
            _devices = new List<Device>();
            _port = port;
            _timeout = timeout;
        }

        public override void Execute()
        {
            _logger.Info(this, $"Начало сканирования по UDP. Порт {_port}");
            var request = new Request
            {
                Scan = new object()
            };
            var requestString = JsonConvert.SerializeObject(request);
            _logger.Debug(this, $"Широковещательный запрос: {requestString}");
            _networkAgent.SendBroadcast(_port, requestString);
            _logger.Debug(this, $"Жду ответы {_timeout} мс");

            _networkAgent.Listen(_port, (recieveString) =>
                {
                    var responce = JsonConvert.DeserializeObject<ScanCommandResponce>(recieveString);
                    _logger.Debug(this, $"Ответ: {recieveString}");
                    var device = new Device
                    {
                        Model = responce.DeviceParameters.Model,
                        IsStandaloneConfiguration = false,
                        Network = new Network
                        {
                            IpAddress = responce.NetworkParameters.Address,
                            Port = responce.NetworkParameters.Port
                        }
                    };
                    _devices.Add(device);
                });
            var timer = new Timer()
            {
                AutoReset = false,
                Interval = _timeout
            };
            timer.Elapsed += (o, e) =>
            {
                _networkAgent.Close();
                _logger.Debug(this, $"Завершение сканирования");
                OnScanCompleted?.Invoke(_devices);
            };
            timer.Start();
        }

        public override void Finally()
        {
            _networkAgent.Close();
        }
    }
}
