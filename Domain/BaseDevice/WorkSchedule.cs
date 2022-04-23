using System;

namespace BaseDevice
{
    public class WorkSchedule
    {
        public TimeSpan StartFrom { get; set; }
        public TimeSpan FinishTo { get; set; }
        public bool AroundTheClock { get; set; }
        public bool AllWeek { get; set; }
        public bool RunInSun { get; set; }
        public bool RunInMon { get; set; }
        public bool RunInTue { get; set; }
        public bool RunInWed { get; set; }
        public bool RunInThu { get; set; }
        public bool RunInFri { get; set; }
        public bool RunInSat { get; set; }
    }
}
