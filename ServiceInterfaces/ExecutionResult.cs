using BaseDevice;

namespace ServiceInterfaces
{
    public abstract class ExecutionResult<TDevice>
        where TDevice : Device
    {
        internal TDevice _device;
        public ExecutionResult(TDevice device)
        {
            _device = device;
        }
    }
}
