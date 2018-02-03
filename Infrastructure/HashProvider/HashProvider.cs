using Interfaces.Infrastructure;
using System.Data.HashFunction;
using System;

namespace Infrastructure.HashProvider
{
    public class HashProvider : IHashProvider
    {
        public ushort HashLenght
        {
            get { return 2; }
        }

        public byte[] GetHash(byte[] data)
        {
            var settings = new CRC.Setting(16, 0x3d65, 0, true, true, 0xffff);
            return new CRC(settings).ComputeHash(data);
        }
    }
}
