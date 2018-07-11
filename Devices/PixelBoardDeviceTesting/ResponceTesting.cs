using PixelBoardDevice.DomainObjects.Zones;
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

        [Fact]
        public void ZoneResponce_Text_FromByteSequence()
        {
            var responce = new ZoneResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0xaa, 0x25, 0x00, 0x17, 0x10, 0x01, 0x00, 0x64, 0x00, 0x32, 0x01, 0x00,
                0x00, 0x40, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x01, 0x05, 0x00, 0x04, 0x54, 0x65,
                0x78, 0x74
            });
            var zone = responce.Cast() as TextZone;
            Assert.Equal(16, zone.Id);
            Assert.Equal(1, zone.ProgramId);
            Assert.Equal(100, zone.X);
            Assert.Equal(50, zone.Y);
            Assert.Equal(256, zone.Width);
            Assert.Equal(64, zone.Height);
            Assert.Equal("Zone", zone.Name);
            Assert.Equal(1, zone.ZoneType);
            Assert.Equal(5, zone.FontId);
            Assert.Equal(0, zone.Alignment);
            Assert.Equal("Text", zone.Text);
        }

        [Fact]
        public void ZoneResponce_Sensor_FromByteSequence()
        {
            var responce = new ZoneResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0xaa, 0x25, 0x00, 0x12, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x02, 0x06, 0x01
            });
            var zone = responce.Cast() as SensorZone;
            Assert.Equal(2, zone.Id);
            Assert.Equal(10, zone.ProgramId);
            Assert.Equal(0, zone.X);
            Assert.Equal(0, zone.Y);
            Assert.Equal(50, zone.Width);
            Assert.Equal(8, zone.Height);
            Assert.Equal("Zone", zone.Name);
            Assert.Equal(2, zone.ZoneType);
            Assert.Equal(6, zone.FontId);
            Assert.Equal(1, zone.Alignment);
        }

        [Fact]
        public void ZoneResponce_Image_FromByteSequence()
        {
            var responce = new ZoneResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0xaa, 0x25, 0x00, 0x11, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x03, 0x07,
            });
            var zone = responce.Cast() as BitmapZone;
            Assert.Equal(2, zone.Id);
            Assert.Equal(10, zone.ProgramId);
            Assert.Equal(0, zone.X);
            Assert.Equal(0, zone.Y);
            Assert.Equal(50, zone.Width);
            Assert.Equal(8, zone.Height);
            Assert.Equal("Zone", zone.Name);
            Assert.Equal(3, zone.ZoneType);
            Assert.Equal(7, zone.BinaryImageId);
        }

        [Fact]
        public void ZoneResponce_Tag_FromByteSequence()
        {
            var responce = new ZoneResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0xaa, 0x25, 0x00, 0x1b, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x04, 0x07, 0x02, 0x08, 0x4d, 0x61, 0x69,
                0x6e, 0x2e, 0x54, 0x61, 0x67,
            });
            var zone = responce.Cast() as TagZone;
            Assert.Equal(2, zone.Id);
            Assert.Equal(10, zone.ProgramId);
            Assert.Equal(0, zone.X);
            Assert.Equal(0, zone.Y);
            Assert.Equal(50, zone.Width);
            Assert.Equal(8, zone.Height);
            Assert.Equal("Zone", zone.Name);
            Assert.Equal(4, zone.ZoneType);
            Assert.Equal(7, zone.FontId);
            Assert.Equal(2, zone.Alignment);
            Assert.Equal("Main.Tag", zone.ExternalSourceTag);
        }

        [Fact]
        public void ZoneResponce_ClockText_FromByteSequence()
        {
            var responce = new ZoneResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0xaa, 0x25, 0x00, 0x13, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x05, 0x06, 0x00, 0x1e
            });
            var zone = responce.Cast() as ClockZone;
            Assert.Equal(2, zone.Id);
            Assert.Equal(10, zone.ProgramId);
            Assert.Equal(0, zone.X);
            Assert.Equal(0, zone.Y);
            Assert.Equal(50, zone.Width);
            Assert.Equal(8, zone.Height);
            Assert.Equal("Zone", zone.Name);
            Assert.Equal(5, zone.ZoneType);
            Assert.Equal(1, zone.ClockType);
            Assert.Equal(1, zone.ClockFormat);
            Assert.True(zone.AllowPeriodicTimeSync);
            Assert.False(zone.AllowScheduledSync);
            Assert.Equal(30, zone.PeriodicSyncInterval);
        }

        [Fact]
        public void ZoneResponce_ClockImage_FromByteSequence()
        {
            var responce = new ZoneResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0xaa, 0x25, 0x00, 0x13, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x05, 0x1d, 0x01, 0x1e
            });
            var zone = responce.Cast() as ClockZone;
            Assert.Equal(2, zone.Id);
            Assert.Equal(10, zone.ProgramId);
            Assert.Equal(0, zone.X);
            Assert.Equal(0, zone.Y);
            Assert.Equal(50, zone.Width);
            Assert.Equal(8, zone.Height);
            Assert.Equal("Zone", zone.Name);
            Assert.Equal(5, zone.ZoneType);
            Assert.Equal(2, zone.ClockType);
            Assert.False(zone.AllowPeriodicTimeSync);
            Assert.True(zone.AllowScheduledSync);
            Assert.Equal(1, zone.ScheduledTimeSync.Hours);
            Assert.Equal(30, zone.ScheduledTimeSync.Minutes);
        }

        [Fact]
        public void ZoneResponce_CountdownTicker_FromByteSequence()
        {
            var responce = new ZoneResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0xaa, 0x25, 0x00, 0x16, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x06, 0x01, 0x00, 0x05, 0x14, 0x02,
                0x2b
            });
            var zone = responce.Cast() as TickerZone;
            Assert.Equal(2, zone.Id);
            Assert.Equal(10, zone.ProgramId);
            Assert.Equal(0, zone.X);
            Assert.Equal(0, zone.Y);
            Assert.Equal(50, zone.Width);
            Assert.Equal(8, zone.Height);
            Assert.Equal("Zone", zone.Name);
            Assert.Equal(6, zone.ZoneType);
            Assert.Equal(2, zone.TickerType);
            Assert.Equal(0, zone.TickerCountDownStartValue.Hours);
            Assert.Equal(5, zone.TickerCountDownStartValue.Minutes);
            Assert.Equal(20, zone.TickerCountDownStartValue.Seconds);
            Assert.Equal(555, zone.TickerCountDownStartValue.Milliseconds);
        }

        [Fact]
        public void BinaryImageResponce_FromByteSequence()
        {
            var responce = new BinaryImageResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0xaa, 0x25, 0x00, 0x14, 0x02, 0x00, 0x10, 0x00, 0x0f, 0x01, 0x02, 0x03,
                0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f
            });
            var image = responce.Cast();
            Assert.Equal(2, image.Id);
            Assert.Equal(16, image.Height);
            Assert.Equal(Convert.ToBase64String(new byte[]
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c,
                0x0d, 0x0e, 0x0f
            }), image.Base64String);
        }
    }
}
