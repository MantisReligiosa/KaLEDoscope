using System;

namespace ServiceInterfaces
{
    public interface INetworkAgent
    {
        ILogger Logger { get; set; }
        void SendBroadcast(int port, IRequestBuilder requestBuilder);
        void Listen(int port, Action<string> messageHandler);
        void Close();
        void Send(string ipAddress, int port, IRequestBuilder requestBuilder);
    }
}
