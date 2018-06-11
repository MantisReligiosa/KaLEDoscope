using BaseDevice;
using CommandProcessing;
using CommandProcessing.Responces;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Requests;

namespace SevenSegmentBoardDevice.Commands
{
    public class UploadBoardTypeCommand : RequestingCommand<UploadBoardTypeRequest, AcceptanceResponce, object>
    {
        public UploadBoardTypeCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger) { }

        public override string Name => "Отправка параметров табло";

        public override object GetRequestData()
        {
            var sevenSegmentBoard = _device as SevenSegmentBoard;
            return sevenSegmentBoard.BoardType;
        }

        public override void ProcessRecievedData(object responceDTO)
        {
        }
    }
}
