using BaseDevice;
using System;

namespace ServiceInterfaces
{
    public class SuccessCommendEventArgs : EventArgs
    {
        public Device Device{get;set;}
    }
}
