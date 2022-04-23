using System;

namespace CommandProcessing.Requests
{
    public class ConfigurationRequest : Request
    {
        public override byte RequestID => 0x0A;

        public override byte[] MakeData(object o)
        {
            return new byte[] { Convert.ToByte(o) };
        }
    }
}
