using Activation;
using Common;
using NSubstitute;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Testing
{
    public class ActivationTesting
    {
        [Fact]
        public void NoLicenseTesting()
        {
            var compressor = Substitute.For<ICompressor>();
            var hardwareProvider = Substitute.For<IHardwareInfoProvider>();
            var activationFile = Substitute.For<IActivationFile>();
            activationFile.Exists().Returns(false);
            var manager = new ActivationManager(compressor, activationFile, hardwareProvider);
            var actualLicenseInfo = manager.ActualLicenseInfo;
            Assert.Equal(String.Empty, actualLicenseInfo.RequestCode);
        }

        [Fact]
        public void RequestCodeTesting()
        {
            var compressor = Substitute.For<ICompressor>();
            var hardwareProvider = Substitute.For<IHardwareInfoProvider>();
            hardwareProvider.DriveSerial.Returns("A");
            hardwareProvider.MemorySerial.Returns("A");
            hardwareProvider.MotherboardSerial.Returns("A");
            hardwareProvider.ProcessorId.Returns("A");
            var activationFile = Substitute.For<IActivationFile>();
            activationFile.Exists().Returns(false);
            var manager = new ActivationManager(compressor, activationFile, hardwareProvider);
            var requestCode = manager.GetRequestCode();
            Assert.Equal("0988-90DD-E069-E9AB-AD63-F19A-0D9E-1F32", requestCode);
        }

        [Fact]
        public void GetActivationKeyTesting()
        {
            var compressor = Substitute.For<ICompressor>();
            var hardwareProvider = Substitute.For<IHardwareInfoProvider>();
            var activationFile = Substitute.For<IActivationFile>();
            activationFile.Exists().Returns(false);
            var manager = new ActivationManager(compressor, activationFile, hardwareProvider);
            var activationKey = manager.GetActivationKey(new LicenseInfo
            {
                RequestCode = "0123-4567-89AB-CDEF-0000-0000-0000-0000",
                ExpirationDate = new DateTime(2025, 1, 1)
            });
            Assert.Equal("CBCB-DFCB-CBCC-CBCB-EFD0-04CB-EFCF-EBCB", activationKey);
        }

        [Fact]
        public void TryActivateTesting()
        {
            var compressor = Substitute.For<ICompressor>();
            var hardwareProvider = Substitute.For<IHardwareInfoProvider>();
            hardwareProvider.DriveSerial.Returns("A123");
            hardwareProvider.MemorySerial.Returns("11dd4");
            hardwareProvider.MotherboardSerial.Returns("asfd345");
            hardwareProvider.ProcessorId.Returns("236");
            var activationFile = Substitute.For<IActivationFile>();
            activationFile.Exists().Returns(false);
            var manager = new ActivationManager(compressor, activationFile, hardwareProvider);
            var requestCode = manager.GetRequestCode();
            var expirationDate = new DateTime(2025, 4, 3);
            var activationKey = manager.GetActivationKey(new LicenseInfo
            {
                RequestCode = requestCode,
                ExpirationDate = expirationDate
            });
            Assert.True(manager.TryActivate(activationKey, out LicenseInfo licenseInfo));
            Assert.Equal(expirationDate.Year, licenseInfo.ExpirationDate.Year);
            Assert.Equal(expirationDate.Month, licenseInfo.ExpirationDate.Month);
            Assert.Equal(expirationDate.Day, licenseInfo.ExpirationDate.Day);
        }
    }
}
