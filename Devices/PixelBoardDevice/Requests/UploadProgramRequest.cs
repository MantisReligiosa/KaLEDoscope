using CommandProcessing;
using PixelBoardDevice.DomainObjects;
using SmartTechnologiesM.Base.Extensions;
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
                program.Id,
                (byte)(program.Name.Length)
            };
            bytes.AddRange(program.Name.ToBytes());
            bytes.Add(program.Order);
            bytes.AddRange(program.Period.ToBytes());
            return bytes.ToArray();
        }
    }
}
