using System.Collections.Generic;
using BaseDevice;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Requests;
using ServiceInterfaces;

namespace PixelBoardDevice.Commands
{
    public class UploadBinaryImageCommand : UploadStorageItemsCommand<UploadBinaryImageRequest, BinaryImage>
    {
        public UploadBinaryImageCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config)
        {
        }

        public override byte StorageId => 4;

        public override List<BinaryImage> GetItems() => ((PixelBoard)_device).BinaryImages ?? new List<BinaryImage>();

        public override string Name => "Отправка изображений";
    }
}
