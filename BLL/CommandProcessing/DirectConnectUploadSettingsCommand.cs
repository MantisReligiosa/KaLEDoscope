using BaseDevice;
using ServiceInterfaces;

namespace CommandProcessing
{
    public class DirectConnectUploadSettingsCommand : DeviceCommand<Device>
    {
        public DirectConnectUploadSettingsCommand(
            Device device,
            INetworkAgent networkAgent,
            IRequestBuilder requestBuilder,
            IResponceProcessor responceProcessor,
            ILogger logger)
            : base(device, networkAgent, requestBuilder, responceProcessor, logger)
        {
        }

        public override string Name => "Применение конфигурации";

        public override void Execute()
        {
            var request = new DTO.BaseRequest
            {
                Device = _device
            };
            _requestBuilder.SetRequest(request);
            _networkAgent.Send(_device.Network.IpAddress, _device.Network.Port, _requestBuilder);
        }

        public override void Finally()
        {
            _networkAgent.Close();
        }
    }
}
