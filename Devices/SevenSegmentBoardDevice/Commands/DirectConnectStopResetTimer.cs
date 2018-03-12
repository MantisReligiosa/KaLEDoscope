using TcpExcange;
using Newtonsoft.Json;
using ServiceInterfaces;
using SevenSegmentBoardDevice;
using SevenSegmentBoardDevice.Commands.DTO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SevenSegmentBoardDevice.Commands
{
    public class DirectConnectStopResetTimer : Command<SevenSegmentBoard, TcpAgent>
    {
        public DirectConnectStopResetTimer(SevenSegmentBoard board, ILogger logger) : base(board, logger) { }

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
