using System;
using System.IO;

namespace ForVlad.Data
{
    /// <summary>
    /// Конфигурация подключения к базе данных из переменных окружения
    /// </summary>
    public class DatabaseConfig
    {
        // Параметры подключения
        public string Server { get; set; }
        public string Database { get; set; }
        public string AuthType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int ConnectionTimeout { get; set; }
        public bool TrustCertificate { get; set; }
        public bool Encrypt { get; set; }

        // Настройки приложения
        public int DaysInMonth { get; set; }
        public int PageSize { get; set; }
        public string UiTheme { get; set; }
        public string DbMode { get; set; }
        public bool SqlLogging { get; set; }

        /// <summary>
        /// Загружает конфигурацию из .env файла и переменных окружения
        /// </summary>
        public static DatabaseConfig Load()
        {
            var config = new DatabaseConfig();

            // Пытаемся загрузить .env файл
            LoadEnvFile();

            // Читаем переменные окружения (с значениями по умолчанию)
            config.Server = GetEnvVar("SQL_SERVER", "(local)\\SQLEXPRESS");
            config.Database = GetEnvVar("SQL_DATABASE", "LeasingSystem");
            config.AuthType = GetEnvVar("SQL_AUTH_TYPE", "Windows");
            config.Username = GetEnvVar("SQL_USERNAME", "");
            config.Password = GetEnvVar("SQL_PASSWORD", "");
            config.ConnectionTimeout = GetEnvInt("SQL_CONNECTION_TIMEOUT", 30);
            config.TrustCertificate = GetEnvBool("SQL_TRUST_CERTIFICATE", true);
            config.Encrypt = GetEnvBool("SQL_ENCRYPT", false);

            config.DaysInMonth = GetEnvInt("DAYS_IN_MONTH", 30);
            config.PageSize = GetEnvInt("PAGE_SIZE", 50);
            config.UiTheme = GetEnvVar("UI_THEME", "Light");
            config.DbMode = GetEnvVar("DB_MODE", "Production");
            config.SqlLogging = GetEnvBool("SQL_LOGGING", false);

            return config;
        }

        /// <summary>
        /// Строит строку подключения на основе конфигурации
        /// </summary>
        public string BuildConnectionString()
        {
            var builder = new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                DataSource = Server,
                InitialCatalog = Database,
                ConnectTimeout = ConnectionTimeout,
                TrustServerCertificate = TrustCertificate,
                Encrypt = Encrypt,
                MultipleActiveResultSets = true
            };

            if (AuthType.Equals("Windows", StringComparison.OrdinalIgnoreCase))
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = Username;
                builder.Password = Password;
            }

            return builder.ConnectionString;
        }

        /// <summary>
        /// Загружает .env файл в переменные окружения
        /// </summary>
        private static void LoadEnvFile()
        {
            // Ищем .env файл в нескольких местах
            var possiblePaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env"),
                Path.Combine(Directory.GetCurrentDirectory(), ".env"),
                Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName ?? "", ".env")
            };

            foreach (var envPath in possiblePaths)
            {
                if (File.Exists(envPath))
                {
                    LoadEnvFileContent(envPath);
                    break;
                }
            }
        }

        /// <summary>
        /// Читает содержимое .env файла
        /// </summary>
        private static void LoadEnvFileContent(string filePath)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();

                    // Пропускаем комментарии и пустые строки
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                        continue;

                    // Парсим KEY=VALUE
                    var separatorIndex = trimmed.IndexOf('=');
                    if (separatorIndex > 0)
                    {
                        var key = trimmed.Substring(0, separatorIndex).Trim();
                        var value = trimmed.Substring(separatorIndex + 1).Trim();

                        // Устанавливаем переменную окружения, если она ещё не задана
                        if (Environment.GetEnvironmentVariable(key) == null)
                        {
                            Environment.SetEnvironmentVariable(key, value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки .env файла: {ex.Message}");
            }
        }

        /// <summary>
        /// Получает значение переменной окружения
        /// </summary>
        private static string GetEnvVar(string key, string defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(key);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        /// <summary>
        /// Получает целочисленное значение переменной окружения
        /// </summary>
        private static int GetEnvInt(string key, int defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (int.TryParse(value, out int result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Получает булево значение переменной окружения
        /// </summary>
        private static bool GetEnvBool(string key, bool defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (bool.TryParse(value, out bool result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Возвращает информацию о конфигурации для отображения
        /// </summary>
        public override string ToString()
        {
            return $"Server: {Server}\n" +
                   $"Database: {Database}\n" +
                   $"AuthType: {AuthType}\n" +
                   $"DbMode: {DbMode}";
        }
    }
}
