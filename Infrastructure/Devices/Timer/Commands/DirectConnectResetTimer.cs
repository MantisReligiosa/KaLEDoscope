using TcpExcange;
using Newtonsoft.Json;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Commands.DTO;

namespace SevenSegmentBoardDevice.Commands
{
    public class DirectConnectResetTimer : Command<SevenSegmentBoard, TcpAgent>
    {
        public DirectConnectResetTimer(SevenSegmentBoard board, ILogger logger) : base(board, logger) { }

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
