namespace Setups.Common.Interfaces
{
    /// <summary>
    /// Интерфейс взаимодействия с SQL сервером
    /// </summary>
    public interface ISqlManager
    {
        /// <summary>
        /// Метод применения миграции к базе данных
        /// </summary>
        /// <param name="siteFolderPath">Путь до папки, содержащей файлы сайта</param>
        /// <param name="migrationFolderPath">Путь до папки, содержащей файлы миграций</param>
        void ApplyMigrations(string siteFolderPath, string migrationFolderPath);

        /// <summary>
        /// Метод проверки корректности строки подключения
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        void ValidateConnectionString(string connectionString);

        /// <summary>
        /// Метод разворачивания базы данных из резервной копии
        /// </summary>
        /// <param name="backupDatabaseName">Наименование базы данных, 
        /// использующееся при создании резервной копии</param>
        /// <param name="backupFilePath">Путь к файлу резервной копии</param>
        /// <param name="connectionString">Строка подключения</param>
        void RestoreDatabase(string backupDatabaseName, string backupFilePath, string connectionString);

        /// <summary>
        /// Метод проверки доступности SQL сервера
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        void CheckServerAvailability(string connectionString);

        /// <summary>
        /// Метод проверки наличия прав доступа пользователя на создание БД
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        /// <returns>Возвращает признак, указывающий имеет ли пользователь права на создание БД</returns>
        bool IsUserHasRestoreDatabaseRights(string connectionString);

        /// <summary>
        /// Метод для проверки существования базы данных
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        /// <returns>Возвращает признак, указывающий существует ли база данных</returns>
        bool IsDatabaseExists(string connectionString);
    }
}