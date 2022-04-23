using CommandProcessing;
using PixelBoardDevice.DomainObjects;
using SmartTechnologiesM.Base.Extensions;
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
                binaryImage.Id
            };
            bytes.AddRange(binaryImage.Height.ToBytes());
            bytes.AddRange(binaryImage.Width.ToBytes());
            bytes.AddRange(binaryImage.Bytes);
            return bytes.ToArray();
        }
    }
}
