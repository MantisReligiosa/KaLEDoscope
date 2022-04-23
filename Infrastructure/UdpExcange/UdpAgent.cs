using ServiceInterfaces;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace UdpExcange
{
    public class UdpAgent : INetworkAgent
    {
        public ILogger Logger { get; set; }
        private UdpClient _udpClient;
        private bool _isClosed = false;
        private readonly IPAddress _ip;

        public UdpAgent(int scanInterfaceIndex)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddresses = host.AddressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray();
            var ipAddressesAmount = ipAddresses.Length;
            _ip = ipAddresses[scanInterfaceIndex < ipAddressesAmount - 1 ? scanInterfaceIndex : ipAddressesAmount - 1];
        }

        private void SendBroadcast(int port, byte[] bytes)
        {
            
            _isClosed = false;

            var endPoint = new IPEndPoint(IPAddress.Broadcast, port);
            Logger.Debug(this, $"Отправка через {_ip}");
            _udpClient = new UdpClient(new IPEndPoint(_ip, 0));
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
            Logger.Debug(this, nameof(Listen));
            _isClosed = false;
            var endPoint = new IPEndPoint(_ip, port);
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
                        Logger.Debug(this, "UDP клиент закрыт");
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
            Logger.Debug(this, $"Ожидаю ответы устройств на {_ip}");
            _udpClient.BeginReceive(asyncCallback, udpState);
        }

        public void Send(string ipAddress, int port, IRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
