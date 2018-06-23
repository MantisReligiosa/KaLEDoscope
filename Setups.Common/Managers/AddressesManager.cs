using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Setups.Common.Managers
{
    /// <summary>
    /// Класс для взаимодействия с IP-адресами сервера
    /// </summary>
    public static class AddressesManager
    {
        /// <summary>
        /// Метод для получения коллекции IP-адресов
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAddresses()
        {
            return Dns.GetHostAddresses(Dns.GetHostName())
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                .Select(a => a.ToString());
        }
    }
}
