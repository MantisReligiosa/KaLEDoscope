using CommandProcessing;

namespace PixelBoardDevice.Requests
{
    public class GetFontsRequest : Request
    {
        public override byte RequestID => 0x22;

        public override byte[] MakeData(object o)
        {
            return new byte[] { (byte)(int)o };
        }
    }
}
