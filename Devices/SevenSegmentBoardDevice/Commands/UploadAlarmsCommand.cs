using BaseDevice;
using CommandProcessing;
using CommandProcessing.Responces;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Requests;

namespace SevenSegmentBoardDevice.Commands
{
    public class UploadAlarmsCommand : RequestingCommand<UploadFramesRequest, AcceptanceResponce, object>
    {
        public UploadAlarmsCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger) { }

        public override string Name => "Отправка списка будильников";

        public override object GetRequestData()
        {
            var sevenSegmentBoard = _device as SevenSegmentBoard;
            return sevenSegmentBoard.AlarmSchedule;
        }

        public override void ProcessRecievedData(object responceDTO)
        {
        }
    }
}
