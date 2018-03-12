using TcpExcange;
using Newtonsoft.Json;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Commands.DTO;

namespace SevenSegmentBoardDevice.Commands
{
    public class DirectConnectStartTimer : Command<SevenSegmentBoard, TcpAgent>
    {
        public DirectConnectStartTimer(SevenSegmentBoard board, ILogger logger) : base(board, logger) { }

        public override string Name => "Пуск секундомера";

        public override void Execute()
        {
            var request = new SevenSegmentBoardDeviceRequest
            {
                Start = new object()
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
