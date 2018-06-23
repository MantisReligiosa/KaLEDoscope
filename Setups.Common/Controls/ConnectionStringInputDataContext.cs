using System.ComponentModel;
using System.Data.SqlClient;

namespace Setups.Common.Controls
{
    /// <summary>
    /// Контекст данных для пользовательского элемента ввода строки подключения
    /// </summary>
    internal class ConnectionStringInputDataContext : INotifyPropertyChanged
    {
        private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder =
            new SqlConnectionStringBuilder();

        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _sqlConnectionStringBuilder.ConnectionString;
            }
            set
            {
                _sqlConnectionStringBuilder.ConnectionString = value;
                OnPropertyChanged(nameof(ConnectionString));
                OnPropertyChanged(nameof(DataSource));
                OnPropertyChanged(nameof(InitialCatalog));
                OnPropertyChanged(nameof(AllowWindowAuthorization));
                OnPropertyChanged(nameof(UseCredits));
                OnPropertyChanged(nameof(Login));
                OnPropertyChanged(nameof(Password));
            }
        }

        /// <summary>
        /// Адрес сервера
        /// </summary>
        public string DataSource
        {
            get
            {
                return _sqlConnectionStringBuilder.DataSource;
            }
            set
            {
                _sqlConnectionStringBuilder.DataSource = value;
                OnPropertyChanged(nameof(DataSource));
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        /// <summary>
        /// Название базы данных
        /// </summary>
        public string InitialCatalog
        {
            get
            {
                return _sqlConnectionStringBuilder.InitialCatalog;
            }
            set
            {
                _sqlConnectionStringBuilder.InitialCatalog = value;
                OnPropertyChanged(nameof(InitialCatalog));
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        /// <summary>
        /// Признак использования авторизации Windows
        /// </summary>
        public bool AllowWindowAuthorization
        {
            get
            {
                return _sqlConnectionStringBuilder.IntegratedSecurity;
            }
            set
            {
                _sqlConnectionStringBuilder.IntegratedSecurity = value;
                if (value)
                {
                    _sqlConnectionStringBuilder.Remove("User ID");
                    _sqlConnectionStringBuilder.Remove("Password");
                }
                OnPropertyChanged(nameof(AllowWindowAuthorization));
                OnPropertyChanged(nameof(Login));
                OnPropertyChanged(nameof(Password));
                OnPropertyChanged(nameof(UseCredits));
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        public bool UseCredits
        {
            get
            {
                return !AllowWindowAuthorization;
            }
        }

        /// <summary>
        /// Логин для авторизации
        /// </summary>
        public string Login
        {
            get
            {
                return _sqlConnectionStringBuilder.UserID;
            }
            set
            {
                _sqlConnectionStringBuilder.UserID = value;
                OnPropertyChanged(nameof(Login));
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        /// <summary>
        /// Пароль для авторизации
        /// </summary>
        public string Password
        {
            get
            {
                return _sqlConnectionStringBuilder.Password;
            }
            set
            {
                _sqlConnectionStringBuilder.Password = value;
                OnPropertyChanged(nameof(Password));
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        /// <summary>
        /// Возникает при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
