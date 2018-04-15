using PixelBoardDevice.DomainObjects;

namespace PixelBoardDevice.Serialization
{
    public class SerializableBoardSize
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public static explicit operator SerializableBoardSize(BoardSize boardSize)
        {
            return new SerializableBoardSize
            {
                Height = boardSize.Height,
                Width = boardSize.Width
            };
        }
    }
}