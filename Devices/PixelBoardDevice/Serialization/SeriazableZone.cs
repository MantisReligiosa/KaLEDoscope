using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using System;
using System.Windows;

namespace PixelBoardDevice.Serialization
{
    public class SeriazableZone
    {
        public byte? ClockFormat { get; set; }
        public int? ClockType { get; set; }
        public string ExternalSourceTag { get; set; }
        public byte? FontId { get; set; }
        public ushort Height { get; set; }
        public byte Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public ushort Width { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public int ZoneType { get; set; }
        public bool? AllowPeriodicTimeSync { get; set; }
        public bool? AllowScheduledSync { get; set; }
        public ushort? PeriodicSyncInterval { get; set; }
        public TimeSpan? ScheduledTimeSync { get; set; }
        public byte? TickerType { get; set; }
        public TimeSpan? TickerCountDownStartValue { get; set; }
        public byte ProgramId { get; set; }
        public byte? BinaryImageId { get; set; }
        public byte? Alignment { get; set; }
        public byte? AnimationId { get; set; }
        public byte? AnimationSpeed { get; set; }
        public byte? AnimationTimeout { get; set; }

        public static explicit operator SeriazableZone(Zone zone)
        {
            var seriazableZone = new SeriazableZone
            {
                FontId = (zone is IFontableZone fontableZone) ? fontableZone.FontId : default,
                Alignment = (byte?)((zone is IFontableZone fontableZone1) ? fontableZone1.Alignment : default),
                Height = zone.Height,
                Id = zone.Id,
                Name = zone.Name,
                Width = zone.Width,
                X = zone.X,
                Y = zone.Y,
                ZoneType = (int)zone.ZoneType,
                AllowPeriodicTimeSync = (zone is ClockZone clockZone2) ? clockZone2.AllowPeriodicTimeSync : default(bool?),
                ProgramId = zone.ProgramId
            };
            if (zone is TextZone textZone)
            {
                seriazableZone.Text = textZone.Text;
                seriazableZone.AnimationId = textZone.AnimationId;
                seriazableZone.AnimationSpeed = textZone.AnimationSpeed;
                seriazableZone.AnimationTimeout = textZone.AnimationTimeout;
            }
            if (zone is BitmapZone bitmapZone)
            {
                seriazableZone.BinaryImageId = bitmapZone.BinaryImageId;
            }
            if (zone is ClockZone clockZone)
            {
                seriazableZone.ClockFormat = clockZone.ClockFormat;
                seriazableZone.ClockType = (int)clockZone.ClockType;
                seriazableZone.AllowScheduledSync = clockZone.AllowScheduledSync;
                seriazableZone.PeriodicSyncInterval = clockZone.PeriodicSyncInterval;
                seriazableZone.ScheduledTimeSync = clockZone.ScheduledTimeSync;

            }
            if (zone is TagZone tagZone)
            {
                seriazableZone.ExternalSourceTag = tagZone.ExternalSourceTag;
            }
            if (zone is TickerZone tickerZone)
            {
                seriazableZone.TickerType = tickerZone.TickerType;
                seriazableZone.TickerCountDownStartValue = tickerZone.TickerCountDownStartValue;

            }
            return seriazableZone;
        }

        public static explicit operator Zone(SeriazableZone serializableZone)
        {
            Zone zoneResult = null;
            switch ((ZoneTypes)serializableZone.ZoneType)
            {
                case ZoneTypes.Text:
                    var textZone = new TextZone()
                    {
                        FontId = serializableZone.FontId,
                        Text = serializableZone.Text,
                        Alignment = (TextAlignment?)serializableZone.Alignment,
                        AnimationId = serializableZone.AnimationId,
                        AnimationSpeed = serializableZone.AnimationSpeed,
                        AnimationTimeout = serializableZone.AnimationTimeout ?? 0
                    };
                    zoneResult = textZone;
                    break;
                case ZoneTypes.Sensor:
                    var sensorZone = new SensorZone
                    {
                        FontId = serializableZone.FontId,
                        Alignment = (TextAlignment?)serializableZone.Alignment
                    };
                    zoneResult = sensorZone;
                    break;
                case ZoneTypes.Picture:
                    var bitmapZone = new BitmapZone
                    {
                        BinaryImageId = serializableZone.BinaryImageId.Value
                    };
                    zoneResult = bitmapZone;
                    break;
                case ZoneTypes.Tag:
                    var tagZone = new TagZone
                    {
                        FontId = serializableZone.FontId,
                        ExternalSourceTag = serializableZone.ExternalSourceTag,
                        Alignment = (TextAlignment?)serializableZone.Alignment
                    };
                    zoneResult = tagZone;
                    break;
                case ZoneTypes.Clock:
                    var clockZone = new ClockZone
                    {
                        ClockType = (ClockTypes)serializableZone.ClockType.Value,
                        AllowPeriodicTimeSync = serializableZone.AllowPeriodicTimeSync.Value,
                        AllowScheduledSync = serializableZone.AllowScheduledSync.Value
                    };
                    if (clockZone.ClockType == ClockTypes.Digital)
                    {
                        clockZone.ClockFormat = serializableZone.ClockFormat.Value;
                        clockZone.FontId = serializableZone.FontId;
                        clockZone.Alignment = (TextAlignment?)serializableZone.Alignment;
                    }

                    if (clockZone.AllowScheduledSync)
                    {
                        clockZone.ScheduledTimeSync = serializableZone.ScheduledTimeSync.Value;
                    }
                    if (clockZone.AllowPeriodicTimeSync)
                    {
                        clockZone.PeriodicSyncInterval = serializableZone.PeriodicSyncInterval.Value;
                    }
                    zoneResult = clockZone;
                    break;
                case ZoneTypes.Ticker:
                    var tickerZone = new TickerZone
                    {
                        TickerType = serializableZone.TickerType.Value,
                        TickerCountDownStartValue = serializableZone.TickerCountDownStartValue.Value,
                        FontId = serializableZone.FontId,
                        Alignment = (TextAlignment?)serializableZone.Alignment
                    };
                    zoneResult = tickerZone;
                    break;
            }
            if (zoneResult != null)
            {
                zoneResult.Id = serializableZone.Id;
                zoneResult.ProgramId = serializableZone.ProgramId;
                zoneResult.X = serializableZone.X;
                zoneResult.Y = serializableZone.Y;
                zoneResult.Width = serializableZone.Width;
                zoneResult.Height = serializableZone.Height;
                zoneResult.Name = serializableZone.Name;
            }
            return zoneResult;
        }
    }
}
