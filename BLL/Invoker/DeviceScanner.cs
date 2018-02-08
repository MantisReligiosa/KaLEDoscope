using DomainData;
using ServiceInterfaces;
using System.Collections.Generic;

namespace CommandProcessing
{
    public class DeviceScanner<TProtocol>
        where TProtocol : IProtocol, new()
    {
        private readonly ILogger _logger;

        public DeviceScanner(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<Device> Search()
        {
            using (var protocol = new TProtocol())
            {
                _logger.Info(this, $"Start scanning {protocol.ToString()}");
                var ip = "192.168.0.71";
                var port = 49000;
                _logger.Debug(this, $"Fake scan found device IP {ip}:{port}");
                return new List<Device>
                {
                    new Device
                    {
                        Name = "Устройство",
                        Network = new Network
                        {
                            IpAddress = ip,
                            Port = port
                        }
                    }
                };
            }
        }
    }
}
