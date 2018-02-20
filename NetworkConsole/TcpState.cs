using System.Net;
using System.Net.Sockets;

namespace NetworkConsole
{
    internal class TcpState
    {
        internal IPEndPoint e;
        internal TcpListener l;

        public TcpState()
        {
        }
    }
}