using IniParser;
using IniParser.Model;
using System.IO;

namespace Configuration
{
    public class Config
    {
        private static Config _config;
        private const string _filePath = "Configuration.ini";
        private readonly FileIniDataParser _parser;
        private readonly IniData _data;

        public string GetParameter(string section, string key)
        {
            return _data[section][key];
        }

        public void SetParameter(string section, string key, string value)
        {
            _data[section][key] = value;
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
