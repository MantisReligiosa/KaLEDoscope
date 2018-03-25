namespace PixelBoardDevice.DomainObjects
{
    public class BinaryFont
    {
        public int Id { get; set; }
        public int Height { get; set; }
        public bool Italic { get; set; }
        public bool Bold { get; set; }
        public string Base64Bitmap { get; set; }
        public string Source { get; set; }
    }
}