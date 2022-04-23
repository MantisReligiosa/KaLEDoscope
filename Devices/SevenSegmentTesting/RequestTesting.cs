using SevenSegmentBoardDevice;
using SevenSegmentBoardDevice.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SevenSegmentTesting
{
    public class RequestTesting
    {
        [Fact]
        public void UploadBoardTypeRequest_Segments_ToByteSequence()
        {
            var request = new UploadBoardTypeRequest
            {
                DeviceID = 0xaaaa
            };
            request.SetRequestData(new BoardType
            {
                DisplayType = Refs.DisplayTypes.First(d => d.Id == 0),
                FontType = Refs.FontTypes.First(),
                DisplayFormat = Refs.DisplayFormats.First(d => d.Capacity == 4)
            });
            Assert.Equal(new byte[]
            {
                0xAA, 0xAA, 0x10, 0x00, 0x01, 0x04
            }, request.GetBytes());
        }

        [Fact]
        public void UploadBoardTypeRequest_Pixels_ToByteSequence()
        {
            var request = new UploadBoardTypeRequest
            {
                DeviceID = 0xaaaa
            };
            request.SetRequestData(new BoardType
            {
                DisplayType = Refs.DisplayTypes.First(d => d.Id == 1),
                FontType = Refs.FontTypes.First(f => f.Id == 2),
                DisplayFormat = Refs.DisplayFormats.First(d => d.Capacity == 9)
            });
            Assert.Equal(new byte[]
            {
                0xAA, 0xAA, 0x10, 0x00, 0x01, 0x69
            }, request.GetBytes());
        }

        [Fact]
        public void UploadAlarmsRequest_ToByteSequence()
        {
            var request = new UploadAlarmsRequest
            {
                DeviceID = 0x0909
            };
            request.SetRequestData(new List<Alarm>
            {
                new Alarm
                {
                    IsActive = true,
                    StartTimeSpan = new TimeSpan(8, 0, 0),
                    Period = new TimeSpan(0, 1, 0)
                },
                new Alarm
                {
                    IsActive = true,
                    StartTimeSpan = new TimeSpan(10, 15, 0),
                    Period = new TimeSpan(0, 0, 0)
                },
                new Alarm
                {
                    IsActive = false,
                    StartTimeSpan = new TimeSpan(12, 0, 0),
                    Period = new TimeSpan(0, 0, 0)
                }
            });
            Assert.Equal(new byte[]
            {
                0x09, 0x09, 0x11, 0x00, 0x10, 0x03, 0x01, 0x08, 0x00, 0x00, 0x01, 0x01, 0x0A, 0x0F, 0x00,
                0x00, 0x00, 0x0C, 0x00, 0x00, 0x00
            }, request.GetBytes());
        }

        [Fact]
        public void UploadFramesRequest_ToByteSequence()
        {
            var request = new UploadFramesRequest
            {
                DeviceID = 8
            };
            request.SetRequestData(new List<DisplayFrame>
            {
                new DisplayFrame
                {
                    IsEnabled = false,
                    DisplayPeriod = 30,
                    Id = 10
                },
                new DisplayFrame
                {
                    IsEnabled = true,
                    DisplayPeriod = 30,
                    Id = 1
                },
                new DisplayFrame
                {
                    IsEnabled = true,
                    DisplayPeriod = 5,
                    Id = 5
                }
            });
            Assert.Equal(new byte[]
            {
                0x00, 0x08, 0x12, 0x00, 0x07, 0x02, 0x01, 0x00, 0x1E, 0x05, 0x00, 0x05
            }, request.GetBytes());
        }

        [Fact]
        public void UploadUploadTimeSyncConfigRequest_ToByteSequence()
        {
            var request = new UploadUploadTimeSyncConfigRequest
            {
                DeviceID = 0x11
            };
            request.SetRequestData(new TimeSyncParameters
            {
                ServerAddress = "time.server.com",
                ServerPort = 12345,
                SourceId = 3,
                ZoneId = "Russian Standard Time",
                SyncPeriod = new TimeSpan(4, 0, 0)
            });
            Assert.Equal(new byte[]
            {
                0x00, 0x11, 0x13, 0x00, 0x2B, 0x03, 0x15, 0x52, 0x75, 0x73, 0x73, 0x69,
                0x61, 0x6E, 0x20, 0x53, 0x74, 0x61, 0x6E, 0x64, 0x61, 0x72, 0x64, 0x20,
                0x54, 0x69, 0x6D, 0x65, 0x0F, 0x74, 0x69, 0x6D, 0x65, 0x2E, 0x73, 0x65,
                0x72, 0x76, 0x65, 0x72, 0x2E, 0x63, 0x6F, 0x6D, 0x30, 0x39, 0x04, 0x00
            }, request.GetBytes());
        }
    }
}
