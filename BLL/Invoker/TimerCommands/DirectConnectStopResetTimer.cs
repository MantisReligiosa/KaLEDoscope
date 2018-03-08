using Newtonsoft.Json;
using ServiceInterfaces;
using SevenSegmentBoardDevice;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CommandProcessing.TimerCommands
{
    public class DirectConnectStopResetTimer : Command<SevenSegmentBoard>
    {
        public DirectConnectStopResetTimer(SevenSegmentBoard board, ILogger logger) : base(board, logger) { }

        public override string Name => "Стоп";

        private TcpClient _tcpClient;

        public override void Execute()
        {
            var request = new DTO.SevenSegmentBoardDeviceRequest
            {
                Stop = new object()
            };
            _tcpClient = new TcpClient();
            _tcpClient.Connect(new IPEndPoint(IPAddress.Parse(_device.Network.IpAddress), _device.Network.Port));
            var requestString = JsonConvert.SerializeObject(request);
            var bytes = Encoding.UTF8.GetBytes(requestString);
            _logger.Debug(this, $"Запрос к {_device.Network.IpAddress}:{_device.Network.Port} {requestString}");
            var stream = _tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
        }

        public override void Finally()
        {
            _tcpClient.Close();
        }
    }
}
