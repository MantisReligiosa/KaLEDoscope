using Newtonsoft.Json;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Commands.DTO;

namespace SevenSegmentBoardDevice.Commands
{
    public class DirectConnectResetTimer : DeviceCommand<SevenSegmentBoard>
    {
        public DirectConnectResetTimer(SevenSegmentBoard board, INetworkAgent networkAgent, ILogger logger) 
            : base(board, networkAgent, logger) { }

        public override string Name => "Сброс секундомера";

        public override void Execute()
        {
            var request = new SevenSegmentBoardDeviceRequest
            {
                Reset = new object()
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
