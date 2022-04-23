using CommandProcessing;
using PixelBoardDevice.DomainObjects;
using SmartTechnologiesM.Base.Extensions;
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
            bytes.AddRange(boardSize.Width.ToBytes());
            bytes.AddRange(boardSize.Height.ToBytes());

            return bytes.ToArray();
        }
    }
}
