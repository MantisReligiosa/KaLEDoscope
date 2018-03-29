using PixelBoardDevice.DomainObjects;
using System;
using System.Drawing;

namespace PixelBoardDevice.UI.POCO
{
    public class ClockType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool AllowFormat { get; set; }
        public Action<Graphics, Zone, BinaryFont> Renderer { get; set; }
    }
}
