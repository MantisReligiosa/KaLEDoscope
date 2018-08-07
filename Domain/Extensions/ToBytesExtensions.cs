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
        public static byte[] ToBytes(this uint value)
        {
            return new byte[]
            {
                (byte)((value>>24)&0xff),
                (byte)((value>>16)&0xff),
                (byte)((value>>8)&0xff),
                (byte)((value)&0xff),
            };
        }
        public static ushort ExtractUshort(this byte[] bytes, int position)
        {
            return (ushort)((bytes[position] << 8) + bytes[position + 1]);
        }
        public static uint ExtractUint(this byte[] bytes, int position)
        {
            return (uint)((bytes[position] << 24) + (bytes[position + 1] << 16) 
                + (bytes[position + 2] << 8) + bytes[position + 3]);
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
