using PixelBoardDevice.DomainObjects.Zones;

namespace PixelBoardDevice.POCO
{
    public class ClockType
    {
        public ClockTypes Type { get; set; }
        public string Name { get; set; }
        public bool AllowFormat { get; set; }
    }
}
