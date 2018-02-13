using DomainData;
using ServiceInterfaces;
using System;
using System.Collections.Generic;

namespace CommandProcessing
{
    public class DeviceScanner<TProtocol>
        where TProtocol : IProtocol, new()
    {
        private readonly ILogger _logger;

        public Dictionary<byte, Func<Device>> DeviceDictionary { get; set; } = new Dictionary<byte, Func<Device>>();

        public DeviceScanner(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<Device> Search()
        {
            using (var protocol = new TProtocol())
            {
                _logger.Info(this, $"Начало сканирования протокола {protocol.ToString()}");
                var ip = "192.168.0.71";
                var port = 49000;
                byte mask = 32;
                var gateway = "192.168.0.1";
                byte deviceTypeId = 1;
                Func<Device> deviceConstructor;
                Device device;

                device = (DeviceDictionary.TryGetValue(deviceTypeId, out deviceConstructor)) ?
                    deviceConstructor() :
                    new Device { Name = "Неизвестное устройство" };
                device.Network = new Network
                {
                    IpAddress = ip,
                    Port = port,
                    SubnetMask = mask,
                    Gateway = gateway
                };
                device.Brightness = new Brightness
                {
                    Mode = Mode.Auto,
                    ManualValue = 5,
                    BrightnessPeriods = new List<BrightnessPeriod>
                    {
                        new BrightnessPeriod
                        {
                            From = new TimeSpan(0,0,0),
                            To = new TimeSpan(8,0,0),
                            Value = 8
                        },
                        new BrightnessPeriod
                        {
                            From = new TimeSpan(8,0,0),
                            To = new TimeSpan(18,0,0),
                            Value = 2
                        },
                        new BrightnessPeriod
                        {
                            From = new TimeSpan(18,0,0),
                            To = new TimeSpan(23,59,59),
                            Value = 8
                        }
                    }
                };
                device.Schedule = new Schedule
                {
                    AroundTheClock = true,
                    FinishTo = new TimeSpan(17, 0, 0),
                    StartFrom = new TimeSpan(8, 0, 0)
                };
                device.Id = 1;
                _logger.Debug(this, $"Принудительное добавление устройства {device} id:{device.Id} {device.Network.IpAddress}:{device.Network.Port}");

                var device1 = new Device
                {
                    Id = 2,
                    Name = "Неизвестное устройство",
                    Network = new Network
                    {
                        IpAddress = "192.168.0.33",
                        Port = port,
                        SubnetMask = mask,
                        Gateway = gateway
                    },
                    Brightness = new Brightness
                    {
                        Mode = Mode.Manual,
                        BrightnessPeriods = new List<BrightnessPeriod>()
                    },
                    Schedule = new Schedule
                    {
                        AroundTheClock = true,
                        FinishTo = new TimeSpan(17, 0, 0),
                        StartFrom = new TimeSpan(8, 0, 0)
                    }
                };
                _logger.Debug(this, $"Принудительное добавление устройства {device1} id:{device1.Id} {device1.Network.IpAddress}:{device1.Network.Port}");

                return new List<Device>
                {
                    device,
                    device1
                };
            }
        }
    }
}
