using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using PixelBoardDevice.DTO;
using System.Collections.Generic;

namespace PixelBoardDevice.Interfaces
{
    public interface IDeviceController
    {
        void ValidateZones(ICollection<Zone> zones);
        Zone BindZone(Zone zone);
        PixelBoard Device { get; }

        ReformatTextResult ReformatText(TextZone textZone, bool trimText);
        void RenderFont(BinaryFont binaryFont);
        void UpdateZoneFont(Zone zone, System.Windows.Media.FontFamily newFont, byte newFontSize, bool italic, bool bold, out byte newBinaryFontId);
    }
}
