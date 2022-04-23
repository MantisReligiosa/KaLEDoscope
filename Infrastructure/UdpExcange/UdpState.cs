using System.Net;
using System.Net.Sockets;

namespace UdpExcange
{
    internal class UdpState
    {
        public IPEndPoint IpEndPoint { get; internal set; }
        public UdpClient UdpClient { get; internal set; }
    }
}
