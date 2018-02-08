using System;

namespace DomainData
{
    public class Schedule
    {
        public TimeSpan StartFrom { get; set; }
        public TimeSpan FinishTo { get; set; }
        public bool AroundTheClock { get; set; }
    }
}