using System.Net;
using System.Net.Sockets;

namespace CommandProcessing
{
    internal class UdpState
    {
        public UdpState()
        {
        }

        public IPEndPoint e { get; internal set; }
        public UdpClient u { get; internal set; }
    }
}