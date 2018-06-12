using PixelBoardDevice.Responces;
using System;
using Xunit;

namespace PixelBoardDeviceTesting
{
    public class ResponceTesting
    {
        [Fact]
        public void BoardConfigResponce_FromByteSequence()
        {
            var responce = new BoardConfigResponce();
            responce.SetByteSequence(new byte[]
            {
                0x11, 0x11, 0x20, 0x00, 0x04, 0x03, 0x84, 0x00, 0x40
            });
            var boardSize = responce.Cast();
            Assert.Equal(900, boardSize.Width);
            Assert.Equal(64, boardSize.Height);
        }

        [Fact]
        public void IdListResponce_FromByteSequence()
        {
            var responce = new IdListResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0x14, 0x23, 0x00, 0x04, 0x01, 0x02, 0x01, 0x02
            });
            var idList = responce.Cast();
            Assert.Equal(2, idList.Items.Count);
            Assert.Contains(idList.Items, i => i == 1);
            Assert.Contains(idList.Items, i => i == 2);
        }

        [Fact]
        public void FontResponce_FromByteSequence()
        {
            var responce = new FontResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0xaa, 0x25, 0x00, 0x15, 0xaa, 0x0a, 0x03, 0x04, 0x46, 0x6F, 0x6E, 0x74,
                0x00, 0x03, 0x41, 0x42, 0x43, 0x00, 0x06, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff
            });
            var font = responce.Cast();
            Assert.Equal(170, font.Id);
            Assert.Equal(10, font.Height);
            Assert.True(font.Bold);
            Assert.True(font.Italic);
            Assert.Equal("Font", font.Source);
            Assert.Equal("ABC", font.Alphabet);
            Assert.Equal(Convert.ToBase64String(new byte[] { 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff }),
                font.Base64Bitmap);
        }

        [Fact]
        public void ProgramResponce_FromByteSequence()
        {
            var responce = new ProgramResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0xaa, 0x25, 0x00, 0x0c, 0x05, 0x07, 0x50, 0x72, 0x6f, 0x67, 0x72, 0x61,
                0x6d, 0x01, 0xff, 0xff
            });
            var program = responce.Cast();
            Assert.Equal(5, program.Id);
            Assert.Equal("Program", program.Name);
            Assert.Equal(1, program.Order);
            Assert.Equal(65535, program.Period);
        }
    }
}
