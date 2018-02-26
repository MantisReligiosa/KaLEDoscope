using DomainData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer
{
    public class BoardClock : Device
    {
        public List<Alarm> AlarmSchedule { get; set; }
        public int BoardTypeId { get; set; }
        public TimeSpan CountdownStartValue { get; set; }
        public int CountdownTypeId { get; set; }
        public int DisplayFormatId { get; set; }
        public int FontTypeId { get; set; }
        public int SyncSourceId { get; set; }
        public TimeSpan TimeSyncPeriod { get; set; }
        public string TimeSyncServerIp { get; set; }
        public int TimeSyncServerPort { get; set; }
        public string TimeZoneId { get; set; }
    }
}
