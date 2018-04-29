using System;

namespace SevenSegmentBoardDevice.Serialization
{
    public class SerializableTimeSyncParameters
    {
        public bool IsIpAddress { get; set; }
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public int SourceId { get; set; }
        public TimeSpan SyncPeriod { get; set; }
        public string ZoneId { get; set; }

        public static explicit operator TimeSyncParameters(SerializableTimeSyncParameters parameters)
        {
            return new TimeSyncParameters
            {
                IsIpAddress = parameters.IsIpAddress,
                ServerAddress = parameters.ServerAddress,
                ServerPort = parameters.ServerPort,
                SourceId = parameters.SourceId,
                SyncPeriod = parameters.SyncPeriod,
                ZoneId = parameters.ZoneId
            };
        }

        public static explicit operator SerializableTimeSyncParameters(TimeSyncParameters parameters)
        {
            return new SerializableTimeSyncParameters
            {
                IsIpAddress = parameters.IsIpAddress,
                ServerAddress = parameters.ServerAddress,
                ServerPort = parameters.ServerPort,
                SourceId = parameters.SourceId,
                SyncPeriod = parameters.SyncPeriod,
                ZoneId = parameters.ZoneId
            };
        }
    }
}
