using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.HashFunction;

namespace ProtocolProcessor
{
    public class Protocol
    {
        public byte[] CRC(byte[] data)
        {
            var settings = new CRC.Setting(16, 0x3d65, 0, true, true, 0xffff);
            var crc = new CRC(settings).ComputeHash(data);
            return crc;
        }
    }
}
