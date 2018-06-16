using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;
using System;
using System.Collections.Generic;

namespace PixelBoardDevice.Requests
{
    public class UploadBinaryImageRequest : Request
    {
        public override byte RequestID => 0x26;

        public override byte[] MakeData(object o)
        {
            var binaryImage = (BinaryImage)o;
            var bytes = new List<byte>
            {
                0x04,
                (byte)binaryImage.Id
            };
            bytes.AddRange(((ushort)binaryImage.Height).ToBytes());
            var bitmap = Convert.FromBase64String(binaryImage.Base64String);
            bytes.AddRange(((ushort)bitmap.Length).ToBytes());
            bytes.AddRange(bitmap);
            return bytes.ToArray();
        }
    }
}
