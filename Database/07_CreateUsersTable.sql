-- ============================================
-- Скрипт создания таблицы Users для аутентификации по логину/паролю
-- Версия: 2.0
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
    
    -- Добавляем пользователей по умолчанию (пароли: admin, manager, accountant, readonly)
    -- Пароли хэшируются как: SHA256(loginsalt + password + usersalt)
    -- В приложении используется динамическое хэширование
    
    -- Admin (password: admin) - SHA256(username + salt + password)
    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('admin', 'Администратор', '771B3793E1EBA37D874595917A11D5BC4894550C755742C78064210A35A9B747', '7777', 'admin@leasing.com', 0, 1, 0);
    
    -- Manager (password: manager)
    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('manager', 'Менеджер', '2438799D4B0D76B8C95B7C507AFA810AA112E212A1D2D3EBC894919EA569BC3F', '7777', 'manager@leasing.com', 1, 1, 0);
    
    -- Accountant (password: accountant)
    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('accountant', 'Бухгалтер', '98F8E8D00F424DED7BE94409B7D5B79BF1E56011EE01E97BC84385B143AADF19', '7777', 'accountant@leasing.com', 2, 1, 0);
    
    -- ReadOnly (password: readonly)
    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('readonly', 'Просмотр', 'B856A5A1A2ECE8C24E831AAB9F65CD3D442ABC111E1577F1A68A8D861DB8F731', '7777', 'readonly@leasing.com', 3, 1, 0);
    
    PRINT 'Пользователи по умолчанию добавлены:';
    PRINT '  - admin / admin (Администратор)';
    PRINT '  - manager / manager (Менеджер)';
    PRINT '  - accountant / accountant (Бухгалтер)';
    PRINT '  - readonly / readonly (Просмотр)';
END
ELSE
BEGIN
    PRINT 'Таблица Users уже существует.';
END
GO

PRINT 'Скрипт создания таблицы Users завершён.';
GO