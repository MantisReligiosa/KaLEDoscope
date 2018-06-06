namespace CommandProcessing.Requests
{
    public class ScanRequest : Request
    {
        public override byte RequestID => 0x00;
        public new ushort DeviceID => 0x00;

        public override byte[] MakeData(object o)
        {
            return new byte[0];
        }
    }
}
