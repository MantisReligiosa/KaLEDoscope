using ServiceInterfaces;
using System;
using System.Net;
using System.Net.Sockets;

namespace TcpExcange
{
    public class TcpAgent : INetworkAgent
    {
        public ILogger Logger { get; set; }

        private TcpClient _tcpClient;
        private TcpListener tcpListener;
        private bool _isClosed = false;

        public void Close()
        {
            _isClosed = true;
            _tcpClient?.Close();
            tcpListener?.Stop();
        }

        public void Listen<TResponce, T>(int port, Action<TResponce> responceHandler)
            where TResponce : IResponce<T>, new()
            where T : class
        {
            _isClosed = false;
            tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            var state = new TcpState
            {
                TcpListener = tcpListener
            };
            tcpListener.Start();
            tcpListener.BeginAcceptSocket(new AsyncCallback(ar =>
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
                    var responce = new TResponce();
                    responce.SetByteSequence(recievedBytes);
                    Logger.Debug(this, $"Ответ по TCP: {responce.ToString()}");
                    responceHandler?.Invoke(responce);
                }
                catch (Exception ex)
                {
                    Logger.Error(this, "Ошибка при получении ответа", ex);
                }
            }), state);
        }

        public void Send(string ipAddress, int port, IRequest request)
        {
            Logger.Debug(this, $"Запрос по TCP к {ipAddress}, порт {port}: {request}");
            Send(ipAddress, port, request.GetBytes());
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


        public void SendBroadcast(int port, IRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
