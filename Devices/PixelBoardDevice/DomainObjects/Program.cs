using SmartTechnologiesM.Base;
using System.Collections.Generic;

namespace PixelBoardDevice.DomainObjects
{
    public class Program : Notified
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public byte Order { get; set; }
        public ushort Period { get; set; }
        public List<Zone> Zones { get; set; }
    }
}
