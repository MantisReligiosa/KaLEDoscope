using BaseDevice;
using Extensions;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Responces;
using ServiceInterfaces;
using System.Linq;

namespace PixelBoardDevice.Commands
{
    public class DownloadZonesCommand : DownloadStorageItemsCommand<ZoneResponce, Zone>
    {
        public DownloadZonesCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config)
        {
        }

        public override byte StorageId => 3;

        public override string Name => "Получение";

        public override void CleanupItemListBeforeRecievingItems()
        {
            var pixelBoard = _device as PixelBoard;
            pixelBoard.Programs.ForEach(p => p.Zones.Clear());
            _device = pixelBoard;
        }

        public override void ProcessRecievedItem(Zone item)
        {
            var pixelBoard = _device as PixelBoard;
            var program = pixelBoard.Programs.FirstOrDefault(p => p.Id == item.ProgramId);
            if (program.IsNull())
                return;
            program.Zones.Add(item);
            _device = pixelBoard;
        }
    }
}
