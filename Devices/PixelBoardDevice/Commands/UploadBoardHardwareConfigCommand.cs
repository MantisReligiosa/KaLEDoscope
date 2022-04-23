using BaseDevice;
using CommandProcessing;
using CommandProcessing.Responces;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Requests;
using ServiceInterfaces;

namespace PixelBoardDevice.Commands
{
    public class UploadBoardHardwareConfigCommand : RequestingCommand<UploadBoardHardwareConfigRequest, AcceptanceResponce, object>
    {
        public UploadBoardHardwareConfigCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "Отправка типа табло";

        public override object GetRequestData()
        {
            var pixelBoard = Device as PixelBoard;
            return pixelBoard.Hardware;
        }

        public override void ProcessRecievedData(object responceDTO)
        {
        }
    }
}
