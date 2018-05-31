using System;

namespace ServiceInterfaces
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
    }
}
