using BaseDevice;
using CommandProcessing.Responces;
using ServiceInterfaces;


namespace CommandProcessing.Commands
{
    public class WorkScheduleCommand : RequestingCommand<WorkScheduleResponce, WorkSchedule>
    {
        public WorkScheduleCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger) { }

        public override string Name => "Запрос расписания работы";

        public override object GetRequestData() => 4;

        public override void ProcessRecievedData(WorkSchedule responceDTO)
        {
            _device.WorkSchedule = responceDTO;
        }
    }
}
