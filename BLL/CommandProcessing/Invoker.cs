using BaseDevice;
using Extensions;
using ServiceInterfaces;
using System;

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
                ((!command.Device.IsNull()) ? $" устройства {command.Device.Name}" : String.Empty));
            command.Execute();
        }
    }
}
