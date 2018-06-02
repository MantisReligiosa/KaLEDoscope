using CommandProcessing;

namespace Testing
{
    public class MockRequest : Request
    {
        public override byte RequestID => 0x1;

        public override byte[] MakeData(object o)
        {
            return new byte[] { 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff };
        }
    }
}
