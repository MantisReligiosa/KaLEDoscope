using Setups.Common.Interfaces;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace Setups.Common.Managers
{
    /// <summary>
    /// Класс для взаимодействия с SQL сервером
    /// </summary>
    public class MsSqlManager : ISqlManager
    {
        /// <summary>
        /// Метод проверки корректности строки подключения
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        public void ValidateConnectionString(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
                try
                {
                    connection.Open();
                }
                catch (System.Exception ex)
                {
                    throw new Exceptions.SqlException(
                        "Не удалось установить соединение с БД с помощью строки подключения",
                        ex);
                }
        }

        /// <summary>
        /// Метод проверки доступности SQL сервера
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        public void CheckServerAvailability(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };

            ValidateConnectionString(builder.ConnectionString);
        }

        /// <summary>
        /// Метод проверки наличия прав доступа пользователя на создание БД
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        /// <returns>Возвращает признак, указывающий имеет ли пользователь права на создание БД</returns>
        public bool IsUserHasRestoreDatabaseRights(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };

            var query = $@"SELECT permission_name
                           FROM fn_my_permissions(NULL, 'DATABASE')
                           WHERE permission_name = 'CREATE DATABASE'";

            using (var connection = new SqlConnection(builder.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                    return reader.HasRows;
            }
        }

        /// <summary>
        /// Метод разворачивания базы данных из резервной копии
        /// </summary>
        /// <param name="backupDatabaseName">Наименование базы данных, 
        /// использующееся при создании резервной копии</param>
        /// <param name="backupFilePath">Путь к файлу резервной копии</param>
        /// <param name="connectionString">Строка подключения</param>
        public void RestoreDatabase(string backupDatabaseName,
            string backupFilePath, string connectionString)
        {
            if (IsDatabaseExists(connectionString))
                return;

            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = builder.InitialCatalog;

            builder.InitialCatalog = "master";

            var query =
                 $@"DECLARE @DefaultFile varchar(max)
                    DECLARE @DefaultLog varchar(max)

                    SELECT
	                    @DefaultFile = CAST(SERVERPROPERTY('instancedefaultdatapath') AS varchar(max)),
	                    @DefaultLog = CAST(SERVERPROPERTY('instancedefaultlogpath') AS varchar(max))

                    DECLARE @File varchar(max)
                    DECLARE @Log varchar(max)

                    SET @File = CONCAT(@DefaultFile, '{databaseName}.mdf')
                    SET @Log = CONCAT(@DefaultLog, '{databaseName}_log.mdf')

                    RESTORE DATABASE {databaseName}
                    FROM DISK = '{backupFilePath}'
                    WITH MOVE '{backupDatabaseName}' TO @File,
	                     MOVE '{backupDatabaseName}_log' TO @Log";

            using (var connection = new SqlConnection(builder.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.CommandTimeout = 60;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Метод для проверки существования базы данных
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        /// <returns>Возвращает признак, указывающий существует ли база данных</returns>
        public bool IsDatabaseExists(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            var query = $@"SELECT [name]
                           FROM sys.databases
                           WHERE [name] = '{builder.InitialCatalog}'";

            builder.InitialCatalog = "master";

            using (var connection = new SqlConnection(builder.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                    return reader.HasRows;
            }
        }

        /// <summary>
        /// Метод применения миграции к базе данных
        /// </summary>
        /// <param name="siteFolderPath">Путь до папки, содержащей файлы сайта</param>
        /// <param name="migrationFolderPath">Путь до папки, содержащей файлы миграций</param>
        public void ApplyMigrations(string siteFolderPath, string migrationFolderPath)
        {
            var processToStart = Path.Combine(migrationFolderPath, "migrate.exe");
            var parameters = $"Migrations.dll /startupConfigurationFile=\"{Path.Combine(siteFolderPath, "web.config")}\"";

            using (var process = Process.Start(processToStart, parameters))
                process.WaitForExit();
        }
    }
}