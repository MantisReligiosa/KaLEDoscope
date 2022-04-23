using BaseDevice;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Requests;
using ServiceInterfaces;
using System.Collections.Generic;

namespace PixelBoardDevice.Commands
{
    public class UploadProgramsCommand : UploadStorageItemsCommand<UploadProgramRequest, Program>
    {
        public UploadProgramsCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config)
        {
        }

        public override byte StorageId => 2;

        public override List<Program> GetItems() => ((PixelBoard)_device).Programs?? new List<Program>();

        public override string Name => "Отправка программ";
    }
}
