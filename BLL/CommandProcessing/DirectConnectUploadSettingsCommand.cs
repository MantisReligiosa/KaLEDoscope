using BaseDevice;
using ServiceInterfaces;

namespace CommandProcessing
{
    public class DirectConnectUploadSettingsCommand : DeviceCommand<Device>
    {
        public DirectConnectUploadSettingsCommand(
            Device device,
            INetworkAgent networkAgent,
            ILogger logger)
            : base(device, networkAgent,   logger)
        {
        }

        public override string Name => "Применение конфигурации";

        public override void Execute()
        {
#warning Перейти к блокам
            _networkAgent.Send(_device.Network.IpAddress, _device.Network.Port, null);
        }
    }
}
