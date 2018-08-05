using BaseDevice;
using CommandProcessing;
using CommandProcessing.Requests;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Responces;
using ServiceInterfaces;

namespace PixelBoardDevice.Commands
{
    public class DownloadBoardConfigCommand : RequestingCommand<ConfigurationRequest, BoardConfigResponce, BoardSize>
    {
        public DownloadBoardConfigCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "Запрос параметров табло";

        public override object GetRequestData() => 0x20;

        public override void ProcessRecievedData(BoardSize responceDTO)
        {
            var device = _device as PixelBoard;
            device.BoardSize = responceDTO;
            _device = device;
        }
    }
}
