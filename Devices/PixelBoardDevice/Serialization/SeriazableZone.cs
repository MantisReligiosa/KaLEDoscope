using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using System;

namespace PixelBoardDevice.Serialization
{
    public class SeriazableZone
    {
        public string BitmapBase64 { get; set; }
        public int? BitmapHeight { get; set; }
        public int? ClockFormat { get; set; }
        public int? ClockType { get; set; }
        public string ExternalSourceTag { get; set; }
        public int? FontId { get; set; }
        public int Height { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public int Width { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int ZoneType { get; set; }
        public bool? AllowPeriodicTimeSync { get; set; }
        public bool? AllowScheduledSync { get; set; }
        public int? PeriodicSyncInterval { get; set; }
        public TimeSpan? ScheduledTimeSync { get; set; }
        public int? TickerType { get; set; }
        public TimeSpan? TickerCountDownStartValue { get; set; }
        public int ProgramId { get; set; }
        public int? BinaryImageId { get; set; }
        public int? Alignment { get; set; }
        public int? AnimationId { get; set; }

        public static explicit operator SeriazableZone(Zone zone)
        {
            var seriazableZone = new SeriazableZone
            {
                FontId = (zone is IFontableZone fontableZone) ? fontableZone.FontId : default(int?),
                Alignment = (zone is IFontableZone fontableZone1) ? fontableZone1.Alignment : default(int?),
                Height = zone.Height,
                Id = zone.Id,
                Name = zone.Name,
                Width = zone.Width,
                X = zone.X,
                Y = zone.Y,
                ZoneType = zone.ZoneType,
                AllowPeriodicTimeSync = (zone is ClockZone clockZone2) ? clockZone2.AllowPeriodicTimeSync : default(bool?),
                ProgramId = zone.ProgramId
            };
            if (zone is TextZone textZone)
            {
                seriazableZone.Text = textZone.Text;
                seriazableZone.AnimationId = textZone.AnimationId;
            }
            if (zone is BitmapZone bitmapZone)
            {
                seriazableZone.BinaryImageId = bitmapZone.BinaryImageId;
            }
            if (zone is ClockZone clockZone)
            {
                seriazableZone.ClockFormat = clockZone.ClockFormat;
                seriazableZone.ClockType = clockZone.ClockType;
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
            switch (serializableZone.ZoneType)
            {
                case 1:
                    var textZone = new TextZone()
                    {
                        FontId = serializableZone.FontId,
                        Text = serializableZone.Text,
                        Alignment = serializableZone.Alignment,
                        AnimationId = serializableZone.AnimationId,
                    };
                    zoneResult = textZone;
                    break;
                case 2:
                    var sensorZone = new SensorZone
                    {
                        FontId = serializableZone.FontId,
                        Alignment = serializableZone.Alignment
                    };
                    zoneResult = sensorZone;
                    break;
                case 3:
                    var bitmapZone = new BitmapZone
                    {
                        BinaryImageId = serializableZone.BinaryImageId.Value
                    };
                    zoneResult = bitmapZone;
                    break;
                case 4:
                    var tagZone = new TagZone
                    {
                        FontId = serializableZone.FontId,
                        ExternalSourceTag = serializableZone.ExternalSourceTag,
                        Alignment = serializableZone.Alignment
                    };
                    zoneResult = tagZone;
                    break;
                case 5:
                    var clockZone = new ClockZone
                    {
                        ClockType = serializableZone.ClockType.Value,
                        AllowPeriodicTimeSync = serializableZone.AllowPeriodicTimeSync.Value,
                        AllowScheduledSync = serializableZone.AllowScheduledSync.Value
                    };
                    if (clockZone.ClockType == 1)
                    {
                        clockZone.ClockFormat = serializableZone.ClockFormat.Value;
                        clockZone.FontId = serializableZone.FontId;
                        clockZone.Alignment = serializableZone.Alignment;
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
                case 6:
                    var tickerZone = new TickerZone
                    {
                        TickerType = serializableZone.TickerType.Value,
                        TickerCountDownStartValue = serializableZone.TickerCountDownStartValue.Value,
                        FontId = serializableZone.FontId,
                        Alignment = serializableZone.Alignment
                    };
                    zoneResult = tickerZone;
                    break;
            }
            zoneResult.Id = serializableZone.Id;
            zoneResult.ProgramId = serializableZone.ProgramId;
            zoneResult.X = serializableZone.X;
            zoneResult.Y = serializableZone.Y;
            zoneResult.Width = serializableZone.Width;
            zoneResult.Height = serializableZone.Height;
            zoneResult.Name = serializableZone.Name;
            return zoneResult;
        }
    }
}