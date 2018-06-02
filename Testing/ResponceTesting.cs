using BaseDevice;
using CommandProcessing.Responces;
using System;
using Xunit;

namespace Testing
{
    public class ResponceTesting
    {
        [Fact]
        public void Responce_ToString()
        {
            var responce = new SomeResponce();
            responce.SetByteSequence(new byte[] { 0xab, 0xcd, 0xef, 0x00, 0x00 });
            Assert.Equal("[AB][CD][EF][00][00]", responce.ToString());
        }

        [Fact]
        public void ScanResponce_FromByteSequence()
        {
            var scanResponce = new ScanResponce();
            scanResponce.SetByteSequence(new byte[]
            {
                0xAB, 0xCD, 0x01, 0x00, 0x20, 0xC0, 0xA8, 0x00, 0x09, 0x01, 0xF4, 0x0B, 0x64, 0x65, 0x76, 0x69,
                0x63, 0x65, 0x4D, 0x6F, 0x64, 0x65, 0x6C, 0xAA, 0x00, 0x05, 0x0A, 0x64, 0x65, 0x76, 0x69, 0x63,
                0x65, 0x4E, 0x61, 0x6D, 0x65
            });
            var device = scanResponce.Cast();
            Assert.Equal(0xabcd, device.Id);
            Assert.Equal("192.168.0.9", device.Network.IpAddress);
            Assert.Equal(500, device.Network.Port);
            Assert.Equal("deviceModel", device.Model);
            Assert.Equal("170.0.5", device.Firmware);
            Assert.Equal("deviceName", device.Name);
        }

        [Fact]
        public void IdentityResponce_FromByteSequence()
        {
            var identityResponce = new IdentityResponce();
            identityResponce.SetByteSequence(new byte[]
            {
                0xAB, 0xCD, 0x03, 0x00, 0x0D, 0xAB, 0xCE, 0x0A, 0x64, 0x65, 0x76, 0x69, 0x63, 0x65, 0x4E, 0x61,
                0x6D, 0x65
            });
            var identity = identityResponce.Cast();
            Assert.Equal(0xabce, identity.Id);
            Assert.Equal("deviceName", identity.Name);
        }

        [Fact]
        public void WorkScheduleResponce_FromByteSequence()
        {
            var workScheduleResponce1 = new WorkScheduleResponce();
            workScheduleResponce1.SetByteSequence(new byte[]
            {
                0x00, 0x01, 0x04, 0x00, 0x05, 0x08, 0x00, 0x13, 0x1E, 0x54
            });
            var workScheduleResponce2 = new WorkScheduleResponce();
            workScheduleResponce2.SetByteSequence(new byte[]
            {
                01, 0x00, 0x04, 0x00, 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0x80
            });
            var schedule1 = workScheduleResponce1.Cast();
            var schedule2 = workScheduleResponce2.Cast();
            Assert.False(schedule1.AroundTheClock);
            Assert.Equal(new TimeSpan(8, 0, 0), schedule1.StartFrom);
            Assert.Equal(new TimeSpan(19, 30, 0), schedule1.FinishTo);
            Assert.False(schedule1.RunInSun);
            Assert.True(schedule1.RunInMon);
            Assert.False(schedule1.RunInTue);
            Assert.True(schedule1.RunInWed);
            Assert.False(schedule1.RunInThu);
            Assert.True(schedule1.RunInFri);
            Assert.False(schedule1.RunInSat);
            Assert.True(schedule2.AroundTheClock);
            Assert.True(schedule2.AllWeek);
        }

        [Fact]
        public void BrightnessResponce_FromByteSequence()
        {
            var brightnessResponce1 = new BrightnessResponce();
            brightnessResponce1.SetByteSequence(new byte[]
            {
                0x11, 0x11, 0x05, 0x00, 0x03, 0x00, 0x00, 0x00
            });
            var brightnessResponce2 = new BrightnessResponce();
            brightnessResponce2.SetByteSequence(new byte[]
            {
                0x11, 0x11, 0x05, 0x00, 0x03, 0x01, 0x05, 0x00
            });
            var brightnessResponce3 = new BrightnessResponce();
            brightnessResponce3.SetByteSequence(new byte[]
            {
                0x11, 0x11, 0x05, 0x00, 0x0D, 0x02, 0x00, 0x02, 0x08, 0x1E, 0x15, 0x00, 0x07, 0x15,
                0x00, 0x08, 0x1E, 0x04
            });
            var brightness1 = brightnessResponce1.Cast();
            var brightness2 = brightnessResponce2.Cast();
            var brightness3 = brightnessResponce3.Cast();
            Assert.Equal(BrightnessMode.Auto, brightness1.Mode);
            Assert.Equal(BrightnessMode.Manual, brightness2.Mode);
            Assert.Equal(5, brightness2.ManualValue);
            Assert.Equal(BrightnessMode.Scheduled, brightness3.Mode);
            Assert.Equal(2, brightness3.BrightnessPeriods.Count);
            Assert.Contains(brightness3.BrightnessPeriods, p =>
                p.From.Equals(new TimeSpan(8, 30, 0)) &&
                p.To.Equals(new TimeSpan(21, 0, 0)) &&
                p.Value == 7
            );
            Assert.Contains(brightness3.BrightnessPeriods, p =>
                p.From.Equals(new TimeSpan(21, 0, 0)) &&
                p.To.Equals(new TimeSpan(8, 30, 0)) &&
                p.Value == 4
            );

        }
    }
}
