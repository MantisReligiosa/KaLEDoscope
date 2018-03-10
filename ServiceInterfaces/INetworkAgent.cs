using System;

namespace ServiceInterfaces
{
    public interface INetworkAgent
    {
        ILogger Logger { get; set; }
        void SendBroadcast(int port, string requestString);
        void Listen(int port, Action<string> messageHandler);
        void Close();
        void Send(string ipAddress, int port, string requestString);
    }
}
