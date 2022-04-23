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
        protected IConfig _config;
        protected string _commandName;
        protected DeviceCommand(TDevice device, INetworkAgent networkAgent, ILogger logger, IConfig config)
        {
            _device = device;
            _logger = logger;
            _config = config;
            _networkAgent = networkAgent;
            _networkAgent.Logger = _logger;
        }

        public abstract string Name { get; }

        public TDevice Device => _device;

        public event EventHandler<SuccessCommandEventArgs> Success;
        public event EventHandler<ExceptionEventArgs> Error;
        public event EventHandler Repeat;

        public void Execute()
        {
            _logger.Info(this, $"{Name}" + ((Device != null) ? $" устройства {Device.Name}" : string.Empty));
            CommandExecute();
        }

        protected abstract void CommandExecute();

        internal protected void RaiseError(Exception exception)
        {
            Error?.Invoke(this, new ExceptionEventArgs { Exception = exception });
        }

        internal protected void RaiseSuccess()
        {
            Success?.Invoke(this, new SuccessCommandEventArgs { Device = _device, Command = this });
        }

        internal protected void RaiseRepeat()
        {
            Repeat?.Invoke(this, EventArgs.Empty);
        }
    }
}
