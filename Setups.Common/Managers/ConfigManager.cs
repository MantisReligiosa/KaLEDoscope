using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Setups.Common.Managers
{
    /// <summary>
    /// Класс для работы с конфигом в формате xml
    /// </summary>
    public static class ConfigManager
    {
        /// <summary>
        /// Метод для изменения параметра Value для тэга в XML-документе
        /// </summary>
        /// <param name="key">Значение параметра key для изменяемого тэга</param>
        /// <param name="value">Новое значение параметра value</param>
        /// <param name="xmlDocument">Изменяемый документ</param>
        public static void SetXmlPropertyValue(string key, string value, XDocument xmlDocument)
        {
            xmlDocument.XPathSelectElement($"configuration/appSettings/add[@key='{key}']")
                .Attribute("value").SetValue(value);
        }

        /// <summary>
        /// Метод для изменения строки подключения в XML-документе
        /// </summary>
        /// <param name="connectionStringName">Имя строки подключения</param>
        /// <param name="connectionStringValue">Значение строки подключения</param>
        /// <param name="xmlDocument">Изменяемый документ</param>
        public static void SetXmlConnectionString(string connectionStringName, string connectionStringValue, XDocument xmlDocument)
        {
            xmlDocument.XPathSelectElement($"configuration/connectionStrings/add[@name='{connectionStringName}']")
                .Attribute("connectionString").SetValue(connectionStringValue);
        }

        /// <summary>
        /// Удалить элементы не пренадлежащие к Application
        /// </summary>
        public static XElement RemoveExceptForApplication(this XElement xmlElement, string applicationName)
        {
            var deleteBeginning = false;
            foreach (XNode r in xmlElement.Nodes().ToList())
            {
                var nodest = r.ToString();
                if (nodest.IndexOf("%Application=") >= 0 && nodest.IndexOf($"%Application=\"{applicationName}\"%") == -1)
                    deleteBeginning = true;

                if (deleteBeginning)
                    r.Remove();

                if (nodest.IndexOf("%/Application%") >= 0)
                    deleteBeginning = false;
            }
            return xmlElement;
        }
    }
}
