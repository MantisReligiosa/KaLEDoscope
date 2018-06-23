using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Setups.Common.Managers
{
    /// <summary>
    /// Класс получения параметров конфигураций
    /// </summary>
    public static class AssemblyManager
    {
        /// <summary>
        /// Метод для получения параметров сборки
        /// </summary>
        /// <param name="guid">Параметр "Guid"</param>
        /// <param name="version">Параметр "Версия"</param>
        public static void GetAssemblyInfo(string assemblyPath,
            out Guid guid, out Version version)
        {
            var assembly = System.Reflection.Assembly.LoadFrom(assemblyPath);

            var guidValue = ((GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)
                .First()).Value;

            guid = new Guid(guidValue);
            version = new Version(assembly.GetName().Version.ToString(3));
        }
    }
}
