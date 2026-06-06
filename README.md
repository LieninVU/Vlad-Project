# LeasingSystem — Система учёта договоров аренды и лизинга спецтехники

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blueviolet)
![WPF](https://img.shields.io/badge/WPF-MVVM-brightgreen)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Express-informational)
![License](https://img.shields.io/badge/License-MIT-red)
![Platform](https://img.shields.io/badge/Platform-Windows%2010/11-blue)

**LeasingSystem** — десктопное WPF-приложение для управления договорами аренды и лизинга специальной техники. Система позволяет вести учёт контрагентов, техники, договоров, графиков платежей и формировать отчётность.

---

## 📋 Оглавление

- [Для кого этот проект](#-для-кого-этот-проект)
- [Требования](#-требования)
- [Быстрый старт (для нетерпеливых)](#-быстрый-старт-для-нетерпеливых)
- [Подробная инструкция по установке](#-подробная-инструкция-по-установке)
- [Установка SQL Server](#-установка-sql-server)
- [Настройка базы данных](#-настройка-базы-данных)
- [Сборка и запуск](#-сборка-и-запуск)
- [Конфигурация](#-конфигурация)
- [Возможности](#-возможности)
- [Структура проекта](#-структура-проекта)
- [Технологии](#-технологии)
- [Скриншоты](#-скриншоты)
- [FAQ — Часто задаваемые вопросы](#-faq--часто-задаваемые-вопросы)
- [Решение проблем](#-решение-проблем)
- [Планы по развитию](#-планы-по-развитию)
- [Лицензия](#-лицензия)
- [Контакты и поддержка](#-контакты-и-поддержка)

---

## 🎯 Для кого этот проект

**LeasingSystem** предназначена для:
- Компаний, сдающих спецтехнику в аренду
- Лизинговых компаний
- Строительных организаций с собственным парком техники
- Малого и среднего бизнеса в сфере аренды оборудования

---

## 📌 Требования

### Минимальные системные требования

| Компонент | Версия | Примечание |
|-----------|--------|------------|
| **Операционная система** | Windows 10 (64-bit) или Windows 11 | Windows 7 не поддерживается |
| **.NET Framework** | 4.7.2 или выше | Обычно предустановлен в Windows 10/11 |
| **SQL Server** | Express 2019+ или LocalDB | Бесплатная версия |
| **ОЗУ** | 4 ГБ минимум | 8 ГБ рекомендуется |
| **Место на диске** | 500 МБ | Для приложения + база данных |

### Необходимое программное обеспечение

Для **запуска готового приложения**:
1. **.NET Framework 4.7.2** — обычно уже установлен в Windows 10/11
2. **Microsoft SQL Server Express** — бесплатная СУБД от Microsoft

Для **разработки и сборки из исходного кода**:
1. **Visual Studio 2019+** (Community Edition — бесплатно)
2. **Рабочая нагрузка**: Разработка классических приложений .NET

---

## ⚡ Быстрый старт (для нетерпеливых)

Если у вас уже установлен SQL Server Express:

```cmd
# 1. Скачайте проект (архив или git clone)
# 2. Откройте SSMS (SQL Server Management Studio)
# 3. Выполните скрипты из папки Database/ по порядку:
#    01_CreateDatabase.sql
#    02_CreateTables.sql
#    03_CreateConstraints.sql
#    04_CreateStoredProcedures.sql
#    05_SeedData.sql

# 4. Откройте ForVlad.sln в Visual Studio
# 5. Нажмите F5 для запуска
```

Если SQL Server не установлен — читайте подробную инструкцию ниже.

---

## 📦 Подробная инструкция по установке

### Шаг 1: Получение проекта

#### Вариант А: Скачивание архива (для обычных пользователей)

1. Скачайте ZIP-архив с проектом
2. Распакуйте архив в удобное место, например:
   ```
   C:\Projects\LeasingSystem\
   ```
3. Убедитесь, что путь **не содержит кириллицы** и пробелов

#### Вариант Б: Клонирование через Git (для разработчиков)

```cmd
# Установите Git, если не установлен: https://git-scm.com/download/win

# Откройте командную строку (Win+R → cmd)
cd C:\Projects
git clone https://github.com/your-repo/ForVlad.git LeasingSystem
cd LeasingSystem
```

### Шаг 2: Проверка .NET Framework

**Windows 10/11** обычно уже имеет .NET Framework 4.7.2.

**Проверка:**
1. Откройте **Панель управления** → **Программы и компоненты**
2. Нажмите **Включение или отключение компонентов Windows**
3. Найдите **.NET Framework 4.8** (включает 4.7.2)

**Если отсутствует:**
- Скачайте с [официального сайта Microsoft](https://dotnet.microsoft.com/download/dotnet-framework/net472)
- Или установите через Windows Update

### Шаг 3: Установка SQL Server

> ⚠️ **Важно:** Без SQL Server приложение работать не будет!

См. раздел [Установка SQL Server](#-установка-sql-server) ниже.

### Шаг 4: Настройка базы данных

См. раздел [Настройка базы данных](#-настройка-базы-данных) ниже.

### Шаг 5: Сборка и запуск

См. раздел [Сборка и запуск](#-сборка-и-запуск) ниже.

---

## 🗄️ Установка SQL Server

### Вариант А: SQL Server Express (рекомендуется)

**SQL Server Express** — бесплатная версия от Microsoft.

1. **Скачайте установщик:**
   - Перейдите на https://www.microsoft.com/ru-ru/sql-server/sql-server-downloads
   - Выберите **Express** (бесплатно)
   - Скачайте установщик

2. **Запустите установку:**
   - Выберите **Базовая** установка
   - Примите лицензионное соглашение
   - Выберите папку установки (по умолчанию подойдёт)
   - Нажмите **Установить**

3. **Запомните имя экземпляра:**
   - По умолчанию: `SQLEXPRESS`
   - Полное имя для подключения: `(local)\SQLEXPRESS`

4. **После установки** появится окно с информацией о подключении:
   ```
   Имя сервера: LAPTOP-XXXXX\SQLEXPRESS
   ```
   Скопируйте и сохраните это имя!

### Вариант Б: SQL Server LocalDB (альтернатива)

**LocalDB** — упрощённая версия для разработчиков.

1. Скачайте **SQL Server Express** (см. выше)
2. При установке выберите **LocalDB**
3. Имя подключения: `(localdb)\MSSQLLocalDB`

### Вариант В: Проверка установленного SQL Server

```cmd
# Откройте командную строку и выполните:
sqlcmd -S "(local)\SQLEXPRESS" -Q "SELECT @@VERSION"
```

Если видите версию SQL Server — всё настроено правильно!

### Установка SQL Server Management Studio (SSMS)

**SSMS** — графическая утилита для работы с базой данных.

1. Скачайте с https://learn.microsoft.com/ru-ru/sql/ssms/download-sql-server-management-studio-ssms
2. Установите (требуется перезапуск)
3. Запустите SSMS
4. Подключитесь к серверу:
   - Имя сервера: `(local)\SQLEXPRESS`
   - Аутентификация: Windows

---

## 🛠️ Настройка базы данных

### Вариант А: Автоматическая настройка через SSMS (рекомендуется)

1. **Запустите SSMS**
2. **Подключитесь к серверу:**
   - Имя сервера: `(local)\SQLEXPRESS`
   - Аутентификация: Windows
   - Нажмите **Подключиться**

3. **Выполните скрипты по порядку:**
   
   Откройте каждый файл и нажмите **F5** (Выполнить):
   
   | Порядок | Файл | Описание |
   |---------|------|----------|
   | 1 | `Database/01_CreateDatabase.sql` | Создание базы данных |
   | 2 | `Database/02_CreateTables.sql` | Создание таблиц |
   | 3 | `Database/03_CreateConstraints.sql` | Ограничения и ключи |
   | 4 | `Database/04_CreateStoredProcedures.sql` | Хранимые процедуры |
   | 5 | `Database/05_SeedData.sql` | Тестовые данные |

4. **Проверьте результат:**
   
   В SSMS выполните:
   ```sql
   USE LeasingSystem;
   SELECT COUNT(*) FROM Counterparties;
   SELECT COUNT(*) FROM Assets;
   SELECT COUNT(*) FROM Contracts;
   ```
   
   Должно вернуть:
   - Counterparties: 3
   - Assets: 3
   - Contracts: 3

### Вариант Б: Настройка через командную строку

```cmd
cd Database

sqlcmd -S "(local)\SQLEXPRESS" -i 01_CreateDatabase.sql
sqlcmd -S "(local)\SQLEXPRESS" -i 02_CreateTables.sql
sqlcmd -S "(local)\SQLEXPRESS" -i 03_CreateConstraints.sql
sqlcmd -S "(local)\SQLEXPRESS" -i 04_CreateStoredProcedures.sql
sqlcmd -S "(local)\SQLEXPRESS" -i 05_SeedData.sql

echo База данных настроена!
```

### Вариант В: Использование диагностического скрипта

```cmd
check_database_connection.bat
```

Скрипт проверит:
- Наличие файла App.config
- Подключение к SQL Server
- Наличие базы данных LeasingSystem

---

## 🚀 Сборка и запуск

### Вариант А: Из Visual Studio (для разработчиков)

1. **Откройте проект:**
   - Дважды кликните на `ForVlad.sln`
   - Или откройте Visual Studio → Файл → Открыть → Проект/решение

2. **Восстановите пакеты (если требуется):**
   ```
   Правый клик на решении → Восстановить пакеты NuGet
   ```

3. **Соберите проект:**
   - Меню: **Сборка** → **Собрать решение**
   - Или нажмите **Ctrl+Shift+B**

4. **Запустите:**
   - Нажмите **F5** (с отладкой)
   - Или **Ctrl+F5** (без отладки)

### Вариант Б: Из командной строки

```cmd
# Перейдите в папку проекта
cd C:\Projects\LeasingSystem

# Сборка через MSBuild
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" ^
    ForVlad.sln /t:Build /p:Configuration=Debug

# Запуск
bin\Debug\ForVlad.exe
```

### Вариант В: Готовый исполняемый файл

Если вы скачали готовый `ForVlad.exe`:

1. Убедитесь, что .NET Framework 4.7.2 установлен
2. Убедитесь, что SQL Server работает
3. Дважды кликните на `ForVlad.exe`

---

## 📊 Конфигурация

### Файл App.config

Основной файл конфигурации — `App.config`.

#### Строка подключения к базе данных

```xml
<connectionStrings>
    <add name="LeasingSystem"
         connectionString="Server=(local)\SQLEXPRESS;Database=LeasingSystem;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true;"
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

**Параметры:**

| Параметр | Описание | Значение по умолчанию |
|----------|----------|----------------------|
| `Server` | Имя сервера SQL Server | `(local)\SQLEXPRESS` |
| `Database` | Имя базы данных | `LeasingSystem` |
| `Trusted_Connection` | Windows-аутентификация | `True` |
| `MultipleActiveResultSets` | Режим MARS | `true` |
| `TrustServerCertificate` | Доверять сертификату | `true` |

#### Изменение имени сервера

Если ваш SQL Server имеет другое имя:

1. Откройте `App.config` в текстовом редакторе
2. Найдите строку `Server=(local)\SQLEXPRESS`
3. Замените на ваше имя сервера:
   ```xml
   Server=YOUR_SERVER_NAME\SQLEXPRESS
   ```
   или для LocalDB:
   ```xml
   Server=(localdb)\MSSQLLocalDB
   ```

4. Пересоберите проект (если используете Visual Studio)

### Таблицы конфигурации

| Файл | Описание |
|------|----------|
| `App.config` | Основная конфигурация |
| `bin\Debug\ForVlad.exe.config` | Скомпилированная конфигурация |
| `Properties\Settings.settings` | Настройки интерфейса |

---

## 🎯 Возможности

### 📑 Учёт контрагентов
- Создание, редактирование, удаление контрагентов
- Хранение реквизитов (ИНН, КПП, ОГРН, адреса)
- Классификация по типам (Юридическое лицо, ИП, Физическое лицо)
- Контактная информация
- История договоров

### 🚜 Учёт техники и оборудования
- Ведение каталога техники с инвентарными номерами
- Классификация по группам:
  - **Vehicle** — Транспортные средства (экскаваторы, бульдозеры)
  - **Equipment** — Оборудование (генераторы, компрессоры)
- Подкатегории (строительная, дорожная техника)
- Характеристики: марка, модель, год выпуска, VIN, мощность
- Отслеживание доступности (свободно/занято)

### 📄 Управление договорами
- Создание договоров аренды и лизинга
- Автоматическая генерация номеров договоров
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
- Отслеживание статусов платежей:
  - **Pending** — Ожидает оплаты
  - **Paid** — Оплачен
  - **Overdue** — Просрочен
- Регистрация дат и способов оплаты

### 📈 Отчётность

#### Финансовые отчёты
- Отчёт по платежам с фильтрацией по периодам
- Фильтрация по статусу (только неоплаченные)
- Статистика просроченных платежей
- Экспорт в CSV

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
- Настройки интерфейса

---

## 🏗️ Структура проекта

```
LeasingSystem/
├── App.xaml                    # Точка входа WPF
├── App.xaml.cs                 # Логика приложения
├── App.config                  # Конфигурация (строка подключения)
├── ForVlad.sln                 # Файл решения Visual Studio
├── ForVlad.csproj              # Файл проекта
│
├── Database/                   # SQL скрипты
│   ├── 01_CreateDatabase.sql   # Создание БД
│   ├── 02_CreateTables.sql     # Создание таблиц
│   ├── 03_CreateConstraints.sql
│   ├── 04_CreateStoredProcedures.sql
│   └── 05_SeedData.sql         # Тестовые данные
│
├── Models/                     # Модели данных
│   ├── Asset.cs                # Техника
│   ├── Contract.cs             # Договоры
│   ├── Counterparty.cs         # Контрагенты
│   ├── PaymentSchedule.cs      # Платежи
│   └── Enums.cs                # Перечисления
│
├── Data/                       # Слой данных
│   ├── ISimpleDataService.cs   # Интерфейс
│   ├── SqlDataService.cs       # Реализация SQL
│   └── DataServiceProvider.cs  # Фабрика
│
├── Services/                   # Бизнес-логика
│   ├── CsvExportService.cs     # Экспорт CSV
│   └── ReportCalculationService.cs
│
├── ViewModels/                 # MVVM ViewModels
│   ├── MainViewModel.cs
│   ├── ContractsViewModel.cs
│   ├── AssetsViewModel.cs
│   └── ...
│
├── Views/                      # XAML представления
│   ├── MainWindow.xaml
│   ├── ContractsView.xaml
│   ├── AssetsView.xaml
│   └── ...
│
├── Converters/                 # WPF конвертеры
│   ├── StatusConverters.cs
│   └── IntToVisibilityConverter.cs
│
└── *.bat                       # Скрипты диагностики
```

---

## 🔧 Технологии

### Язык и платформа
- **C# 8.0** — Язык программирования
- **.NET Framework 4.7.2** — Платформа выполнения
- **WPF** — UI-фреймворк (Windows Presentation Foundation)

### Архитектура
- **MVVM** — Model-View-ViewModel паттерн
- **Repository Pattern** — Работа с данными через сервисы

### База данных
- **Microsoft SQL Server Express** — СУБД (бесплатно)
- **ADO.NET** — Доступ к данным
- **T-SQL** — Язык запросов

### Инструменты
- **Visual Studio 2019+** — IDE (Community Edition бесплатно)
- **SSMS** — SQL Server Management Studio

---

## 📸 Скриншоты

### Главное окно
```
┌─────────────────────────────────────────────────────────────────┐
│  LeasingSystem v1.0                              [_][□][×]      │
├─────────────────────────────────────────────────────────────────┤
│ [Договоры] [Техника] [Контрагенты] [Отчёты] [Настройки]         │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  [+ Новый договор] [✏️ Редактировать] [🗑️ Удалить] [🔄 Обновить]│
│                                                                 │
│  Поиск: [________________]  Статус: [Все ▼]                     │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │ Номер    │ Контрагент    │ Тип  │ Статус  │ Период    │Сумма││
│  ├─────────────────────────────────────────────────────────────┤│
│  │ AR-2024-001 │ ООО "СтройТех" │ Аренда │ Active │ 01.06-01.09 │...│
│  │ LS-2024-002 │ ИП Петров      │ Лизинг │ Signed │ 01.07-01.07 │...│
│  └─────────────────────────────────────────────────────────────┘│
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## ❓ FAQ — Часто задаваемые вопросы

### Общие вопросы

#### **Вопрос: Приложение не запускается, появляется ошибка**

**Ответ:** Проверьте по порядку:
1. Установлен ли .NET Framework 4.7.2?
2. Запущен ли SQL Server?
3. Создана ли база данных LeasingSystem?
4. Правильно ли указана строка подключения в App.config?

#### **Вопрос: Где взять SQL Server Express?**

**Ответ:** 
- Скачайте бесплатно с официального сайта: https://www.microsoft.com/ru-ru/sql-server/sql-server-downloads
- Выберите версию **Express** (бесплатно)
- При установке выберите **Базовая** установка

#### **Вопрос: Как изменить имя сервера SQL Server?**

**Ответ:**
1. Откройте файл `App.config`
2. Найдите строку `connectionString`
3. Измените `Server=(local)\SQLEXPRESS` на ваш сервер
4. Пересоберите проект

#### **Вопрос: Как узнать имя моего SQL Server?**

**Ответ:**
```cmd
# В командной строке:
sqlcmd -L

# Или в SSMS при подключении нажмите "Browse for more"
```

#### **Вопрос: Можно ли использовать MySQL/PostgreSQL?**

**Ответ:** Нет, текущая версия поддерживает только Microsoft SQL Server.

### Вопросы по установке

#### **Вопрос: Скрипты базы данных выдают ошибку**

**Ответ:**
1. Убедитесь, что SQL Server установлен и запущен
2. Запустите SSMS от имени администратора
3. Выполняйте скрипты строго по порядку (01, 02, 03...)
4. Проверьте, что у вас есть права sysadmin

#### **Вопрос: Ошибка "Cannot attach database"**

**Ответ:**
База данных уже существует. Удалите её и запустите скрипты заново:
```sql
USE master;
ALTER DATABASE LeasingSystem SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE LeasingSystem;
```

#### **Вопрос: Ошибка подключения к (local)\SQLEXPRESS**

**Ответ:**
1. Проверьте, что служба SQL Server запущена:
   ```cmd
   net start MSSQL$SQLEXPRESS
   ```
2. Проверьте брандмауэр Windows
3. Попробуйте имя `.\SQLEXPRESS` или `localhost\SQLEXPRESS`

### Вопросы по использованию

#### **Вопрос: Как добавить новую технику?**

**Ответ:**
1. Перейдите на вкладку **Техника**
2. Нажмите **+ Добавить технику**
3. Заполните поля формы
4. Нажмите **Сохранить**

#### **Вопрос: Как создать договор аренды?**

**Ответ:**
1. Сначала добавьте контрагента (вкладка **Контрагенты**)
2. Добавьте технику (вкладка **Техника**)
3. Перейдите на вкладку **Договоры**
4. Нажмите **+ Новый договор**
5. Заполните форму и сохраните

#### **Вопрос: Как отметить платёж как оплаченный?**

**Ответ:**
1. Перейдите в **Отчёты → Финансовые отчёты**
2. Найдите нужный платёж в таблице
3. Выделите строку и нажмите **✓ Платёж**

#### **Вопрос: Как сбросить демо-данные?**

**Ответ:**
1. Перейдите на вкладку **Настройки**
2. Нажмите **🔄 Сбросить демо-данные**
3. Подтвердите действие

---

## 🆘 Решение проблем

### Ошибка: "Имя 'Assembly' не существует в текущем контексте"

**Причина:** Отсутствует директива `using System.Reflection;`

**Решение:** Добавьте в начало файла:
```csharp
using System.Reflection;
```

### Ошибка: "Не удалось найти тип или имя пространства имен 'ISimpleDataService'"

**Причина:** Отсутствует директива `using ForVlad.Data;`

**Решение:** Добавьте в начало файла:
```csharp
using ForVlad.Data;
```

### Ошибка: "Конфликт инструкции INSERT с ограничением CHECK"

**Причина:** Попытка сохранения некорректных данных (например, сумма ≤ 0)

**Решение:** Проверьте вводимые данные перед сохранением.

### Ошибка: "A network-related or instance-specific error occurred"

**Причина:** SQL Server не запущен или недоступен

**Решение:**
```cmd
# Проверьте статус службы:
sc query MSSQL$SQLEXPRESS

# Запустите службу:
net start MSSQL$SQLEXPRESS
```

### Ошибка: "Cannot open database 'LeasingSystem' requested by the login"

**Причина:** База данных не создана

**Решение:** Выполните скрипт `01_CreateDatabase.sql`

### Диагностика

Для автоматической диагностики запустите:
```cmd
check_database_connection.bat
```

или

```cmd
check_project.bat
```

---

## 🔄 Планы по развитию

### 🎯 В ближайших планах
- [ ] Аутентификация пользователей
- [ ] Ролевой доступ (администратор, менеджер)
- [ ] Экспорт в Excel (xlsx)
- [ ] Печать договоров и актов
- [ ] Интеграция с 1С

### 💡 Идеи на будущее
- [ ] Мобильное приложение
- [ ] Онлайн-версия (ASP.NET Core)
- [ ] Email-уведомления о просроченных платежах
- [ ] Дашборд с визуализацией
- [ ] REST API для интеграций

### 🐛 Известные ограничения
- Только Windows (не работает на macOS/Linux)
- Только SQL Server (нет поддержки PostgreSQL, MySQL)
- Нет миграций БД (используются SQL-скрипты)

---

## 📄 Лицензия

Проект распространяется по лицензии **MIT License**.

```
MIT License

Copyright (c) 2024-2025

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

## 📞 Контакты и поддержка

### Получение помощи

- **Email:** support@yourcompany.com
- **GitHub Issues:** [Создать issue](https://github.com/your-repo/ForVlad/issues)
- **Telegram:** @leasing_system_support

### Сообщение об ошибке

При сообщении об ошибке укажите:
1. Версию Windows
2. Версию .NET Framework
3. Версию SQL Server
4. Текст ошибки (скриншот)
5. Шаги для воспроизведения

---

## 📚 Дополнительные ресурсы

- [Документация .NET Framework](https://docs.microsoft.com/ru-ru/dotnet/)
- [Документация SQL Server](https://docs.microsoft.com/ru-ru/sql/)
- [WPF Tutorial](https://wpf-tutorial.com/)
- [MVVM Pattern](https://learn.microsoft.com/ru-ru/dotnet/architecture/maui/mvvm)

---

**© 2024-2025 LeasingSystem — Система учёта лизинга и аренды спецтехники**
