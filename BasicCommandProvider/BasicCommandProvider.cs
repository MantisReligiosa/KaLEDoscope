using Abstractions;
using DomainData;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicCommandProvider
{
    public class BasicCommandProvider : ICommandContainer
    {
        public Dictionary<Func<Device, IProtocol, bool>, Command> Commands => new Dictionary<Func<Device, IProtocol, bool>, Command>
        {
            { (d,p)=>p is DirectConnect.DirectConnect,new Command() }
        };
    }
}
