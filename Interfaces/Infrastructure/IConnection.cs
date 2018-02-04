using DomainEntities;
using System;

namespace Interfaces.Infrastructure
{
    public interface IConnection
    {
        void Send(byte[] data);
        event EventHandler<ResponceInformation> ResponceRecieved;
        void Close();
    }
}
