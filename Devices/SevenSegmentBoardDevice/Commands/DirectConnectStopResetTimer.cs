using Newtonsoft.Json;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Commands.DTO;

namespace SevenSegmentBoardDevice.Commands
{
    public class DirectConnectStopResetTimer : DeviceCommand<SevenSegmentBoard>
    {
        public DirectConnectStopResetTimer(SevenSegmentBoard board, INetworkAgent networkAgent, ILogger logger)
            : base(board, networkAgent, logger) { }

        public override string Name => "Стоп";

        public override void Execute()
        {
            var request = new SevenSegmentBoardDeviceRequest
            {
                Stop = new object()
            };
            var requestString = JsonConvert.SerializeObject(request);
            _networkAgent.Send(_device.Network.IpAddress, _device.Network.Port, requestString);
        }

        public override void Finally()
        {
            _networkAgent.Close();
        }
    }
}
