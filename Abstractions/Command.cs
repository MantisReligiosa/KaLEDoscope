namespace Abstractions
{
    public abstract class Command
    {
        public Command(IProtocol protocol)
        { }

        public abstract void Execute();
    }
}
