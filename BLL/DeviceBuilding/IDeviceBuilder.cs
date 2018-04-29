using BaseDevice;
using ServiceInterfaces;

namespace DeviceBuilding
{
    public interface IDeviceBuilder
    {
        string Model { get; }
        string DisplayName { get; }
        Device UpdateCustomSettings(Device device);
        ControlsPack GetControlsPack(Device device, ILogger logger);
        string SerializeDevice(Device device);
        Device DeserializeDevice(string text);
        object GetSerializable(Device device);
        Device FromSerializable(object serializableDevice);
    }
}
