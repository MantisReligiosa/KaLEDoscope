using System;

namespace SevenSegmentBoardDevice.Serialization
{
    public class SerializableAlarm
    {
        public bool IsActive { get; private set; }
        public TimeSpan Period { get; private set; }
        public TimeSpan StartTimeSpan { get; private set; }

        public static explicit operator SerializableAlarm(Alarm alarm)
        {
            return new SerializableAlarm
            {
                IsActive = alarm.IsActive,
                Period = alarm.Period,
                StartTimeSpan = alarm.StartTimeSpan
            };
        }

        public static explicit operator Alarm(SerializableAlarm alarm)
        {
            return new Alarm
            {
                IsActive = alarm.IsActive,
                Period = alarm.Period,
                StartTimeSpan = alarm.StartTimeSpan
            };
        }
    }
}
