using PixelBoardDevice.DomainObjects;

namespace PixelBoardDevice.Serialization
{
    public class SerializableFont
    {
        public string Base64Bitmap { get; set; }
        public bool Bold { get; set; }
        public int Height { get; set; }
        public int Id { get; set; }
        public bool Italic { get; set; }
        public string Source { get; set; }

        public static explicit operator SerializableFont(BinaryFont font)
        {
            return new SerializableFont
            {
                Base64Bitmap = font.Base64Bitmap,
                Bold = font.Bold,
                Height = font.Height,
                Id = font.Id,
                Italic = font.Italic,
                Source = font.Source
            };
        }

        public static explicit operator BinaryFont(SerializableFont font)
        {
            return new BinaryFont
            {
                Base64Bitmap = font.Base64Bitmap,
                Bold = font.Bold,
                Height = font.Height,
                Id = font.Id,
                Italic = font.Italic,
                Source = font.Source
            };
        }
    }
}