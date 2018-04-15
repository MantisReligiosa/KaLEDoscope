using BaseDeviceSerialization;
using PixelBoardDevice.DomainObjects;
using System.Collections.Generic;
using System.Linq;

namespace PixelBoardDevice.Serialization
{
    public class SerializablePixelDevice : SerializableBaseDevice
    {
        public SerializablePixelDevice()
            : base()
        { }

        public SerializablePixelDevice(PixelBoard device)
            : base(device)
        {
            Alphabet = device.Alphabet;
            BoardSize = (SerializableBoardSize)device.BoardSize;
            Fonts = device.Fonts.Select(f => (SerializableFont)f).ToList();
            Programs = device.Programs.Select(p => (SerializableProgram)p).ToList();
        }

        public string Alphabet { get; set; }
        public SerializableBoardSize BoardSize { get; set; }
        public List<SerializableFont> Fonts { get; set; }
        public List<SerializableProgram> Programs { get; set; }

        public static explicit operator SerializablePixelDevice(PixelBoard device)
        {
            return new SerializablePixelDevice(device);
        }
    }
}
