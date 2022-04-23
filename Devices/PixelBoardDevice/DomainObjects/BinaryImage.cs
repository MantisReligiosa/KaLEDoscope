namespace PixelBoardDevice.DomainObjects
{
    public class BinaryImage
    {
        public byte Id { get; set; }
        public byte[] Bytes { get; set; }
        public ushort Height { get; set; }
        public ushort Width { get; set; }
    }
}
