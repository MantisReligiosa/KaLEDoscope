using PixelBoardDevice.DomainObjects;

namespace PixelBoardDevice.Serialization
{
    public class SerializableBinaryImage
    {
        public int Id { get; set; }
        public string Base64String { get; set; }
        public int Height { get; set; }

        public static explicit operator SerializableBinaryImage(BinaryImage binaryImage)
        {
            return new SerializableBinaryImage
            {
                Id = binaryImage.Id,
                Base64String = binaryImage.Base64String,
                Height = binaryImage.Height
            };
        }

        public static explicit operator BinaryImage(SerializableBinaryImage binaryImage)
        {
            return new BinaryImage
            {
                Id = binaryImage.Id,
                Base64String = binaryImage.Base64String,
                Height = binaryImage.Height
            };
        }
    }
}
