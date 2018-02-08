using System;

namespace ServiceInterfaces
{
    public interface IProtocol : IDisposable
    {
        void EstablishConnection();
        ResponceArguments GetValue(RequestArguments arguments);
        ResponceArguments SetValue(RequestArguments arguments);
    }
}
