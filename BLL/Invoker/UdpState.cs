using System.Net;
using System.Net.Sockets;

namespace CommandProcessing
{
    internal class UdpState
    {
        public IPEndPoint IpEndPoint { get; internal set; }
        public UdpClient UdpClient { get; internal set; }
    }
}