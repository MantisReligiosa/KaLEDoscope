using Interfaces.Infrastructure;
using DomainEntities;
using System;
using System.Net;
using System.Net.Sockets;

namespace Infrastructure.UdpConnection
{
    public class UdpConnection : IConnection
    {
        private bool _isBroadcast = false;

        internal UdpClient _udpClient;
        internal IPEndPoint _endpoint;
        private int _port;

        public UdpConnection(int port)
        {
            _port = port;
            _endpoint = new IPEndPoint(IPAddress.Broadcast, port);
            _udpClient = new UdpClient();
            _udpClient.Connect(_endpoint);
            _udpClient.EnableBroadcast = true;
            _isBroadcast = true;
        }

        public UdpConnection(string address, int port)
        {
            _endpoint = new IPEndPoint(IPAddress.Parse(address), port);
            _udpClient = new UdpClient(_endpoint);
        }

        public event EventHandler<ResponceInformation> ResponceRecieved;

        public void Send(byte[] data)
        {
            _udpClient.Send(data, data.Length);
            _udpClient.Close();
            _endpoint = new IPEndPoint(IPAddress.Any, _port);
            _udpClient = new UdpClient(_endpoint);
            _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), this);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var client = ((UdpConnection)(ar.AsyncState))._udpClient;
            var endPoint = ((UdpConnection)(ar.AsyncState))._endpoint;
            var bytes = client.EndReceive(ar, ref endPoint);
            ResponceRecieved?.Invoke(this, new ResponceInformation
            {
                Data = bytes,
                SenderAddress = endPoint.Address.ToString(),
                SenderPort = endPoint.Port
            });
            _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), this);
        }

        public void Close()
        {
            _udpClient.Close();
        }
    }
}
