using System;

namespace Extensions
{
    public static class SubnetMaskExtension
    {
        public static byte SubnetToByte(this byte[] bytes, int position)
        {
            byte result = 0;
            var mask = new byte[4];
            Array.Copy(bytes, position, mask, 0, 4);
            var counter = 0;
            while (mask[counter] == 0xff)
            {
                counter++;
                result += 8;
            }
            var bitPosition = 1;
            var zeroesCount = 0;
            if (mask[counter] == 0)
            {
                return result;
            }
            while ((mask[counter] & (1 << (bitPosition - 1))) == 0)
            {
                bitPosition++;
                zeroesCount++;
            }
            return (byte)(result + 8 - zeroesCount);
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
