using System.Collections.Generic;

namespace PixelBoardDevice.DomainObjects
{
    public class Program
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int Period { get; set; }
        public List<Zone> Zones { get; set; }
    }
}
