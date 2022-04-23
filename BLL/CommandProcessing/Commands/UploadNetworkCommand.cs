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
            ApplyChangingPort();
            ApplyChangingIpAddress();
        }

        private void ApplyChangingIpAddress()
        {
            if (!_device.Network.IsIpAddressEdited)
                return;
            _device.Network.ActualIpAddress = _device.Network.EditedIpAddress;
            _device.Network.EditedIpAddress = string.Empty;
        }

        private void ApplyChangingPort()
        {
            if (!_device.Network.IsPortEdited)
                return;
            _device.Network.ActualPort = _device.Network.EditedPort;
            _device.Network.EditedPort = default;
        }
    }
}
