using System;
using System.Net;

namespace NetworkConsole
{
    public interface IProvider
    {
        void StartListen(IPEndPoint endPoint);
        event EventHandler<BytesRecievedEventArgs> OnBytesRecieved;
        void Close();
        void Connect(IPEndPoint ipEndpoint);
        void Send(byte[] bytes, int length);
    }
}