using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;
using System;
using System.Collections.Generic;

namespace PixelBoardDevice.Requests
{
    public class UploadFontRequest : Request
    {
        public override byte RequestID => 0x26;

        public override byte[] MakeData(object o)
        {
            var binaryFont = (BinaryFont)o;
            var bytes = new List<byte>()
            {
                0x01,
                (byte)binaryFont.Id,
                (byte)binaryFont.Height,
                (byte)((binaryFont.Italic ? 2 : 0) + (binaryFont.Bold ? 1 : 0)),
                (byte)binaryFont.Source.Length
            };
            bytes.AddRange(binaryFont.Source.ToBytes());
            bytes.AddRange(((ushort)(binaryFont.Alphabet.Length)).ToBytes());
            bytes.AddRange(binaryFont.Alphabet.ToBytes());
            var bitmap = Convert.FromBase64String(binaryFont.Base64Bitmap);
            bytes.AddRange(((ushort)(bitmap.Length)).ToBytes());
            bytes.AddRange(bitmap);
            return bytes.ToArray();
        }
    }
}
