using ServiceInterfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpExcange
{
    public class UdpAgent : INetworkAgent
    {
#warning Почему public?
        public ILogger Logger { get; set; }
        private UdpClient _udpClient;
        private bool _isClosed = false;

        private void SendBroadcast(int port, byte[] bytes)
        {
            _isClosed = false;
            var endPoint = new IPEndPoint(IPAddress.Broadcast, port);
            _udpClient = new UdpClient();
            _udpClient.Connect(endPoint);
            _udpClient.Send(bytes, bytes.Length);
            Close();
        }

        public void SendBroadcast(int port, IRequestBuilder requestBuilder)
        {
            Logger.Debug(this, $"Широковещательный запрос по UDP, порт {port}: {requestBuilder.GetString()}");
            SendBroadcast(port, requestBuilder.GetBytes());
        }

        public void Close()
        {
            _udpClient?.Close();
            _isClosed = true;
            _messageHandler = null;
        }

        private Action<string> _messageHandler;
        public void Listen(int port, Action<string> messageHandler)
        {
            _isClosed = false;
            _messageHandler = messageHandler;
            var endPoint = new IPEndPoint(IPAddress.Any, port);
            _udpClient = new UdpClient(endPoint);
            var udpState = new UdpState
            {
                IpEndPoint = endPoint,
                UdpClient = _udpClient
            };
            _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpState);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (_isClosed)
                {
                    return;
                }
                var udpClient = ((UdpState)(ar.AsyncState)).UdpClient;
                var ipEndPoint = ((UdpState)(ar.AsyncState)).IpEndPoint;
                var recieveBytes = udpClient.EndReceive(ar, ref ipEndPoint);
                var recieveString = Encoding.UTF8.GetString(recieveBytes);
                _messageHandler?.Invoke(recieveString);
                var udpState = new UdpState
                {
                    IpEndPoint = ipEndPoint,
                    UdpClient = udpClient
                };
                udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpState);
            }
            catch (Exception ex)
            {
                Logger.Error(this, "UDP error", ex);
            }
        }

        public void Send(string ipAddress, int port, IRequestBuilder requestBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
