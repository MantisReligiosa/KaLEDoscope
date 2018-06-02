using BaseDevice;
using CommandProcessing.DTO;
using CommandProcessing.Responces;
using ServiceInterfaces;

namespace CommandProcessing.Commands
{
    public class IdentityCommand : RequestingCommand<IdentityResponce, Identity>
    {
        public IdentityCommand(Device device, INetworkAgent networkAgent, ILogger logger)
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
