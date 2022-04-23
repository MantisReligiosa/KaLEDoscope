using CommandProcessing;
using System.Collections.Generic;
using System.Linq;

namespace SevenSegmentBoardDevice.Requests
{
    public class UploadAlarmsRequest : Request
    {
        public override byte RequestID => 0x11;

        public override byte[] MakeData(object o)
        {
            var alarms = o as List<Alarm>;
            var result = new List<byte>
            {
                (byte)alarms.Count
            };
            result.AddRange(alarms.SelectMany(alarm => new[]
            {
                (byte)(alarm.IsActive ? 1 : 0),
                (byte)(alarm.StartTimeSpan.Hours),
                (byte)(alarm.StartTimeSpan.Minutes),
                (byte)(alarm.Period.Hours),
                (byte)(alarm.Period.Minutes)
            }));
            return result.ToArray();
        }
    }
}
