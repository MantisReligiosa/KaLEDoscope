using BaseDevice;
using CommandProcessing;
using CommandProcessing.Responces;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Requests;

namespace SevenSegmentBoardDevice.Commands
{
    public class UploadFramesCommand : RequestingCommand<UploadFramesRequest, AcceptanceResponce, object>
    {
        public UploadFramesCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger) { }

        public override string Name => "Отправка списка фреймов";

        public override object GetRequestData()
        {
            var sevenSegmentBoard = _device as SevenSegmentBoard;
            return sevenSegmentBoard.DisplayFrames;
        }

        public override void ProcessRecievedData(object responceDTO)
        {
        }
    }
}
