using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;
using System.Collections.Generic;

namespace PixelBoardDevice.Requests
{
    public class UploadProgramRequest : Request
    {
        public override byte RequestID => 0x26;

        public override byte[] MakeData(object o)
        {
            var program = (Program)o;
            var bytes = new List<byte>
            {
                0x02,
                (byte)program.Id,
                (byte)(program.Name.Length)
            };
            bytes.AddRange(program.Name.ToBytes());
            bytes.Add((byte)program.Order);
            bytes.AddRange(((ushort)program.Period).ToBytes());
            return bytes.ToArray();
        }
    }
}
