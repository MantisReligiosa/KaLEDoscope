using System.Collections.Generic;
using BaseDevice;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Requests;
using ServiceInterfaces;

namespace PixelBoardDevice.Commands
{
    public class UploadBinaryImageCommand : UploadStorageItemsCommand<UploadBinaryImageRequest, BinaryImage>
    {
        public UploadBinaryImageCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger)
        {
        }

        public override byte StorageId => 4;

        public override List<BinaryImage> Items => ((PixelBoard)_device).BinaryImages;

        public override string Name => "Отправка изображений";
    }
}
