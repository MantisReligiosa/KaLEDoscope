using BaseDevice;
using CommandProcessing.Commands;
using DeviceBuilding;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandProcessing
{
    public class DeviceScanner
    {
        private readonly IConfig _config;
        private readonly ILogger _logger;
        private readonly DeviceFactory _deviceFactory;
        private readonly Invoker _invoker;
        private readonly INetworkAgent _networkAgent;
        public event Action<List<Device>> OnScanCompleted;

        public DeviceScanner(DeviceFactory deviceFactory, INetworkAgent networkAgent,
            IConfig config, ILogger logger)
        {
            _logger = logger;
            _config = config;
            _deviceFactory = deviceFactory;
            _invoker = new Invoker(_logger);
            _networkAgent = networkAgent;
        }

        public void StartSearch()
        {
            var directConnectScanCommand = new ScanCommand(_networkAgent, _logger, _config);
            directConnectScanCommand.OnScanCompleted += DirectConnectScanCommand_OnScanCompleted;
            directConnectScanCommand.Error += DirectConnectScanCommand_Error;
            _invoker.Invoke(directConnectScanCommand);
        }

        private void DirectConnectScanCommand_Error(object sender, ExceptionEventArgs e)
        {
            _logger.Error(this, "Ошибка при сканировании", e.Exception);
        }

        private void DirectConnectScanCommand_OnScanCompleted(List<Device> devices)
        {
            if (devices.Any())
                _logger.Info(this, "Начинаю распознавание");
            OnScanCompleted?.Invoke(devices.Select(d =>
            {
                return _deviceFactory.Customize(d);
            }
            ).ToList());
            if (devices.Any())
                _logger.Info(this, "Распознавание закончено");
        }
    }
}
