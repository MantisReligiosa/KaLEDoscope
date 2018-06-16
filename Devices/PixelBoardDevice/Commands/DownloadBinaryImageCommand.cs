using BaseDevice;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Responces;
using ServiceInterfaces;

namespace PixelBoardDevice.Commands
{
    public class DownloadBinaryImageCommand : DownloadStorageItemsCommand<BinaryImageResponce, BinaryImage>
    {
        public DownloadBinaryImageCommand(Device device, INetworkAgent networkAgent, ILogger logger,
            int port = 500, int timeout = 100)
            : base(device, networkAgent, logger, port, timeout)
        {
        }

        public override byte StorageId => 4;

        public override string Name => "Получение изображений";

        public override void CleanupItemListBeforeRecievingItems()
        {
            var pixelBoard = _device as PixelBoard;
            pixelBoard.BinaryImages.Clear();
            _device = pixelBoard;
        }

        public override void ProcessRecievedItem(BinaryImage item)
        {
            var pixelBoard = _device as PixelBoard;
            pixelBoard.BinaryImages.Add(item);
            _device = pixelBoard;
        }
    }
}
