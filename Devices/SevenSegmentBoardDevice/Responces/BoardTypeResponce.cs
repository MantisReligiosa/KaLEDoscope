using CommandProcessing;
using Extensions;
using System.Linq;

namespace SevenSegmentBoardDevice.Responces
{
    public class BoardTypeResponce : Responce<BoardType>
    {
        public override byte ResponceID => 0x10;

        public override BoardType Cast()
        {
            var dataByte = _bytes[5];
            var displayTypeId = (dataByte.GetBit(0) ? 2 : 0) + (dataByte.GetBit(1) ? 1 : 0);
            var fontId = (dataByte.GetBit(2) ? 2 : 0) + (dataByte.GetBit(3) ? 1 : 0);
            var capacity = (dataByte.GetBit(4) ? 8 : 0) + (dataByte.GetBit(5) ? 4 : 0)
                + (dataByte.GetBit(6) ? 2 : 0) + (dataByte.GetBit(7) ? 1 : 0);
            return new BoardType
            {
                DisplayType = Refs.DisplayTypes.FirstOrDefault(f => f.Id == displayTypeId),
                FontType = Refs.FontTypes.FirstOrDefault(f => f.Id == fontId),
                DisplayFormat = Refs.DisplayFormats.FirstOrDefault(f => f.Capacity == capacity)
            };
        }
    }
}
