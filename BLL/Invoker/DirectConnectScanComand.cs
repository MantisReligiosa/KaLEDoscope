using DirectConnect;
using DomainData;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandProcessing
{
    public class DirectConnectScanComand : Command<Device, DirectConnection>
    {
        public DirectConnectScanComand() : base(null)
        {

        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
