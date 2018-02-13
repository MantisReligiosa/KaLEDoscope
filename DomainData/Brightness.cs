using System;
using System.Collections.Generic;

namespace DomainData
{
    public class Brightness
    {
        public Mode Mode { get; set; }
        public int ManualValue { get; set; }
        public List<BrightnessPeriod> BrightnessPeriods { get; set; }
    }

    public class BrightnessPeriod
    {
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public int Value { get; set; }
    }

    public enum Mode
    {
        Auto,
        Manual,
        Scheduled
    }
}