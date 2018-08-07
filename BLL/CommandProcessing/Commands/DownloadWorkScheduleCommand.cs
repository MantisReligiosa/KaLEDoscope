using BaseDevice;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;

namespace CommandProcessing.Commands
{
    public class DownloadWorkScheduleCommand : RequestingCommand<ConfigurationRequest, WorkScheduleResponce, WorkSchedule>
    {
        public DownloadWorkScheduleCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "Запрос расписания работы";

        public override object GetRequestData() => 4;

        public override void ProcessRecievedData(WorkSchedule responceDTO)
        {
            _device.WorkSchedule = responceDTO;
        }
    }
}
