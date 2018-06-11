using CommandProcessing;
using Extensions;
using System;
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
            var result = new byte[1 + frames.Count(f => f.IsEnabled) * 3];
            result[0] = (byte)frames.Count(f => f.IsEnabled);
            var counter = 0;
            foreach (var frame in frames.Where(f => f.IsEnabled))
            {
                result[1 + counter * 3] = (byte)frame.Id;
                var durationBytes = ((ushort)(frame.DisplayPeriod)).ToBytes();
                Array.Copy(durationBytes, 0, result, 2 + counter * 3, 2);
                counter++;
            }
            return result;
        }
    }
}
