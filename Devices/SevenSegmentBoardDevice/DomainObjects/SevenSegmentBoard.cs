using BaseDevice;
using System.Collections.Generic;

namespace SevenSegmentBoardDevice
{
    public class SevenSegmentBoard : Device
    {
        public List<Alarm> AlarmSchedule { get; set; }
        public BoardType BoardType { get; set; }
        public StopWatchParameters StopWatchParameters { get; set; }
        public TimeSyncParameters TimeSyncParameters { get; set; }
        public List<DisplayFrame> DisplayFrames { get; set; }
    }
}
