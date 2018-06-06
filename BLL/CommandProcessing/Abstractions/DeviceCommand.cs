using BaseDevice;
using ServiceInterfaces;
using System;

namespace CommandProcessing
{
    public abstract class DeviceCommand<TDevice> : IDeviceCommand<TDevice>
        where TDevice : Device
    {
        protected TDevice _device;
        protected INetworkAgent _networkAgent;
        protected ILogger _logger;
        protected string _commandName;
        protected DeviceCommand(TDevice device, INetworkAgent networkAgent, ILogger logger)
        {
            _device = device;
            _logger = logger;
            _networkAgent = networkAgent;
            _networkAgent.Logger = _logger;
        }

        public abstract string Name { get; }

        public TDevice Device => _device;

        public event EventHandler<SuccessCommandEventArgs> Success;
        public event EventHandler<ExceptionEventArgs> Error;
        public event EventHandler Repeat;

        public abstract void Execute();

        internal protected void RaiseError(Exception exception)
        {
            Error?.Invoke(this, new ExceptionEventArgs { Exception = exception });
        }

        internal protected void RaiseSuccess()
        {
            Success?.Invoke(this, new SuccessCommandEventArgs { Device = _device });
        }

        internal protected void RaiseRepeat()
        {
            Repeat?.Invoke(this, EventArgs.Empty);
        }
    }
}
