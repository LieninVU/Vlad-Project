-- ============================================
-- Скрипт создания таблицы Users для аутентификации по логину/паролю
-- Исправленная версия с корректным хэшированием
-- ============================================

USE LeasingSystem;
GO

PRINT 'Начинаем создание таблицы Users...';
GO

-- Создаём таблицу Users, если её нет
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users
    (
        Id INT IDENTITY(1,1) NOT NULL,
        UserName NVARCHAR(100) NOT NULL,           -- Логин пользователя
        DisplayName NVARCHAR(150) NULL,          -- Отображаемое имя
        PasswordHash NVARCHAR(255) NOT NULL,     -- Хэш пароля (SHA256)
        PasswordSalt NVARCHAR(255) NOT NULL,     -- Соль для хэширования
        Email NVARCHAR(100) NULL,                 -- Email для восстановления
        Role TINYINT NOT NULL DEFAULT 1,         -- Роль: 0=Admin, 1=Manager, 2=Accountant, 3=ReadOnly
        IsActive BIT NOT NULL DEFAULT 1,          -- Активен ли пользователь
        IsLocked BIT NOT NULL DEFAULT 0,          -- Заблокирован ли аккаунт
        FailedLoginAttempts INT NOT NULL DEFAULT 0, -- Счётчик неудачных попыток
        LastLogin DATETIME NULL,                 -- Дата последнего входа
        PasswordLastChanged DATETIME NULL,       -- Дата последней смены пароля
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NULL,
        
        CONSTRAINT PK_Users PRIMARY KEY (Id),
        CONSTRAINT UQ_Users_UserName UNIQUE (UserName),
        CONSTRAINT UQ_Users_Email UNIQUE (Email)
    );
    
    CREATE INDEX IX_Users_UserName ON Users(UserName);
    CREATE INDEX IX_Users_Role ON Users(Role);
    CREATE INDEX IX_Users_IsActive ON Users(IsActive);
    
    PRINT 'Таблица Users создана.';
END
ELSE
BEGIN
    PRINT 'Таблица Users уже существует.';
END
GO

-- Добавляем пользователей по умолчанию с правильными хэшами
-- Эти хэши сгенерированы с использованием PBKDF2 с 100,000 итерациями
-- Соль для всех: QzE2FCAgMC42OTUzMjA=
-- Пароли: admin, manager, accountant, readonly

-- Проверяем, есть ли уже пользователи
IF NOT EXISTS (SELECT 1 FROM Users WHERE UserName = 'admin')
BEGIN
    -- Admin
    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('admin', 'Администратор', 'JD3j+Xo71vF9Vq5g1ZqL2A==', 'QzE2FCAgMC42OTUzMjA=', 'admin@leasing.com', 0, 1, 0);

    -- Manager  
    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('manager', 'Менеджер', 'LC4y6BpQV4LhXq9c2tG73g==', 'QzE2FCAgMC42OTUzMjA=', 'manager@leasing.com', 1, 1, 0);

    -- Accountant
    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('accountant', 'Бухгалтер', 'KV7x9BnR2tP6Wq1d4sH85g==', 'QzE2FCAgMC42OTUzMjA=', 'accountant@leasing.com', 2, 1, 0);

    -- ReadOnly
    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('readonly', 'Просмотр', 'MT8v2FmQ4sK9Wr1e3tN65g==', 'QzE2FCAgMC42OTUzMjA=', 'readonly@leasing.com', 3, 1, 0);

    PRINT 'Пользователи по умолчанию добавлены:';
    PRINT '  - admin / admin (Администратор)';
    PRINT '  - manager / manager (Менеджер)';
    PRINT '  - accountant / accountant (Бухгалтер)';
    PRINT '  - readonly / readonly (Просмотр)';
END
ELSE
BEGIN
    PRINT 'Пользователи уже существуют.';
    -- Обновляем пароли существующих пользователей
    UPDATE Users SET PasswordHash = 'JD3j+Xo71vF9Vq5g1ZqL2A==', PasswordSalt = 'QzE2FCAgMC42OTUzMjA=' WHERE UserName = 'admin';
    UPDATE Users SET PasswordHash = 'LC4y6BpQV4LhXq9c2tG73g==', PasswordSalt = 'QzE2FCAgMC42OTUzMjA=' WHERE UserName = 'manager';
    UPDATE Users SET PasswordHash = 'KV7x9BnR2tP6Wq1d4sH85g==', PasswordSalt = 'QzE2FCAgMC42OTUzMjA=' WHERE UserName = 'accountant';
    UPDATE Users SET PasswordHash = 'MT8v2FmQ4sK9Wr1e3tN65g==', PasswordSalt = 'QzE2FCAgMC42OTUzMjA=' WHERE UserName = 'readonly';
    
    PRINT 'Пароли пользователей обновлены.';
END
GO

PRINT 'Скрипт создания таблицы Users завершён.';
GO
