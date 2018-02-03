using Infrastructure.Indexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class IndexerTests
    {
        [Fact]
        public void CheckForStatic()
        {
            var indexer1 = new Indexer();
            var indexer2 = new Indexer();

            var index1 = indexer1.GetIdentifier();
            var index2 = indexer2.GetIdentifier();

            Assert.Equal(0,index1);
            Assert.Equal(1, index2);
        }

        [Fact]
        public void CheckOverflow()
        {
            var indexer = new Indexer();
            indexer.Set(ushort.MaxValue);
            var index = indexer.GetIdentifier();
            Assert.Equal(0, index);
        }
    }
}
