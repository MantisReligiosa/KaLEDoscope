using BaseDevice;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Requests;
using ServiceInterfaces;
using System.Collections.Generic;

namespace PixelBoardDevice.Commands
{
    public class UploadProgramsCommand : UploadStorageItemsCommand<UploadProgramRequest, Program>
    {
        public UploadProgramsCommand(Device device, INetworkAgent networkAgent, ILogger logger,
            int port = 500, int timeout = 100)
            : base(device, networkAgent, logger, port, timeout)
        {
        }

        public override byte StorageId => 2;

        public override List<Program> Items => ((PixelBoard)_device).Programs;

        public override string Name => "Отправка программ";
    }
}
