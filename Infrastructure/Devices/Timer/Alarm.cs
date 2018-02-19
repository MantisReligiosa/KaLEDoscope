using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer
{
    public class Alarm
    {
        public bool IsActive { get; set; }
        public TimeSpan StartTimeSpan { get; set; }
        public TimeSpan Period { get; set; }
    }
}
