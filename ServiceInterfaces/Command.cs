using System.Runtime.Remoting.Messaging;
using BaseDevice;

namespace ServiceInterfaces
{
    public abstract class Command<TDevice> : ICommand
        where TDevice : Device
    {
        protected TDevice _device;
        protected ILogger _logger;
        protected string _commandName;
        protected Command(TDevice device, ILogger logger)
        {
            _device = device;
            _logger = logger;
        }

        public abstract string Name { get; }

        public Device Device => _device;

        public abstract void Execute();
        public abstract void Finally();
    }
}
