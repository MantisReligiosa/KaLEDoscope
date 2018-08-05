using BaseDevice;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;

namespace CommandProcessing.Commands
{
    public class DownloadBrightnessCommand : RequestingCommand<ConfigurationRequest, BrightnessResponce, Brightness>
    {
        public DownloadBrightnessCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "Запрос параметров яркости";

        public override object GetRequestData() => 5;

        public override void ProcessRecievedData(Brightness responceDTO)
        {
            _device.Brightness = responceDTO;
        }
    }
}
