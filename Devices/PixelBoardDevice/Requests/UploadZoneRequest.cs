using CommandProcessing;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using SmartTechnologiesM.Base.Extensions;
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
                zone.Id,
                zone.ProgramId
            };
            bytes.AddRange(zone.X.ToBytes());
            bytes.AddRange(zone.Y.ToBytes());
            bytes.AddRange(zone.Width.ToBytes());
            bytes.AddRange(zone.Height.ToBytes());
            bytes.Add((byte)zone.Name.Length);
            bytes.AddRange(zone.Name.ToBytes());
            bytes.Add((byte)zone.ZoneType);
            if (zone is TextZone textZone)
            {
                bytes.Add(textZone.FontId ?? 0);
                bytes.Add((byte?)textZone.Alignment ?? 0);
                bytes.Add(textZone.AnimationId ?? 0);
                bytes.Add(textZone.AnimationSpeed ?? 0);
                bytes.Add(textZone.AnimationTimeout);
                bytes.Add((byte)(textZone.Text?.Length ?? 0));
                if (textZone.Text != null)
                    bytes.AddRange(textZone.Text.ToBytes());
            }
            else if (zone is SensorZone sensorZone)
            {
                bytes.Add(sensorZone.FontId ?? 0);
                bytes.Add((byte?)sensorZone.Alignment ?? 0);
            }
            else if (zone is BitmapZone bitmapZone)
            {
                bytes.Add(bitmapZone.BinaryImageId);
            }
            else if (zone is TagZone tagZone)
            {
                bytes.Add(tagZone.FontId ?? 0);
                bytes.Add((byte?)tagZone.Alignment ?? 0);
                bytes.Add((byte)tagZone.ExternalSourceTag.Length);
                bytes.AddRange(tagZone.ExternalSourceTag.ToBytes());
            }
            else if (zone is ClockZone clockZone)
            {
                var clockTypeMask = (clockZone.ClockType == ClockTypes.Digital) ? 0x00 : 0b00010000;
                var formatTypeMask = (clockZone.ClockType == ClockTypes.Analog) ? 0x00 : clockZone.ClockFormat << 2;
                var periodicSyncMask = (clockZone.AllowPeriodicTimeSync) ? 0b00000010 : 0x00;
                var scheduledSuncMask = (clockZone.AllowScheduledSync) ? 0x01 : 0x00;
                bytes.Add((byte)(clockTypeMask | formatTypeMask | periodicSyncMask | scheduledSuncMask));
                if (clockZone.AllowPeriodicTimeSync)
                {
                    bytes.AddRange(clockZone.PeriodicSyncInterval.ToBytes());
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
                bytes.Add((byte)(clockZone.ClockType == ClockTypes.Digital ? clockZone.FontId ?? 0 : 0x00));
                bytes.Add((byte)(clockZone.ClockType == ClockTypes.Digital ? clockZone.Alignment ?? 0 : 0x00));
            }
            else if (zone is TickerZone tickerZone)
            {
                bytes.Add((byte)(tickerZone.TickerType == 1 ? 0 : 1));
                bytes.Add((byte)tickerZone.TickerCountDownStartValue.Hours);
                bytes.Add((byte)tickerZone.TickerCountDownStartValue.Minutes);
                bytes.Add((byte)tickerZone.TickerCountDownStartValue.Seconds);
                bytes.AddRange(((ushort)tickerZone.TickerCountDownStartValue.Milliseconds).ToBytes());
                bytes.Add(tickerZone.FontId ?? 0);
                bytes.Add((byte?)tickerZone.Alignment ?? 0);
            }
            return bytes.ToArray();
        }
    }
}
