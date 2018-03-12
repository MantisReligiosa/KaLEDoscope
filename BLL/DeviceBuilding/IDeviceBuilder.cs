using BaseDevice;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DeviceBuilding
{
    public interface IDeviceBuilder
    {
        string Model { get; }
        Device UpdateCustomSettings(Device device);
        Dictionary<string, Func<Device, ILogger, UserControl>> Controls { get; }
    }
}
