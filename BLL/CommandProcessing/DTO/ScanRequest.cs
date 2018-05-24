namespace CommandProcessing.DTO
{
    public class ScanRequest : Request
    {
        public override byte ID => 0x00;

        public override ushort DataArrayLength => 0;

        public override byte[] GetData()
        {
            return new byte[] { };
        }
    }
}
