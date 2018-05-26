using BaseDevice;
namespace ServiceInterfaces
{
    public interface IDeviceCommand<out TDevice>
        where TDevice : Device
    {
        void Execute();
        void Finally();
        string Name { get; }
        TDevice Device { get; }
    }
}