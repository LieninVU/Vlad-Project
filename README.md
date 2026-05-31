# LeasingSystem — Система учёта договоров аренды и лизинга спецтехники

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blueviolet)
![WPF](https://img.shields.io/badge/WPF-MVVM-brightgreen)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Express-informational)
![License](https://img.shields.io/badge/License-MIT-red)

**LeasingSystem** — десктопное WPF-приложение для управления договорами аренды и лизинга специальной техники. Система позволяет вести учёт контрагентов, техники, договоров, графиков платежей и формировать отчётность.

---

## 📋 Оглавление

- [📌 Требования](#-требования)
- [🚀 Установка и запуск](#-установка-и-запуск)
- [📊 Конфигурация](#-конфигурация)
- [🎯 Возможности](#-возможности)
- [🏗️ Структура проекта](#-структура-проекта)
- [🔧 Технологии](#-технологии)
- [📝 API и интеграции](#-api-и-интеграции)
- [🔄 Планы по развитию](#-планы-по-развитию)
- [📄 Лицензия](#-лицензия)

---

## 📌 Требования

### Системные требования

| Компонент | Версия | Примечание |
|-----------|--------|------------|
| Операционная система | Windows 10/11 | Windows 7 не поддерживается |
| .NET Framework | 4.7.2 | Обязателен |
| SQL Server | Express 2019+ | Поддерживается также LocalDB |
| Visual Studio | 2019+ | Для сборки проекта |

### Зависимости NuGet

Все необходимые пакеты включены в проект через стандартные сборки .NET Framework. Дополнительные пакеты не требуются.

---

## 🚀 Установка и запуск

### 1. Клонирование репозитория

```bash
cd C:\Users\STAR BUTTERFLY\source\repos
git clone https://github.com/your-repo/ForVlad.git
cd ForVlad
```

### 2. Настройка базы данных

#### Вариант А: Автоматическая настройка (рекомендуется)

Выполните скрипты из папки `Database/` в указанном порядке:

1. **01_CreateDatabase.sql** — Создание базы данных LeasingSystem
2. **02_CreateTables.sql** — Создание таблиц (Counterparties, Assets, Contracts и др.)
3. **03_CreateConstraints.sql** — Создание ограничений CHECK и внешних ключей
4. **04_CreateStoredProcedures.sql** — Создание хранимых процедур
5. **05_SeedData.sql** — Заполнение демонстрационными данными

#### Вариант Б: Проверка подключения

Запустите диагностический скрипт:

```cmd
check_database_connection.bat
```

Скрипт проверит:
- Наличие файла App.config
- Существующее подключение к SQL Server
- Наличие базы данных LeasingSystem

### 3. Сборка проекта

Откройте решение `ForVlad.sln` в Visual Studio и выполните:

1. **Build → Build Solution** (Ctrl+Shift+B)
2. Или используйте компилятор из командной строки:
   ```cmd
   "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" ForVlad.sln /t:Build /p:Configuration=Debug
   ```

### 4. Запуск приложения

```cmd
# Из Visual Studio
F5 (Start Debugging)

# Из командной строки (после сборки)
cd bin\Debug
ForVlad.exe
```

### 5. Первое запуск

При первом запуске приложение:
- Протестирует подключение к базе данных
- Загрузит демонстрационные данные (если база пустая)
- Отобразит главное окно с интерфейсом управления

---

## 📊 Конфигурация

### Файлы конфигурации

| Файл | Описание |
|------|----------|
| `App.config` | Основная конфигурация приложения |
| `bin\Debug\ForVlad.exe.config` | Скомпилированная конфигурация (генерируется автоматически) |
| `Properties\Settings.settings` | Настройки пользовательского интерфейса |

### Строка подключения к базе данных

Редактируйте файл `App.config`:

```xml
<connectionStrings>
    <add name="LeasingSystem"
         connectionString="Server=(local)\SQLEXPRESS;Database=LeasingSystem;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true;"
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

**Параметры подключения:**
- `Server` — имя сервера SQL Server (по умолчанию: `(local)\SQLEXPRESS`)
- `Database` — имя базы данных (по умолчанию: `LeasingSystem`)
- `Trusted_Connection=True` — Windows аутентификация
- `MultipleActiveResultSets=true` — поддержка MARS
- `TrustServerCertificate=true` — игнорирование ошибок SSL

### Переменные окружения (опционально)

| Переменная | Описание | Значение по умолчанию |
|------------|----------|----------------------|
| `SQL_SERVER_INSTANCE` | Имя экземпляра SQL Server | `(local)\SQLEXPRESS` |

---

## 🎯 Возможности

### 📑 Учёт контрагентов
- Создание, редактирование, удаление контрагентов
- Хранение реквизитов (ИНН, КПП, ОГРН, адреса)
- Классификация по типам (Юридическое лицо, ИП)
- Контактная информация

### 🚜 Учёт техники и оборудования
- Ведение каталога техники с инвентарными номерами
- Классификация по группам:
  - **Vehicle** — Транспортные средства
  - **Equipment** — Оборудование
- Подкатегории (строительная, дорожная и т.д.)
- Характеристики: марка, модель, год выпуска, VIN, мощность двигателя
- Отслеживание доступности (свободно/занято)

### 📄 Управление договорами
- Создание договоров аренды и лизинга
- Генерация уникальных номеров договоров
- Привязка к контрагентам и технике
- Указание сроков, сумм, условий оплаты
- Статусы договоров:
  - **Draft** — Черновик
  - **Signed** — Подписан
  - **Active** — Действующий
  - **Suspended** — Приостановлен
  - **Completed** — Завершён
  - **Terminated** — Расторгнут

### 💰 Графики платежей
- Автоматическое формирование графиков по договорам
- Отслеживание status платежей:
  - **Pending** — Ожидает оплаты
  - **Paid** — Оплачен
  - **Overdue** — Просрочен
- Дата оплаты, сумма, способ оплаты

### 📈 Отчётность

#### Финансовые отчёты
- Отчёт по платежам с фильтрацией по периодам
- Фильтрация по статусу (только неоплаченные)
- Группировка по контрагентам
- Статистика просроченных платежей

#### Отчёты по загрузке техники
- Коэффициент использования (в %)
- Доход от аренды по каждой единице техники
- Анализ по периодам
- Предупреждения о низкой/высокой загрузке

### 🔍 Дополнительные функции
- Поиск и фильтрация по всем сущностям
- Экспорт отчётов в CSV
- Тестирование подключения к базе данных
- Инициализация демонстрационных данных
- Настройки интерфейса (тема, размер страницы)

---

## 🏗️ Структура проекта

```
ForVlad/
├── App.xaml              # Точка входа WPF-приложения
├── App.xaml.cs           # Логика приложения
├── ForVlad.csproj        # Файл проекта
├── App.config            # Конфигурация подключения к БД
│
├── Database/             # SQL скрипты
│   ├── 01_CreateDatabase.sql
│   ├── 02_CreateTables.sql
│   ├── 03_CreateConstraints.sql
│   ├── 04_CreateStoredProcedures.sql
│   └── 05_SeedData.sql
│
├── Models/               # Модели данных
│   ├── Asset.cs          # Техника и оборудование
│   ├── Contract.cs       # Договоры
│   ├── Counterparty.cs   # Контрагенты
│   ├── ContractSpecification.cs
│   ├── PaymentSchedule.cs
│   └── Enums.cs          # Перечисления (статусы, типы)
│
├── Data/                 # Слой доступа к данным
│   ├── ISimpleDataService.cs    # Интерфейс сервиса данных
│   ├── SqlDataService.cs        # Реализация через SQL Server
│   ├── SimpleDataService.cs     # Реализация в памяти (не используется)
│   └── DataServiceProvider.cs  # Фабрика сервисов
│
├── Services/             # Бизнес-сервисы
│   ├── CsvExportService.cs     # Экспорт в CSV
│   └── ReportCalculationService.cs
│
├── ViewModels/           # MVVM - Модели представления
│   ├── ViewModelBase.cs
│   ├── RelayCommand.cs
│   ├── MainViewModel.cs
│   ├── ContractsViewModel.cs
│   ├── AssetsViewModel.cs
│   ├── CounterpartiesViewModel.cs
│   ├── ActiveContractsViewModel.cs
│   ├── FinancialReportsViewModel.cs
│   ├── UtilizationReportsViewModel.cs
│   └── SettingsViewModel.cs
│
├── Views/                # XAML вьюхи
│   ├── MainWindow.xaml
│   ├── MainWindowSimple.xaml
│   ├── ContractsView.xaml
│   ├── AssetsView.xaml
│   ├── CounterpartiesView.xaml
│   ├── ActiveContractsView.xaml
│   ├── FinancialReportsView.xaml
│   ├── UtilizationReportsView.xaml
│   └── SettingsView.xaml
│
├── Converters/           # WPF конвертеры
│   ├── StatusConverters.cs
│   ├── IntToVisibilityConverter.cs
│   └── DaysRemainingConverter.cs
│
├── Properties/           # Свойства проекта
│   ├── AssemblyInfo.cs
│   ├── Resources.resx
│   └── Settings.settings
│
└── *.bat                # Скрипты сборки и диагностики
```

---

## 🔧 Технологии

### Язык и платформа
- **C# 8.0** — Язык программирования
- **.NET Framework 4.7.2** — Платформа выполнения
- **WPF (Windows Presentation Foundation)** — UI-фреймворк

### Паттерны и архитектура
- **MVVM (Model-View-ViewModel)** — Паттерн проектирования
- **Dependency Injection** — Внедрение зависимостей
- **Repository Pattern** — Работа с данными через сервисы

### База данных
- **Microsoft SQL Server Express** — СУБД
- **ADO.NET** — Доступ к данным через SqlConnection
- **T-SQL** — Язык запросов

### Инструменты разработки
- **Visual Studio 2019+** — IDE
- **Git** — Контроль версий

---

## 📝 API и интеграции

### Внутренний API (ISimpleDataService)

Сервис данных предоставляет единый интерфейс для работы с сущностями:

```csharp
// Интерфейс сервиса данных
public interface ISimpleDataService
{
    // Контрагенты
    List<Counterparty> GetCounterparties();
    Counterparty GetCounterparty(int id);
    void SaveCounterparty(Counterparty counterparty);
    void DeleteCounterparty(int id);
    
    // Техника
    List<Asset> GetAssets();
    Asset GetAsset(int id);
    void SaveAsset(Asset asset);
    void DeleteAsset(int id);
    
    // Договоры
    List<Contract> GetContracts();
    Contract GetContract(int id);
    void SaveContract(Contract contract);
    void DeleteContract(int id);
    
    // Графики платежей
    List<PaymentSchedule> GetPaymentSchedules(int? contractId = null);
    void MarkPaymentPaid(int paymentId, DateTime? paymentDate = null);
    
    // Отчётность
    List<PaymentReportRow> GetPaymentReport(DateTime? dueFrom, DateTime? dueTo, bool unpaidOnly);
    List<AssetUtilizationRow> GetAssetUtilizationReport(DateTime periodStart, DateTime periodEnd, AssetGroup? assetGroup);
    
    // Утилиты
    void InitializeTestData();
    void ResetDemoData();
    bool TestConnection(out string errorMessage);
    string GenerateContractNumber(ContractType contractType);
}
```

### Пример использования

```csharp
// Создание сервиса
var dataService = DataServiceProvider.Create();

// Получение списка договоров
var contracts = dataService.GetContracts();

// Создание нового договора
var newContract = new Contract
{
    ContractNumber = dataService.GenerateContractNumber(ContractType.Rental),
    ContractType = ContractType.Rental,
    CounterpartyId = 1,
    StartDate = DateTime.Now,
    EndDate = DateTime.Now.AddMonths(6),
    TotalAmount = 500000,
    Status = ContractStatus.Draft
};

dataService.SaveContract(newContract);
```

---

## 🔄 Планы по развитию

### 🎯 В ближайших планах
- [ ] Добавление аутентификации пользователей
- [ ] Реализация ролевого доступа (администратор, менеджер)
- [ ] Экспорт отчётов в Excel (xlsx)
- [ ] Печать договоров и актов
- [ ] Интеграция с 1С

### 💡 Идеи на будущее
- [ ] Мобильное приложение для проверки техники на месте
- [ ] Онлайн-версия с ASP.NET Core
- [ ] Уведомления о просроченных платежах по email
- [ ] Дашборд с визуализацией данных
- [ ] API для интеграции с другими системами

### 🐛 Известные ограничения
- Поддерживается только SQL Server (нет поддержки PostgreSQL, MySQL)
- Нет миграций базы данных (используются SQL скрипты)
- Windows-only приложение

---

## 📄 Лицензия

Проект распространяется по лицензии **MIT License**.

```
MIT License

Copyright (c) 2026

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 🆘 Решение проблем

### ❌ Ошибка: "Имя 'Assembly' не существует в текущем контексте"

**Причина:** Отсутствует директива `using System.Reflection;`

**Решение:** Добавьте в начало файла:
```csharp
using System.Reflection;
```

### ❌ Ошибка: "Не удалось найти тип или имя пространства имен 'ISimpleDataService'"

**Причина:** Отсутствует директива `using ForVlad.Data;`

**Решение:** Добавьте в начало файла:
```csharp
using ForVlad.Data;
```

### ❌ Ошибка: "Конфликт инструкции INSERT с ограничением CHECK 'CK_Contracts_TotalAmount_Positive'"

**Причина:** Пытка сохранения договора с суммой ≤ 0

**Решение:** Убедитесь, что `TotalAmount > 0` перед сохранением.

### ❌ Ошибка подключения к SQL Server

**Решение:**
1. Проверьте, запущена ли служба SQL Server Express
2. Убедитесь, что имя сервера указано правильно в App.config
3. Выполните диагностический скрипт: `check_database_connection.bat`

### 🔍 Диагностика

Запустите скрипт проверки:
```cmd
check_database_connection.bat
```

или
```cmd
check_project.bat
```

---

## 📞 Контакты и поддержка

Для вопросов и предложений:
- Email: support@yourcompany.com
- GitHub Issues: [Создать issue](https://github.com/your-repo/ForVlad/issues)

---

**© 2026 ForVlad — Система учёта лизинга и аренды спецтехники**
