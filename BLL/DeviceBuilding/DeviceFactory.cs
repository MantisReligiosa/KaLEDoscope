using System.Linq;
using BaseDevice;
using DeviceBuilding;
using ServiceInterfaces;
using System.Collections.Generic;
using System;

namespace DeviceBuilding
{
    public class DeviceFactory
    {
        private readonly ILogger _logger;
        private readonly List<IDeviceBuilder> _builders = new List<IDeviceBuilder>();

        public DeviceFactory(ILogger logger)
        {
            _logger = logger;
        }

        public void AddBuilder(IDeviceBuilder deviceBuilder)
        {
            _builders.Add(deviceBuilder);
        }

        public Device Customize(object device) => Customize(device as Device);

        public Device Customize(Device device)
        {
            var builder = _builders.FirstOrDefault(b => b.Model.Equals(device.Model));
            if (builder != null)
            {
                device = builder.UpdateCustomSettings(device);
            }
            else
            {
                device.Name = "Неизвестное устройство";
                device.Brightness = new Brightness
                {
                    BrightnessPeriods = new List<BrightnessPeriod>()
                };
                device.WorkSchedule = new WorkSchedule
                {
                };
            }
            _logger.Info(this, $"Устройство IP {device.Network.IpAddress} модель {device.Model} распознано как {device.Name}");
            return device;
        }

        public IDeviceBuilder GetBuilder(string deviceModel)
        {
            return _builders.FirstOrDefault(b => b.Model.Equals(deviceModel));
        }
    }
}
