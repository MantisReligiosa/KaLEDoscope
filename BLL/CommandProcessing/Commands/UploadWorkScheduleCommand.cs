using BaseDevice;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;

namespace CommandProcessing.Commands
{
    public class UploadWorkScheduleCommand : RequestingCommand<UploadWorkScheduleRequest, AcceptanceResponce, object>
    {
        public UploadWorkScheduleCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "Отправка расписания работы";

        public override object GetRequestData() => _device.WorkSchedule;

        public override void ProcessRecievedData(object responceDTO)
        {
        }
    }
}
