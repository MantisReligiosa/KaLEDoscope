using System.Net;
using System.Net.Sockets;

namespace NetworkConsole
{
    internal class UdpState
    {
        internal IPEndPoint Endpoint;
        internal UdpClient Client;
    }
}