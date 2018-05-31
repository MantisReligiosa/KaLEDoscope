using Extensions;
using System.Text;

namespace ServiceInterfaces
{
    public abstract class Request
    {
        private object _t;
        public abstract byte RequestID { get; }
        public ushort DeviceID { get; set; }
        public void SetRequestData(object data)
        {
            _t = data;
        }
        public abstract byte[] MakeData(object o);
        public byte[] GetBytes()
        {
            var datas = MakeData(_t);
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
            var sb = new StringBuilder();
            foreach (var b in GetBytes())
            {
                sb.Append($"[{b:X2}]");
            }
            return sb.ToString();
        }
    }
}
