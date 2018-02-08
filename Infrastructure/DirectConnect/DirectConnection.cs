using ServiceInterfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DirectConnect
{
    public class DirectConnection : IProtocol
    {
        private readonly TcpClient _tcpClient;
        private readonly string IpAddess;
        private readonly int Port;

        public DirectConnection()
        {
            _tcpClient = new TcpClient();
        }

        public DirectConnection(string ipAddress, int port, int recieveTimeout = 0, int sendTimeout = 0)
            : this()
        {
            IpAddess = ipAddress;
            Port = port;
            _tcpClient.ReceiveTimeout = recieveTimeout;
            _tcpClient.SendTimeout = sendTimeout;
        }

        public void EstablishConnection()
        {
            _tcpClient.Connect(new IPEndPoint(IPAddress.Parse(IpAddess), Port));
        }

        public ResponceArguments GetValue(RequestArguments arguments)
        {
            Send(arguments.ToRequestSequence());
            var responce = Recieve();
            return ResponceArguments.Parse(responce);
        }

        public ResponceArguments SetValue(RequestArguments arguments)
        {
            Send(arguments.ToRequestSequence());
            return new ResponceArguments();
        }

        private void Send(string message)
        {
            NetworkStream tcpStream = _tcpClient.GetStream();
            byte[] sendBytes = Encoding.UTF8.GetBytes(message);
            tcpStream.Write(sendBytes, 0, sendBytes.Length);
        }

        private string Recieve()
        {
            NetworkStream tcpStream = _tcpClient.GetStream();
            byte[] bytes = new byte[_tcpClient.ReceiveBufferSize];
            tcpStream.Read(bytes, 0, _tcpClient.ReceiveBufferSize);
            string returnData = Encoding.UTF8.GetString(bytes);
            return returnData;
        }

        public void Dispose()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                _tcpClient.Close();
            }
        }
    }
}
