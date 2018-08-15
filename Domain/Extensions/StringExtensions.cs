using System;
using System.Collections.Generic;

namespace Extensions
{
    public static class StringExtensions
    {
        public static byte[] GetBytesFromHexString(this string bytesStr)
        {
            var bytes = new List<byte>();
            bytesStr = bytesStr.ToUpper();
            bytesStr = bytesStr.Replace("-", "");
            byte b = 0;
            var isFirst = true;
            foreach (var c in bytesStr)
            {
                byte b1;
                if (Char.IsDigit(c))
                    b1 = (byte)(c - '0');
                else
                    b1 = (byte)(c - 'A' + 10);
                if (isFirst)
                {
                    b = (byte)(b1 << 4);
                }
                else
                {
                    b += b1;
                    bytes.Add(b);
                    b = 0;
                }
                isFirst = !isFirst;
            }
            return bytes.ToArray();
        }
    }
}
