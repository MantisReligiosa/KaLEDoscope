using System;

namespace ServiceInterfaces
{
    public interface INetworkAgent
    {
        ILogger Logger { get; set; }
        void SendBroadcast(int port, IRequest request);
        void Listen<TResponce, T>(int port, Action<TResponce> responceHandler)
            where TResponce : IResponce<T>, new()
            where T : class;
        void Close();
        void Send(string ipAddress, int port, IRequest request);
    }
}
