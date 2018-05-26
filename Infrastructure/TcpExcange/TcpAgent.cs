using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServiceInterfaces;

namespace TcpExcange
{
    public class TcpAgent : INetworkAgent
    {
        public ILogger Logger { get; set; }

        private TcpClient _tcpClient;
        private TcpListener tcpListener;
        private Action<string> _messageHandler;
        private bool _isClosed = false;

        public void Close()
        {
            _isClosed = true;
            _tcpClient?.Close();
            tcpListener?.Stop();
            _messageHandler = null;
        }

        public void Listen(int port, Action<string> messageHandler)
        {
            _isClosed = false;
            _messageHandler = messageHandler;
            tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            var s = new TcpState
            {
                TcpListener = tcpListener
            };
            tcpListener.Start();
            tcpListener.BeginAcceptSocket(new AsyncCallback(DoAcceptSocketCallback), s);
        }

        private void DoAcceptSocketCallback(IAsyncResult ar)
        {
            var listener = ((TcpState)(ar.AsyncState)).TcpListener;
            try
            {
                if (_isClosed)
                    return;
                var client = listener.EndAcceptTcpClient(ar);
                var stream = client.GetStream();
                var data = new byte[1024];
                var recievedBytesAmount = stream.Read(data, 0, data.Length);
                var recievedBytes = new byte[recievedBytesAmount];
                Array.Copy(data, recievedBytes, recievedBytesAmount);
                var recieveString = Encoding.UTF8.GetString(recievedBytes);
                _messageHandler?.Invoke(recieveString);
            }
            catch (Exception ex)
            {
                Logger.Error(this, "Ошибка при получении ответа", ex);
            }
        }

        public void Send(string ipAddress, int port, IRequestBuilder requestBuilder)
        {
            Logger.Debug(this, $"Запрос по TCP к {ipAddress}, порт {port}: {requestBuilder.GetString()}");
            Send(ipAddress, port, requestBuilder.GetBytes());
        }

        private void Send(string ipAddress, int port, byte[] bytes)
        {
            var ipEndpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            _tcpClient = new TcpClient();
            _tcpClient.Connect(ipEndpoint);
            var stream = _tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
            Close();
        }


        public void SendBroadcast(int port, IRequestBuilder requestBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
