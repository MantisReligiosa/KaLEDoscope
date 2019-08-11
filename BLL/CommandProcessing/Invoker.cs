using BaseDevice;
using Extensions;
using ServiceInterfaces;

namespace CommandProcessing
{
    public class Invoker
    {
        private readonly ILogger _logger;
        public Invoker(ILogger logger)
        {
            _logger = logger;
        }

        public void Invoke<T>(IDeviceCommand<T> command)
            where T : Device
        {
            _logger.Info(this, $"{command.Name}" +
                ((!command.Device.IsNull()) ? $" устройства {command.Device.Name}" : string.Empty));
            command.Execute();
        }
    }
}
