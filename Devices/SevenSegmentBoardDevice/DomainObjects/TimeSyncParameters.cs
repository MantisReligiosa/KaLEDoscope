using System;

namespace SevenSegmentBoardDevice
{
    public class TimeSyncParameters
    {
        public TimeSpan SyncPeriod { get; set; }
        public int SourceId { get; set; }
        public string ZoneId { get; set; }
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public bool IsIpAddress { get; set; }
    }
}
