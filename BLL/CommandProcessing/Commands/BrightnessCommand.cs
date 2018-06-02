using BaseDevice;
using CommandProcessing.Responces;
using ServiceInterfaces;

namespace CommandProcessing.Commands
{
    public class BrightnessCommand : RequestingCommand<BrightnessResponce, Brightness>
    {
        public BrightnessCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger) { }

        public override string Name => "Запрос параметров яркости";

        public override object GetRequestData() => 5;

        public override void ProcessRecievedData(Brightness responceDTO)
        {
            _device.Brightness = responceDTO;
        }
    }
}
