using BaseDevice;

namespace ServiceInterfaces
{
    public abstract class Command<TDevice> : ICommand
        where TDevice : Device
    {
        protected TDevice _device;
        protected ILogger _logger;
        protected Command(TDevice device, ILogger logger)
        {
            _device = device;
            _logger = logger;
        }
        public abstract void Execute();
    }
}
