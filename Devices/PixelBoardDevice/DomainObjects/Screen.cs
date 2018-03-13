using System.Collections.Generic;

namespace PixelBoardDevice
{
    public class Screen
    {
        public int Order { get; set; }
        public int Period { get; set; }
        public List<Zone> Zones { get; set; }
    }
}