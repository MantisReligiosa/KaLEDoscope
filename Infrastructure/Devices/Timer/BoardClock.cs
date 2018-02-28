using BaseDevice;
using System.Collections.Generic;

namespace Timer
{
    public class BoardClock : Device
    {
        public List<Alarm> AlarmSchedule { get; set; }
        public BoardType BoardType { get; set; }
        public StopWatchParameters StopWatchParameters { get; set; }
        public TimeSyncParameters TimeSyncParameters { get; set; }
    }
}
