using CommandProcessing;
using Extensions;
using System;

namespace SevenSegmentBoardDevice.Requests
{
    public class UploadUploadTimeSyncConfigRequest : Request
    {
        public override byte RequestID => 0x13;

        public override byte[] MakeData(object o)
        {
            var parameters = o as TimeSyncParameters;
            var result = new byte[7 + parameters.ZoneId.Length + parameters.ServerAddress.Length];
            result[0] = (byte)parameters.SourceId;
            result[1] = (byte)parameters.ZoneId.Length;
            Array.Copy(parameters.ZoneId.ToBytes(), 0, result, 2, parameters.ZoneId.Length);
            result[2 + parameters.ZoneId.Length] = (byte)parameters.ServerAddress.Length;
            Array.Copy(parameters.ServerAddress.ToBytes(), 0, result,
                3 + parameters.ZoneId.Length, parameters.ServerAddress.Length);
            Array.Copy(((ushort)parameters.ServerPort).ToBytes(), 0, result,
                3 + parameters.ZoneId.Length + parameters.ServerAddress.Length, 2);
            result[5 + parameters.ZoneId.Length + parameters.ServerAddress.Length] =
                (byte)parameters.SyncPeriod.Hours;
            result[6 + parameters.ZoneId.Length + parameters.ServerAddress.Length] =
                (byte)parameters.SyncPeriod.Minutes;
            return result;
        }
    }
}
