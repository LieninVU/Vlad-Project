using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ForVlad.Data
{
    public class DatabaseConnection
    {
        private static string _cachedConnectionString;
        
        /// <summary>
        /// Получает строку подключения с кэшированием
        /// </summary>
        public static string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(_cachedConnectionString))
                return _cachedConnectionString;
            
            // Способ 1: Из ConfigurationManager
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
            
            // Способ 2: Фолбэк на хардкод (для отладки)
            _cachedConnectionString = BuildConnectionString();
            return _cachedConnectionString;
        }
        
        /// <summary>
        /// Строит строку подключения программно
        /// </summary>
        private static string BuildConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = @"(local)\SQLEXPRESS",
                InitialCatalog = "LeasingSystem",
                IntegratedSecurity = true,
                MultipleActiveResultSets = true,
                TrustServerCertificate = true,
                ConnectTimeout = 30
            };
            
            return builder.ConnectionString;
        }
        
        /// <summary>
        /// Тестирует подключение к базе данных
        /// </summary>
        public static bool TestConnection()
        {
            string connectionString = GetConnectionString();
            
            Console.WriteLine($"[INFO] Попытка подключения с строкой: {connectionString}");
            
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    using (var command = new SqlCommand("SELECT @@VERSION", connection))
                    {
                        object result = command.ExecuteScalar();
                        string version = result?.ToString() ?? "Unknown";
                        
                        Console.WriteLine($"[SUCCESS] Подключение успешно!");
                        Console.WriteLine($"[INFO] SQL Server Version: {version.Substring(0, Math.Min(100, version.Length))}...");
                        
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[ERROR] Ошибка SQL Server:");
                Console.WriteLine($"   Message: {ex.Message}");
                Console.WriteLine($"   Error Number: {ex.Number}");
                Console.WriteLine($"   State: {ex.State}");
                Console.WriteLine($"   Class: {ex.Class}");
                
                // Пробуем альтернативные варианты
                return TryAlternativeConnections();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Общая ошибка: {ex.Message}");
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
                @"127.0.0.1\SQLEXPRESS"
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
                        
                        Console.WriteLine($"[SUCCESS] Подключение успешно с сервером: {server}");
                        Console.WriteLine($"[INFO] Обновите App.config, используя: Server={server}");
                        
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
        /// Возвращает отображаемое имя подключения (например: "(local)\SQLEXPRESS / LeasingSystem")
        /// </summary>
        public static string GetDisplayName()
        {
            try
            {
                string connectionString = GetConnectionString();
                var builder = new SqlConnectionStringBuilder(connectionString);
                
                string dataSource = builder.DataSource;
                string initialCatalog = builder.InitialCatalog;
                
                if (string.IsNullOrEmpty(dataSource) && string.IsNullOrEmpty(initialCatalog))
                {
                    return "Не определено";
                }
                
                return $"{dataSource} / {initialCatalog}";
            }
            catch
            {
                return "Не определено";
            }
        }
    }
}