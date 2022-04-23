using PixelBoardDevice.DomainObjects;

namespace PixelBoardDevice.Serialization
{
    public class SerializableBoardSize
    {
        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public static explicit operator SerializableBoardSize(BoardSize boardSize)
        {
            return new SerializableBoardSize
            {
                Height = boardSize.Height,
                Width = boardSize.Width
            };
        }

        public static explicit operator BoardSize(SerializableBoardSize serializableBoardSize)
        {
            return new BoardSize
            {
                Height = serializableBoardSize.Height,
                Width = serializableBoardSize.Width
            };
        }
    }
}
