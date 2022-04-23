using System;
using System.Net;
using System.Net.Sockets;

namespace NetworkConsole
{
    public class UdpProvider : IProvider
    {
        private UdpClient _udpClient;
        public event EventHandler<BytesRecievedEventArgs> OnBytesRecieved;

        public void Close()
        {
            _udpClient?.Close();
        }

        public void Connect(IPEndPoint ipEndpoint)
        {
            _udpClient = new UdpClient();
            _udpClient.Connect(ipEndpoint);
        }

        public void Send(byte[] bytes, int length)
        {
            _udpClient.Send(bytes, length);
        }

        public void StartListen(IPEndPoint endPoint)
        {
            _udpClient = new UdpClient(endPoint);
            var state = new UdpState
            {
                Endpoint = endPoint,
                Client = _udpClient
            };
            _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), state);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var client = ((UdpState)(ar.AsyncState)).Client;
            var endpoint = ((UdpState)(ar.AsyncState)).Endpoint;

            var receiveBytes = client.EndReceive(ar, ref endpoint);
            OnBytesRecieved?.Invoke(this, new BytesRecievedEventArgs
            {
                Bytes = receiveBytes,
                SenderAddress = endpoint.Address.ToString()
            });

            UdpState state = new UdpState
            {
                Endpoint = endpoint,
                Client = client
            };
            client.BeginReceive(new AsyncCallback(ReceiveCallback), state);
        }
    }
}