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
        const int READ_BUFFER_SIZE = 1024;
        private byte[] _readBuffer = new byte[READ_BUFFER_SIZE];

        private TcpClient _tcpClient;

        public void Close()
        {
            _tcpClient?.Close();
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
            //TcpListener listener = new TcpListener(endPoint);
            _tcpClient = new TcpClient(endPoint);
            _tcpClient.Connect(endPoint);
            _tcpClient.GetStream().BeginRead(this._readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);
        }

        private void DoRead(IAsyncResult ar)
        {
            int BytesRead;

            BytesRead = this._tcpClient.GetStream().EndRead(ar);

            if (BytesRead < 1)
            {
                return;
            }

            OnBytesRecieved?.Invoke(this, new BytesRecievedEventArgs
            {
                Bytes = _readBuffer,
                SenderAddress = _tcpClient.Client.RemoteEndPoint.ToString()
            });

            _tcpClient.GetStream().BeginRead(this._readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(DoRead), null);
        }
    }
}
