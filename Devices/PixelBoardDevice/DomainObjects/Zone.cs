namespace PixelBoardDevice
{
    public abstract class Zone
    {
        public abstract string Name { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Id { get; set; }
        public bool IsValid { get; set; }
    }
}