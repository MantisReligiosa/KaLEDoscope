using CommandProcessing;
using Extensions;

namespace SevenSegmentBoardDevice.Responces
{
    public class TimeSyncParametersResponce : Responce<TimeSyncParameters>
    {
        public override byte ResponceID => 0x13;

        public override TimeSyncParameters Cast()
        {
            var zoneNameLength = _bytes[6];
            var ipAddressNameLength = _bytes[7 + zoneNameLength];
            return new TimeSyncParameters
            {
                SourceId = _bytes[5],
                ZoneId = _bytes.ExtractString(7, zoneNameLength),
                ServerAddress = _bytes.ExtractString(8 + zoneNameLength, ipAddressNameLength),
                ServerPort = _bytes.ExtractUshort(8 + zoneNameLength + ipAddressNameLength),
                SyncPeriod = new System.TimeSpan(
                    _bytes[10 + zoneNameLength + ipAddressNameLength],
                    _bytes[11 + zoneNameLength + ipAddressNameLength], 0)
            };
        }
    }
}
