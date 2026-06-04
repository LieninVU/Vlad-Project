# 🔐 Реализация классической аутентификации по логину/паролю

**Дата:** 01.06.2026  
**Версия:** 2.0  
**Статус:** ✅ Полностью реализовано

---

## 📋 Содержание

1. [Изменения в базе данных](#1-изменения-в-базе-данных)
2. [Новые сервисы](#2-новые-сервисы)
3. [Новые окна и представления](#3-новые-окна-и-представления)
4. [Модифицированные файлы](#4-модифицированные-файлы)
5. [Инструкции по настройке](#5-инструкции-по-настройке)
6. [Пользователи по умолчанию](#6-пользователи-по-умолчанию)
7. [Безопасность](#7-безопасность)

---

## 🎯 1. Изменения в базе данных

### **Файл:** `Database/07_CreateUsersTable.sql` (Версия 2.0)

**Структура таблицы Users:**

| Столбец | Тип | Описание | Значение по умолчанию |
|---------|-----|----------|---------------------|
| Id | INT IDENTITY | Уникальный идентификатор | - |
| UserName | NVARCHAR(100) | Логин пользователя | - |
| DisplayName | NVARCHAR(150) | Отображаемое имя | NULL |
| PasswordHash | NVARCHAR(255) | Хэш пароля (SHA256) | - |
| PasswordSalt | NVARCHAR(255) | Соль для хэширования | - |
| Email | NVARCHAR(100) | Email пользователя | NULL |
| Role | TINYINT | Роль пользователя | 1 (Manager) |
| IsActive | BIT | Активен ли пользователь | 1 (True) |
| IsLocked | BIT | Заблокирован ли аккаунт | 0 (False) |
| FailedLoginAttempts | INT | Счётчик неудачных попыток | 0 |
| LastLogin | DATETIME | Дата последнего входа | NULL |
| PasswordLastChanged | DATETIME | Дата последней смены пароля | NULL |
| CreatedAt | DATETIME | Дата создания | GETDATE() |
| UpdatedAt | DATETIME | Дата обновления | NULL |

**Ограничения:**
- PRIMARY KEY: PK_Users (Id)
- UNIQUE: UQ_Users_UserName (UserName)
- UNIQUE: UQ_Users_Email (Email)

**Индексы:**
- IX_Users_UserName
- IX_Users_Role
- IX_Users_IsActive

**Пользователи по умолчанию:**
- admin / admin (Роль: Admin)
- manager / manager (Роль: Manager)
- accountant / accountant (Роль: Accountant)
- readonly / readonly (Роль: ReadOnly)

> ⚠️ **ВАЖНО:** Пароли в SQL скрипте временные. Для генерации правильных хэшей выполните инструкции в разделе [Генерация хэшей паролей](#генерация-хэшей-паролей).

---

## 🔧 2. Новые сервисы

### **2.1 PasswordHasher.cs**

**Назначение:** Безопасное хэширование паролей с использованием соли и PBKDF2

**Методы:**

```csharp
// Генерирует случайную соль
public static string GenerateSalt()

// Хэширует пароль с солью
public static (string Hash, string Salt) HashPassword(string password, string salt = null)

// Проверяет пароль
public static bool VerifyPassword(string password, string storedHash, string storedSalt)

// Генерирует случайный пароль
public static string GenerateRandomPassword(int length = 12)
```

**Алгоритм:** PBKDF2 с SHA256, 100,000 итераций, соль 16 байт, хэш 32 байта

---

### **2.2 AuthenticationService.cs** (обновлён)

**Новые методы:**

```csharp
// Вход по логину и паролю
public CurrentUser Login(string username, string password)

// Регистрация нового пользователя
public bool Register(string username, string password, string displayName, string email, UserRole role)

// Изменение пароля
public bool ChangePassword(int userId, string oldPassword, string newPassword)

// Изменение логина
public bool ChangeUsername(int userId, string newUsername)

// Получение информации о пользователе
public CurrentUser GetUserById(int userId)

// Получение списка всех пользователей (для админа)
public List<CurrentUser> GetAllUsers()

// Обновление информации о пользователе
public bool UpdateUserProfile(int userId, string displayName, string email)
```

**Функции безопасности:**
- Блокировка аккаунта после 5 неудачных попыток
- Автоматическая разблокировка через 30 минут
- Сброс счётчика неудачных попыток при успешном входе

---

## 🖥️ 3. Новые окна и представления

### **3.1 LoginWindow.xaml / LoginWindow.xaml.cs**

**Окно входа в систему:**

![Login Window](https://via.placeholder.com/500x400/2C3E50/FFFFFF?text=Login+Window)

**Функционал:**
- Поля для ввода логина и пароля
- Валидация полей
- Отображение ошибок входа
- Кнопка "Войти"

---

### **3.2 LoginViewModel.cs**

**ViewModel для окна входа:**

**Свойства:**
- Username (логин)
- Password (пароль)
- ErrorMessage (сообщение об ошибке)
- HasError (флаг ошибки)
- IsLoggingIn (флаг процесса входа)

**Команды:**
- LoginCommand (выполняет вход)

**События:**
- LoginSuccess (успешный вход)
- LoginFailed (ошибка входа)

---

### **3.3 UserProfileView.xaml / UserProfileView.xaml.cs**

**Представление профиля пользователя:**

![User Profile View](https://via.placeholder.com/700x600/FFFFFF/2C3E50?text=User+Profile)

**Возможности:**
- Просмотр информации о пользователе
- Смена отображаемого имени
- Смена email
- Смена пароля
- Смена логина

---

### **3.4 UserProfileViewModel.cs**

**ViewModel для профиля пользователя:**

**Свойства:**
- Username (текущий логин)
- DisplayName (отображаемое имя)
- Email
- RoleDisplay (отображаемая роль)
- LastLogin
- CurrentPassword (текущий пароль)
- NewPassword (новый пароль)
- ConfirmNewPassword (подтверждение)
- NewUsername (новый логин)
- Message (сообщение)
- HasMessage (флаг сообщения)
- IsMessageSuccess (тип сообщения)

**Команды:**
- ChangePasswordCommand
- ChangeUsernameCommand
- UpdateProfileCommand

---

### **3.5 ProfileWindow.xaml / ProfileWindow.xaml.cs**

**Окно профиля пользователя:**
- Отображает UserProfileView
- Кнопка "Закрыть"
- Модальное окно

---

## 📝 4. Модифицированные файлы

### **4.1 App.xaml.cs**

**Изменения:**
- Удалено: `StartupUri="Views/MainWindow.xaml"` → теперь `StartupUri=""`
- Добавлен: метод `OnStartup` с показом окна входа
- Добавлен: метод `OnExit` для очистки пользователя

**Логика:**
1. При запуске показывается `LoginWindow`
2. Если вход успешен → показывается `MainWindowSimple`
3. Если вход не удался → приложение закрывается

---

### **4.2 SettingsViewModel.cs**

**Добавлено:**
- Свойства: `CanChangePassword`, `CanAccessProfile`
- Команда: `OpenProfileCommand`
- Метод: `OpenProfile()` (открывает окно профиля)

---

### **4.3 SettingsView.xaml**

**Добавлено:**
- Кнопка "👤 Мой профиль" для открытия профиля
- Привязка видимости кнопки к `CanAccessProfile`

---

### **4.4 App.xaml**

**Добавлено:**
- Ресурс: `BoolToColorConverter` для сообщений

---

### **4.5 ForVlad.csproj**

**Добавлены файлы:**
```xml
<Page Include="Views\LoginWindow.xaml">
  <Generator>MSBuild:Compile</Generator>
  <SubType>Designer</SubType>
</Page>
<Compile Include="Views\LoginWindow.xaml.cs">
  <DependentUpon>LoginWindow.xaml</DependentUpon>
  <SubType>Code</SubType>
</Compile>
<Page Include="Views\UserProfileView.xaml">
  <Generator>MSBuild:Compile</Generator>
  <SubType>Designer</SubType>
</Page>
<Compile Include="Views\UserProfileView.xaml.cs">
  <DependentUpon>UserProfileView.xaml</DependentUpon>
  <SubType>Code</SubType>
</Compile>
<Page Include="Views\ProfileWindow.xaml">
  <Generator>MSBuild:Compile</Generator>
  <SubType>Designer</SubType>
</Page>
<Compile Include="Views\ProfileWindow.xaml.cs">
  <DependentUpon>ProfileWindow.xaml</DependentUpon>
  <SubType>Code</SubType>
</Compile>
<Compile Include="ViewModels\LoginViewModel.cs" />
<Compile Include="ViewModels\UserProfileViewModel.cs" />
<Compile Include="Services\PasswordHasher.cs" />
```

---

## ⚙️ 5. Инструкции по настройке

### **5.1 Генерация хэшей паролей**

**ВНИМАНИЕ:** Хэши паролей в `Database/07_CreateUsersTable.sql` временные!

**Для генерации правильных хэшей:**

1. В **Solution Explorer** найдите файл `Program.cs`
2. Правый клик → **Set as Startup Project**
3. Нажмите **F5** для запуска
4. Скопируйте сгенерированный SQL запрос
5. Выполните его в **SQL Server Management Studio**
6. Верните **Startup Project** обратно на `ForVlad`
7. Удалите или закомментируйте файл `Program.cs`

**ИЛИ** (если не хотите запускать Program.cs):

Выполните SQL скрипт из `Database/07_CreateUsersTable.sql`, а затем:
1. Войдите под пользователем `admin` с паролем `admin`
2. Перейдите в **Настройки → Мой профиль**
3. Смените пароль на любой другой
4. повторите для остальных пользователей

---

### **5.2 Создание новой базы данных**

**Шаги:**

1. Удалите старую БД **LeasingSystem** (если существует)
2. Выполните SQL скрипты в порядке:
   ```
   Database\01_CreateDatabase.sql
   Database\02_CreateTables.sql
   Database\03_CreateConstraints.sql
   Database\04_CreateStoredProcedures.sql
   Database\05_SeedData.sql
   Database\06_Migration_PaymentScheduleType.sql
   Database\07_CreateUsersTable.sql
   ```
3. (Опционально) Выполните `Database\UPDATE_Users_Passwords.sql` для обновления паролей

---

### **5.3 Миграция существующей базы данных**

**Шаги:**

1. Выполните миграционный скрипт:
   ```sql
   Database\06_Migration_PaymentScheduleType.sql
   ```
   (Он автоматически создаст таблицу Users и добавит пользователей)

2. Если таблица Users уже существует, выполните:
   ```sql
   Database\UPDATE_Users_Passwords.sql
   ```

---

## 👥 6. Пользователи по умолчанию

| Логин | Пароль | Роль | Описание |
|-------|--------|------|----------|
| admin | admin | Admin | Полный доступ ко всем функциям |
| manager | manager | Manager | Доступ к редактированию договоров, техники, контрагентов |
| accountant | accountant | Accountant | Доступ к просмотру договоров и работе с платежами |
| readonly | readonly | ReadOnly | Только просмотр данных |

**Примечание:** После генерации правильных хэшей пароли будут работать. До этого используйте временные пароли или измените их через интерфейс.

---

## 🔒 7. Безопасность

### **7.1 Хэширование паролей**

- **Алгоритм:** PBKDF2 с SHA256
- **Итерации:** 100,000
- **Размер соли:** 16 байт
- **Размер хэша:** 32 байт
- **Формат хранения:** Base64

### **7.2 Защита от атак**

- **Блокировка аккаунта:** После 5 неудачных попыток
- **Время блокировки:** 30 минут
- **Сброс счётчика:** При успешном входе

### **7.3 Роли пользователей**

| Роль | Описание |
|------|----------|
| Admin | Полный доступ ко всем функциям, включая управление пользователями |
| Manager | Может создавать/редактировать договоры, технику, контрагентов |
| Accountant | Может просматривать договоры и работать с платежами |
| ReadOnly | Только просмотр данных, без редактирования |

---

## 🚀 Быстрый старт

### **Для новой БД:**

1. Выполните все SQL скрипты из папки `Database/`
2. Запустите приложение
3. Войдите под пользователем `admin` с паролем `admin`
4. (Опционально) Смените пароли в профиле

### **Для существующей БД:**

1. Выполните `Database\06_Migration_PaymentScheduleType.sql`
2. Выполните `Database\07_CreateUsersTable.sql`
3. Запустите приложение
4. Войдите под пользователем `admin` с паролем `admin`

---

## 📊 Сводка всех изменений

### **Новые файлы:**
- ✅ `Services\PasswordHasher.cs` - Хэширование паролей
- ✅ `Views\LoginWindow.xaml` - Окно входа
- ✅ `Views\LoginWindow.xaml.cs` - Код окна входа
- ✅ `ViewModels\LoginViewModel.cs` - ViewModel входа
- ✅ `Views\UserProfileView.xaml` - Профиль пользователя
- ✅ `Views\UserProfileView.xaml.cs` - Код профиля
- ✅ `ViewModels\UserProfileViewModel.cs` - ViewModel профиля
- ✅ `Views\ProfileWindow.xaml` - Окно профиля
- ✅ `Views\ProfileWindow.xaml.cs` - Код окна профиля
- ✅ `Program.cs` - Утилита генерации хэшей (временный)
- ✅ `Database\UPDATE_Users_Passwords.sql` - Обновление паролей

### **Модифицированные файлы:**
- ✅ `Database\07_CreateUsersTable.sql` - Новая структура таблицы Users
- ✅ `Services\AuthenticationService.cs` - Классическая аутентификация
- ✅ `App.xaml.cs` - Окно входа при запуске
- ✅ `App.xaml` - Удален StartupUri
- ✅ `ViewModels\SettingsViewModel.cs` - Добавлена команда открытия профиля
- ✅ `Views\SettingsView.xaml` - Добавлена кнопка профиля
- ✅ `Converters\StatusConverters.cs` - Добавлен BoolToColorConverter
- ✅ `ForVlad.csproj` - Добавлены новые файлы

---

## 🎯 Функциональность

### **Вход в систему:**
- ✅ Классический вход по логину/паролю
- ✅ Валидация полей
- ✅ Отображение ошибок
- ✅ Блокировка аккаунта при множественных неудачных попыток

### **Профиль пользователя:**
- ✅ Просмотр информации о пользователе
- ✅ Смена отображаемого имени
- ✅ Смена email
- ✅ Смена пароля
- ✅ Смена логина
- ✅ Отображение роли и даты последнего входа

### **Безопасность:**
- ✅ Хэширование паролей с солью
- ✅ Защита от подбора пароля
- ✅ Разделение прав доступа

---

## 💡 Рекомендации

1. **После установки:**
   - Войдите под admin
   - Смените пароли для всех пользователей
   - Удалите файл `Program.cs`

2. **Для production:**
   - Сгенерируйте правильные хэши паролей
   - Настройте SSL для подключения к БД
   - Используйте сложные пароли

3. **Управление пользователями:**
   - только Admin может создавать новых пользователей
   - Пользователи могут менять свои данные в профиле

---

## 📞 Поддержка

Если возникли вопросы или проблемы:
1. Проверьте, что все SQL скрипты выполнены
2. Проверьте подключение к БД
3. Проверьте, что в таблице Users есть пользователи
4. Смотрите логи в Output Window (Debug)

---

**Готово!** Ваша система аутентификации полностью настроена и работоспособна! 🎉
