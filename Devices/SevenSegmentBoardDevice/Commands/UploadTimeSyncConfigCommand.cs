using BaseDevice;
using CommandProcessing;
using CommandProcessing.Responces;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Requests;

namespace SevenSegmentBoardDevice.Commands
{
    public class UploadTimeSyncConfigCommand : RequestingCommand<UploadUploadTimeSyncConfigRequest, AcceptanceResponce, object>
    {
        public UploadTimeSyncConfigCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
                : base(device, networkAgent, logger, config) { }

        public override string Name => "Отправка параметров синхронизации времени";

        public override object GetRequestData()
        {
            var sevenSegmentBoard = _device as SevenSegmentBoard;
            return sevenSegmentBoard.TimeSyncParameters;
        }

        public override void ProcessRecievedData(object responceDTO)
        {
        }
    }
}
