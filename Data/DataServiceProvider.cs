using System;

namespace ForVlad.Data
{
    public static class DataServiceProvider
    {
        public static ISimpleDataService Create()
        {
            var service = new SqlDataService();
            if (!service.TestConnection(out var error))
            {
                throw new InvalidOperationException(
                    "Не удалось подключиться к базе данных LeasingSystem на сервере (local)\\SQLEXPRESS.\r\n\r\n" +
                    error + "\r\n\r\n" +
                    "Убедитесь, что:\r\n" +
                    "1. SQL Server Express запущен;\r\n" +
                    "2. Выполнены скрипты из папки Database (01–05);\r\n" +
                    "3. База LeasingSystem создана и доступна.");
            }
            return service;
        }
    }
}
