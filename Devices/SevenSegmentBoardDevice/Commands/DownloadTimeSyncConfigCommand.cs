using BaseDevice;
using CommandProcessing;
using CommandProcessing.Requests;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Responces;

namespace SevenSegmentBoardDevice.Commands
{
    public class DownloadTimeSyncConfigCommand : RequestingCommand<ConfigurationRequest, TimeSyncParametersResponce, TimeSyncParameters>
    {
        public DownloadTimeSyncConfigCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger) { }

        public override string Name => "Запрос параметров синхронизации времени";

        public override object GetRequestData() => 0x13;

        public override void ProcessRecievedData(TimeSyncParameters responceDTO)
        {
            var device = _device as SevenSegmentBoard;
            device.TimeSyncParameters = responceDTO;
            _device = device;
        }
    }
}
