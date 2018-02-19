using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            var s = new UdpState();
            s.e = endPoint;
            s.u = _udpClient;
            _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), s);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                UdpClient u = ((UdpState)(ar.AsyncState)).u;
                IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

                var receiveBytes = u.EndReceive(ar, ref e);
                OnBytesRecieved?.Invoke(this, new BytesRecievedEventArgs
                {
                    Bytes = receiveBytes,
                    SenderAddress = e.Address.ToString()
                });

                UdpState s = new UdpState();
                s.e = e;
                s.u = u;
                u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                throw new Exception("UDP error", ex);
            }
        }
    }
}
