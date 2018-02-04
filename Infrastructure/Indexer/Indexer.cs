using Interfaces.Infrastructure;
namespace Infrastructure.Indexer
{
    public class Indexer : IFrameIndexer
    {
        private static ushort _counter;
        public ushort GetIdentifier()
        {
            return _counter++;
        }

        public void Reset()
        {
            Set(default(ushort));
        }

        public void Set(ushort value)
        {
            _counter = ++value;
        }
    }
}
