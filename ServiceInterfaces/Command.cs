using DomainData;

namespace ServiceInterfaces
{
    public abstract class Command<TDevice> : ICommand
        where TDevice : Device
    {
        protected readonly TDevice _device;
        protected readonly IProtocol _protocol;
        protected ILogger _logger;
        protected Command(TDevice device, ILogger logger)
        {
            _device = device;
            _logger = logger;
        }
        public abstract void Execute();
    }
}
