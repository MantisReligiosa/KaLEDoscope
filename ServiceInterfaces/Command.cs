using BaseDevice;

namespace ServiceInterfaces
{
    public abstract class Command<TDevice, TNetworkAgent> : ICommand
        where TDevice : Device
        where TNetworkAgent : INetworkAgent, new()
    {
        protected TDevice _device;
        protected INetworkAgent _networkAgent;
        protected ILogger _logger;
        protected string _commandName;
        protected Command(TDevice device, ILogger logger)
        {
            _device = device;
            _logger = logger;
            _networkAgent = new TNetworkAgent();
            _networkAgent.Logger = _logger;
        }

        public abstract string Name { get; }

        public Device Device => _device;

        public abstract void Execute();
        public abstract void Finally();
    }
}
