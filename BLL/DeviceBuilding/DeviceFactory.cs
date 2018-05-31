using System.Linq;
using BaseDevice;
using ServiceInterfaces;
using System.Collections.Generic;
using Newtonsoft.Json;
using Extensions;
using System;

namespace DeviceBuilding
{
    public class DeviceFactory
    {
        private readonly ILogger _logger;

        public DeviceFactory(ILogger logger)
        {
            _logger = logger;
        }

        private readonly List<IDeviceBuilder> _builders = new List<IDeviceBuilder>();

        public void AddBuilder(IDeviceBuilder deviceBuilder)
        {
            _builders.Add(deviceBuilder);
        }

        public Device Customize(object device) => Customize(device as Device);

        public Device Customize(Device device)
        {
            var builder = _builders.FirstOrDefault(b => b.Model.Equals(device.Model));
            if (!builder.IsNull())
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

        public Func<Device, INetworkAgent, ILogger, DeviceCommand<Device>> GetDownloadCommands(string model)
        {
            return _builders.FirstOrDefault(b => b.Model.Equals(model)).GetDownloadCommands();
        }

        public IDeviceBuilder GetDeviceBuilder(string model)
        {
            return _builders.FirstOrDefault(b => b.Model.Equals(model));
        }

        public Device DeserializeDevice(string text)
        {
            var device = JsonConvert.DeserializeObject<Device>(text);
            _logger.Debug(this, "Извлечение базовой информации");
            var builder = _builders.FirstOrDefault(b => b.Model.Equals(device.Model));
            if (!builder.IsNull())
            {
                device = builder.DeserializeDevice(text);
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
            _logger.Info(this, $"Устройство ID {device.Id} модель {device.Model} распознано как {device.Name}");
            return device;
        }

        public string SerializeDevice(Device device)
        {
            var builder = _builders.FirstOrDefault(b => b.Model.Equals(device.Model));
            return builder.SerializeDevice(device);
        }

        public IEnumerable<IDeviceBuilder> GetBuilderList()
        {
            return _builders;
        }

        public Device GetNewDevice(string model, int id)
        {
            var deviceBuilder = _builders.FirstOrDefault(b => b.Model.Equals(model));
            return deviceBuilder.UpdateCustomSettings(new Device
            {
                Id = id,
                Name = $"{deviceBuilder.DisplayName}",
                Model = deviceBuilder.Model,
                IsStandaloneConfiguration = true
            });
        }

        public Device FromSerializable(string model, object content)
        {
            var deviceBuilder = _builders.FirstOrDefault(b => b.Model.Equals(model));
            return deviceBuilder.FromSerializable(content);
        }

        public object GetSerializable(Device device)
        {
            var deviceBuilder = _builders.FirstOrDefault(b => b.Model.Equals(device.Model));
            return deviceBuilder.GetSerializable(device);
        }

        public ControlsPack GetControlsPack(Device device, ILogger logger)
        {
            var deviceBuilder = _builders.FirstOrDefault(b => b.Model.Equals(device.Model));
            return deviceBuilder.GetControlsPack(device, logger);
        }
    }
}
