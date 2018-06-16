using BaseDevice;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Requests;
using ServiceInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace PixelBoardDevice.Commands
{
    public class UploadZonesCommand : UploadStorageItemsCommand<UploadZoneRequest, Zone>
    {
        public UploadZonesCommand(Device device, INetworkAgent networkAgent, ILogger logger,
            int port = 500, int timeout = 100)
            : base(device, networkAgent, logger, port, timeout)
        {
        }

        public override byte StorageId => 3;

        public override List<Zone> Items =>
            ((PixelBoard)_device).Programs.SelectMany(p => p.Zones).ToList();

        public override string Name => "Отправка программ";
    }
}
