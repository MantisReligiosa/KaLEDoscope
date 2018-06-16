using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;
using System.Collections.Generic;

namespace PixelBoardDevice.Requests
{
    public class UploadZoneRequest : Request
    {
        public override byte RequestID => 0x26;

        public override byte[] MakeData(object o)
        {
            var zone = (Zone)o;
            var bytes = new List<byte>
            {
                0x03,
                (byte)zone.Id,
                (byte)zone.ProgramId
            };
            bytes.AddRange(((ushort)zone.X).ToBytes());
            bytes.AddRange(((ushort)zone.Y).ToBytes());
            bytes.AddRange(((ushort)zone.Width).ToBytes());
            bytes.AddRange(((ushort)zone.Height).ToBytes());
            bytes.Add((byte)zone.Name.Length);
            bytes.AddRange(zone.Name.ToBytes());
            bytes.Add((byte)zone.ZoneType);
            switch (zone.ZoneType)
            {
                case 1:
                    bytes.Add((byte)zone.FontId);
                    bytes.Add((byte)zone.Text.Length);
                    bytes.AddRange(zone.Text.ToBytes());
                    break;
                case 2:
                    bytes.Add((byte)zone.FontId);
                    break;
                case 3:
                    bytes.Add((byte)zone.BinaryImageId);
                    break;
                case 4:
                    bytes.Add((byte)zone.FontId);
                    bytes.Add((byte)zone.ExternalSourceTag.Length);
                    bytes.AddRange(zone.ExternalSourceTag.ToBytes());
                    break;
                case 5:
                    var clockTypeMask = (zone.ClockType == 1) ? 0x00 : 0b00010000;//1 - текст, 2 - графические
                    var formatTypeMask = (zone.ClockType == 2) ? 0x00 : (byte)(zone.ClockFormat << 2);
                    var periodicSyncMask = (zone.AllowPeriodicTimeSync) ? 0b00000010 : 0x00;
                    var scheduledSuncMask = (zone.AllowScheduledSync) ? 0x01 : 0x00;
                    bytes.Add((byte)(clockTypeMask | formatTypeMask | periodicSyncMask | scheduledSuncMask));
                    if (zone.AllowPeriodicTimeSync)
                    {
                        bytes.AddRange(((ushort)zone.PeriodicSyncInterval).ToBytes());
                    }
                    else if (zone.AllowScheduledSync)
                    {
                        bytes.Add((byte)zone.ScheduledTimeSync.Hours);
                        bytes.Add((byte)zone.ScheduledTimeSync.Minutes);
                    }
                    else
                    {
                        bytes.AddRange(new byte[] { 0x00, 0x00 });
                    }
                    break;
                case 6:
                    bytes.Add((byte)(zone.TickerType == 1 ? 0 : 1));
                    bytes.Add((byte)zone.TickerCountDownStartValue.Hours);
                    bytes.Add((byte)zone.TickerCountDownStartValue.Minutes);
                    bytes.Add((byte)zone.TickerCountDownStartValue.Seconds);
                    bytes.AddRange(((ushort)zone.TickerCountDownStartValue.Milliseconds).ToBytes());
                    break;
            }
            return bytes.ToArray();
        }
    }
}
