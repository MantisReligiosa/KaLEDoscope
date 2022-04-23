using BaseDevice;
using System;

namespace ServiceInterfaces
{
    public class SuccessCommandEventArgs : EventArgs
    {
        public Device Device{get;set;}
        public IDeviceCommand<Device> Command { get; set; }
    }
}
