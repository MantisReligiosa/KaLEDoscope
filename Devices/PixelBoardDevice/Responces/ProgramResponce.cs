using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;
using System;

namespace PixelBoardDevice.Responces
{
    public class ProgramResponce : Responce<Program>
    {
        public override byte ResponceID => 0x25;

        public override Program Cast()
        {
            var nameLength = _bytes[6];
            var program = new Program
            {
                Id = _bytes[5],
                Name = _bytes.ExtractString(7, nameLength),
                Order = _bytes[7 + nameLength],
                Period = _bytes.ExtractUshort(8 + nameLength)
            };
            return program;
        }
    }
}
