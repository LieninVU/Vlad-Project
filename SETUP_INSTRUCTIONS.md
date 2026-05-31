# Инструкция по настройке проекта

## 📋 Что было сделано

Я внес изменения в ваш существующий проект ForVlad, добавив:

### 1. Слой данных (Data/)
- `LeasingDbContext.cs` - контекст базы данных Entity Framework
- `Repository.cs` - базовый репозиторий для работы с EF
- `UnitOfWork.cs` - Unit of Work для управления транзакциями

### 2. Сервисы (Services/)
- `ContractService.cs` - сервис для управления договорами
- `AssetService.cs` - сервис для управления техникой/оборудованием
- `CounterpartyService.cs` - сервис для управления контрагентами

### 3. Обновления
- `App.config` - добавлены строки подключения к SQL Server
- `ForVlad.csproj` - добавлены ссылки на Entity Framework
- `MainViewModel.cs` - обновлен для работы с сервисами
- `MainWindowSimple.xaml` - добавлен статус подключения к БД

## 🚀 Шаги по настройке

### 1. Установка Entity Framework
Откройте проект в Visual Studio 2022 и установите пакеты NuGet:

**Способ 1: Package Manager Console**
```powershell
Install-Package EntityFramework -Version 6.4.4
```

**Способ 2: NuGet Package Manager**
1. Правой кнопкой по проекту → "Manage NuGet Packages"
2. В поиске введите "EntityFramework"
3. Установите версию 6.4.4

### 2. Создание базы данных
Выполните SQL скрипты из папки `Scripts/` в следующем порядке:
1. `01_CreateDatabase.sql`
2. `02_CreateTables.sql`
3. `03_CreateConstraints.sql`
4. `04_CreateStoredProcedures.sql`
5. `05_SeedTestData.sql`

### 3. Настройка подключения
В файле `App.config` проверьте строку подключения:
```xml
<add name="LeasingDbConnection" 
     connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LeasingSystemDb;Integrated Security=True;" />
```

### 4. Запуск приложения
1. Убедитесь, что база данных создана
2. Запустите приложение (F5)
3. В статусной строке должно появиться сообщение "Подключение к базе данных установлено"

## 🔧 Проверка работы

### Тест подключения к БД
При запуске приложение автоматически проверяет подключение к базе данных. Результат отображается в верхней части окна.

### Навигация
Используйте боковое меню для перехода между разделами:
- **Договоры** - загружает активные договоры из БД
- **Контрагенты** - загружает всех контрагентов из БД
- **Техника/Оборудование** - загружает технику из БД

## 🐛 Возможные проблемы

### Ошибка: "Не удалось загрузить файл или сборку 'EntityFramework'"
**Решение**: Установите пакет EntityFramework через NuGet Package Manager.

### Ошибка: "Не удалось подключиться к базе данных"
**Решение**:
1. Проверьте, что SQL Server запущен
2. Убедитесь, что база данных `LeasingSystemDb` существует
3. Проверьте строку подключения в `App.config`

### Ошибка: "Имя 'PaymentStatus' не существует в текущем контексте"
**Решение**: Добавьте в `Enums.cs`:
```csharp
public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    Overdue = 3,
    Cancelled = 4
}
```

## 📁 Структура проекта после изменений

```
ForVlad/
├── Data/
│   ├── LeasingDbContext.cs
│   ├── Repository.cs
│   └── UnitOfWork.cs
├── Services/
│   ├── ContractService.cs
│   ├── AssetService.cs
│   └── CounterpartyService.cs
├── Models/ (существующие файлы)
├── ViewModels/ (обновленные файлы)
├── Views/ (обновленные файлы)
├── App.config (обновлен)
├── ForVlad.csproj (обновлен)
└── SETUP_INSTRUCTIONS.md (этот файл)
```

## 📞 Поддержка

Если возникнут проблемы:
1. Проверьте сообщения в статусной строке приложения
2. Убедитесь, что все пакеты NuGet установлены
3. Проверьте подключение к SQL Server
4. Убедитесь, что база данных создана и содержит таблицы