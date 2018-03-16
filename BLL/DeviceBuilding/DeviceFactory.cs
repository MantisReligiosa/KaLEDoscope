using System.Linq;
using BaseDevice;
using ServiceInterfaces;
using System.Collections.Generic;

namespace DeviceBuilding
{
    public class DeviceFactory
    {
        private readonly ILogger _logger;

        public DeviceFactory(ILogger logger)
        {
            _logger = logger;
        }

        public List<IDeviceBuilder> Builders { get; set; } = new List<IDeviceBuilder>();

        public Device Customize(object device) => Customize(device as Device);

        public Device Customize(Device device)
        {
            var builder = Builders.FirstOrDefault(b => b.Model.Equals(device.Model));
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
    }
}
