using System.Net;
using System.Net.Sockets;

namespace NetworkConsole
{
    internal class TcpState
    {
        internal IPEndPoint endpoint;
        internal TcpListener listener;

        public TcpState()
        {
        }
    }
}