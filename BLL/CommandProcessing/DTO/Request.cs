using Extensions;

namespace CommandProcessing.DTO
{
    public abstract class Request
    {
        public abstract byte ID { get; }
        public ushort DeviceId { get; set; }
        public abstract ushort DataArrayLength { get; }
        public abstract byte[] GetData();
        public byte[] GetBytes()
        {
            var length = DataArrayLength + 5;
            var bytes = new byte[length];
            DeviceId.ToBytes().CopyTo(bytes, 0);
            bytes[2] = ID;
            DataArrayLength.ToBytes().CopyTo(bytes, 3);
            if (DataArrayLength > 0)
            {
                GetData().CopyTo(bytes, 5);
            }
            return bytes;
        }
    }
}