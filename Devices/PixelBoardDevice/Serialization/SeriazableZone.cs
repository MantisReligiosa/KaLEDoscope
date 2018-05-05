using System;
using PixelBoardDevice.DomainObjects;

namespace PixelBoardDevice.Serialization
{
    public class SeriazableZone
    {
        public string BitmapBase64 { get; set; }
        public int BitmapHeight { get; set; }
        public int ClockFormat { get; set; }
        public int ClockType { get; set; }
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
        public bool AllowPeriodicTimeSync { get; set; }
        public bool AllowScheduledSync { get; set; }
        public int PeriodicSyncInterval { get; set; }
        public TimeSpan ScheduledTimeSync { get; set; }

        public static explicit operator SeriazableZone(Zone zone)
        {
            return new SeriazableZone
            {
                BitmapBase64 = zone.BitmapBase64,
                BitmapHeight = zone.BitmapHeight,
                ClockFormat = zone.ClockFormat,
                ClockType = zone.ClockType,
                ExternalSourceTag = zone.ExternalSourceTag,
                FontId = zone.FontId,
                Height = zone.Height,
                Id = zone.Id,
                Name = zone.Name,
                Text = zone.Text,
                Width = zone.Width,
                X = zone.X,
                Y = zone.Y,
                ZoneType = zone.ZoneType,
                AllowPeriodicTimeSync = zone.AllowPeriodicTimeSync,
                AllowScheduledSync = zone.AllowScheduledSync,
                PeriodicSyncInterval = zone.PeriodicSyncInterval,
                ScheduledTimeSync = zone.ScheduledTimeSync
            };
        }

        public static explicit operator Zone(SeriazableZone zone)
        {
            return new Zone
            {
                BitmapBase64 = zone.BitmapBase64,
                BitmapHeight = zone.BitmapHeight,
                ClockFormat = zone.ClockFormat,
                ClockType = zone.ClockType,
                ExternalSourceTag = zone.ExternalSourceTag,
                FontId = zone.FontId,
                Height = zone.Height,
                Id = zone.Id,
                Name = zone.Name,
                Text = zone.Text,
                Width = zone.Width,
                X = zone.X,
                Y = zone.Y,
                ZoneType = zone.ZoneType,
                AllowPeriodicTimeSync = zone.AllowPeriodicTimeSync,
                AllowScheduledSync = zone.AllowScheduledSync,
                PeriodicSyncInterval = zone.PeriodicSyncInterval,
                ScheduledTimeSync = zone.ScheduledTimeSync
            };
        }
    }
}