using BaseDevice;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;

namespace CommandProcessing.Commands
{
    public class UploadNetworkCommand : RequestingCommand<UploadNetworkRequest, AcceptanceResponce, object>
    {
        public UploadNetworkCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "Отправка сетевых параметров";

        public override object GetRequestData() => _device.Network;

        public override void ProcessRecievedData(object responceDTO)
        {
            _device.Network.ApplyChangingIpAddress();
        }
    }
}
