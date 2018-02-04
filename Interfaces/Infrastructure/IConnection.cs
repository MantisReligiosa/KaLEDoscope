using DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Infrastructure
{
    public interface IConnection
    {
        void Send(byte[] data);
        event EventHandler<byte[]> RecponceRecieved;
    }
}
