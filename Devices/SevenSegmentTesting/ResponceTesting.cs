using SevenSegmentBoardDevice.Responces;
using Xunit;

namespace SevenSegmentTesting
{
    public class ResponceTesting
    {
        [Fact]
        public void BoardTypeResponce_Segments_FromByteSequence()
        {
            var boardTypeResponce = new BoardTypeResponce();
            boardTypeResponce.SetByteSequence(new byte[]
            {
                0xAA, 0xAA, 0x10, 0x00, 0x01, 0x04
            });
            var boardType = boardTypeResponce.Cast();
            Assert.Equal(0, boardType.DisplayType.Id);
            Assert.Equal(4, boardType.DisplayFormat.Capacity);
        }

        [Fact]
        public void BoardTypeResponce_Pixels_FromByteSequence()
        {
            var boardTypeResponce = new BoardTypeResponce();
            boardTypeResponce.SetByteSequence(new byte[]
            {
                0xAA, 0xAA, 0x10, 0x00, 0x01, 0x69
            });
            var boardType = boardTypeResponce.Cast();
            Assert.Equal(1, boardType.DisplayType.Id);
            Assert.Equal(9, boardType.DisplayFormat.Capacity);
            Assert.Equal(2, boardType.FontType.Id);
        }

        [Fact]
        public void AlarmsResponce_FromByteSequence()
        {
            var alarmsResponce = new AlarmsResponce();
            alarmsResponce.SetByteSequence(new byte[]
            {
                0x09, 0x09, 0x11, 0x00, 0x10, 0x03, 0x01, 0x08, 0x00, 0x00, 0x01, 0x01, 0x0A, 0x0F, 0x00,
                0x00, 0x00, 0x0C, 0x00, 0x00, 0x00
            });
            var alarms = alarmsResponce.Cast();
            Assert.Equal(3, alarms.Count);
            Assert.Contains(alarms, a =>
                a.IsActive &&
                a.StartTimeSpan.Hours == 8 && a.StartTimeSpan.Minutes == 0 &&
                a.Period.Hours == 0 && a.Period.Minutes == 1);
            Assert.Contains(alarms, a =>
                a.IsActive &&
                a.StartTimeSpan.Hours == 10 && a.StartTimeSpan.Minutes == 15 &&
                a.Period.Hours == 0 && a.Period.Minutes == 0);
            Assert.Contains(alarms, a =>
                !a.IsActive &&
                a.StartTimeSpan.Hours == 12 && a.StartTimeSpan.Minutes == 00 &&
                a.Period.Hours == 0 && a.Period.Minutes == 0);
        }

        [Fact]
        public void FramesResponce_FromByteSequence()
        {
            var frameResponce = new FramesResponce();
            frameResponce.SetByteSequence(new byte[]
            {
                0x00, 0x08, 0x12, 0x00, 0x07, 0x02, 0x01, 0x00, 0x1E, 0x05, 0x00, 0x05
            });
            var frames = frameResponce.Cast();
            Assert.Equal(2, frames.Count);
            Assert.Contains(frames, f =>
                f.Id == 1 &&
                f.Duration == 30);
            Assert.Contains(frames, f =>
                f.Id == 5 &&
                f.Duration == 5);
        }

        [Fact]
        public void TimeSyncParametersResponce_FromByteSequence()
        {
            var responce = new TimeSyncParametersResponce();
            responce.SetByteSequence(new byte[]
            {
                0x00, 0x11, 0x13, 0x00, 0x2B, 0x03, 0x15, 0x52, 0x75, 0x73, 0x73, 0x69,
                0x61, 0x6E, 0x20, 0x53, 0x74, 0x61, 0x6E, 0x64, 0x61, 0x72, 0x64, 0x20,
                0x54, 0x69, 0x6D, 0x65, 0x0F, 0x74, 0x69, 0x6D, 0x65, 0x2E, 0x73, 0x65,
                0x72, 0x76, 0x65, 0x72, 0x2E, 0x63, 0x6F, 0x6D, 0x30, 0x39, 0x04, 0x00
            });
            var timeSyncParams = responce.Cast();
            Assert.Equal(3, timeSyncParams.SourceId);
            Assert.Equal("Russian Standard Time", timeSyncParams.ZoneId);
            Assert.Equal("time.server.com", timeSyncParams.ServerAddress);
            Assert.Equal(12345, timeSyncParams.ServerPort);
            Assert.Equal(4, timeSyncParams.SyncPeriod.Hours);
            Assert.Equal(0, timeSyncParams.SyncPeriod.Minutes);
        }
    }
}
