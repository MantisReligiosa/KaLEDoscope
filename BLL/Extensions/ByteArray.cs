using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class ByteArray
    {
        public static byte[] Revert(this byte[] value)
        {
            var result = new byte[value.Length];
            for (int i = value.Length - 1; i >= 0; i--)
            {
                result[value.Length - i - 1] = value[i];
            }
            return result;
        }
    }
}
