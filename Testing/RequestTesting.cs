using BaseDevice;
using CommandProcessing;
using CommandProcessing.DTO;
using CommandProcessing.Requests;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace Testing
{
    public class RequestTesting
    {
        [Fact]
        public void Request_GetBytes()
        {
            var request = Substitute.For<Request>();
            request.RequestID.Returns((byte)0x01);
            request.DeviceID = 0x1234;
            request.MakeData(Arg.Any<object>()).Returns(new byte[] { 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff });
            var bytes = request.GetBytes();
            Assert.Equal(new byte[] { 0x12, 0x34, 0x01, 0x00, 0x06, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff }, bytes);
        }

        [Fact]
        public void Request_ToString()
        {
            var request = Substitute.For<Request>();
            request.RequestID.Returns((byte)0x01);
            request.DeviceID = 0x1234;
            request.MakeData(Arg.Any<object>()).Returns(new byte[] { 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff });
            var requestString = request.ToString();
            Assert.Equal("[12][34][01][00][06][AA][BB][CC][DD][EE][FF]", requestString);
        }

        [Fact]
        public void ScanRequest_ByteSequence()
        {
            var scanRequest = new ScanRequest();
            Assert.Equal(new byte[] { 0, 0, 0, 0, 0 }, scanRequest.GetBytes());
        }

        [Fact]
        public void ConfigurationRequest_ByteSequence()
        {
            var request = new ConfigurationRequest
            {
                DeviceID = 0xf00
            };
            request.SetRequestData(5);
            Assert.Equal(new byte[] { 0x0F, 0x00, 0x0A, 0x00, 0x01, 0x05 }, request.GetBytes());
        }

        [Fact]
        public void UploadIdentityRequest_ByteSequence()
        {
            var request = new UploadIdentityRequest
            {
                DeviceID = 0xabcd
            };
            request.SetRequestData(new Identity
            {
                Id = 43982,
                Name = "deviceName"
            });
            Assert.Equal(new byte[]
            {
                0xAB, 0xCD, 0x03, 0x00, 0x0B, 0x0A, 0x64, 0x65, 0x76, 0x69, 0x63, 0x65,
                0x4E, 0x61, 0x6D, 0x65
            }, request.GetBytes());
        }

        [Fact]
        public void UploadNetworkRequest_ToByteSequence()
        {
            var request = new UploadNetworkRequest
            {
                DeviceID = 255
            };
            request.SetRequestData(new Network
            {
                IpAddress = "192.168.0.9",
                Port = 500,
                SubnetMask = 24,
                Gateway = "192.168.0.100",
                DnsServer = "192.168.0.101",
                AlternativeDnsServer = "192.168.0.102"
            });
            Assert.Equal(new byte[]
            {
               0x00, 0xFF, 0x02, 0x00, 0x16, 0xC0, 0xA8, 0x00, 0x09, 0x01, 0xF4, 0xFF, 0xFF, 0xFF, 0x00, 0xC0, 0xA8,
               0x00, 0x64, 0xC0, 0xA8, 0x00, 0x65, 0xC0, 0xA8, 0x00, 0x66
            }, request.GetBytes());
        }

        [Fact]
        public void UploadWorkScheduleRequest_Schedule_ToByteSequence()
        {
            var request = new UploadWorkScheduleRequest
            {
                DeviceID = 1
            };
            request.SetRequestData(new WorkSchedule
            {
                StartFrom = new System.TimeSpan(8, 0, 0),
                FinishTo = new System.TimeSpan(19, 30, 0),
                RunInSun = false,
                RunInMon = true,
                RunInTue = false,
                RunInWed = true,
                RunInThu = false,
                RunInFri = true,
                RunInSat = false,
                AllWeek = false,
                AroundTheClock = false
            });
            Assert.Equal(new byte[]
            {
               0x00, 0x01, 0x04, 0x00, 0x05, 0x08, 0x00, 0x13, 0x1E, 0x54
            }, request.GetBytes());
        }

        [Fact]
        public void UploadWorkScheduleRequest_NonStopSchedule_ToByteSequence()
        {
            var request = new UploadWorkScheduleRequest
            {
                DeviceID = 0x100
            };
            request.SetRequestData(new WorkSchedule
            {
                StartFrom = new System.TimeSpan(8, 0, 0),
                FinishTo = new System.TimeSpan(19, 30, 0),
                RunInSun = false,
                RunInMon = true,
                RunInTue = false,
                RunInWed = true,
                RunInThu = false,
                RunInFri = true,
                RunInSat = false,
                AllWeek = true,
                AroundTheClock = true
            });
            Assert.Equal(new byte[]
            {
               0x01, 0x00, 0x04, 0x00, 0x05, 0xff, 0xff, 0xff, 0xff, 0x80
            }, request.GetBytes());
        }

        [Fact]
        public void UploadBrightnessRequest_Auto_ToByteSequence()
        {
            var request = new UploadBrightnessRequest
            {
                DeviceID = 0x1111
            };
            request.SetRequestData(new Brightness
            {
                Mode = BrightnessMode.Auto,
                ManualValue = 14
            });
            Assert.Equal(new byte[]
            {
               0x11, 0x11, 0x05, 0x00, 0x03, 0x00, 0x00, 0x00
            }, request.GetBytes());
        }

        [Fact]
        public void UploadBrightnessRequest_Manual_ToByteSequence()
        {
            var request = new UploadBrightnessRequest
            {
                DeviceID = 0x1111
            };
            request.SetRequestData(new Brightness
            {
                Mode = BrightnessMode.Manual,
                ManualValue = 5
            });
            Assert.Equal(new byte[]
            {
               0x11, 0x11, 0x05, 0x00, 0x03, 0x01, 0x05, 0x00
            }, request.GetBytes());
        }

        [Fact]
        public void UploadBrightnessRequest_Scheduled_ToByteSequence()
        {
            var request = new UploadBrightnessRequest
            {
                DeviceID = 0x1111
            };
            request.SetRequestData(new Brightness
            {
                Mode = BrightnessMode.Scheduled,
                ManualValue = 5,
                BrightnessPeriods = new List<BrightnessPeriod>
                {
                    new BrightnessPeriod
                    {
                        From = new TimeSpan(8, 30, 0),
                        To = new TimeSpan(21, 0, 0),
                        Value = 7
                    },
                    new BrightnessPeriod
                    {
                        From = new TimeSpan(21, 0, 0),
                        To = new TimeSpan(8, 30, 0),
                        Value = 4
                    }
                }
            });
            Assert.Equal(new byte[]
            {
               0x11, 0x11, 0x05, 0x00, 0x0D, 0x02, 0x00, 0x02, 0x08, 0x1E, 0x15, 0x00, 0x07, 0x15, 0x00, 0x08,
               0x1E, 0x04
            }, request.GetBytes());
        }
    }
}
