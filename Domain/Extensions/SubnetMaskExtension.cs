using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class SubnetMaskExtension
    {
        public static bool TrySubnetToByte(this byte[] bytes, int position, out byte subnetMaskByte)
        {
            subnetMaskByte = default(byte);
            var fullByte = (byte)0xff;
            var terminate = false;
            var allowedBytes = new Dictionary<byte, byte>
                {
                    {0,0 },
                    {1,128 },
                    {2,192 },
                    {3,224 },
                    {4,240 },
                    {5,248 },
                    {6,252 },
                    {7,254 }
                };
            if (bytes.Length < 4 + position)
                return false;
            var maskBytes = new byte[4];
            Array.Copy(bytes, position, maskBytes, 0, 4);
            foreach (var b in maskBytes)
            {
                if (terminate && b != 0)
                {
                    return false;
                }
                if (b == fullByte)
                {
                    subnetMaskByte += 8;
                }
                else
                {
                    if (!allowedBytes.Any(kvp => kvp.Value == b))
                    {
                        return false;
                    }
                    var pair = allowedBytes.FirstOrDefault(kvp => kvp.Value == b);
                    terminate = true;
                    subnetMaskByte += pair.Key;
                }
            }
            return true;
        }

        public static byte[] ByteToSubnetMask(this byte subnetByte)
        {
            var mask = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                if (subnetByte > (i + 1) * 8)
                {
                    mask[i] = 0xff;
                }
                else
                {
                    var bitsLeft = subnetByte - i * 8;
                    for (int b = 0; b < bitsLeft; b++)
                    {
                        mask[i] = (byte)(mask[i] >> 1);
                        mask[i] |= 0b10000000;
                    }
                    return mask;
                }
            }
            return mask;
        }
    }
}
