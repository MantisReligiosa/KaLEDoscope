using BaseDevice;
using DeviceBuilding;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandProcessing
{
    public class UdpDeviceScanner
    {
        private readonly ILogger _logger;
        private readonly List<IDeviceBuilder> _deviceBuilders;
        public event Action<List<Device>> OnScanCompleted;

        public UdpDeviceScanner(ILogger logger, List<IDeviceBuilder> deviceBuilders)
        {
            _logger = logger;
            _deviceBuilders = deviceBuilders;
        }

        public void StartSearch()
        {
            var directConnectScanCommand = new DirectConnectScanCommand(_logger);
            directConnectScanCommand.OnScanCompleted += DirectConnectScanCommand_OnScanCompleted;
            directConnectScanCommand.Execute();
        }

        private void DirectConnectScanCommand_OnScanCompleted(List<Device> devices)
        {
            _logger.Info(this, "Распознавание устройств");
            var findedDevices = devices ?? new List<Device>();
            OnScanCompleted?.Invoke(findedDevices.Select(d =>
            {
                var deviceBuilder = _deviceBuilders.FirstOrDefault(builder => builder.Model.Equals(d.Model));
                if (deviceBuilder == null)
                {
                    return d;
                }
                else
                {
                    return deviceBuilder.UpdateCustomSettings(d);
                }
            }
            ).ToList());
            _logger.Info(this, "Распознавание закончено");
        }
    }
}
