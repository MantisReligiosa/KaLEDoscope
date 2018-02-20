using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkConsole
{
    public class TcpProvider : IProvider
    {
        public event EventHandler<BytesRecievedEventArgs> OnBytesRecieved;

        private TcpClient _tcpClient;
        private TcpListener _tcpListener;

        public void Close()
        {
            _tcpClient?.Close();
            _tcpListener?.Stop();
        }

        public void Connect(IPEndPoint ipEndpoint)
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(ipEndpoint);
        }

        public void Send(byte[] bytes, int length)
        {
            var stream = _tcpClient.GetStream();
            stream.Write(bytes, 0, length);
        }

        public void StartListen(IPEndPoint endPoint)
        {
            _tcpListener = new TcpListener(endPoint);
            var s = new TcpState();
            s.e = endPoint;
            s.l = _tcpListener;
            _tcpListener.Start();
            _tcpListener.BeginAcceptSocket(new AsyncCallback(DoAcceptSocketCallback), s);
        }

        public void DoAcceptSocketCallback(IAsyncResult ar)
        {
            try
            {
                TcpListener listener = ((TcpState)(ar.AsyncState)).l;
                IPEndPoint e = ((TcpState)(ar.AsyncState)).e;

                var client = listener.EndAcceptTcpClient(ar);
                var stream = client.GetStream();
                var data = new byte[1024];
                var recievedBytesAmount = stream.Read(data, 0, data.Length);
                var recievedBytes = new byte[recievedBytesAmount];
                Array.Copy(data, recievedBytes, recievedBytesAmount);
                OnBytesRecieved?.Invoke(this, new BytesRecievedEventArgs
                {
                    Bytes = recievedBytes,
                    SenderAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()
                });

                var s = new TcpState();
                s.e = e;
                s.l = listener;
                _tcpListener.BeginAcceptSocket(new AsyncCallback(DoAcceptSocketCallback), s);
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                throw new Exception("TCP error", ex);
            }
        }
    }
}
