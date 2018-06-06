using BaseDevice;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;

namespace CommandProcessing.Commands
{
    public class UploadWorkScheduleCommand : RequestingCommand<UploadWorkScheduleRequest, AcceptanceResponce, object>
    {
        public UploadWorkScheduleCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger) { }

        public override string Name => "Отправка расписания работы";

        public override object GetRequestData() => _device.Network;

        public override void ProcessRecievedData(object responceDTO)
        {
        }
    }
}
