using Common;
using Extensions;
using Force.Crc32;
using Newtonsoft.Json;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Activation
{
    public class ActivationManager : IActivationManager
    {
        private readonly ICompressor _compressor;
        private readonly IActivationFile _activationFile;
        private readonly IHardwareInfoProvider _hardwareInfoProvider;
        public LicenseInfo ActualLicenseInfo { get; set; }
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public ActivationManager(ICompressor compressor, IActivationFile activationFile, IHardwareInfoProvider hardwareInfoProvider)
        {
            _compressor = compressor;
            _activationFile = activationFile;
            _hardwareInfoProvider = hardwareInfoProvider;
            _key = Encoding.ASCII.GetBytes("1f5gF$7gn5ugRj5uHfk&lg548G5m*ff6");
            _iv = Encoding.ASCII.GetBytes("fTG%85jGfi@4j*Ei");
            if (_activationFile.IsNull() || !_activationFile.Exists())
            {
                ActualLicenseInfo = new LicenseInfo
                {
                    ExpirationDate = DateTime.Now,
                    RequestCode = string.Empty
                };
                return;
            }
            var bytes = _activationFile.Read();
            var text = _compressor.Unzip(bytes);
            ActualLicenseInfo = JsonConvert.DeserializeObject<LicenseInfo>(text);
        }

        public void ApplyLicense(LicenseInfo licenseInfo)
        {
            ActualLicenseInfo = licenseInfo;
            Submit();
        }

        private void Submit()
        {
            var serialized = JsonConvert.SerializeObject(ActualLicenseInfo);
            var datas = _compressor.Zip(serialized);
            _activationFile.Write(datas);
        }

        public string GetRequestCode()
        {
            var id = GetUniqueHardwareId();
            return GetMD5Code(id);
        }

        private string GetMD5Code(string code)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bytes = new UTF8Encoding().GetBytes(code);
                return GetHexString(md5.ComputeHash(bytes));
            }
        }

        private string GetUniqueHardwareId()
        {
            var processorId = _hardwareInfoProvider.ProcessorId;
            var motherboardSerial = _hardwareInfoProvider.MotherboardSerial;
            var memorySerial = _hardwareInfoProvider.MemorySerial;
            var driveSerial = _hardwareInfoProvider.DriveSerial;
            var hardwareParameters = string.Concat(
                processorId, motherboardSerial, memorySerial, driveSerial);

            return hardwareParameters;
        }

        private string GetHexString(byte[] bytes)
        {
            var hex = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];
                hex.Append($"{b:X2}");
                if (i + 1 != bytes.Length && (i + 1) % 2 == 0)
                {
                    hex.Append("-");
                }
            }
            var result = hex.ToString();
            return result;
        }

        public string GetActivationKey(LicenseInfo licenseInfo)
        {
            var requestBytes = licenseInfo.RequestCode.GetBytesFromHexString();
            var year = (byte)(licenseInfo.ExpirationDate.Year % 100);
            var month = (byte)licenseInfo.ExpirationDate.Month;
            var day = (byte)licenseInfo.ExpirationDate.Day;
            var crc1 = Crc32Algorithm.Compute(requestBytes, 0, 8);
            var crc2 = Crc32Algorithm.Compute(requestBytes, 8, 8);
            var message = new List<byte>()
            {
                year,month, day
            };
            message.AddRange(crc1.ToBytes());
            message.AddRange(crc2.ToBytes());

            byte[] encMessage;
            using (var rijndael = new RijndaelManaged
            {
                Key = _key,
                IV = _iv
            })
            {
                encMessage = EncryptBytes(rijndael, message.ToArray());
            }
            return GetHexString(encMessage);
        }

        private byte[] EncryptBytes(
            SymmetricAlgorithm alg,
            byte[] message)
        {
            if ((message == null) || (message.Length == 0))
            {
                return message;
            }

            if (alg == null)
            {
                throw new ArgumentNullException("alg");
            }

            using (var stream = new MemoryStream())
            using (var encryptor = alg.CreateEncryptor())
            using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(message, 0, message.Length);
                encrypt.FlushFinalBlock();
                return stream.ToArray();
            }
        }

        private byte[] DecryptBytes(
            SymmetricAlgorithm alg,
            byte[] message)
        {
            if ((message == null) || (message.Length == 0))
            {
                return message;
            }

            if (alg == null)
            {
                throw new ArgumentNullException("alg");
            }

            using (var stream = new MemoryStream())
            using (var decryptor = alg.CreateDecryptor())
            using (var encrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(message, 0, message.Length);
                encrypt.FlushFinalBlock();
                return stream.ToArray();
            }
        }

        public bool TryActivate(string activationKey, out LicenseInfo licenseInfo)
        {
            licenseInfo = null;
            var requestCode = GetRequestCode();
            var encActivationBytes = activationKey.GetBytesFromHexString();
            var requestBytes = requestCode.GetBytesFromHexString();
            var crc1_expected = Crc32Algorithm.Compute(requestBytes, 0, 8);
            var crc2_expected = Crc32Algorithm.Compute(requestBytes, 8, 8);
            byte[] activationBytes;
            using (var rijndael = new RijndaelManaged())
            {
                rijndael.Key = _key;
                rijndael.IV = _iv;
                activationBytes = DecryptBytes(rijndael, encActivationBytes);
            }
            var crc1_actual = activationBytes.ExtractUint(3);
            var crc2_actual = activationBytes.ExtractUint(7);
            if ((crc1_actual != crc1_expected) || (crc2_actual != crc2_expected))
                return false;
            licenseInfo = new LicenseInfo
            {
                ExpirationDate = new DateTime(2000 + activationBytes[0],
                    activationBytes[1], activationBytes[2]),
                RequestCode = requestCode
            };
            return true;
        }
    }
}
