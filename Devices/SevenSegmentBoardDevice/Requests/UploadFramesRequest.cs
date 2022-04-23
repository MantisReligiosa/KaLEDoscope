using CommandProcessing;
using SmartTechnologiesM.Base.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SevenSegmentBoardDevice.Requests
{
    public class UploadFramesRequest : Request
    {
        public override byte RequestID => 0x12;

        public override byte[] MakeData(object o)
        {
            if (!(o is List<DisplayFrame> frames))
            {
                return new byte[0];
            }
            var result = new List<byte>
            {
                (byte)frames.Count(f => f.IsEnabled)
            };
            foreach (var frame in frames.Where(f => f.IsEnabled))
            {
                result.Add((byte)frame.Id);
                var durationBytes = ((ushort)(frame.DisplayPeriod)).ToBytes();
                result.AddRange(durationBytes);
            }
            return result.ToArray();
        }
    }
}
