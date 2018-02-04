using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UdpConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = 57343;
            IPEndPoint e = new IPEndPoint(IPAddress.Any, port);
            UdpClient u = new UdpClient(e);
            var bytes = u.Receive(ref e);
            Console.Write("Recieved: ");
            Console.WriteLine($"IP {e.Address.ToString()}");
            foreach (var b in bytes)
            {
                Console.Write("[{0:X}]", b);
            }
            Console.WriteLine();
            Console.WriteLine("Done");
            u.Close();
            var u1 = new UdpClient("192.168.0.35", port);
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("Sending!");
                u1.Send(new byte[] {
                0x00, 0x02, 0x00, 0x07, 0x10, 0xaa,0x01,0xaa,0xff, 0x02,0xe0
            }, 11);
                Thread.Sleep(10000);
            }
            Console.ReadKey();
        }
    }
}
