using DomainData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer
{
    public class TimerDevice : Device
    {
        public int BoardTypeId { get; set; }
        public int DisplayFormatId { get; set; }
        public int FontTypeId { get; set; }
    }
}
