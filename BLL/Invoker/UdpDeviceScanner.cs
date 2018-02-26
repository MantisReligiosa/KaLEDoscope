using DomainData;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandProcessing
{
    public class UdpDeviceScanner
    {
        private readonly ILogger _logger;
        private readonly Invoker _invoker;

        private DeviceFactory.DeviceFactory _deviceFactory { get; set; }
        public event Action<List<Device>> OnScanCompleted;

        public UdpDeviceScanner(ILogger logger, DeviceFactory.DeviceFactory deviceFactory)
        {
            _logger = logger;
            _invoker = new Invoker();
            _deviceFactory = deviceFactory;
        }

        public void StartSearch()
        {
            var directConnectScanCommand = new DirectConnectScanCommand(_logger);
            _invoker.SetCommand(directConnectScanCommand);
            directConnectScanCommand.OnScanCompleted += DirectConnectScanCommand_OnScanCompleted;
            _invoker.Run();

        }

        private void DirectConnectScanCommand_OnScanCompleted(List<Device> devices)
        {
            _logger.Info(this, "Распознавание устройств");
            var findedDevices = devices ?? new List<Device>();
            OnScanCompleted?.Invoke(findedDevices.Select(d => _deviceFactory.Customize(d)).ToList());
        }
    }
}
