using BaseDevice;
using CommandProcessing;
using CommandProcessing.Requests;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Responces;
using System.Collections.Generic;

namespace SevenSegmentBoardDevice.Commands
{
    public class DownloadAlarmsCommand : RequestingCommand<ConfigurationRequest, AlarmsResponce, List<Alarm>>
    {
        public DownloadAlarmsCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger) { }

        public override string Name => "Запрос списка будильников";

        public override object GetRequestData() => 0x11;

        public override void ProcessRecievedData(List<Alarm> responceDTO)
        {
            var device = _device as SevenSegmentBoard;
            device.AlarmSchedule = responceDTO;
            _device = device;
        }
    }
}
