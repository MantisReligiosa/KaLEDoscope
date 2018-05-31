using ServiceInterfaces;

namespace CommandProcessing
{
    public class ConfigurationRequest : Request
    {
        public override byte RequestID => 0x0A;

        public override byte[] MakeData(object o)
        {
            return new byte[] { (byte)o };
        }
    }
}
