using System.Net;
using System.Net.Sockets;

namespace TcpExcange
{
    internal class TcpState
    {
        public IPEndPoint Endpoint;
        public TcpListener TcpListener;
    }
}
