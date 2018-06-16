using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;
using System;

namespace PixelBoardDevice.Responces
{
    public class BinaryImageResponce : Responce<BinaryImage>
    {
        public override byte ResponceID => 0x25;

        public override BinaryImage Cast()
        {
            var height = _bytes.ExtractUshort(6);
            var length = _bytes.ExtractUshort(8);
            var bitmap = new byte[length];
            Array.Copy(_bytes, 10, bitmap, 0, length);
            return new BinaryImage
            {
                Id = _bytes[5],
                Height = height,
                Base64String = Convert.ToBase64String(bitmap)
        };
    }
}
}
