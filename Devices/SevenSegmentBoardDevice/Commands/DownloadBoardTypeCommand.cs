using BaseDevice;
using CommandProcessing;
using CommandProcessing.Requests;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Responces;

namespace SevenSegmentBoardDevice.Commands
{
    public class DownloadBoardTypeCommand : RequestingCommand<ConfigurationRequest, BoardTypeResponce, BoardType>
    {
        public DownloadBoardTypeCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "Запрос параметров табло";

        public override object GetRequestData() => 0x10;

        public override void ProcessRecievedData(BoardType responceDTO)
        {
            var device = _device as SevenSegmentBoard;
            device.BoardType = responceDTO;
            _device = device;
        }
    }
}
