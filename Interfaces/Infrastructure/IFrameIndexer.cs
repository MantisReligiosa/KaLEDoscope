namespace Interfaces.Infrastructure
{
    public interface IFrameIndexer
    {
        ushort GetIdentifier();
        void Reset();
        void Set(ushort value);
    }
}
