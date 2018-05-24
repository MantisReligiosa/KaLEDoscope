using System.Text;

namespace Extensions
{
    public static class BytesExtensions
    {
        public static byte[] ToBytes(this ushort u)
        {
            var b1 = (byte)(u >> 8);
            var b2 = (byte)(u & 0xff);
            return new byte[] { b1, b2 };
        }
        public static string ToByteString(this byte[] bytes)
        {
            var stringBuilder = new StringBuilder();
            foreach(var b in bytes)
            {
                stringBuilder.Append($"[{b:X}]");
            }
            return stringBuilder.ToString();
        }
    }
}
