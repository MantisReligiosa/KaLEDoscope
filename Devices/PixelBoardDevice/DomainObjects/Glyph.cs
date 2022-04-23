namespace PixelBoardDevice.DomainObjects
{
    public class Glyph
    {
        public char Symbol { get; set; }
        public ushort Width { get; set; }
        public byte[] Image { get; set; }
    }
}
