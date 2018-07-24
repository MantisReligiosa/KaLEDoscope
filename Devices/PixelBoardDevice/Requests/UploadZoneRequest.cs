using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
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
            if (zone is TextZone textZone)
            {
                bytes.Add((byte)textZone.FontId);
                bytes.Add((byte)(textZone.Alignment ?? 0));
                bytes.Add((byte)(textZone.AnimationId ?? 0));
                bytes.Add((byte)textZone.Text.Length);
                bytes.AddRange(textZone.Text.ToBytes());
            }
            else if (zone is SensorZone sensorZone)
            {
                bytes.Add((byte)sensorZone.FontId);
                bytes.Add((byte)(sensorZone.Alignment ?? 0));
            }
            else if (zone is BitmapZone bitmapZone)
            {
                bytes.Add((byte)bitmapZone.BinaryImageId);
            }
            else if (zone is TagZone tagZone)
            {
                bytes.Add((byte)tagZone.FontId);
                bytes.Add((byte)(tagZone.Alignment ?? 0));
                bytes.Add((byte)tagZone.ExternalSourceTag.Length);
                bytes.AddRange(tagZone.ExternalSourceTag.ToBytes());
            }
            else if (zone is ClockZone clockZone)
            {
                var clockTypeMask = (clockZone.ClockType == 1) ? 0x00 : 0b00010000;//1 - текст, 2 - графические
                var formatTypeMask = (clockZone.ClockType == 2) ? 0x00 : (byte)(clockZone.ClockFormat << 2);
                var periodicSyncMask = (clockZone.AllowPeriodicTimeSync) ? 0b00000010 : 0x00;
                var scheduledSuncMask = (clockZone.AllowScheduledSync) ? 0x01 : 0x00;
                bytes.Add((byte)(clockTypeMask | formatTypeMask | periodicSyncMask | scheduledSuncMask));
                if (clockZone.AllowPeriodicTimeSync)
                {
                    bytes.AddRange(((ushort)clockZone.PeriodicSyncInterval).ToBytes());
                }
                else if (clockZone.AllowScheduledSync)
                {
                    bytes.Add((byte)clockZone.ScheduledTimeSync.Hours);
                    bytes.Add((byte)clockZone.ScheduledTimeSync.Minutes);
                }
                else
                {
                    bytes.AddRange(new byte[] { 0x00, 0x00 });
                }
                bytes.Add((byte)(clockZone.ClockType == 1 ? clockZone.FontId ?? 0 : 0x00));
                bytes.Add((byte)(clockZone.ClockType == 1 ? clockZone.Alignment ?? 0 : 0x00));
            }
            else if (zone is TickerZone tickerZone)
            {
                bytes.Add((byte)(tickerZone.TickerType == 1 ? 0 : 1));
                bytes.Add((byte)tickerZone.TickerCountDownStartValue.Hours);
                bytes.Add((byte)tickerZone.TickerCountDownStartValue.Minutes);
                bytes.Add((byte)tickerZone.TickerCountDownStartValue.Seconds);
                bytes.AddRange(((ushort)tickerZone.TickerCountDownStartValue.Milliseconds).ToBytes());
                bytes.Add((byte)(tickerZone.FontId ?? 0));
                bytes.Add((byte)(tickerZone.Alignment ?? 0));
            }
            return bytes.ToArray();
        }
    }
}
