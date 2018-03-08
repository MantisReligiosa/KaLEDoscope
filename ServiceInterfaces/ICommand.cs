using BaseDevice;
namespace ServiceInterfaces
{
    public interface ICommand
    {
        void Execute();
        void Finally();
        string Name { get; }
        Device Device { get; }
    }
}