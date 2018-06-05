using BaseDevice;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;

namespace CommandProcessing.Commands
{
    public class DownloadNetworkCommand : RequestingCommand<ConfigurationRequest, NetworkResponce, Network>
    {
        public DownloadNetworkCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger) { }

        public override string Name => "Запрос сетевых параметров";

        public override object GetRequestData() => 2;

        public override void ProcessRecievedData(Network responceDTO)
        {
            _device.Network = responceDTO;
        }
    }
}
