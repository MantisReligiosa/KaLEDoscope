using BaseDevice;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;

namespace CommandProcessing.Commands
{
    public class UploadBrightnessCommand : RequestingCommand<UploadBrightnessRequest, AcceptanceResponce, object>
    {
        public UploadBrightnessCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "Отправка настройки яркости";

        public override object GetRequestData() => _device.Brightness;

        public override void ProcessRecievedData(object responceDTO)
        {
        }
    }
}
