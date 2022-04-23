using SmartTechnologiesM.Base.Extensions;

namespace CommandProcessing.Requests
{
    public class SyncTimeRequest : Request
    {
        public override byte RequestID => 0x06;

        public override byte[] MakeData(object o)
        {
            var seconds = (long)o;
            return seconds.ToBytes();
        }
    }
}
