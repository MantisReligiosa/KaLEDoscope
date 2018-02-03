using Bll.Frames;
using Infrastructure.HashProvider;
using Interfaces.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class CommandBuilderTest
    {
        [Fact]
        public void CheckForCorrectCommandBuilding()
        {
            var indexer = new Mock<IIndexer>();
            indexer.Setup(i => i.GetIdentifier()).Returns(() => 2);
            IHashProvider hashProvider = new HashProvider();
            var commandBuilder = new RequestBuilder(indexer.Object, hashProvider);
            var command = commandBuilder.Build(1, 1, new byte[] { });
            Assert.Equal(new byte[] {
                0x00, 0x02, 0x00, 0x02, 0x01, 0x01,0x17,0x93
            }, command);
        }
    }
}
