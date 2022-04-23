using IniParser;
using IniParser.Model;
using ServiceInterfaces;
using SmartTechnologiesM.Base.Extensions;
using System.IO;

namespace Configuration
{
    public class Config : IConfig
    {
        private static Config _config;
        private const string _filePath = "Configuration.ini";
        private readonly FileIniDataParser _parser;
        private readonly IniData _data;

        public string Key => "1f5gF$7gn5ugRj5uHfk&lg548G5m*ff7";
        public string IV => "fTG%85jGfi@4j*Eq";

        public int ScanPort
        {
            get => GetParameter("Network", "ScanPort", 30000);
            set => SetParameter("Network", "ScanPort", value.ToString());
        }
        public int ScanPeriod
        {
            get => GetParameter("Network", "ScanPeriod", 10000);
            set => SetParameter("Network", "ScanPeriod", value.ToString());
        }
        public int AutosavePeriod
        {
            get => GetParameter("Autosave", "Period", 5000);
            set => SetParameter("Autosave", "Period", value.ToString());
        }
        public string AutosaveFilename
        {
            get => GetParameter("Autosave", "Filename", "temp");
            set => SetParameter("Autosave", "Filename", value);
        }
        public int RequestPort
        {
            get => GetParameter("Network", "RequestPort", 500);
            set => SetParameter("Network", "RequestPort", value.ToString());
        }
        public int ResponceTimeout
        {
            get => GetParameter("Network", "ResponceTimeout", 100);
            set => SetParameter("Network", "ResponceTimeout", value.ToString());
        }
        public int ScanInterfaceNumber
        {
            get => GetParameter("Network", "ScanInterfaceIndex", 0);
            set => SetParameter("Network", "ScanInterfaceIndex", value.ToString());
        }

        private int GetParameter(string section, string key, int defaultValue)
        {
            var value = GetConfig()._data[section][key];
            if (value.IsNull() || !int.TryParse(value, out var result))
            {
                return defaultValue;
            }
            else
            {
                return result;
            }
        }

        private string GetParameter(string section, string key, string defaultValue)
        {
            var value = GetConfig()._data[section][key];
            if (value.IsNull())
            {
                return defaultValue;
            }
            return value;
        }

        private void SetParameter(string section, string key, string value)
        {
            GetConfig()._data[section][key] = value;
        }

        private Config()
        {
            _parser = new FileIniDataParser();
            if (!File.Exists(_filePath))
            {
                File.Create(_filePath).Close();
            }
            _data = _parser.ReadFile(_filePath);
        }

        public void Save()
        {
            _parser.WriteFile(_filePath, _data);
        }

        public static Config GetConfig()
        {
            if (_config == null)
            {
                _config = new Config();
            }
            return _config;
        }
    }
}
