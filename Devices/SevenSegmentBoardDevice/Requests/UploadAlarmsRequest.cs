using CommandProcessing;
using System.Collections.Generic;

namespace SevenSegmentBoardDevice.Requests
{
    public class UploadAlarmsRequest : Request
    {
        public override byte RequestID => 0x11;

        public override byte[] MakeData(object o)
        {
            var alarms = o as List<Alarm>;
            var result = new byte[1 + alarms.Count * 5];
            result[0] = (byte)alarms.Count;
            var counter = 0;
            foreach (var alarm in alarms)
            {
                result[1 + counter * 5] = (byte)(alarm.IsActive ? 1 : 0);
                result[2 + counter * 5] = (byte)(alarm.StartTimeSpan.Hours);
                result[3 + counter * 5] = (byte)(alarm.StartTimeSpan.Minutes);
                result[4 + counter * 5] = (byte)(alarm.Period.Hours);
                result[5 + counter * 5] = (byte)(alarm.Period.Minutes);
                counter++;
            }
            return result;
        }
    }
}
