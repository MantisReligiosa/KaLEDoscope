using BaseDevice;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Responces;
using ServiceInterfaces;

namespace PixelBoardDevice.Commands
{
    public class DownloadProgramsCommands : DownloadStorageItemsCommand<ProgramResponce, Program>
    {
        public DownloadProgramsCommands(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config)
        {
        }

        public override byte StorageId => 2;

        public override string Name => "Получение программ";

        public override void CleanupItemListBeforeRecievingItems()
        {
            var pixelBoard = _device as PixelBoard;
            pixelBoard.Programs.Clear();
            _device = pixelBoard;
        }

        public override void ProcessRecievedItem(Program item)
        {
            var pixelBoard = _device as PixelBoard;
            pixelBoard.Programs.Add(item);
            _device = pixelBoard;
        }
    }
}
