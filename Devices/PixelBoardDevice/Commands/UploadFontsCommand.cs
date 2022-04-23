using BaseDevice;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Requests;
using ServiceInterfaces;
using System.Collections.Generic;

namespace PixelBoardDevice.Commands
{
    public class UploadFontsCommand : UploadStorageItemsCommand<UploadFontRequest, BinaryFont>
    {
        public UploadFontsCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config)
        {
        }

        public override byte StorageId => 1;

        public override List<BinaryFont> GetItems() => ((PixelBoard)_device).Fonts ?? new List<BinaryFont>();

        public override string Name => "Отправка шрифтов";
    }
}
