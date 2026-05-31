# Инструкция по пересозданию базы данных LeasingSystem

## Проблема
При запуске приложения возникали ошибки:
1. "Ошибка подключения к базе данных. Заданное приведение является недопустимым."
2. "Недопустимое имя столбца OGRN"
3. Конфликты PK и FK при выполнении скрипта 05_SeedData.sql
4. Проблемы с кодировкой в существующих данных
5. InvalidCastException при переходе на вкладку Финансовые отчеты

## Решение

### Шаг 1: Удалите старую базу данных (если существует)
1. Откройте SQL Server Management Studio
2. Подключитесь к вашему серверу SQL Server
3. В обозревателе объектов найдите базу данных `LeasingSystem`
4. Щелкните правой кнопкой мыши и выберите "Удалить"
5. Подтвердите удаление

### Шаг 2: Создайте новую базу данных
Выполните скрипты в следующем порядке:

1. **01_CreateDatabase.sql** - Создает базу данных LeasingSystem
2. **02_CreateTables.sql** - Создает все таблицы
3. **03_CreateConstraints.sql** - Создает ограничения, индексы и представления
4. **04_CreateStoredProcedures.sql** - Создает хранимые процедуры
5. **05_SeedData.sql** - Заполняет базу тестовыми данными

Вы можете выполнить все скрипты сразу через SQL Server Management Studio:
- Откройте каждый файл .sql
- Нажмите F5 для выполнения

ИЛИ через командную строку:
```bash
sqlcmd -S .\SQLEXPRESS -i "01_CreateDatabase.sql" -d master
sqlcmd -S .\SQLEXPRESS -i "02_CreateTables.sql" -d LeasingSystem
sqlcmd -S .\SQLEXPRESS -i "03_CreateConstraints.sql" -d LeasingSystem
sqlcmd -S .\SQLEXPRESS -i "04_CreateStoredProcedures.sql" -d LeasingSystem
sqlcmd -S .\SQLEXPRESS -i "05_SeedData.sql" -d LeasingSystem
```
(Замените `.\SQLEXPRESS` на ваше имя сервера)

### Шаг 3: Проверьте соединение
Выполните файл `check_database_connection.bat` чтобы убедиться, что подключение к базе данных работает корректно.

## Что было исправлено

### В SQL скриптах:
1. **05_SeedData.sql**:
   - Добавлена очистка таблиц перед вставкой данных
   - Добавлен сброс идентификаторов (DBCC CHECKIDENT)
   - Использованы латинские префиксы для номеров договоров (AR, LS, DR вместо АР, ЛЗ, ЧЕР) для избежания проблем с кодировкой
   - Убедиться, что для AssetGroup=1 (Equipment) поля VehicleBrand и VehicleModel равны NULL

### В C# коде:

1. **Models/ReportModels.cs**:
   - Изменен тип `TotalAmount` в `PaymentReportRow` с `decimal` на `decimal?` (nullable)

2. **Services/ReportCalculationService.cs**:
   - Добавлена обработка null значений в методе `BuildFinancialSummary`:
     ```csharp
     .Sum(r => r.TotalAmount ?? 0)
     ```

3. **Data/SqlDataService.cs**:
   - Исправлен расчет `total` в методе `GetPaymentReport`:
     ```csharp
     var total = payment.TotalAmount ?? payment.Amount;
     ```

4. **ViewModels/FinancialReportsViewModel.cs**:
   - Добавлена проверка на null при экспорте в CSV:
     ```csharp
     (p.TotalAmount ?? 0).ToString("N2")
     ```

5. **Data/EntityMapper.cs**:
   - Добавлена проверка на null для Description в ReadPayment

## Дополнительные рекомендации

1. **Кодировка**: Убедитесь, что все строковые значения в базе данных используют Unicode (предварение строки `N'...'`).

2. **Права доступа**: Убедитесь, что пользователь, указанный в строке подключения, имеет достаточно прав на создание и изменение базы данных.

3. **Строка подключения**: Проверьте, что строка подключения в `DatabaseConnection.cs` или `App.config` правильная.

4. **Если ошибки продолжаются**:
   - Проверьте файл `App.config` - строка подключения должна выглядеть примерно так:
     ```xml
     <add name="LeasingSystem" connectionString="Server=.\SQLEXPRESS;Database=LeasingSystem;Integrated Security=True;" />
     ```
   - Убедитесь, что SQL Server запущен

## Проверка успешности
После выполнения всех шагов:
1. Приложение должно запускаться без ошибок
2. Вкладка "Финансовые отчеты" должна открываться без InvalidCastException
3. Все данные должны отображаться корректно (без проблем с кодировкой)
