using SmartTechnologiesM.Base;
using System;
using System.Collections.Generic;

namespace BaseDevice
{
    public class Brightness
    {
        public BrightnessMode Mode { get; set; }
        public ushort ManualValue { get; set; }
        public ushort MaxValue => 100;
        public ushort Increment => 10;
        public List<BrightnessPeriod> BrightnessPeriods { get; set; }
    }

    public class BrightnessPeriod : Notified
    {
        public TimeSpan From { get; set; }

        public string PeriodStart
        {
            get
            {
                return From.ToString(@"hh\:mm");
            }
            set
            {
                if (!TimeSpan.TryParse(value, out TimeSpan timeSpan))
                {
                    timeSpan = new TimeSpan(0, 0, 0);
                }
                From = timeSpan;
                OnPropertyChanged(nameof(PeriodStart));
            }
        }

        public TimeSpan To { get; set; }
        public string PeriodEnd
        {
            get
            {
                return To.ToString(@"hh\:mm");
            }
            set
            {
                if (!TimeSpan.TryParse(value, out TimeSpan timeSpan))
                {
                    timeSpan = new TimeSpan(0, 0, 0);
                }
                To = timeSpan;
                OnPropertyChanged(nameof(PeriodEnd));
            }
        }


        public int Value
        {
            get;
            set;
        }
    }

    public enum BrightnessMode
    {
        Auto = 0,
        Manual = 1,
        Scheduled = 2
    }
}
