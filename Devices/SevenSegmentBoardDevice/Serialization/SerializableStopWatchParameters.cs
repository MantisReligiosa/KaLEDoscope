using System;

namespace SevenSegmentBoardDevice.Serialization
{
    public class SerializableStopWatchParameters
    {
        public TimeSpan CountdownStartValue { get; set; }
        public int CountdownTypeId { get; set; }

        public static explicit operator StopWatchParameters(SerializableStopWatchParameters parameters)
        {
            return new StopWatchParameters
            {
                CountdownStartValue = parameters.CountdownStartValue,
                CountdownTypeId = parameters.CountdownTypeId
            };
        }

        public static explicit operator SerializableStopWatchParameters(StopWatchParameters parameters)
        {
            return new SerializableStopWatchParameters
            {
                CountdownStartValue = parameters.CountdownStartValue,
                CountdownTypeId = parameters.CountdownTypeId
            };
        }
    }
}
