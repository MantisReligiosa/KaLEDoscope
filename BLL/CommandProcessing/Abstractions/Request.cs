using ServiceInterfaces;
using SmartTechnologiesM.Base.Extensions;

namespace CommandProcessing
{
    public abstract class Request : IRequest
    {
        private object _data;
        public abstract byte RequestID { get; }
        public ushort DeviceID { get; set; }
        public void SetRequestData(object data)
        {
            _data = data;
        }
        public abstract byte[] MakeData(object o);
        public byte[] GetBytes()
        {
            var datas = MakeData(_data);
            var dataLength = (ushort)datas.Length;
            var requestLength = (ushort)(datas.Length + 5);
            var bytes = new byte[requestLength];
            DeviceID.ToBytes().CopyTo(bytes, 0);
            bytes[2] = RequestID;
            dataLength.ToBytes().CopyTo(bytes, 3);
            if (dataLength > 0)
            {
                datas.CopyTo(bytes, 5);
            }
            return bytes;
        }

        public override string ToString()
        {
            return GetBytes().ToStringExtend();
        }
    }
}
