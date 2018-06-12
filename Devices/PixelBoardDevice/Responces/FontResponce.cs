using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;
using System;

namespace PixelBoardDevice.Responces
{
    public class FontResponce : Responce<BinaryFont>
    {
        public override byte ResponceID => 0x25;

        public override BinaryFont Cast()
        {
            var sourceStringLength = _bytes[8];
            var alphabetLength = _bytes.ExtractUshort(9 + sourceStringLength);
            var bitmapLength = _bytes.ExtractUshort(11 + sourceStringLength + alphabetLength);
            return new BinaryFont
            {
                Id = _bytes[5],
                Height = _bytes[6],
                Italic = _bytes[7].GetBit(7),
                Bold = _bytes[7].GetBit(6),
                Source = _bytes.ExtractString(9, sourceStringLength),
                Alphabet = _bytes.ExtractString(11 + sourceStringLength, alphabetLength),
                Base64Bitmap = Convert.ToBase64String(_bytes, 13 + sourceStringLength + alphabetLength, 
                    bitmapLength)
            };
        }
    }
}
