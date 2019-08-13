using BaseDevice;
using CommandProcessing;
using CommandProcessing.Requests;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Responces;
using ServiceInterfaces;

namespace PixelBoardDevice.Commands
{
    public class DownloadBoardHardwareConfigCommand : RequestingCommand<ConfigurationRequest, BoardHardwareConfigResponce, BoardHardware>
    {
        public DownloadBoardHardwareConfigCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "Запрос типа табло";

        public override object GetRequestData() => 0x27;

        public override void ProcessRecievedData(BoardHardware responceDTO)
        {
            var device = _device as PixelBoard;
            device.Hardware = responceDTO;
            _device = device;
        }
    }
}
