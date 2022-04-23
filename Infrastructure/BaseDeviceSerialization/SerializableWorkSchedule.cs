using BaseDevice;
using System;

namespace BaseDeviceSerialization
{
    public class SerializableWorkSchedule
    {
        public bool AllWeek { get; set; }
        public bool AroundTheClock { get; set; }
        public TimeSpan FinishTo { get; set; }
        public bool RunInFri { get; set; }
        public bool RunInMon { get; set; }
        public bool RunInSat { get; set; }
        public bool RunInSun { get; set; }
        public bool RunInThu { get; set; }
        public bool RunInTue { get; set; }
        public bool RunInWed { get; set; }
        public TimeSpan StartFrom { get; set; }

        public static explicit operator SerializableWorkSchedule(WorkSchedule workSchedule)
        {
            return new SerializableWorkSchedule
            {
                AllWeek = workSchedule.AllWeek,
                AroundTheClock = workSchedule.AroundTheClock,
                FinishTo = workSchedule.FinishTo,
                RunInFri = workSchedule.RunInFri,
                RunInMon = workSchedule.RunInMon,
                RunInSat = workSchedule.RunInSat,
                RunInSun = workSchedule.RunInSun,
                RunInThu = workSchedule.RunInThu,
                RunInTue = workSchedule.RunInTue,
                RunInWed = workSchedule.RunInWed,
                StartFrom = workSchedule.StartFrom
            };
        }

        public static explicit operator WorkSchedule(SerializableWorkSchedule serializableWorkSchedule)
        {
            return new WorkSchedule
            {
                AllWeek = serializableWorkSchedule.AllWeek,
                AroundTheClock = serializableWorkSchedule.AroundTheClock,
                FinishTo = serializableWorkSchedule.FinishTo,
                RunInFri = serializableWorkSchedule.RunInFri,
                RunInMon = serializableWorkSchedule.RunInMon,
                RunInSat = serializableWorkSchedule.RunInSat,
                RunInSun = serializableWorkSchedule.RunInSun,
                RunInThu = serializableWorkSchedule.RunInThu,
                RunInTue = serializableWorkSchedule.RunInTue,
                RunInWed = serializableWorkSchedule.RunInWed,
                StartFrom = serializableWorkSchedule.StartFrom
            };
        }
    }
}
