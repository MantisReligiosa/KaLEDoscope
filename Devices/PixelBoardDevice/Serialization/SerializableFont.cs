using PixelBoardDevice.DomainObjects;

namespace PixelBoardDevice.Serialization
{
    public class SerializableFont
    {
        public bool Bold { get; set; }
        public byte Height { get; set; }
        public byte Id { get; set; }
        public bool Italic { get; set; }
        public string Source { get; set; }
        public Glyph[] Alphabet { get; set; }

        public static explicit operator SerializableFont(BinaryFont font)
        {
            return new SerializableFont
            {
                Bold = font.Bold,
                Height = font.Height,
                Id = font.Id,
                Italic = font.Italic,
                Source = font.Source,
                Alphabet = font.Alphabet
            };
        }

        public static explicit operator BinaryFont(SerializableFont font)
        {
            return new BinaryFont
            {
                Bold = font.Bold,
                Height = font.Height,
                Id = font.Id,
                Italic = font.Italic,
                Source = font.Source,
                Alphabet = font.Alphabet
            };
        }
    }
}
