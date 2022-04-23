using CommandProcessing;
using CommandProcessing.Responces;
using NSubstitute;
using PixelBoardDevice.Commands;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using PixelBoardDevice.Requests;
using ServiceInterfaces;
using SmartTechnologiesM.Base.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace PixelBoardDeviceTesting
{
    public class CommandTesting
    {
        private readonly INetworkAgent _networkAgent;
        private readonly ILogger _logger;
        private readonly IConfig _config;
        private readonly PixelBoard _device;

        public CommandTesting()
        {
            _networkAgent = Substitute.For<INetworkAgent>();
            _logger = Substitute.For<ILogger>();
            _config = Substitute.For<IConfig>();
            _config.ResponceTimeout = int.MaxValue;
            _device = new PixelBoard { Id = 256 };
        }

        [Fact]
        public void UploadHardwareTesting()
        {
            _device.Hardware = new BoardHardware
            {
                Type = BoardHardwareType.Hub12
            };
            ushort commandLength = 0x0001;
            byte requestId = 0x27;
            _networkAgent.When(x => x.Send(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<UploadBoardHardwareConfigRequest>())).Do(x =>
            {
                var request = x.Arg<IRequest>();
                var bytes = request.GetBytes();
                Assert.Equal(new byte[]{
                    _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                    requestId,
                    commandLength.ToBytes()[0], commandLength.ToBytes()[1],
                    (byte)_device.Hardware.Type
                    }, bytes);
            });
            _networkAgent.When(x => x.Listen<AcceptanceResponce, object>(Arg.Any<int>(), Arg.Any<Action<AcceptanceResponce>>())).Do(x =>
            {
                var action = x.Arg<Action<AcceptanceResponce>>();
                var responce = new AcceptanceResponce();
                responce.SetByteSequence(ResponceBytes.Accepted);
                action.Invoke(responce);
            });
            var cmd = new UploadBoardHardwareConfigCommand(_device, _networkAgent, _logger, _config);
            cmd.Error += Cmd_Error;
            cmd.Execute();
        }

        [Fact]
        public void UploadFontsTesting()
        {
            byte requestId = 0x26;
            byte storageId = 0x01;
            _device.Fonts = new List<BinaryFont>
            {
                new BinaryFont
                {
                    Alphabet = new Glyph[]
                    {
                        new Glyph
                        {
                            Image = new byte[]{1, 2, 3},
                            Symbol = 'A',
                            Width = 10,
                        },
                        new Glyph
                        {
                            Image = new byte[]{4, 5, 6},
                            Symbol = 'B',
                            Width = 10
                        }
                    },
                    Bold = true,
                    Height = 5,
                    GlyphHeight = 10,
                    Id = 1,
                    Italic = true,
                    Source = "Arial"
                },
                new BinaryFont
                {
                    Alphabet = new Glyph[]
                    {
                        new Glyph
                        {
                            Image = new byte[]{2, 2, 2},
                            Symbol = 'B',
                            Width = 8
                        }
                    },
                    Bold = false,
                    Height = 15,
                    GlyphHeight = 20,
                    Id = 2,
                    Italic = false,
                    Source = "Tahoma"
                }
            };
            ushort[] commandLengths = new ushort[] { 0x001c, 0x0015 };
            Assert.True(TestUploadToStorage(
                new UploadFontsCommand(_device, _networkAgent, _logger, _config),
                new byte[][]
                {
                    new byte[]
                    {
                        _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[0].ToBytes()[0], commandLengths[0].ToBytes()[1],
                        storageId,
                        _device.Fonts[0].Id,
                        _device.Fonts[0].GlyphHeight,
                        0x03, //Bold, Italic
                        (byte)_device.Fonts[0].Source.Length,
                        0x41, //A
                        0x72, //r
                        0x69, //i
                        0x61, //a
                        0x6c, //l
                        ((ushort)_device.Fonts[0].Alphabet.Length).ToBytes()[0], ((ushort)_device.Fonts[0].Alphabet.Length).ToBytes()[1],
                        0x41, //A
                        0x00, 0x00, //Offset
                        _device.Fonts[0].Alphabet[0].Width.ToBytes()[0], _device.Fonts[0].Alphabet[0].Width.ToBytes()[1],
                        0x42, //A
                        0x00, 0x03, //Offset
                        _device.Fonts[0].Alphabet[1].Width.ToBytes()[0], _device.Fonts[0].Alphabet[1].Width.ToBytes()[1],
                        0x01, 0x02, 0x03, 0x04, 0x05, 0x06
                    },
                    new byte[]
                    {
                        _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[1].ToBytes()[0], commandLengths[1].ToBytes()[1],
                        storageId,
                        _device.Fonts[1].Id,
                        _device.Fonts[1].GlyphHeight,
                        0x00, //No Italic, no Bold
                        (byte)_device.Fonts[1].Source.Length,
                        0x54, //T
                        0x61, //a
                        0x68, //h
                        0x6f, //o
                        0x6d, //m
                        0x61, //a
                        ((ushort)_device.Fonts[1].Alphabet.Length).ToBytes()[0], ((ushort)_device.Fonts[1].Alphabet.Length).ToBytes()[1],
                        0x42, //B
                        0x00, 0x00, //Offset
                        _device.Fonts[1].Alphabet[0].Width.ToBytes()[0], _device.Fonts[1].Alphabet[0].Width.ToBytes()[1],
                        0x02, 0x02, 0x02
                    }
                }));
        }

        [Fact]
        public void UploadEmptyFontTesting()
        {
            ushort commandLength = 0x000c;
            byte requestId = 0x26;
            byte storageId = 0x01;
            ushort alphabetLength = 0;
            byte fontId = 0;
            byte glyphHeight = 12;
            _device.Fonts = new List<BinaryFont>
            {
                new BinaryFont
                {
                    Alphabet = null,
                    Height = 5,
                    GlyphHeight = glyphHeight,
                    Id = fontId,
                    Source = "Arial"
                }
            };
            Assert.True(TestUploadToStorage(
                new UploadFontsCommand(_device, _networkAgent, _logger, _config),
                new byte[][]
                {
                    new byte[]
                    {
                        _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLength.ToBytes()[0], commandLength.ToBytes()[1],
                        storageId,
                        fontId,
                        glyphHeight,
                        0x00, //No Italic, no Bold
                        (byte)_device.Fonts[0].Source.Length, //4 chars:
                        0x41, //A
                        0x72, //r
                        0x69, //i
                        0x61, //a
                        0x6c, //l
                        alphabetLength.ToBytes()[0], alphabetLength.ToBytes()[1]
                    }
                }));
        }

        [Fact]
        public void UploadPicturesTesting()
        {
            byte requestId = 0x26;
            byte storageId = 0x04;
            ushort[] commandLengths = new ushort[] { 0x0009, 0x000b };

            _device.BinaryImages = new List<BinaryImage>
            {
                new BinaryImage
                {
                    Id = 0x01,
                    Height = 0x000a,
                    Width = 0x0014,
                    Bytes = new byte[]{0x01, 0x02, 0x03 }
                },
                new BinaryImage
                {
                    Id = 0x02,
                    Height = 0x000f,
                    Width = 0x001e,
                    Bytes = new byte[]{ 0x05, 0x05, 0x05, 0x05, 0x05 }
                }
            };
            Assert.True(TestUploadToStorage(
                new UploadBinaryImageCommand(_device, _networkAgent, _logger, _config),
                new byte[][]
                {
                    new byte[]
                    {
                         _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[0].ToBytes()[0], commandLengths[0].ToBytes()[1],
                        storageId,
                        _device.BinaryImages[0].Id,
                        _device.BinaryImages[0].Height.ToBytes()[0], _device.BinaryImages[0].Height.ToBytes()[1],
                        _device.BinaryImages[0].Width.ToBytes()[0], _device.BinaryImages[0].Width.ToBytes()[1],
                        _device.BinaryImages[0].Bytes[0], _device.BinaryImages[0].Bytes[1], _device.BinaryImages[0].Bytes[2]
                    },
                    new byte[]
                    {
                         _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[1].ToBytes()[0], commandLengths[1].ToBytes()[1],
                        storageId,
                        _device.BinaryImages[1].Id,
                        _device.BinaryImages[1].Height.ToBytes()[0], _device.BinaryImages[1].Height.ToBytes()[1],
                        _device.BinaryImages[1].Width.ToBytes()[0], _device.BinaryImages[1].Width.ToBytes()[1],
                        _device.BinaryImages[1].Bytes[0], _device.BinaryImages[1].Bytes[1], _device.BinaryImages[1].Bytes[2],
                        _device.BinaryImages[1].Bytes[3], _device.BinaryImages[1].Bytes[4]
                    }
                }));
        }

        [Fact]
        public void UploadProgramsTesting()
        {
            byte requestId = 0x26;
            byte storageId = 0x02;
            ushort[] commandLengths = new ushort[] { 0x000e, 0x000e };
            _device.Programs = new List<Program>
            {
                new Program
                {
                    Id = 1,
                    Name = "Program1",
                    Order = 1,
                    Period = 10,
                    Zones = new List<Zone>{ }
                },
                new Program
                {
                    Id = 2,
                    Name = "Program2",
                    Order = 2,
                    Period = 25,
                    Zones = new List<Zone>{ }
                }
            };
            Assert.True(TestUploadToStorage(
                new UploadProgramsCommand(_device, _networkAgent, _logger, _config),
                new byte[][]
                {
                    new byte[]
                    {
                        _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[0].ToBytes()[0], commandLengths[0].ToBytes()[1],
                        storageId,
                        _device.Programs[0].Id,
                        (byte)_device.Programs[0].Name.Length,
                        0x50, 0x72, 0x6f, 0x67, 0x72, 0x61, 0x6d, 0x31, //Program1
                        _device.Programs[0].Order,
                        _device.Programs[0].Period.ToBytes()[0], _device.Programs[0].Period.ToBytes()[1]
                    },
                    new byte[]
                    {
                         _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[0].ToBytes()[0], commandLengths[0].ToBytes()[1],
                        storageId,
                        _device.Programs[1].Id,
                        (byte)_device.Programs[0].Name.Length,
                        0x50, 0x72, 0x6f, 0x67, 0x72, 0x61, 0x6d, 0x32, //Program2
                        _device.Programs[1].Order,
                        _device.Programs[1].Period.ToBytes()[0], _device.Programs[1].Period.ToBytes()[1]
                    }
                }));
        }

        [Fact]
#warning В тесте расшифровывать массивы байтов до конца
        public void UploadZonesTesting()
        {
            byte requestId = 0x26;
            byte storageId = 0x03;
            ushort[] commandLengths = new ushort[] { 0x0010, 0x0011, 0x0019, 0x0014, 0x0027, 0x001b };

            var program1 = new Program
            {
                Id = 0x03,
                Name = "P1",
                Order = 0x01,
                Period = 0x22,
                Zones = new List<Zone>()
            };
            var program2 = new Program
            {
                Id = 0x12,
                Name = "P2",
                Order = 0x02,
                Period = 0xaf,
                Zones = new List<Zone>()
            };
            program1.Zones.Add(new BitmapZone
            {
                Id = 0x03,
                BinaryImageId = 0xa2,
                Height = 0x0a,
                Width = 0x0b,
                IsValid = true,
                Name = "Z2",
                ProgramId = program1.Id,
                X = 0x0102,
                Y = 0x00f0
            });
            program1.Zones.Add(new SensorZone
            {
                Id = 0x40,
                Name = "Z4",
                Height = 0x1102,
                Width = 0xa0b2,
                ProgramId = program1.Id,
                X = 0x1111,
                Y = 0xaaaa,
                FontId = 0x12,
                Alignment = System.Windows.TextAlignment.Left
            });
            program1.Zones.Add(new TextZone
            {
                Id = 0x12,
                Height = 0x5678,
                Alignment = System.Windows.TextAlignment.Right,
                FontId = 0xff,
                ProgramId = program1.Id,
                IsValid = true,
                Name = "Z1",
                Text = "TEXT",
                Width = 0xa0a0,
                X = 0xf1e2,
                Y = 0xedab,
                AnimationId = 0x11,
                AnimationSpeed = 0x22,
                AnimationTimeout = 0x33
            });
            program2.Zones.Add(new ClockZone
            {
                Id = 3,
                Alignment = null,
                AllowPeriodicTimeSync = true,
                AllowScheduledSync = false,
                ClockFormat = 1,
                ClockType = ClockTypes.Analog,
                FontId = 4,
                Height = 10,
                IsValid = true,
                Name = "Z3",
                PeriodicSyncInterval = 0x0019,
                ProgramId = program2.Id,
                Sample = "123",
                ScheduledTimeSync = new TimeSpan(1, 0, 0),
                Width = 20,
                X = 5,
                Y = 5
            });
            program2.Zones.Add(new TagZone
            {
                Id = 5,
                ExternalSourceTag = "123",
                ProgramId = program2.Id,
                FontId = 2,
                Alignment = System.Windows.TextAlignment.Center
            });
            program2.Zones.Add(new TickerZone
            {
                Id = 6,
                TickerCountDownStartValue = new TimeSpan(0, 5, 0),
                ProgramId = program2.Id
            });
            _device.Programs = new List<Program> { program1, program2 };
            Assert.True(TestUploadToStorage(
                new UploadZonesCommand(_device, _networkAgent, _logger, _config),
                new byte[][]
                {
                    new byte[]
                    {
                        _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[0].ToBytes()[0], commandLengths[0].ToBytes()[1],
                        storageId,
                        _device.Programs[0].Zones[0].Id,
                        _device.Programs[0].Id,
                        _device.Programs[0].Zones[0].X.ToBytes()[0], _device.Programs[0].Zones[0].X.ToBytes()[1],
                        _device.Programs[0].Zones[0].Y.ToBytes()[0], _device.Programs[0].Zones[0].Y.ToBytes()[1],
                        _device.Programs[0].Zones[0].Width.ToBytes()[0], _device.Programs[0].Zones[0].Width.ToBytes()[1],
                        _device.Programs[0].Zones[0].Height.ToBytes()[0], _device.Programs[0].Zones[0].Height.ToBytes()[1],
                        (byte)_device.Programs[0].Zones[0].Name.Length,
                        0x5a, 0x32, //Z2
                        (byte)_device.Programs[0].Zones[0].ZoneType,
                        ((BitmapZone)_device.Programs[0].Zones[0]).BinaryImageId
                    },
                    new byte[]
                    {
                        _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[1].ToBytes()[0], commandLengths[1].ToBytes()[1],
                        storageId,
                        _device.Programs[0].Zones[1].Id,
                        _device.Programs[0].Id,
                        _device.Programs[0].Zones[1].X.ToBytes()[0], _device.Programs[0].Zones[1].X.ToBytes()[1],
                        _device.Programs[0].Zones[1].Y.ToBytes()[0], _device.Programs[0].Zones[1].Y.ToBytes()[1],
                        _device.Programs[0].Zones[1].Width.ToBytes()[0], _device.Programs[0].Zones[1].Width.ToBytes()[1],
                        _device.Programs[0].Zones[1].Height.ToBytes()[0], _device.Programs[0].Zones[1].Height.ToBytes()[1],
                        (byte)_device.Programs[0].Zones[1].Name.Length,
                        0x5a, 0x34, //Z4
                        (byte)_device.Programs[0].Zones[1].ZoneType,
                        (byte)((SensorZone)_device.Programs[0].Zones[1]).FontId,
                        (byte)((SensorZone)_device.Programs[0].Zones[1]).Alignment
                    },
                    new byte[]
                    {
                        _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[2].ToBytes()[0], commandLengths[2].ToBytes()[1],
                        storageId,
                        _device.Programs[0].Zones[2].Id,
                        _device.Programs[0].Id,
                        _device.Programs[0].Zones[2].X.ToBytes()[0], _device.Programs[0].Zones[2].X.ToBytes()[1],
                        _device.Programs[0].Zones[2].Y.ToBytes()[0], _device.Programs[0].Zones[2].Y.ToBytes()[1],
                        _device.Programs[0].Zones[2].Width.ToBytes()[0], _device.Programs[0].Zones[2].Width.ToBytes()[1],
                        _device.Programs[0].Zones[2].Height.ToBytes()[0], _device.Programs[0].Zones[2].Height.ToBytes()[1],
                        (byte)_device.Programs[0].Zones[2].Name.Length,
                        0x5a, 0x31, //Z1
                        (byte)_device.Programs[0].Zones[2].ZoneType,
                        (byte)((TextZone)_device.Programs[0].Zones[2]).FontId,
                        (byte)((TextZone)_device.Programs[0].Zones[2]).Alignment,
                        (byte)((TextZone)_device.Programs[0].Zones[2]).AnimationId,
                        (byte)((TextZone)_device.Programs[0].Zones[2]).AnimationSpeed,
                        ((TextZone)_device.Programs[0].Zones[2]).AnimationTimeout,
                        (byte)((TextZone)_device.Programs[0].Zones[2]).Text.Length,
                        0x54, 0x45, 0x58, 0x54 // TEXT
                    },
                    new byte[]
                    {
                        _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[3].ToBytes()[0], commandLengths[3].ToBytes()[1],
                        storageId,
                        _device.Programs[1].Zones[0].Id,
                        _device.Programs[1].Id,
                        _device.Programs[1].Zones[0].X.ToBytes()[0], _device.Programs[1].Zones[0].X.ToBytes()[1],
                        _device.Programs[1].Zones[0].Y.ToBytes()[0], _device.Programs[1].Zones[0].X.ToBytes()[1],
                        _device.Programs[1].Zones[0].Width.ToBytes()[0], _device.Programs[1].Zones[0].Width.ToBytes()[1],
                        _device.Programs[1].Zones[0].Height.ToBytes()[0], _device.Programs[1].Zones[0].Height.ToBytes()[1],
                        (byte)_device.Programs[1].Zones[0].Name.Length,
                        0x5a, 0x33, //Z3
                        (byte)_device.Programs[1].Zones[0].ZoneType,
                        0x12, // Clock flags: analog, periodic sync, no scheduled sync
                        ((ClockZone)_device.Programs[1].Zones[0]).PeriodicSyncInterval.ToBytes()[0],
                        ((ClockZone)_device.Programs[1].Zones[0]).PeriodicSyncInterval.ToBytes()[1],
                        0x00, 0x00 //Заглушка
                    },
                    new byte[]
                    {
                        _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[4].ToBytes()[0], commandLengths[4].ToBytes()[1],
                        storageId,
                        _device.Programs[1].Zones[1].Id,
                        _device.Programs[1].Id,
                        _device.Programs[1].Zones[1].X.ToBytes()[0], _device.Programs[1].Zones[1].X.ToBytes()[1],
                        _device.Programs[1].Zones[1].Y.ToBytes()[0], _device.Programs[1].Zones[1].Y.ToBytes()[1],
                        _device.Programs[1].Zones[1].Width.ToBytes()[0], _device.Programs[1].Zones[1].Width.ToBytes()[1],
                        _device.Programs[1].Zones[1].Height.ToBytes()[0], _device.Programs[1].Zones[1].Height.ToBytes()[1],
                        (byte)_device.Programs[1].Zones[1].Name.Length,
                        0xd2, 0xfd, 0xe3, 0x20, 0xe2, 0xed, 0xe5, 0xf8, 0xed, 0xe5, 0xe3, 0xee, 0x20, 0xf1, 0xe5, 0xf0, 0xe2, 0xe5, 0xf0, 0xe0, //Тэг внешнего сервера
                        (byte)_device.Programs[1].Zones[1].ZoneType,
                        (byte)((TagZone)_device.Programs[1].Zones[1]).FontId,
                        (byte)((TagZone)_device.Programs[1].Zones[1]).Alignment,
                        0x03, 0x31, 0x32, 0x33
                    },
                    new byte[]
                    {
                        _device.Id.ToBytes()[0], _device.Id.ToBytes()[1],
                        requestId,
                        commandLengths[5].ToBytes()[0], commandLengths[5].ToBytes()[1],
                        storageId,
                        _device.Programs[1].Zones[2].Id,
                        _device.Programs[1].Id,
                        _device.Programs[1].Zones[2].X.ToBytes()[0], _device.Programs[1].Zones[2].X.ToBytes()[1],
                        _device.Programs[1].Zones[2].Y.ToBytes()[0], _device.Programs[1].Zones[2].Y.ToBytes()[1],
                        _device.Programs[1].Zones[2].Width.ToBytes()[0], _device.Programs[1].Zones[2].Width.ToBytes()[1],
                        _device.Programs[1].Zones[2].Height.ToBytes()[0], _device.Programs[1].Zones[2].Height.ToBytes()[1],
                        (byte)_device.Programs[1].Zones[2].Name.Length,
                        0xd2, 0xe0, 0xe9, 0xec, 0xe5, 0xf0, //Таймер
                        (byte)_device.Programs[1].Zones[2].ZoneType,
                        (byte)(((TickerZone)_device.Programs[1].Zones[2]).TickerType == 1 ? 0 : 1),
                        0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00
                    }
                }));
        }

        private bool TestUploadToStorage<TUploadRequest, TStorageItem>(UploadStorageItemsCommand<TUploadRequest, TStorageItem> uploadCommand, byte[][] expectedRequestBytes)
            where TUploadRequest : Request, new()
            where TStorageItem : class
        {
            var requestIndex = 0;
            //Проверка запроса на очистку хранилища
            _networkAgent.When(x => x.Send(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CleanupStorageRequest>())).Do(x =>
            {
                var request = x.Arg<IRequest>();
                var bytes = request.GetBytes();
                Assert.Equal(new byte[]{
                        0x01, 0x00, 0x21, 0x00, 0x01, uploadCommand.StorageId
                    }, bytes);
            });

            _networkAgent.When(x => x.Send(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<TUploadRequest>())).Do(x =>
            {
                var request = x.Arg<IRequest>();
                var actualBytes = request.GetBytes();
                var expectedBytes = expectedRequestBytes[requestIndex];
                Assert.Equal(expectedBytes, actualBytes);
                requestIndex++;
            });

            _networkAgent.When(x => x.Listen<AcceptanceResponce, object>(Arg.Any<int>(), Arg.Any<Action<AcceptanceResponce>>())).Do(x =>
            {
                var action = x.Arg<Action<AcceptanceResponce>>();
                var responce = new AcceptanceResponce();
                responce.SetByteSequence(ResponceBytes.Accepted);
                action.Invoke(responce);
            });
            uploadCommand.Error += Cmd_Error;
            uploadCommand.Execute();
            return true;
        }

        private void Cmd_Error(object sender, ExceptionEventArgs e)
        {
            throw e.Exception;
        }
    }
}
