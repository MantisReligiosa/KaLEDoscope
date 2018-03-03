using System.Net;
using System.Net.Sockets;

namespace CommandProcessing
{
    internal class TcpState
    {
        public IPEndPoint Endpoint;
        public TcpListener TcpListener;
    }
}
