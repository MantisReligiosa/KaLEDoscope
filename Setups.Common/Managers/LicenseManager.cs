using Setups.Common.Exceptions;
using System;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Setups.Common.Managers
{
    /// <summary>
    /// Класс для генерации активационной информации
    /// </summary>
    public class LicenseManager
    {
        private const string _salt = "$2a$10$SdjajHU2LSmnQSbelTsQdO";

        /// <summary>
        /// Метод для получения кода запроса на получение ключа активации
        /// </summary>
        /// <returns>Возвращает строку, содержащую код запроса</returns>
        public string GetRequestCode()
        {
            var id = GetUniqueHardwareId();
            return GetMD5Code(id);
        }

        /// <summary>
        /// Метод для получения кода в шестнадцатиричном формате
        /// </summary>
        /// <param name="code">Строка для преобразования</param>
        /// <returns>Возвращает строку, содержащую шестнадцатиричный код</returns>
        private string GetMD5Code(string code)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bytes = new UTF8Encoding().GetBytes(code);
                return GetHexString(md5.ComputeHash(bytes));
            }
        }

        /// <summary>
        ///  Метод для получения идентификатора оборудования
        /// </summary>
        /// <returns>Возвращает строку, содержащую идентификатор оборудования</returns>
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

        /// <summary>
        /// Получение информации об оборудовании
        /// </summary>
        /// <param name="hardwareArea">Область данных об оборудовании</param>
        /// <param name="parameterName">Наименование параметра поиска</param>
        /// <returns></returns>
        private static string GetHardwareParameter(string hardwareArea, string parameterName)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher($"SELECT * FROM {hardwareArea}"))
                    return searcher.Get()
                        .Cast<ManagementObject>()
                        .First()[parameterName]
                        ?.ToString() ?? string.Empty;
            }
            catch (Exception e)
            {
                throw new LicenseException("Ошибка генерации кода запроса", e);
            }
        }

        /// <summary>
        /// Метод для преобразования массива байт в шестнадцатиричный код
        /// </summary>
        /// <param name="bytes">Массив байт</param>
        /// <returns>Возвращает строку, содержащую шестнадцатиричный код</returns>
        private string GetHexString(byte[] bytes)
        {
            var hex = string.Empty;

            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];

                var n = (int)b;
                var n1 = n & 15;
                var n2 = (n >> 4) & 15;

                hex += n2 > 9
                    ? ((char)(n2 - 10 + 'A')).ToString()
                    : n2.ToString();

                hex += n1 > 9
                   ? ((char)(n1 - 10 + 'A')).ToString()
                   : n1.ToString();

                if (i + 1 != bytes.Length && (i + 1) % 2 == 0)
                    hex += "-";
            }

            return hex;
        }

        /// <summary>
        /// Метод для получения кода активации
        /// </summary>
        /// <param name="requestCode">Код запроса</param>
        /// <returns>Возвращает строку, содержащую код активации</returns>
        public string GetActivationCode(string requestCode)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(requestCode, _salt);
            return GetMD5Code(hash);
        }

        /// <summary>
        /// Метод для верификации кода активации
        /// </summary>
        /// <param name="activationCode">Код активации</param>
        /// <returns>Возвращает признак успешности верификации</returns>
        public bool IsActivationCodeValid(string activationCode)
        {
            var requestCode = GetRequestCode();
            return activationCode == GetActivationCode(requestCode);
        }
    }
}
