using BaseDevice;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Timers;

namespace CommandProcessing.Commands
{
#warning Нужны тесты
    public class ScanCommand : DeviceCommand<Device>
    {
        private readonly int _port;
        private readonly int _timeout;
        private readonly List<Device> _devices;

        public event Action<List<Device>> OnScanCompleted;
        public override string Name => "Поиск устройств";

        public ScanCommand(INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(null, networkAgent, logger, config)
        {
            _devices = new List<Device>();
            _port = config.ScanPort;
            _timeout = config.ScanPeriod;
        }

        protected override void CommandExecute()
        {
            var request = new ScanRequest();
            _networkAgent.SendBroadcast(_port, request);
            _logger.Debug(this, $"Жду ответы {_timeout} мс");
            var timer = new Timer()
            {
                AutoReset = false,
                Interval = _timeout
            };
            timer.Elapsed += (o, e) =>
            {
                _networkAgent.Close();
                _logger.Info(this, $"Поиск завершен");
                OnScanCompleted?.Invoke(_devices);
            };
            timer.Start();

            StartListen();
        }

        private void StartListen()
        {
            try
            {
                _networkAgent.Listen<ScanResponce, Device>(_port, OnDeviceAnswered);

            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
        }

        private void OnDeviceAnswered(ScanResponce responce)
        {
            var device = responce.Cast();
            _logger.Debug(this, $"Устройство IP{device.Network.IpAddress} откликнулось");
            _devices.Add(device);
            StartListen();
        }
    }
}
