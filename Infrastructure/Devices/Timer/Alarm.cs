using System;

namespace SevenSegmentBoardDevice
{
    public class Alarm
    {
        public bool IsActive { get; set; }
        public TimeSpan StartTimeSpan { get; set; }
        public TimeSpan Period { get; set; }
    }
}
