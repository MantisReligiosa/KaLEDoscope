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
        ControlsPack GetControlsPack(Device device, ILogger logger);
    }
}
