using Bll.Commands;
using DomainEntities;
using Infrastructure.HashProvider;
using Interfaces.Infrastructure;
using Moq;
using Xunit;

namespace Tests
{
    public class CommandBuilderTest
    {
        [Fact]
        public void CheckForCorrectRequestBuilding()
        {
            var indexer = new Mock<IFrameIndexer>();
            indexer.Setup(i => i.GetIdentifier()).Returns(() => 2);
            IHashProvider hashProvider = new HashProvider();
            var frameBuilder = new FrameBuilder(indexer.Object, hashProvider);

            var request = frameBuilder.BuildRequest(new Frame
            {
                UnitId = 0x10,
                CommandId = 0xaa,
                Data = new byte[] {0x01,0xaa,0xff }
            });
            Assert.Equal(new byte[] {
                0x00, 0x02, 0x00, 0x07, 0x10, 0xaa,0x01,0xaa,0xff, 0x02,0xe0
            }, request);
        }

        [Fact]
        public void CheckForCorrectResponceParcing()
        {
            var indexer = new Mock<IFrameIndexer>();
            indexer.Setup(i => i.GetIdentifier()).Returns(() => 2);
            IHashProvider hashProvider = new HashProvider();
            var frameBuilder = new FrameBuilder(indexer.Object, hashProvider);
            var responce = new byte[] {
                0x00, 0x02, 0x00, 0x07, 0x10, 0xaa,0x01,0xaa,0xff, 0x02,0xe0
            };

            var frame = frameBuilder.ParseResponce(responce);
            Assert.Equal(2, frame.Index);
            Assert.Equal(7, frame.Lenght);
            Assert.Equal(0x10,frame.UnitId);
            Assert.Null(frame.CommandId);
        }
    }
}
