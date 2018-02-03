using Infrastructure.HashProvider;
using Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class TestCRCFormat
    {
        [Fact]
        public void Should_ReturnCRC()
        {
            IHashProvider provider = new HashProvider();
            var array = new byte[] { 0x00, 0x02, 0x00, 0x02, 0x01, 0x01 };
            var result = provider.GetHash(array).Reverse().ToArray();
            var actual = (result[0] << 8) + result[1];
            Assert.Equal(0x1793, actual);
        }
    }
}
