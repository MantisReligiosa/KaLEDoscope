using DomainData;

namespace ServiceInterfaces
{
    public abstract class Command<TDevice, TProtocol> : ICommand
        where TDevice : Device
        where TProtocol : IProtocol
    {
        internal readonly TDevice _device;
        internal readonly IProtocol _protocol;
        public Command(TDevice device)
        {
            _device = device;
        }
        public abstract void Execute();
    }
}
