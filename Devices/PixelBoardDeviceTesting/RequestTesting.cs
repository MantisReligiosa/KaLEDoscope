using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DTO;
using PixelBoardDevice.Requests;
using System;
using Xunit;

namespace PixelBoardDeviceTesting
{
    public class RequestTesting
    {
        [Fact]
        public void GetIdListRequest_ToByteSequence()
        {
            var request = new GetIdListRequest
            {
                DeviceID = 0x14
            };
            request.SetRequestData(2);
            Assert.Equal(new byte[]
            {
                0x00, 0x14, 0x22, 0x00, 0x01, 0x02
            }, request.GetBytes());
        }

        [Fact]
        public void GetStorageItemRequest_ToByteSequence()
        {
            var request = new GetStorageItemRequest
            {
                DeviceID = 0x2222
            };
            request.SetRequestData(new StorageItemIndex
            {
                StorageId = 1,
                ItemId = 170
            });
            Assert.Equal(new byte[]
            {
                0x22, 0x22, 0x24, 0x00, 0x02, 0x01, 0xAA
            }, request.GetBytes());
        }

        [Fact]
        public void CleanupStorageRequest_ToByteSequence()
        {
            var request = new CleanupStorageRequest
            {
                DeviceID = 0x14
            };
            request.SetRequestData(2);
            Assert.Equal(new byte[]
            {
                0x00, 0x14, 0x21, 0x00, 0x01, 0x02
            }, request.GetBytes());
        }

        [Fact]
        public void UploadFontRequest_ToByteSequence()
        {
            var request = new UploadFontRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new BinaryFont
            {
                Id = 170,
                Height = 10,
                Bold = true,
                Italic = true,
                Source = "Font",
                Alphabet = "ABC",
                Base64Bitmap = Convert.ToBase64String(new byte[] { 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff })
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x16, 0x01, 0xaa, 0x0a, 0x03, 0x04, 0x46, 0x6F, 0x6E,
                0x74, 0x00, 0x03, 0x41, 0x42, 0x43, 0x00, 0x06, 0xaa, 0xbb, 0xcc, 0xdd, 0xee,
                0xff
            }, request.GetBytes());
        }

        [Fact]
        public void UploadProgramRequest_ToByteSequence()
        {
            var request = new UploadProgramRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new Program
            {
                Id = 5,
                Name = "Program",
                Order = 1,
                Period = 65535
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x0d, 0x02, 0x05, 0x07, 0x50, 0x72, 0x6f, 0x67, 0x72, 0x61,
                0x6d, 0x01, 0xff, 0xff
            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_Text_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new Zone
            {
                Id = 16,
                ProgramId = 1,
                X = 100,
                Y = 50,
                Width = 256,
                Height = 64,
                Name = "Zone",
                ZoneType = 1,
                FontId = 5,
                Text = "Text"
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x17, 0x03, 0x10, 0x01, 0x00, 0x64, 0x00, 0x32, 0x01, 0x00,
                0x00, 0x40, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x01, 0x05, 0x04, 0x54, 0x65, 0x78, 0x74

            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_Sensor_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new Zone
            {
                Id = 2,
                ProgramId = 10,
                X = 0,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
                ZoneType = 2,
                FontId = 6
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x12, 0x03, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x02, 0x06

            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_Image_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new Zone
            {
                Id = 2,
                ProgramId = 10,
                X = 0,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
                ZoneType = 3,
                BinaryImageId = 255
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x12, 0x03, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x03, 0xff

            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_Tag_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new Zone
            {
                Id = 2,
                ProgramId = 10,
                X = 0,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
                ZoneType = 4,
                FontId = 7,
                ExternalSourceTag = "Main.Tag"
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x1b, 0x03, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x04, 0x07, 0x08, 0x4d, 0x61, 0x69, 0x6e,
                0x2e, 0x54, 0x61, 0x67

            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_ClockText_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new Zone
            {
                Id = 2,
                ProgramId = 10,
                X = 0,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
                ZoneType = 5,
                ClockType = 1,
                ClockFormat = 1,
                AllowPeriodicTimeSync = true,
                AllowScheduledSync = false,
                PeriodicSyncInterval = 30
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x14, 0x03, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x05, 0x06, 0x00, 0x1e

            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_ClockImage_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new Zone
            {
                Id = 2,
                ProgramId = 10,
                X = 0,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
                ZoneType = 5,
                ClockType = 2,
                AllowPeriodicTimeSync = false,
                AllowScheduledSync = true,
                ScheduledTimeSync = new TimeSpan(1, 30, 0)
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x14, 0x03, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x05, 0x11, 0x01, 0x1e

            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_CountdownTicker_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new Zone
            {
                Id = 2,
                ProgramId = 10,
                X = 0,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
                ZoneType = 6,
                TickerType = 2,
                TickerCountDownStartValue = new TimeSpan(0, 0, 5, 20, 555)
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x17, 0x03, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x06, 0x01, 0x00, 0x05, 0x14, 0x02,
                0x2b
            }, request.GetBytes());
        }

        [Fact]
        public void UploadBinaryImageRequest_ToByteSequence()
        {
            var request = new UploadBinaryImageRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new BinaryImage
            {
                Id = 2,
                Height = 16,
                Base64String = Convert.ToBase64String(new byte[]
                {
                    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d,
                    0x0e, 0x0f
                })
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x15, 0x04, 0x02, 0x00, 0x10, 0x00, 0x0f, 0x01, 0x02, 0x03,
                0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f
            }, request.GetBytes());
        }
    }
}
