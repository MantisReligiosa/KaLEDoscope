using CommandProcessing;
using System;

namespace PixelBoardDevice.Requests
{
    public class GetIdListRequest : Request
    {
        public override byte RequestID => 0x22;

        public override byte[] MakeData(object o)
        {
            return new byte[] { Convert.ToByte(o) };
        }
    }
}
