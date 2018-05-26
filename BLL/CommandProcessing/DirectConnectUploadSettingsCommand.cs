using BaseDevice;
using Newtonsoft.Json;
using ServiceInterfaces;

namespace CommandProcessing
{
    public class DirectConnectUploadSettingsCommand : DeviceCommand<Device>
    {
        public DirectConnectUploadSettingsCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger)
        {
        }

        public override string Name => "Применение конфигурации";

        public override void Execute()
        {
            var request = new DTO.Request
            {
                Device = _device
            };
            var requestString = JsonConvert.SerializeObject(request);
            _logger.Debug(this, $"Запрос к {_device.Network.IpAddress}:{_device.Network.Port} {requestString}");
            _networkAgent.Send(_device.Network.IpAddress, _device.Network.Port, requestString);
        }

        public override void Finally()
        {
            _networkAgent.Close();
        }
    }
}
