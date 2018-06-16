using BaseDevice;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Requests;
using ServiceInterfaces;
using System.Collections.Generic;

namespace PixelBoardDevice.Commands
{
    public class UploadFontsCommand : UploadStorageItemsCommand<UploadFontRequest, BinaryFont>
    {
        public UploadFontsCommand(Device device, INetworkAgent networkAgent, ILogger logger,
            int port = 500, int timeout = 100)
            : base(device, networkAgent, logger, port, timeout)
        {
        }

        public override byte StorageId => 1;

        public override List<BinaryFont> Items => ((PixelBoard)_device).Fonts;

        public override string Name => "Отправка шрифтов";
    }
}
