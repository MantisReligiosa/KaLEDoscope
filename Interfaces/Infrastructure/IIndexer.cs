namespace Interfaces.Infrastructure
{
    public interface IIndexer
    {
        ushort GetIdentifier();
        void Reset();
        void Set(ushort value);
    }
}
