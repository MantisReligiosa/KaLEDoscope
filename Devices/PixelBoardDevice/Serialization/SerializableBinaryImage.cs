using PixelBoardDevice.DomainObjects;

namespace PixelBoardDevice.Serialization
{
    public class SerializableBinaryImage
    {
        public byte Id { get; set; }
        public byte[] Bytes { get; set; }
        public ushort Height { get; set; }
        public ushort Width { get; set; }

        public static explicit operator SerializableBinaryImage(BinaryImage binaryImage)
        {
            return new SerializableBinaryImage
            {
                Id = binaryImage.Id,
                Bytes = binaryImage.Bytes,
                Height = binaryImage.Height,
                Width = binaryImage.Width
            };
        }

        public static explicit operator BinaryImage(SerializableBinaryImage binaryImage)
        {
            return new BinaryImage
            {
                Id = binaryImage.Id,
                Bytes = binaryImage.Bytes,
                Height = binaryImage.Height,
                Width = binaryImage.Width
            };
        }
    }
}
