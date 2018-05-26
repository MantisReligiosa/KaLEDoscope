using BaseDevice;

namespace ServiceInterfaces
{
    public abstract class DeviceCommand<TDevice> : IDeviceCommand<TDevice>
        where TDevice : Device
    {
        protected TDevice _device;
        protected INetworkAgent _networkAgent;
        protected IRequestBuilder _requestBuilder;
        protected IResponceProcessor _responceProcessor;
        protected ILogger _logger;
        protected string _commandName;
        protected DeviceCommand(TDevice device, INetworkAgent networkAgent, IRequestBuilder requestBuilder, IResponceProcessor responceProcessor, ILogger logger)
        {
            _device = device;
            _logger = logger;
            _networkAgent = networkAgent;
            _requestBuilder = requestBuilder;
            _responceProcessor = responceProcessor;
            _networkAgent.Logger = _logger;
        }

        public abstract string Name { get; }

        public TDevice Device => _device;

        public abstract void Execute();
        public abstract void Finally();
    }
}
