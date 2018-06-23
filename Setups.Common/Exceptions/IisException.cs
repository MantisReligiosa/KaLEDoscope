using System;

namespace Setups.Common.Exceptions
{
    /// <summary>
    /// Класс, представляющий собой исключение, возникшее в результате работы с IIS
    /// </summary>
    public class IisException : Exception
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="message">Сообщение, которое содержит исключение</param>
        public IisException(string message) : base(message)
        {
        }
    }
}
