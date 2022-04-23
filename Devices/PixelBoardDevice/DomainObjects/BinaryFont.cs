namespace PixelBoardDevice.DomainObjects
{
    public class BinaryFont
    {
        public byte Id { get; set; }
        public byte Height { get; set; }
        public byte GlyphHeight { get; set; }
        public bool Italic { get; set; }
        public bool Bold { get; set; }
        public string Source { get; set; }
        public Glyph[] Alphabet { get; set; }
    }
}