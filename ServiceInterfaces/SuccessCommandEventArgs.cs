using BaseDevice;
using System;

namespace ServiceInterfaces
{
    public class SuccessCommandEventArgs : EventArgs
    {
        public Device Device{get;set;}
    }
}
