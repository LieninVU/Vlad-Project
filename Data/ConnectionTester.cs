using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ForVlad.Data
{
    public static class ConnectionTester
    {
        public static void TestAllConnectionMethods()
        {
            Console.WriteLine("=== ТЕСТИРОВАНИЕ ПОДКЛЮЧЕНИЯ К SQL SERVER ===\n");
            
            // Метод 1: Из конфигурации
            TestFromConfig();
            
            // Метод 2: Хардкод для проверки
            TestHardcoded();
            
            // Метод 3: SqlConnectionStringBuilder
            TestWithBuilder();
        }
        
        private static void TestFromConfig()
        {
            Console.WriteLine("1. Проверка подключения из App.config:");
            
            try
            {
                // Проверяем, есть ли секция connectionStrings
                if (ConfigurationManager.ConnectionStrings.Count == 0)
                {
                    Console.WriteLine("   ❌ Секция connectionStrings пуста или не найдена!\n");
                    return;
                }
                
                // Выводим все доступные строки подключения
                Console.WriteLine($"   Найдено строк подключения: {ConfigurationManager.ConnectionStrings.Count}");
                foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
                {
                    Console.WriteLine($"   - Name: '{cs.Name}'");
                    Console.WriteLine($"     Provider: '{cs.ProviderName}'");
                    Console.WriteLine($"     ConnectionString: '{cs.ConnectionString}'");
                }
                
                // Пытаемся получить конкретную строку
                var settings = ConfigurationManager.ConnectionStrings["LeasingSystem"];
                if (settings == null)
                {
                    Console.WriteLine("   ❌ Строка подключения 'LeasingSystem' не найдена!\n");
                    return;
                }
                
                string connectionString = settings.ConnectionString;
                Console.WriteLine($"   ✅ Строка подключения получена: {connectionString}\n");
                
                TestConnection(connectionString, "   ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Ошибка: {ex.Message}\n");
            }
        }
        
        private static void TestHardcoded()
        {
            Console.WriteLine("2. Проверка с хардкодом (SQL Server Express):");
            
            // Пробуем разные варианты
            string[] connectionStrings = {
                "Server=(local)\\SQLEXPRESS;Database=LeasingSystem;Integrated Security=True;TrustServerCertificate=true;",
                "Server=localhost\\SQLEXPRESS;Database=LeasingSystem;Integrated Security=True;TrustServerCertificate=true;",
                "Server=.\\SQLEXPRESS;Database=LeasingSystem;Integrated Security=True;TrustServerCertificate=true;"
            };
            
            foreach (var connStr in connectionStrings)
            {
                Console.WriteLine($"   Пробуем: {connStr}");
                if (TestConnection(connStr, "   "))
                {
                    Console.WriteLine("   ✅ Успешно! Используйте эту строку.\n");
                    return;
                }
            }
            
            Console.WriteLine("   ❌ Все варианты не удались\n");
        }
        
        private static void TestWithBuilder()
        {
            Console.WriteLine("3. Проверка с SqlConnectionStringBuilder:");
            
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = @"(local)\SQLEXPRESS",
                InitialCatalog = "LeasingSystem",
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                ConnectTimeout = 15
            };
            
            Console.WriteLine($"   Строка: {builder.ConnectionString}");
            TestConnection(builder.ConnectionString, "   ");
        }
        
        private static bool TestConnection(string connectionString, string indent = "")
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    Console.WriteLine($"{indent}Открытие подключения...");
                    connection.Open();
                    
                    Console.WriteLine($"{indent}Выполнение тестового запроса...");
                    using (var command = new SqlCommand("SELECT @@VERSION", connection))
                    {
                        var result = command.ExecuteScalar();
                        Console.WriteLine($"{indent}✅ Подключение успешно!");
                        Console.WriteLine($"{indent}Версия SQL Server: {result.ToString().Substring(0, Math.Min(80, result.ToString().Length))}...\n");
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"{indent}❌ Ошибка SQL Server:");
                Console.WriteLine($"{indent}   Message: {ex.Message}");
                Console.WriteLine($"{indent}   Error Number: {ex.Number}");
                Console.WriteLine($"{indent}   State: {ex.State}");
                Console.WriteLine($"{indent}   Class: {ex.Class}\n");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{indent}❌ Общая ошибка: {ex.Message}\n");
                return false;
            }
        }
    }
}