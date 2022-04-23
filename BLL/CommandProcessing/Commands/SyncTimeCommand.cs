using BaseDevice;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;
using System;

namespace CommandProcessing.Commands
{
    public class SyncTimeCommand : RequestingCommand<SyncTimeRequest, AcceptanceResponce, object>
    {
        public SyncTimeCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "Синхронизация времени";

        public override object GetRequestData() => Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds);

        public override void ProcessRecievedData(object responceDTO)
        {
        }
    }
}
