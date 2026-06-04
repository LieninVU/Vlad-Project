# Решение проблем проекта LeasingSystem

## Основные проблемы, которые были решены

### 1. Ошибка при создании техники (SQL CHECK Constraint Error)

**Проблема:**
```
System.Data.SqlClient.SqlException: "Конфликт инструкции INSERT с ограничением CHECK "CK_Assets_VehicleFields"."
```

**Причина:**
Ограничение `CK_Assets_VehicleFields` в таблице Assets проверяло только поле `VehicleBrand`, но не проверяло `VehicleModel`.
При создании техники типа Vehicle (AssetGroup=0) требовалось, чтобы оба поля были заполнены.

**Решение:**

#### A. Исправление ограничения в БД
Создан SQL скрипт: `Database\04_Fix_Assets_VehicleFields_Constraint.sql`

```sql
-- Удаляем старое ограничение
ALTER TABLE Assets DROP CONSTRAINT CK_Assets_VehicleFields;

-- Создаём новое ограничение с проверкой обоих полей
ALTER TABLE Assets ADD CONSTRAINT CK_Assets_VehicleFields CHECK (
    (AssetGroup = 0 AND VehicleBrand IS NOT NULL AND VehicleModel IS NOT NULL) OR 
    (AssetGroup = 1 AND VehicleBrand IS NULL AND VehicleModel IS NULL)
);
```

#### B. Валидация в модели данных (SqlDataService.cs)
Добавлена полная валидация всех обязательных полей перед сохранением:

- Для AssetGroup.Vehicle: Manufacturer и Model обязательны
- Для AssetGroup.Equipment: Manufacturer и Model должны быть пустыми
- Проверка инвентарного номера, наименования, подкатегории
- Проверка тарифов (хотя бы одна ставка > 0)
- Проверка года выпуска, мощности двигателя, веса

#### C. Валидация в UI (AssetsViewModel.cs)
В форме создания/редактирования техники добавлена валидация:
- Проверка всех обязательных полей
- Отображение понятных сообщений об ошибках
- Проверка в зависимости от типа техники

---

### 2. Валидация во всех формах

#### AssetsViewModel.cs
✅ Полная валидация при сохранении техники:
- Общие поля: InventoryNumber, Name, Subcategory
- Тарифы: HourlyRate, DailyRate, MonthlyRentalRate
- Для Vehicle: Manufacturer, Model обязательны
- Для Equipment: Manufacturer, Model должны быть пустыми
- Дополнительные поля: EnginePower, Weight, YearOfManufacture

#### ContractsViewModel.cs  
✅ Валидация при сохранении договора:
- Номер договора
- Контрагент
- Сумма договора > 0
- Даты: SignedDate <= StartDate <= EndDate
- Финансовые поля >= 0
- Спецификации: AssetId, Quantity > 0, UnitPrice > 0

#### CounterpartiesViewModel.cs
✅ Валидация при сохранении контрагента:
- ИНН (10-12 символов)
- КПП (9 символов или пустой)
- Email (содержит @ и . или пустой)

---

### 3. Система аутентификации

#### Реализовано:

1. **Модель пользователя** (`Services/AuthenticationService.cs`):
   - CurrentUser класс с полями: Id, UserName, DisplayName, Role, Email, IsAuthenticated
   - Поддержка ролей: Admin, Manager, Accountant, ReadOnly
   - Проверка разрешений: HasPermission(), CanEditContract(), CanDelete, CanAccessSettings

2. **Хэширование паролей** (`Services/PasswordHasher.cs`):
   - Использует PBKDF2 с 100,000 итерациями
   - Генерация случайной соли
   - Безопасное хэширование и проверка паролей

3. **Сервис аутентификации** (`AuthenticationService.cs`):
   - Login(username, password): вход по логину/паролю
   - Register(): регистрация нового пользователя
   - ChangePassword(): смена пароля
   - ChangeUsername(): смена логина
   - GetUserById(), GetAllUsers()
   - Защита от брутфорса (блокировка после 5 неудачных попыток)

4. **Таблица пользователей** (`Database/07_CreateUsersTable.sql`):
   - Пользователи по умолчанию: admin, manager, accountant, readonly
   - Хранение хэша и соли пароля
   - Поля: UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked

5. **Интерфейс входа** (`Views/LoginWindow.xaml`, `ViewModels/LoginViewModel.cs`):
   - Окно входа с полями логин/пароль
   - Валидация полей
   - Отображение ошибок входа

6. **Интерфейс профиля** (`Views/UserProfileView.xaml`, `ViewModels/UserProfileViewModel.cs`):
   - Просмотр информации о пользователе
   - Смена пароля
   - Смена логина
   - Обновление информации профиля (DisplayName, Email)

7. **Интеграция с приложением** (`App.xaml.cs`):
   - При запуске показывается окно входа
   - После успешного входа открывается главное окно
   - Очистка текущего пользователя при выходе

---

### 4. Исправление ошибок компиляции

**Проблема:** Файлы с русским текстом в кодировке Windows-1251 вызывали ошибки компиляции.

**Решение:**
Создан PowerShell скрипт `fix_encoding.ps1` для конвертации файлов в UTF-8.

Запустите его перед компиляцией:
```powershell
powershell -ExecutionPolicy Bypass -File fix_encoding.ps1
```

Файлы для конвертации:
- Data\SqlDataService.cs
- Data\DatabaseConnection.cs
- Services\AssetAvailabilityService.cs
- ViewModels\CounterpartiesViewModel.cs

---

### 5. Генерация графика платежей

Реализовано в `Services/PaymentScheduleGenerator.cs`:
- Генерация ежемесячного графика
- Генерация одноразового платежа
- Генерация кастомного графика
- Автоматическая генерация при сохранении договора

---

### 6. Проверка доступности актива

Реализовано в `Services/AssetAvailabilityService.cs`:
- IsAvailable(assetId, startDate, endDate, excludeContractId): проверяет доступность
- GetBusyPeriods(assetId): возвращает периоды занятости
- Используется при создании спецификаций договора

---

## Как использовать обновления

### 1. Обновите базу данных

Выполните следующие SQL скрипты:
```bash
sqlcmd -S (local)\SQLEXPRESS -d LeasingSystem -i Database\04_Fix_Assets_VehicleFields_Constraint.sql
```

Или в SQL Server Management Studio:
1. Откройте `Database\04_Fix_Assets_VehicleFields_Constraint.sql`
2. Выполните скрипт

### 2. Исправьте кодировку файлов

Запустите PowerShell скрипт:
```powershell
cd C:\Users\STAR BUTTERFLY\source\repos\ForVlad\ForVlad
powershell -ExecutionPolicy Bypass -File fix_encoding.ps1
```

### 3. Создайте пользователей по умолчанию

Выполните:
```bash
sqlcmd -S (local)\SQLEXPRESS -d LeasingSystem -i Database\07_CreateUsersTable.sql
```

Пользователи:
- **admin** / **admin** (Администратор)
- **manager** / **manager** (Менеджер)
- **accountant** / **accountant** (Бухгалтер)
- **readonly** / **readonly** (Просмотр)

### 4. Скомпилируйте проект

В Visual Studio:
1. Откройте `ForVlad.csproj`
2. Убедитесь, что выбран .NET Framework 4.7.2
3. Скомпилируйте проект (Ctrl+Shift+B)

### 5. Запустите приложение

1. Убедитесь, что SQL Server запущен
2. Запустите приложение (F5)
3. Введите логин и пароль (например, admin/admin)
4. Начните работу с системой

---

## Функции, доступные в интерфейсе

### Аутентификация
- ✅ Вход по логину/паролю
- ✅ Смена пароля в профиле
- ✅ Смена логина в профиле
- ✅ Блокировка аккаунта после 5 неудачных попыток

### Техника (Assets)
- ✅ Создание новой техники с полной валидацией
- ✅ Редактирование техники
- ✅ Просмотр списка техники
- ✅ Фильтрация по категории и подкатегории
- ✅ Поиск по названию, инвентарному номеру
- ✅ Переключение доступности

### Договора (Contracts)
- ✅ Создание договора с валидацией
- ✅ Редактирование договора
- ✅ Добавление спецификаций (техника в договоре)
- ✅ Автоматическая генерация графика платежей
- ✅ Проверка доступности техники на период

### Контрагенты (Counterparties)
- ✅ Создание и редактирование контрагентов
- ✅ Валидация ИНН, КПП, Email

---

## Структура проекта

```
ForVlad/
├── App.xaml, App.xaml.cs          # Точка входа, настройка приложения
├── Models/                       # Модели данных
│   ├── Asset.cs                  # Техника
│   ├── Contract.cs               # Договор
│   ├── Counterparty.cs           # Контрагент
│   └── Enums.cs                  # Перечисления
│
├── ViewModels/                   # ViewModels (MVVM)
│   ├── AssetsViewModel.cs        # Управление техникой
│   ├── ContractsViewModel.cs     # Управление договорами
│   ├── CounterpartiesViewModel.cs # Управление контрагентами
│   ├── LoginViewModel.cs         # Вход в систему
│   ├── UserProfileViewModel.cs   # Профиль пользователя
│   └── ViewModelBase.cs          # Базовый класс
│
├── Views/                        # XAML файлы интерфейса
│   ├── LoginWindow.xaml           # Окно входа
│   ├── UserProfileView.xaml      # Профиль пользователя
│   ├── AssetsView.xaml           # Просмотр техники
│   ├── ContractsView.xaml        # Просмотр договоров
│   └── ...
│
├── Services/                     # Сервисы
│   ├── AuthenticationService.cs   # Аутентификация
│   ├── PasswordHasher.cs         # Хэширование паролей
│   ├── AssetAvailabilityService.cs # Проверка доступности
│   └── PaymentScheduleGenerator.cs # Генерация графика
│
├── Data/                         # Работа с данными
│   ├── SqlDataService.cs         # SQL сервис данных
│   └── DatabaseConnection.cs     # Подключение к БД
│
└── Database/                     # SQL скрипты
    ├── 01_CreateDatabase.sql      # Создание БД
    ├── 02_CreateTables.sql        # Создание таблиц
    ├── 03_CreateConstraints.sql   # Ограничения
    ├── 04_Fix_Assets_VehicleFields_Constraint.sql # Исправление ограничения
    └── 07_CreateUsersTable.sql    # Таблица пользователей
```

---

## Дополнительные улучшения

### Валидация в UI

Для улучшения UX, рекомендуется добавить:

1. **Подсветку обязательных полей** (красная звёздочка)
2. **Подсказки** (Tooltip) для каждого поля
3. **Мгновенную валидацию** при изменении значений

Пример для TextBox:
```xaml
<TextBox Text="{Binding Manufacturer, UpdateSourceTrigger=PropertyChanged}" 
         ToolTip="Обязательное поле для транспортных средств">
    <TextBox.Style>
        <Style TargetType="TextBox">
            <Style.Triggers>
                <DataTrigger Binding="{Binding HasManufacturerError}" Value="True">
                    <Setter Property="BorderBrush" Value="Red"/>
                    <Setter Property="ToolTip" Value="Укажите производителя"/>
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </TextBox.Style>
</TextBox>
```

### Логирование

Для отладки рекомендуется добавить логирование:
```csharp
// В App.xaml.cs
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        LogManager.Configure(); // Настройка логирования
        
        try {
            // Код запуска
        } catch (Exception ex) {
            Logger.Error("Ошибка при запуске", ex);
            MessageBox.Show("Критическая ошибка: " + ex.Message);
        }
    }
}
```

---

## Заключение

Все основные проблемы были решены:

✅ **Исправлена ошибка CHECK Constraint** при создании техники
✅ **Добавлена полная валидация** во всех формах
✅ **Реализована аутентификация** с логином/паролем
✅ **Добавлены пользователи по умолчанию**
✅ **Исправлены ошибки компиляции** (необходима конвертация кодировки)
✅ **Добавлена генерация графика платежей**
✅ **Добавлена проверка доступности актива**

Для полного функционирования:
1. Выполните SQL скрипты для обновления БД
2. Конвертируйте файлы в UTF-8
3. Скомпилируйте и запустите проект
4. Войдите в систему под одним из пользователей по умолчанию

---

*Документация обновлена: 01.06.2026*
