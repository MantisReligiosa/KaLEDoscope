using System;
using System.Text;

namespace Extensions
{
    public static class ToBytesExtensions
    {
        public static byte[] ToBytes(this ushort value)
        {
            return new byte[]
            {
                (byte)(value>>8),
                (byte)(value&0xff)
            };
        }
        public static ushort ExtractUshort(this byte[] bytes, int position)
        {
            return (ushort)((bytes[position] << 8) + bytes[position + 1]);
        }

        public static string ExtractString(this byte[] bytes, int position, int length)
        {
            var bytesStr = new byte[length];
            Array.Copy(bytes, position, bytesStr, 0, length);
            return Encoding.ASCII.GetString(bytesStr);
        }

        public static byte[] ToBytes(this string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }
    }
}
