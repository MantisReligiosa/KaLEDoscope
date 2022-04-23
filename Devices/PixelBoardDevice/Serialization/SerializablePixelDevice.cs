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
            BoardSize = (SerializableBoardSize)device.BoardSize;
            Fonts = device.Fonts.Select(f => (SerializableFont)f).ToList();
            Programs = device.Programs.Select(p => (SerializableProgram)p).ToList();
            BinaryImages = device.BinaryImages.Select(i => (SerializableBinaryImage)i).ToList();
            HardwareType = (int)device.Hardware.Type;
        }

        public SerializableBoardSize BoardSize { get; set; }
        public List<SerializableFont> Fonts { get; set; }
        public List<SerializableProgram> Programs { get; set; }
        public List<SerializableBinaryImage> BinaryImages { get; set; }
        public int? HardwareType { get; set; }

        public static explicit operator SerializablePixelDevice(PixelBoard device)
        {
            return new SerializablePixelDevice(device);
        }

        public static explicit operator PixelBoard(SerializablePixelDevice serializableDevice)
        {
            var pixelBoard = new PixelBoard();
            serializableDevice.FillBasicParameters(pixelBoard);
            pixelBoard.BoardSize = (BoardSize)serializableDevice.BoardSize;
            pixelBoard.Fonts = serializableDevice.Fonts.Select(f => (BinaryFont)f).ToList();
            pixelBoard.Programs = serializableDevice.Programs.Select(p => (Program)p).ToList();
            pixelBoard.BinaryImages = serializableDevice.BinaryImages?.Select(i => (BinaryImage)i).ToList() ?? new List<BinaryImage>();
            pixelBoard.Hardware = new BoardHardware
            {
                Type = serializableDevice.HardwareType.HasValue ? (BoardHardwareType)serializableDevice.HardwareType : BoardHardwareType.RsPanel12x12
            };
            return pixelBoard;
        }
    }
}
