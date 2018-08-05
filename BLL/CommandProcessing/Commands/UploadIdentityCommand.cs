using BaseDevice;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;

namespace CommandProcessing.Commands
{
    public class UploadIdentityCommand : RequestingCommand<UploadIdentityRequest, AcceptanceResponce, object>
    {
        public UploadIdentityCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "Отправка идентификатора";

        public override object GetRequestData() => new DTO.Identity
        {
            Id = _device.Id,
            Name = _device.Name
        };

        public override void ProcessRecievedData(object responceDTO)
        {
        }
    }
}
