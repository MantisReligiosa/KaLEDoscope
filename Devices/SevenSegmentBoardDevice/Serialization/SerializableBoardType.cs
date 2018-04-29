namespace SevenSegmentBoardDevice.Serialization
{
    public class SerializableBoardType
    {
        public int DisplayFormatId { get; set; }
        public int FontTypeId { get; set; }
        public int TypeId { get; set; }

        public static explicit operator BoardType(SerializableBoardType boardType)
        {
            return new BoardType
            {
                DisplayFormatId = boardType.DisplayFormatId,
                FontTypeId = boardType.FontTypeId,
                TypeId = boardType.TypeId
            };
        }

        public static explicit operator SerializableBoardType(BoardType boardType)
        {
            return new SerializableBoardType
            {
                DisplayFormatId = boardType.DisplayFormatId,
                FontTypeId = boardType.FontTypeId,
                TypeId = boardType.TypeId
            };
        }
    }
}
