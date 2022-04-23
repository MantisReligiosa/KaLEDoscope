using BaseDevice;
using System;

namespace ServiceInterfaces
{
    public interface IDeviceCommand<out TDevice>
        where TDevice : Device
    {
        void Execute();
        string Name { get; }
        TDevice Device { get; }

        event EventHandler<SuccessCommandEventArgs> Success;
        event EventHandler<ExceptionEventArgs> Error;
        event EventHandler Repeat;
    }
}
