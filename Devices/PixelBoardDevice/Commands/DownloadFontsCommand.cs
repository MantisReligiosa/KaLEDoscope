using BaseDevice;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Responces;
using ServiceInterfaces;

namespace PixelBoardDevice.Commands
{
    public class DownloadFontsCommand : DownloadStorageItemsCommand<FontResponce, BinaryFont>
    {
        public DownloadFontsCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger)
        {
        }

        public override byte StorageId => 1;

        public override string Name => "Получение шрифтов";

        public override void CleanupItemListBeforeRecievingItems()
        {
            var pixelBoard = _device as PixelBoard;
            pixelBoard.Fonts.Clear();
            _device = pixelBoard;
        }

        public override void ProcessRecievedItem(BinaryFont item)
        {
            var pixelBoard = _device as PixelBoard;
            pixelBoard.Fonts.Add(item);
            _device = pixelBoard;
        }
    }
}
