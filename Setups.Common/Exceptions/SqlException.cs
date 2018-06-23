using System;

namespace Setups.Common.Exceptions
{
    /// <summary>
    /// Класс, представляющий собой исключение, возникшее в результате работы с IIS
    /// </summary>
    public class SqlException : Exception
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="message">Сообщение, которое содержит исключение</param>
        public SqlException(string message) : base(message)
        {
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="message">Сообщение, которое содержит исключение</param>
        /// <param name="ex">Внутреннее исключение</param>
        public SqlException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
