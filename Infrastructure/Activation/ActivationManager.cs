using Newtonsoft.Json;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Activation
{
    public class ActivationManager : IActivationManager
    {
        private readonly string _activationFile = "Activation.key";
        private readonly ICompressor _compressor;
        private readonly ActivationInfo _activationInfo;

        public ActivationManager(ICompressor compressor)
        {
            _compressor = compressor;
            IsActivationInfoExists = File.Exists(_activationFile);
            if (!IsActivationInfoExists)
            {
                IsActivationInfoExists = false;
                _activationInfo = new ActivationInfo
                {
                    Trials = new List<TrialInfo>(),
                    IsFullAccess = false
                };
                return;
            }
            var bytes = File.ReadAllBytes(_activationFile);
            var text = _compressor.Unzip(bytes);
            _activationInfo = JsonConvert.DeserializeObject<ActivationInfo>(text);
        }

        public bool IsActivationInfoExists { get; private set; }

        public bool IsFullAccess => _activationInfo?.IsFullAccess ?? false;

        public DateTime TrialExpirationDate => _activationInfo.Trials?.Max(t => t.ExpirationDate) ?? new DateTime();

        public bool IsTrialKeyOriginal(string activationKey) => !_activationInfo?.Trials?.Any(t => t.ActivationKey.Equals(activationKey)) ?? true;

        public void AddTrial(string activationKey, DateTime trialExpirationDate)
        {
            _activationInfo.Trials.Add(new TrialInfo
            {
                ActivationKey = activationKey,
                ExpirationDate = trialExpirationDate
            });
            Submit();
        }

        private void Submit()
        {
            var serialized = JsonConvert.SerializeObject(_activationInfo);
            var datas = _compressor.Zip(serialized);
            File.WriteAllBytes(_activationFile, datas);
        }

        public void SetFullAccess()
        {
            _activationInfo.IsFullAccess = true;
            Submit();
        }

        private const string _salt = "$2a$10$SdjajHU2LSmnQSbelTsQdO";

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
            var processorID = GetHardwareParameter("Win32_Processor", "ProcessorId");
            var motherboardSerial = GetHardwareParameter("Win32_BaseBoard", "SerialNumber");
            var memorySerial = GetHardwareParameter("Win32_PhysicalMemory", "SerialNumber");
            var driveSerial = GetHardwareParameter("Win32_DiskDrive", "SerialNumber");

            var hardwareParameters = string.Concat(
                processorID, motherboardSerial, memorySerial, driveSerial);

            return hardwareParameters;
        }

        private static string GetHardwareParameter(string hardwareArea, string parameterName)
        {
            using (var searcher = new ManagementObjectSearcher($"SELECT * FROM {hardwareArea}"))
                return searcher.Get()
                    .Cast<ManagementObject>()
                    .First()[parameterName]
                    ?.ToString() ?? string.Empty;
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

        public string GetFullyActivationKey(string requestCode)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(requestCode, _salt);
            return GetMD5Code(hash);
        }

        public string GetTrialActivationKey(string requestCode)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(GetTrialRequestCode(requestCode), _salt);
            var key = GetMD5Code(hash);
            return key;
        }

        private string GetTrialRequestCode(string r) => r + "Trial";

        public bool IsFullyActivationKeyValid(string activationKey)
        {
            bool result;
            var requestCode = GetRequestCode();
            var correctActivationKey = GetFullyActivationKey(requestCode);
            result = correctActivationKey.Equals(activationKey);
            return result;
        }

        public bool IsTrialActivationKeyValid(string activationKey)
        {
            bool result;
            try
            {
                var requestCode = GetRequestCode();
                var correctActivationKey = GetTrialActivationKey(requestCode);
                result = correctActivationKey.Equals(activationKey);
                return result;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

    }
}
