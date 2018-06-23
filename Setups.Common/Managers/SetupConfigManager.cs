using Setups.Common.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Setups.Common.Managers
{
    /// <summary>
    /// Класс для работы с файлом конфигурации установки
    /// </summary>
    public static class SetupConfigManager
    {
        /// <summary>
        /// Метод для чтения данных из пользовательского файла конфигурации
        /// </summary>
        /// <param name="msiFilePath">Путь к пользовательскому файлу конфигурации</param>
        /// <returns>Возвращает коллекцию элементов конфигурационного файла</returns>
        public static IEnumerable<SetupConfigItem> Get(string msiFilePath)
        {
            if (!File.Exists(msiFilePath))
                throw new FileNotFoundException();

            var fileName = Path.GetFileNameWithoutExtension(msiFilePath);
            var configPath = Path.Combine(Path.GetDirectoryName(msiFilePath),
                string.Concat(fileName, ".config"));

            if (!File.Exists(configPath))
                return new SetupConfigItem[] { };

            return File.ReadAllLines(configPath)
                .Select(a => a.Split('='))
                .Where(a => a.Length > 1)
                .Select(a => new SetupConfigItem
                {
                    Name = a[0],
                    Value = string.Join("=", a.Skip(1))
                });
        }
    }
}
