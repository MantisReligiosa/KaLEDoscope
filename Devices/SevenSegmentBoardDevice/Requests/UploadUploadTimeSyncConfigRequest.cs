using CommandProcessing;
using SmartTechnologiesM.Base.Extensions;
using System.Collections.Generic;

namespace SevenSegmentBoardDevice.Requests
{
    public class UploadUploadTimeSyncConfigRequest : Request
    {
        public override byte RequestID => 0x13;

        public override byte[] MakeData(object o)
        {
            var parameters = o as TimeSyncParameters;
            var result = new List<byte>
            {
                (byte)parameters.SourceId,
                (byte)parameters.ZoneId.Length
            };
            result.AddRange(parameters.ZoneId.ToBytes());
            result.Add((byte)parameters.ServerAddress.Length);
            result.AddRange(parameters.ServerAddress.ToBytes());
            result.AddRange(((ushort)parameters.ServerPort).ToBytes());
            result.Add((byte)parameters.SyncPeriod.Hours);
            result.Add((byte)parameters.SyncPeriod.Minutes);
            return result.ToArray();
        }
    }
}
