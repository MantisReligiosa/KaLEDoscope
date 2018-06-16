using Extensions;
using System;
using System.Linq;

namespace PixelBoardDevice.DomainObjects
{
    public class Zone
    {
        public virtual string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Id { get; set; }
        public bool IsValid { get; set; }
        public int ZoneType { get; set; }
        public int? FontId { get; set; }
        public string Text { get; set; }
        public bool IsFonted
        {
            get
            {
                return (new int[] { (int)ZoneTypes.Text, (int)ZoneTypes.Sensor, (int)ZoneTypes.MQTT, (int)ZoneTypes.Clock, (int)ZoneTypes.Ticker }).Contains(ZoneType);
            }
        }
        public int ProgramId { get; set; }
        public string ExternalSourceTag { get; set; }
        public int BinaryImageId { get; set; }
        public int ClockType { get; set; }
        public int ClockFormat { get; set; }
        public bool AllowPeriodicTimeSync { get; set; }
        public bool AllowScheduledSync { get; set; }
        public int PeriodicSyncInterval { get; set; }
        public TimeSpan ScheduledTimeSync { get; set; }
        public int TickerType { get; set; }
        public TimeSpan TickerCountDownStartValue { get; set; }

        public bool IntersectWith(Zone z)
        {
            if ((z.X + z.Width).Between(X, X + Width) && (z.Y + z.Height).Between(Y, Y + Height))
                return true;
            if ((z.X + z.Width).Between(X, X + Width) && z.Y.Between(Y, Y + Height))
                return true;
            if (z.X.Between(X, X + Width) && (z.Y + z.Height).Between(Y, Y + Height))
                return true;
            if (z.X.Between(X, X + Width) && z.Y.Between(Y, Y + Height))
                return true;
            return false;
        }
    }

    public enum ZoneTypes
    {
        Text = 1,
        Sensor,
        Picture,
        MQTT,
        Clock,
        Ticker
    }

}