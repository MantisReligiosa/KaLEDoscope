using Newtonsoft.Json;
using ServiceInterfaces;
using TcpExcange;

namespace SevenSegmentBoardDevice.Commands
{
    public class DirectConnectPauseTimer : Command<SevenSegmentBoard, TcpAgent>
    {
        public DirectConnectPauseTimer(SevenSegmentBoard board, ILogger logger) : base(board, logger) { }

        public override string Name => "Пауза секундомера";

        public override void Execute()
        {
            var request = new DTO.SevenSegmentBoardDeviceRequest
            {
                Pause = new object()
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
