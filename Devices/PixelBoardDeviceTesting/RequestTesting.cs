using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using PixelBoardDevice.DTO;
using PixelBoardDevice.Requests;
using SmartTechnologiesM.Base.Extensions;
using System;
using Xunit;

namespace PixelBoardDeviceTesting
{
#warning В тестах расшифровывать массивы байтов
    public class RequestTesting
    {
        [Fact]
        public void GetIdListRequest_ToByteSequence()
        {
            ushort deviceId = 0x0014;
            byte requestId = 0x22;
            byte data = 0x02;
            ushort dataLength = 0x0001;
            var request = new GetIdListRequest
            {
                DeviceID = deviceId
            };
            request.SetRequestData(2);
            Assert.Equal(new byte[]
            {
                deviceId.ToBytes()[0], deviceId.ToBytes()[1],
                requestId,
                dataLength.ToBytes()[0], dataLength.ToBytes()[1],
                data
            }, request.GetBytes());
        }

        [Fact]
        public void GetStorageItemRequest_ToByteSequence()
        {
            ushort deviceId = 0x2222;
            byte requestId = 0x24;
            byte storageId = 0x01;
            byte itemId = 0xaa;
            ushort dataLength = 0x0002;
            var request = new GetStorageItemRequest
            {
                DeviceID = deviceId
            };
            request.SetRequestData(new StorageItemIndex
            {
                StorageId = storageId,
                ItemId = itemId
            });
            Assert.Equal(new byte[]
            {
                deviceId.ToBytes()[0], deviceId.ToBytes()[1],
                requestId,
                dataLength.ToBytes()[0], dataLength.ToBytes()[1],
                storageId,
                itemId
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
                Id = 0xaa,
                Height = 0x0a,
                GlyphHeight = 0x14,
                Bold = true,
                Italic = true,
                Source = "Font",
                Alphabet = new Glyph[]
                {
                    new Glyph
                    {
                        Symbol = 'A',
                        Image = new byte[]{0xaa, 0xbb},
                        Width = 0x08
                    },
                    new Glyph
                    {
                        Symbol = 'B',
                        Image = new byte[]{0xcc, 0xdd},
                        Width = 0x09
                    },
                    new Glyph
                    {
                        Symbol = 'C',
                        Image = new byte[]{0xee, 0xff},
                        Width = 0x0a
                    }
                }
            });
            var actualBytes = request.GetBytes();
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x20, 0x01, 0xaa, 0x14, 0x03, 0x04, 0x46, 0x6f, 0x6e,
                0x74, 0x00, 0x03, 0x41, 0x00, 0x00, 0x00, 0x08, 0x42, 0x00, 0x02, 0x00, 0x09,
                0x43, 0x00, 0x04, 0x00, 0x0a, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff
            }, actualBytes);
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
                Order = 0x01,
                Period = 0xffff
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
            request.SetRequestData(new TextZone
            {
                Id = 0x10,
                ProgramId = 0x01,
                X = 0x0064,
                Y = 0x0032,
                Width = 0x0100,
                Height = 0x0040,
                Name = "Zone",
                FontId = 0x05,
                Alignment = 0x00,
                AnimationId = 0x01,
                Text = "Text"
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x1b, 0x03, 0x10, 0x01, 0x00, 0x64, 0x00, 0x32, 0x01, 0x00,
                0x00, 0x40, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x01, 0x05, 0x00, 0x01, 0x00, 0x00, 0x04, 
                0x54, 0x65, 0x78, 0x74

            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_NewText_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new TextZone
            {
                Id = 16,
                ProgramId = 1,
                X = 0,
                Y = 0,
                Width = 10,
                Height = 10,
                Alignment = 0,
                AnimationId = 0
            });
            request.GetBytes();
        }


        [Fact]
        public void UploadZoneRequest_Sensor_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new SensorZone
            {
                Id = 2,
                ProgramId = 10,
                X = 0,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
                FontId = 6,
                Alignment = System.Windows.TextAlignment.Right
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x13, 0x03, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x02, 0x06, 0x01

            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_Image_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new BitmapZone
            {
                Id = 2,
                ProgramId = 10,
                X = 0,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
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
            request.SetRequestData(new TagZone
            {
                Id = 2,
                ProgramId = 10,
                X = 0,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
                FontId = 7,
                Alignment = System.Windows.TextAlignment.Center,
                ExternalSourceTag = "Main.Tag"
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x1c, 0x03, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x04, 0x07, 0x02, 0x08, 0x4d, 0x61, 0x69, 0x6e,
                0x2e, 0x54, 0x61, 0x67

            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_ClockText_ToByteSequence()
        {
            ushort devId = 0x00aa;
            byte requestId = 0x26;
            ushort commandLength = 0x16;
            byte storageId = 3;
            var zone = new ClockZone
            {
                Id = 2,
                ProgramId = 10,
                X = 1,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
                ClockType = ClockTypes.Digital,
                ClockFormat = 1,
                AllowPeriodicTimeSync = true,
                AllowScheduledSync = false,
                FontId = 5,
                Alignment = System.Windows.TextAlignment.Right,
                PeriodicSyncInterval = 30
            };
            var request = new UploadZoneRequest
            {
                DeviceID = devId
            };
            request.SetRequestData(zone);
            Assert.Equal(new byte[]
            {
                devId.ToBytes()[0], devId.ToBytes()[1],
                requestId,
                commandLength.ToBytes()[0], commandLength.ToBytes()[1],
                storageId,
                zone.Id,
                zone.ProgramId,
                zone.X.ToBytes()[0], zone.X.ToBytes()[1],
                zone.Y.ToBytes()[0], zone.Y.ToBytes()[1],
                zone.Width.ToBytes()[0], zone.Width.ToBytes()[1],
                zone.Height.ToBytes()[0], zone.Height.ToBytes()[1],
                0x04, 0x5a, 0x6f, 0x6e, 0x65, //Zone name
                0x05, //Zone type
                0x06, //Bit flags
                zone.PeriodicSyncInterval.ToBytes()[0], zone.PeriodicSyncInterval.ToBytes()[1],
                zone.FontId.Value,
                (byte)zone.Alignment.Value

            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_ClockText_ToByteSequence1()
        {
            ushort devId = 0xf369;
            byte requestId = 0x26;
            ushort commandLength = 0x16;
            byte storageId = 3;
            var zone = new ClockZone
            {
                Id = 1,
                ProgramId = 0,
                X = 0,
                Y = 0,
                Width = 0x40,
                Height = 0x20,
                Name = "Часы",
                ClockType = ClockTypes.Digital,
                ClockFormat = 2,
                AllowPeriodicTimeSync = false,
                AllowScheduledSync = false,
                FontId = 2,
                Alignment = 0,
                PeriodicSyncInterval = 30
            };
            var request = new UploadZoneRequest
            {
                DeviceID = devId
            };
            request.SetRequestData(zone);
            var actual = new byte[]
            {
                devId.ToBytes()[0], devId.ToBytes()[1],
                requestId,
                commandLength.ToBytes()[0], commandLength.ToBytes()[1],
                storageId,
                zone.Id,
                zone.ProgramId,
                zone.X.ToBytes()[0], zone.X.ToBytes()[1],
                zone.Y.ToBytes()[0], zone.Y.ToBytes()[1],
                zone.Width.ToBytes()[0], zone.Width.ToBytes()[1],
                zone.Height.ToBytes()[0], zone.Height.ToBytes()[1],
                0x04, 0xd7, 0xe0, 0xf1, 0xfb, //Zone name
                0x05, //Zone type
                0x08, //Bit flags
                0, 0, //PeriodicSyncInterval ignored
                zone.FontId.Value,
                (byte)zone.Alignment.Value
            };

            Assert.Equal(actual, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_ClockImage_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new ClockZone
            {
                Id = 2,
                ProgramId = 10,
                X = 0,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
                ClockType = ClockTypes.Analog,
                AllowPeriodicTimeSync = false,
                AllowScheduledSync = true,
                ScheduledTimeSync = new TimeSpan(1, 30, 0),
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x16, 0x03, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x05, 0x11, 0x01, 0x1e, 0x00, 0x00
            }, request.GetBytes());
        }

        [Fact]
        public void UploadZoneRequest_CountdownTicker_ToByteSequence()
        {
            var request = new UploadZoneRequest
            {
                DeviceID = 0xaa
            };
            request.SetRequestData(new TickerZone
            {
                Id = 2,
                ProgramId = 10,
                X = 0,
                Y = 0,
                Width = 50,
                Height = 8,
                Name = "Zone",
                TickerType = 2,
                TickerCountDownStartValue = new TimeSpan(0, 0, 5, 20, 555),
                FontId = 5,
                Alignment = System.Windows.TextAlignment.Right
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x19, 0x03, 0x02, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x32,
                0x00, 0x08, 0x04, 0x5a, 0x6f, 0x6e, 0x65, 0x06, 0x01, 0x00, 0x05, 0x14, 0x02, 0x2b,
                0x05, 0x01
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
                Width = 32,
                Bytes = new byte[]
                {
                    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d,
                    0x0e, 0x0f
                }
            });
            Assert.Equal(new byte[]
            {
                0x00, 0xaa, 0x26, 0x00, 0x15, 0x04, 0x02, 0x00, 0x10, 0x00, 0x20, 0x01, 0x02, 0x03,
                0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f
            }, request.GetBytes());
        }

        [Fact]
        public void UploadBoardConfigRequest_ToByteSequence()
        {
            var request = new UploadBoardConfigRequest
            {
                DeviceID = 0x1111
            };
            request.SetRequestData(new BoardSize
            {
                Height = 64,
                Width = 900
            });
            Assert.Equal(new byte[]
            {
                0x11, 0x11, 0x20, 0x00, 0x04, 0x03, 0x84, 0x00, 0x40
            }, request.GetBytes());
        }

        [Fact]
        public void UploadBoardHardwareConfigReques_ToByteSequence()
        {
            var request = new UploadBoardHardwareConfigRequest
            {
                DeviceID = 0x1111
            };
            request.SetRequestData(new BoardHardware
            {
                Type = BoardHardwareType.RsPanel16x16
            });
            Assert.Equal(new byte[]
            {
                0x11, 0x11, 0x27, 0x00, 0x01, 0x03
            }, request.GetBytes());
        }
    }
}
