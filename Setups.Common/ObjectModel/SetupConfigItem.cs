namespace Setups.Common.ObjectModel
{
    /// <summary>
    /// Класс элемента конфигурации параметров установки
    /// </summary>
    public class SetupConfigItem
    {
        /// <summary>
        /// Наименование параметра установки
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Значение параметра установки
        /// </summary>
        public string Value { get; set; }
    }
}
