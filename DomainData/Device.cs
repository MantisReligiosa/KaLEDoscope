using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainData
{
    public class Device
    {
        public string Name { get; set; }
        public WorkSchedule WorkSchedule { get; set; }
        public Brightness Brightness { get; set; }
        public Network Network { get; set; }
        public int Id { get; set; }
    }
}
