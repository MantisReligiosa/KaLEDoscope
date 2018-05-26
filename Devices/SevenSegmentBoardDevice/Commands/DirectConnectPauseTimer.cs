using Newtonsoft.Json;
using ServiceInterfaces;

namespace SevenSegmentBoardDevice.Commands
{
    public class DirectConnectPauseTimer : DeviceCommand<SevenSegmentBoard>
    {
        public DirectConnectPauseTimer(SevenSegmentBoard board, INetworkAgent networkAgent, ILogger logger)
            : base(board, networkAgent, logger) { }

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
