using System;

namespace ServiceInterfaces
{
    public interface INetworkAgent
    {
        ILogger Logger { get; set; }
        void SendBroadcast(int port, Request request);
        void Listen<TResponce, T>(int port, Action<TResponce> responceHandler)
            where TResponce : Responce<T>, new()
            where T : class, new();
        void Close();
        void Send(string ipAddress, int port, Request request);
    }
}
