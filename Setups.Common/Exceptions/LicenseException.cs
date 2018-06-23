using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setups.Common.Exceptions
{
    /// <summary>
    /// Класс, представляющий собой исключение при генерации лицензионных ключей
    /// </summary>
    public class LicenseException : Exception
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="message">Сообщение, которое содержит исключение</param>
        public LicenseException(string message) : base(message)
        {
        }

        // <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="message">Сообщение, которое содержит исключение</param>
        /// <param name="ex">Внутреннее исключение</param>
        public LicenseException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
