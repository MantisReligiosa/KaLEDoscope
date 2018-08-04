using Extensions;

namespace PixelBoardDevice.DomainObjects
{
    public abstract class Zone
    {
        public abstract string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Id { get; set; }
        public bool IsValid { get; set; }
        public abstract int ZoneType { get; }
        public int ProgramId { get; set; }

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
