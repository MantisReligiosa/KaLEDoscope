using BaseDevice;
using ServiceInterfaces;
using System;
using System.Collections.Generic;

namespace DeviceBuilding
{
    public interface IDeviceFactory
    {
        void AddBuilder(IDeviceBuilder deviceBuilder);
        Device DeserializeDevice(string text);
        Device FromSerializable(string model, object content);
        List<IDeviceBuilder> GetBuilderList();
        ControlsPack GetControlsPack(Device device, ILogger logger);
        Device GetNewDevice(string model, ushort id);
        object GetSerializable(Device device);
        List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> GetUploadCommands(string model);
        string SerializeDevice(Device device);
        Device Customize(Device device);
        Device Customize(object device);
    }
}
