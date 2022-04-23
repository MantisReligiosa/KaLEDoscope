using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkConsole
{
    public class BytesRecievedEventArgs : EventArgs
    {
        public byte[] Bytes { get; set; }
        public string SenderAddress { get; set; }
    }
}
