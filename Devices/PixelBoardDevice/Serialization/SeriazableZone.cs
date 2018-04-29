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
                ZoneType = zone.ZoneType
            };
        }

        public static explicit operator Zone(SeriazableZone seriazableZone)
        {
            return new Zone
            {
                BitmapBase64 = seriazableZone.BitmapBase64,
                BitmapHeight = seriazableZone.BitmapHeight,
                ClockFormat = seriazableZone.ClockFormat,
                ClockType = seriazableZone.ClockType,
                ExternalSourceTag = seriazableZone.ExternalSourceTag,
                FontId = seriazableZone.FontId,
                Height = seriazableZone.Height,
                Id = seriazableZone.Id,
                Name = seriazableZone.Name,
                Text = seriazableZone.Text,
                Width = seriazableZone.Width,
                X = seriazableZone.X,
                Y = seriazableZone.Y,
                ZoneType = seriazableZone.ZoneType
            };
        }

    }
}