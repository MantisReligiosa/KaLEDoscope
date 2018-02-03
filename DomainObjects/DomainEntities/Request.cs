using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainEntities
{
    public class Request
    {
        public byte Command { get; set; }
        public byte UnitId { get; set; }
        public byte[] Parameters { get; set; }
    }
}
