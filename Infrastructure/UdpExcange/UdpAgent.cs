using ServiceInterfaces;
using System;
using System.Net;
using System.Net.Sockets;

namespace UdpExcange
{
    public class UdpAgent : INetworkAgent
    {
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

        public void SendBroadcast(int port, IRequest request)
        {
            Logger.Debug(this, $"Широковещательный запрос по UDP, порт {port}: {request.ToString()}");
            SendBroadcast(port, request.GetBytes());
        }

        public void Close()
        {
            _udpClient?.Close();
            _isClosed = true;
        }

        public void Listen<TResponce, T>(int port, Action<TResponce> responceHandler)
            where TResponce : IResponce<T>, new()
            where T : class
        {
            _isClosed = false;
            var endPoint = new IPEndPoint(IPAddress.Any, port);
            _udpClient = new UdpClient(endPoint);
            var udpState = new UdpState
            {
                IpEndPoint = endPoint,
                UdpClient = _udpClient
            };
            var asyncCallback = new AsyncCallback(ar =>
            {
                try
                {
                    if (_isClosed)
                    {
                        return;
                    }
                    var udpClient = ((UdpState)(ar.AsyncState)).UdpClient;
                    var ipEndPoint = ((UdpState)(ar.AsyncState)).IpEndPoint;
                    var recievedBytes = udpClient.EndReceive(ar, ref ipEndPoint);
                    var responce = new TResponce();
                    responce.SetByteSequence(recievedBytes);
                    Logger.Debug(this, $"Ответ по UDP, порт {port}: {responce.ToString()}");
                    _udpClient.Close();
                    responceHandler?.Invoke(responce);
                }
                catch (Exception ex)
                {
                    Logger.Error(this, "UDP error", ex);
                }
            });
            _udpClient.BeginReceive(asyncCallback, udpState);
        }


        public void Send(string ipAddress, int port, IRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
