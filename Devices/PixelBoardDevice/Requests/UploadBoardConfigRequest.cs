using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;
using System.Collections.Generic;

namespace PixelBoardDevice.Requests
{
    public class UploadBoardConfigRequest : Request
    {
        public override byte RequestID => 0x20;

        public override byte[] MakeData(object o)
        {
            var boardSize = o as BoardSize;
            var bytes = new List<byte>();
            bytes.AddRange(((ushort)boardSize.Width).ToBytes());
            bytes.AddRange(((ushort)boardSize.Height).ToBytes());

            return bytes.ToArray();
        }
    }
}
