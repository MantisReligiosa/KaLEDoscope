using System.Collections.Generic;
using BaseDevice;

namespace PixelBoardDevice.DomainObjects
{
    public class PixelBoard : Device
    {
        public List<BinaryFont> Fonts { get; set; }
        public BoardSize BoardSize { get; set; }
        public List<Program> Programs { get; set; }
        public List<BinaryImage> BinaryImages { get; set; }
    }
}
