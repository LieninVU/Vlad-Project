using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ForVlad.Data
{
    public class DatabaseConnection
    {
        private static string _cachedConnectionString;
        private static DatabaseConfig _config;
        
        /// <summary>
        /// Получает конфигурацию базы данных
        /// </summary>
        public static DatabaseConfig Config
        {
            get
            {
                if (_config == null)
                    _config = DatabaseConfig.Load();
                return _config;
            }
        }
        
        /// <summary>
        /// Получает строку подключения с кэшированием
        /// </summary>
        public static string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(_cachedConnectionString))
                return _cachedConnectionString;
            
            // Способ 1: Из .env файла (приоритет!)
            try
            {
                if (Config.DbMode.Equals("Production", StringComparison.OrdinalIgnoreCase))
                {
                    _cachedConnectionString = Config.BuildConnectionString();
                    Console.WriteLine($"[INFO] Используется конфигурация из .env: {Config.Server}");
                    return _cachedConnectionString;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Не удалось загрузить .env: {ex.Message}");
            }
            
            // Способ 2: Из ConfigurationManager (App.config)
            try
            {
                var settings = ConfigurationManager.ConnectionStrings["LeasingSystem"];
                if (settings != null)
                {
                    _cachedConnectionString = settings.ConnectionString;
                    return _cachedConnectionString;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Не удалось прочитать из конфигурации: {ex.Message}");
            }
            
            // Способ 3: Фолбэк на конфигурацию по умолчанию
            _cachedConnectionString = Config.BuildConnectionString();
            return _cachedConnectionString;
        }
        
        /// <summary>
        /// Тестирует подключение к базе данных
        /// </summary>
        public static bool TestConnection()
        {
            string connectionString = GetConnectionString();
            
            Console.WriteLine($"\n[INFO] Тестирование подключения...");
            Console.WriteLine($"[INFO] Сервер: {Config.Server}");
            Console.WriteLine($"[INFO] База данных: {Config.Database}");
            Console.WriteLine($"[INFO] Режим: {Config.DbMode}");
            
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    using (var command = new SqlCommand("SELECT @@VERSION", connection))
                    {
                        object result = command.ExecuteScalar();
                        string version = result?.ToString() ?? "Unknown";
                        
                        Console.WriteLine($"\n[SUCCESS] Подключение успешно!");
                        Console.WriteLine($"[INFO] SQL Server: {version.Substring(0, Math.Min(80, version.Length))}...");
                        
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"\n[ERROR] Ошибка SQL Server:");
                Console.WriteLine($"   Код ошибки: {ex.Number}");
                Console.WriteLine($"   Сообщение: {ex.Message}");
                Console.WriteLine($"\n[INFO] Проверьте файл .env с настройками подключения");
                
                // Пробуем альтернативные варианты
                return TryAlternativeConnections();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Общая ошибка: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Пробует альтернативные варианты подключения
        /// </summary>
        private static bool TryAlternativeConnections()
        {
            Console.WriteLine("\n[INFO] Пробуем альтернативные варианты подключения...");
            
            string[] servers = {
                @"(local)\SQLEXPRESS",
                @"localhost\SQLEXPRESS",
                @".\SQLEXPRESS",
                @"127.0.0.1\SQLEXPRESS",
                @"(localdb)\MSSQLLocalDB"
            };
            
            foreach (var server in servers)
            {
                Console.WriteLine($"\n[INFO] Пробуем сервер: {server}");
                
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = server,
                    InitialCatalog = "LeasingSystem",
                    IntegratedSecurity = true,
                    TrustServerCertificate = true,
                    ConnectTimeout = 5
                };
                
                try
                {
                    using (var connection = new SqlConnection(builder.ConnectionString))
                    {
                        connection.Open();
                        
                        Console.WriteLine($"[SUCCESS] Подключение успешно!");
                        Console.WriteLine($"\n[INFO] Создайте файл .env со следующим содержимым:");
                        Console.WriteLine("─────────────────────────────────────");
                        Console.WriteLine($"SQL_SERVER={server}");
                        Console.WriteLine("SQL_DATABASE=LeasingSystem");
                        Console.WriteLine("SQL_AUTH_TYPE=Windows");
                        Console.WriteLine("─────────────────────────────────────");
                        
                        // Кэшируем рабочую строку
                        _cachedConnectionString = builder.ConnectionString;
                        
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[FAILED] {ex.Message}");
                }
            }
            
            Console.WriteLine("\n[ERROR] Не удалось подключиться ни к одному серверу");
            Console.WriteLine("\n[INFO] Убедитесь, что SQL Server установлен и запущен");
            Console.WriteLine("[INFO] Проверьте службу: services.msc → SQL Server (SQLEXPRESS)");
            return false;
        }
        
        /// <summary>
        /// Создает и возвращает новое подключение
        /// </summary>
        public static SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(GetConnectionString());
            return connection;
        }
        
        /// <summary>
        /// Выполняет скалярный запрос
        /// </summary>
        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    
                    return command.ExecuteScalar();
                }
            }
        }
        
        /// <summary>
        /// Выполняет запрос, не возвращающий данные
        /// </summary>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    
                    return command.ExecuteNonQuery();
                }
            }
        }
        
        /// <summary>
        /// Возвращает отображаемое имя подключения
        /// </summary>
        public static string GetDisplayName()
        {
            try
            {
                return $"{Config.Server} / {Config.Database}";
            }
            catch
            {
                return "Не определено";
            }
        }
        
        /// <summary>
        /// Сбрасывает кэш строки подключения (для перезагрузки конфигурации)
        /// </summary>
        public static void ResetCache()
        {
            _cachedConnectionString = null;
            _config = null;
        }
    }
}
