
using SmartTechnologiesM.Base.Extensions;

namespace PixelBoardDevice.DomainObjects
{
    public abstract class Zone
    {
        public abstract string Name { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public ushort Height { get; set; }
        public ushort Width { get; set; }
        public byte Id { get; set; }
        public bool IsValid { get; set; }
        public abstract ZoneTypes ZoneType { get; }
        public byte ProgramId { get; set; }

        public bool IntersectWith(Zone z)
        {
            if ((z.X + z.Width).Between(X, X + Width) && (z.Y + z.Height).Between(Y, Y + Height))
                return true;
            if ((z.X + z.Width).Between(X, X + Width) && z.Y.Between(Y, (ushort)(Y + Height)))
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
        Tag,
        Clock,
        Ticker
    }

}
