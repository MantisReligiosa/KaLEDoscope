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
        private readonly IDeviceFactory _deviceFactory;
        private readonly INetworkAgent _networkScanAgent;
        public event Action<List<Device>> OnScanCompleted;

        public DeviceScanner(DeviceFactory deviceFactory, INetworkAgent networkScanAgent, IConfig config, ILogger logger)
        {
            _logger = logger;
            _config = config;
            _deviceFactory = deviceFactory;
            _networkScanAgent = networkScanAgent;
        }

        public void StartSearch()
        {
            var directConnectScanCommand = new ScanCommand(_networkScanAgent, _logger, _config);
            directConnectScanCommand.OnScanCompleted += DirectConnectScanCommand_OnScanCompleted;
            directConnectScanCommand.Error += DirectConnectScanCommand_Error;
            directConnectScanCommand.Execute();
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
                var customizedDevice = _deviceFactory.Customize(d);
                return customizedDevice;
            }
            ).ToList());
            if (devices.Any())
                _logger.Info(this, "Распознавание закончено");
        }
    }
}
