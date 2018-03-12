using BaseDevice;
using DeviceBuilding;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandProcessing
{
    public class DeviceScanner
    {
        private readonly ILogger _logger;
        private readonly DeviceFactory _deviceFactory;
        private readonly Invoker _invoker;
        public event Action<List<Device>> OnScanCompleted;

        public DeviceScanner(ILogger logger, DeviceFactory deviceFactory)
        {
            _logger = logger;
            _deviceFactory = deviceFactory;
            _invoker = new Invoker(_logger);
        }

        public void StartSearch()
        {
            var directConnectScanCommand = new DirectConnectScanCommand(_logger);
            directConnectScanCommand.OnScanCompleted += DirectConnectScanCommand_OnScanCompleted;
            _invoker.Invoke(directConnectScanCommand);
        }

        private void DirectConnectScanCommand_OnScanCompleted(List<Device> devices)
        {
            var findedDevices = devices ?? new List<Device>();
            _logger.Info(this, "Начинаю распознавание");
            OnScanCompleted?.Invoke(findedDevices.Select(d =>
            {
                return _deviceFactory.Customize(d);
            }
            ).ToList());
            _logger.Info(this, "Распознавание закончено");
        }
    }
}
