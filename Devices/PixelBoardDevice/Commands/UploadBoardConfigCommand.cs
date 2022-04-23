using BaseDevice;
using CommandProcessing;
using CommandProcessing.Responces;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Requests;
using ServiceInterfaces;

namespace PixelBoardDevice.Commands
{
    public class UploadBoardConfigCommand : RequestingCommand<UploadBoardConfigRequest, AcceptanceResponce, object>
    {
        public UploadBoardConfigCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config)
        {
        }

        public override string Name => "Отправка конфигурации табло";

        public override object GetRequestData()
        {
            var pixelBoard = _device as PixelBoard;
            return pixelBoard.BoardSize;
        }

        public override void ProcessRecievedData(object responceDTO)
        {
        }
    }
}
