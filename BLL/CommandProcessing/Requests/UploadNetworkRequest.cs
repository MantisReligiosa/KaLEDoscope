using BaseDevice;
using SmartTechnologiesM.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandProcessing.Requests
{
    public class UploadNetworkRequest : Request
    {
        public override byte RequestID => 0x02;

        public override byte[] MakeData(object o)
        {
            var network = o as Network;
            var bytes = new List<byte>();
            var ip = network.IpAddress.Split('.');
            bytes.AddRange(ip.Select(b => Convert.ToByte(b)));
            bytes.AddRange(((ushort)network.Port).ToBytes());
            bytes.AddRange(network.SubnetMask.ByteToSubnetMask());
            if (string.IsNullOrWhiteSpace(network.Gateway))
            {
                bytes.AddRange(new byte[] { 0, 0, 0, 0 });
            }
            else
            {
                var gateway = network.Gateway.Split('.');
                bytes.AddRange(gateway.Select(b => Convert.ToByte(b)));
            }
            if (string.IsNullOrWhiteSpace(network.DnsServer))
            {
                bytes.AddRange(new byte[] { 0, 0, 0, 0 });
            }
            else
            {
                var dns = network.DnsServer.Split('.');
                bytes.AddRange(dns.Select(b => Convert.ToByte(b)));
            }
            if (network.AlternativeDnsServer.IsNull())
            {
                bytes.AddRange(new byte[] { 0, 0, 0, 0 });
            }
            else
            {
                var alternateDns = network.AlternativeDnsServer.Split('.');
                bytes.AddRange(alternateDns.Select(b => Convert.ToByte(b)));
            }
            return bytes.ToArray();
        }
    }
}
