using BaseDevice;
using ServiceInterfaces;
using System;
using System.Collections.Generic;

namespace DeviceFactory
{
    public class DeviceFactory
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, Func<Device, Device>> _transformations = new Dictionary<string, Func<Device, Device>>();

        public DeviceFactory(ILogger logger)
        {
            _logger = logger;
        }

        public void AddTransformation(string model, Func<Device, Device> transformation)
        {
            Func<Device, Device> safetyTransformation = (device) =>
            {
                return transformation(device ?? new Device());
            };
            _transformations[model] = safetyTransformation;
        }

        public Device Customize(object device) => Customize(device as Device);

        public Device Customize(Device device)
        {
            Func<Device, Device> customize;
            if (_transformations.TryGetValue(device.Model, out customize))
            {
                device = customize(device);
            }
            else
            {
                device.Name = "Неизвестное устройство";
            }
            _logger.Info(this, $"Устройство IP {device.Network.IpAddress} модель {device.Model} распознано как {device.Name}");
            return device;
        }
    }
}
