using DomainData;
using ServiceInterfaces;

namespace Abstractions
{
    public abstract class Command<TDevice, TProtocol> : ICommand
        where TDevice : Device
        where TProtocol : IProtocol
    {
        private readonly TDevice _device;
        private readonly IProtocol _protocol;
        public Command(TDevice device, TProtocol protocol)
        {
            _device = device;
            _protocol = protocol;
        }
        public abstract void Execute();
    }
}
