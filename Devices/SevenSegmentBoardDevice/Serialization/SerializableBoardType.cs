using System.Linq;

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
                DisplayFormat = Refs.DisplayFormats.FirstOrDefault(d => d.Id == boardType.DisplayFormatId),
                FontType = Refs.FontTypes.FirstOrDefault(f => f.Id == boardType.FontTypeId),
                DisplayType = Refs.DisplayTypes.FirstOrDefault(d => d.Id == boardType.TypeId)
            };
        }

        public static explicit operator SerializableBoardType(BoardType boardType)
        {
            return new SerializableBoardType
            {
                DisplayFormatId = boardType.DisplayFormat.Id,
                FontTypeId = boardType.FontType?.Id ?? 0,
                TypeId = boardType.DisplayType.Id
            };
        }
    }
}
