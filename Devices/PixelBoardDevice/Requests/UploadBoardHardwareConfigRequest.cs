using CommandProcessing;
using PixelBoardDevice.DomainObjects;

namespace PixelBoardDevice.Requests
{
    public class UploadBoardHardwareConfigRequest : Request
    {
        public override byte RequestID => 0x27;

        public override byte[] MakeData(object o)
        {
            var boardHardware = o as BoardHardware;
            return new byte[] { (byte)boardHardware.Type };
        }
    }
}
