using BaseDevice;
using CommandProcessing.DTO;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;

namespace CommandProcessing.Commands
{
    public class DownloadIdentityCommand : RequestingCommand<ConfigurationRequest, IdentityResponce, Identity>
    {
        public DownloadIdentityCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger) { }

        public override string Name => "Запрос идентификатора";

        public override object GetRequestData() => 3;

        public override void ProcessRecievedData(Identity responceDTO)
        {
            _device.Id = responceDTO.Id;
            _device.Name = responceDTO.Name;
        }
    }
}
