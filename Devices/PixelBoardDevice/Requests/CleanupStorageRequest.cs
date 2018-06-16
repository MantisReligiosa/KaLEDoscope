using CommandProcessing;

namespace PixelBoardDevice.Requests
{
    public class CleanupStorageRequest : Request
    {
        public override byte RequestID => 0x21;

        public override byte[] MakeData(object o)
        {
            return new byte[] { (byte)(int)o };
        }
    }
}
