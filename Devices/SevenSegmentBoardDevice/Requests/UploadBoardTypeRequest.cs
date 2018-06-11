using CommandProcessing;

namespace SevenSegmentBoardDevice.Requests
{
    public class UploadBoardTypeRequest : Request
    {
        public override byte RequestID => 0x10;

        public override byte[] MakeData(object o)
        {
            var boardType = o as BoardType;
            var flags = boardType.DisplayType.Id << 6;
            if (boardType.DisplayType.IsFontEnabled)
            {
                flags += (boardType.FontType.Id << 4);
            }
            flags += (boardType.DisplayFormat.Capacity);
            return new byte[] { (byte)flags };
        }
    }
}
