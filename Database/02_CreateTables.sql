-- ============================================
-- Скрипт создания таблиц базы данных LeasingSystem
-- Версия: 2.0 (с полной поддержкой Unicode для кириллицы)
-- 100% совместим с SQL Server 2019
-- ============================================

USE LeasingSystem;
GO

PRINT 'Начинаем создание таблиц...';
GO

-- ----------------------------------------------------------------------
-- Таблица: Counterparties
-- ----------------------------------------------------------------------
CREATE TABLE Counterparties
(
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    CounterpartyType TINYINT NOT NULL,
    Inn NVARCHAR(12) NOT NULL,
    Kpp NVARCHAR(9) NULL,
    LegalAddress NVARCHAR(500) NULL,
    ActualAddress NVARCHAR(500) NULL,
    ContactPerson NVARCHAR(100) NULL,
    Phone NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Notes NVARCHAR(1000) NULL,
    IsActive BIT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    CONSTRAINT PK_Counterparties PRIMARY KEY (Id),
    CONSTRAINT UQ_Counterparties_Inn UNIQUE (Inn)
);
GO

ALTER TABLE Counterparties ADD CONSTRAINT DF_Counterparties_CounterpartyType DEFAULT (0) FOR CounterpartyType;
ALTER TABLE Counterparties ADD CONSTRAINT DF_Counterparties_IsActive DEFAULT (1) FOR IsActive;
ALTER TABLE Counterparties ADD CONSTRAINT DF_Counterparties_CreatedAt DEFAULT (GETDATE()) FOR CreatedAt;

CREATE INDEX IX_Counterparties_Name ON Counterparties(Name);
CREATE INDEX IX_Counterparties_IsActive ON Counterparties(IsActive);
GO

PRINT 'Таблица Counterparties создана.';
GO

-- ----------------------------------------------------------------------
-- Таблица: Assets
-- ----------------------------------------------------------------------
CREATE TABLE Assets
(
    Id INT IDENTITY(1,1) NOT NULL,
    InventoryNumber NVARCHAR(50) NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    AssetGroup TINYINT NOT NULL,
    VehicleBrand NVARCHAR(100) NULL,
    VehicleModel NVARCHAR(100) NULL,
    VinNumber NVARCHAR(50) NULL,
    ManufactureYear INT NULL,
    EnginePower DECIMAL(10,2) NULL,
    RegistrationNumber NVARCHAR(20) NULL,
    EquipmentType NVARCHAR(100) NULL,
    Weight DECIMAL(10,2) NULL,
    PowerRequirements NVARCHAR(100) NULL,
    VehicleSubcategory TINYINT NULL,
    EquipmentSubcategory TINYINT NULL,
    HourlyRate DECIMAL(12,2) NOT NULL,
    DailyRate DECIMAL(12,2) NOT NULL,
    MonthlyRate DECIMAL(12,2) NULL,
    AssetCondition TINYINT NOT NULL,
    IsAvailable BIT NOT NULL,
    Description NVARCHAR(1000) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    CONSTRAINT PK_Assets PRIMARY KEY (Id),
    CONSTRAINT UQ_Assets_InventoryNumber UNIQUE (InventoryNumber)
);
GO

ALTER TABLE Assets ADD CONSTRAINT DF_Assets_AssetCondition DEFAULT (0) FOR AssetCondition;
ALTER TABLE Assets ADD CONSTRAINT DF_Assets_IsAvailable DEFAULT (1) FOR IsAvailable;
ALTER TABLE Assets ADD CONSTRAINT DF_Assets_CreatedAt DEFAULT (GETDATE()) FOR CreatedAt;

CREATE INDEX IX_Assets_AssetGroup ON Assets(AssetGroup);
CREATE INDEX IX_Assets_IsAvailable ON Assets(IsAvailable);
CREATE INDEX IX_Assets_Name ON Assets(Name);
GO

PRINT 'Таблица Assets создана.';
GO

-- Добавление новых столбцов для обратной совместимости
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'PurchasePrice')
    ALTER TABLE Assets ADD PurchasePrice DECIMAL(18,2) NOT NULL DEFAULT 0;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'ResidualValue')
    ALTER TABLE Assets ADD ResidualValue DECIMAL(18,2) NOT NULL DEFAULT 0;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'Manufacturer')
    ALTER TABLE Assets ADD Manufacturer NVARCHAR(100) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'Model')
    ALTER TABLE Assets ADD Model NVARCHAR(100) NULL;
GO

PRINT 'Новые столбцы Assets добавлены.';

-- ----------------------------------------------------------------------
-- Таблица: Contracts
-- ----------------------------------------------------------------------
CREATE TABLE Contracts
(
    Id INT IDENTITY(1,1) NOT NULL,
    ContractNumber NVARCHAR(30) NOT NULL,
    ContractType TINYINT NOT NULL,
    ContractStatus TINYINT NOT NULL,
    CounterpartyId INT NOT NULL,
    SignedDate DATE NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    TotalAmount DECIMAL(12,2) NOT NULL,
    PaymentTerms NVARCHAR(500) NULL,
    PaymentScheduleType TINYINT NOT NULL DEFAULT 1,
    Notes NVARCHAR(2000) NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    CONSTRAINT PK_Contracts PRIMARY KEY (Id),
    CONSTRAINT UQ_Contracts_ContractNumber UNIQUE (ContractNumber),
    CONSTRAINT FK_Contracts_Counterparties FOREIGN KEY (CounterpartyId) REFERENCES Counterparties(Id)
);
GO

ALTER TABLE Contracts ADD CONSTRAINT DF_Contracts_ContractStatus DEFAULT (0) FOR ContractStatus;
ALTER TABLE Contracts ADD CONSTRAINT DF_Contracts_CreatedAt DEFAULT (GETDATE()) FOR CreatedAt;

CREATE INDEX IX_Contracts_ContractStatus ON Contracts(ContractStatus);
CREATE INDEX IX_Contracts_StartDate_EndDate ON Contracts(StartDate, EndDate);
CREATE INDEX IX_Contracts_CounterpartyId ON Contracts(CounterpartyId);
GO

PRINT 'Таблица Contracts создана.';
GO

-- ----------------------------------------------------------------------
-- Таблица: ContractSpecifications
-- ----------------------------------------------------------------------
CREATE TABLE ContractSpecifications
(
    Id INT IDENTITY(1,1) NOT NULL,
    ContractId INT NOT NULL,
    AssetId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(12,2) NOT NULL,
    PeriodType TINYINT NOT NULL,
    AdditionalConditions NVARCHAR(500) NULL,
    CONSTRAINT PK_ContractSpecifications PRIMARY KEY (Id),
    CONSTRAINT FK_ContractSpecifications_Contracts FOREIGN KEY (ContractId) REFERENCES Contracts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ContractSpecifications_Assets FOREIGN KEY (AssetId) REFERENCES Assets(Id)
);
GO

ALTER TABLE ContractSpecifications ADD CONSTRAINT DF_ContractSpecifications_Quantity DEFAULT (1) FOR Quantity;

CREATE INDEX IX_ContractSpecifications_ContractId ON ContractSpecifications(ContractId);
CREATE INDEX IX_ContractSpecifications_AssetId ON ContractSpecifications(AssetId);
GO

PRINT 'Таблица ContractSpecifications создана.';
GO

-- ----------------------------------------------------------------------
-- Таблица: PaymentSchedules
-- ----------------------------------------------------------------------
CREATE TABLE PaymentSchedules
(
    Id INT IDENTITY(1,1) NOT NULL,
    ContractId INT NOT NULL,
    PaymentNumber INT NOT NULL,
    Description NVARCHAR(200) NOT NULL,
    DueDate DATE NOT NULL,
    Amount DECIMAL(12,2) NOT NULL,
    IsPaid BIT NOT NULL,
    PaidDate DATE NULL,
    PaymentMethod TINYINT NULL,
    PaymentReference NVARCHAR(500) NULL,
    CONSTRAINT PK_PaymentSchedules PRIMARY KEY (Id),
    CONSTRAINT FK_PaymentSchedules_Contracts FOREIGN KEY (ContractId) REFERENCES Contracts(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_PaymentSchedules_Contract_Payment UNIQUE (ContractId, PaymentNumber)
);
GO

ALTER TABLE PaymentSchedules ADD CONSTRAINT DF_PaymentSchedules_IsPaid DEFAULT (0) FOR IsPaid;

CREATE INDEX IX_PaymentSchedules_ContractId ON PaymentSchedules(ContractId);
CREATE INDEX IX_PaymentSchedules_DueDate ON PaymentSchedules(DueDate);
CREATE INDEX IX_PaymentSchedules_IsPaid ON PaymentSchedules(IsPaid);
GO

PRINT 'Таблица PaymentSchedules создана.';
GO

-- ----------------------------------------------------------------------
-- Таблица: AuditLogs
-- ----------------------------------------------------------------------
CREATE TABLE AuditLogs
(
    Id INT IDENTITY(1,1) NOT NULL,
    TableName NVARCHAR(100) NOT NULL,
    RecordId INT NOT NULL,
    ActionType NVARCHAR(20) NOT NULL,
    OldValues NVARCHAR(MAX) NULL,
    NewValues NVARCHAR(MAX) NULL,
    ChangedBy NVARCHAR(100) NULL,
    ChangedAt DATETIME NOT NULL,
    CONSTRAINT PK_AuditLogs PRIMARY KEY (Id)
);
GO

ALTER TABLE AuditLogs ADD CONSTRAINT DF_AuditLogs_ChangedAt DEFAULT (GETDATE()) FOR ChangedAt;

CREATE INDEX IX_AuditLogs_TableName_RecordId ON AuditLogs(TableName, RecordId);
CREATE INDEX IX_AuditLogs_ChangedAt ON AuditLogs(ChangedAt);
GO

PRINT 'Таблица AuditLogs создана.';
GO

-- ----------------------------------------------------------------------
-- Таблица: Users
-- ----------------------------------------------------------------------
CREATE TABLE Users
(
    Id INT IDENTITY(1,1) NOT NULL,
    UserName NVARCHAR(100) NOT NULL,
    DisplayName NVARCHAR(150) NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    PasswordSalt NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NULL,
    Role TINYINT NOT NULL DEFAULT 1,
    IsActive BIT NOT NULL DEFAULT 1,
    IsLocked BIT NOT NULL DEFAULT 0,
    FailedLoginAttempts INT NOT NULL DEFAULT 0,
    LastLogin DATETIME NULL,
    PasswordLastChanged DATETIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    CONSTRAINT PK_Users PRIMARY KEY (Id),
    CONSTRAINT UQ_Users_UserName UNIQUE (UserName),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO

CREATE INDEX IX_Users_UserName ON Users(UserName);
CREATE INDEX IX_Users_Role ON Users(Role);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);
GO

PRINT 'Таблица Users создана.';
GO

-- Добавляем пользователей по умолчанию
IF NOT EXISTS (SELECT 1 FROM Users WHERE UserName = 'admin')
BEGIN
    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('admin', N'Администратор', 'c2c3e2d88c67c4ef159ecab486741d807bad6f162193153b8a450695221a9ec6', 'QzE2FCAgMC42OTUzMjA=', 'admin@leasing.com', 0, 1, 0);

    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('manager', N'Менеджер', 'b6d5a1f198248377e5a5216f9ed62a513d30989d8acd210003426baf758366d2', 'QzE2FCAgMC42OTUzMjA=', 'manager@leasing.com', 1, 1, 0);

    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('accountant', N'Бухгалтер', '2287daba83068f4a01ea9945e15b10111aa836f3bdc8ab06d87ec9dc80263219', 'QzE2FCAgMC42OTUzMjA=', 'accountant@leasing.com', 2, 1, 0);

    INSERT INTO Users (UserName, DisplayName, PasswordHash, PasswordSalt, Email, Role, IsActive, IsLocked)
    VALUES ('readonly', N'Просмотр', '8d208d878f44481b59317e0b1c5d27196d628bdb7d32b1fb555b68df590c43f8', 'QzE2FCAgMC42OTUzMjA=', 'readonly@leasing.com', 3, 1, 0);

    PRINT 'Пользователи по умолчанию добавлены: admin/manager/accountant/readonly.';
END
ELSE
BEGIN
    PRINT 'Пользователи уже существуют.';
END
GO

-- ----------------------------------------------------------------------
-- Таблица: SystemSettings
-- ----------------------------------------------------------------------
CREATE TABLE SystemSettings
(
    Id INT IDENTITY(1,1) NOT NULL,
    SettingKey NVARCHAR(100) NOT NULL,
    SettingValue NVARCHAR(MAX) NULL,
    Description NVARCHAR(500) NULL,
    IsActive BIT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NULL,
    CONSTRAINT PK_SystemSettings PRIMARY KEY (Id),
    CONSTRAINT UQ_SystemSettings_SettingKey UNIQUE (SettingKey)
);
GO

ALTER TABLE SystemSettings ADD CONSTRAINT DF_SystemSettings_IsActive DEFAULT (1) FOR IsActive;
ALTER TABLE SystemSettings ADD CONSTRAINT DF_SystemSettings_CreatedAt DEFAULT (GETDATE()) FOR CreatedAt;

CREATE INDEX IX_SystemSettings_SettingKey ON SystemSettings(SettingKey);
GO

PRINT 'Таблица SystemSettings создана.';
GO

-- ----------------------------------------------------------------------
-- Функции русской локализации перечислений (TINYINT/BIT -> NVARCHAR)
-- ----------------------------------------------------------------------
PRINT 'Создание функций русской локализации...';
GO

CREATE OR ALTER FUNCTION dbo.fn_ContractStatusRu(@Value TINYINT)
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS BEGIN RETURN CASE @Value
    WHEN 0 THEN N'Черновик' WHEN 1 THEN N'Подписан' WHEN 2 THEN N'Действующий'
    WHEN 3 THEN N'Приостановлен' WHEN 4 THEN N'Завершён' WHEN 5 THEN N'Расторгнут'
    ELSE N'Неизвестно' END; END;
GO

CREATE OR ALTER FUNCTION dbo.fn_ContractTypeRu(@Value TINYINT)
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS BEGIN RETURN CASE @Value
    WHEN 0 THEN N'Аренда' WHEN 1 THEN N'Лизинг' ELSE N'Неизвестно' END; END;
GO

CREATE OR ALTER FUNCTION dbo.fn_PaymentStatusRu(@IsPaid BIT, @DueDate DATE)
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS BEGIN
    IF @IsPaid = 1 RETURN N'Оплачен';
    IF @DueDate < CAST(GETDATE() AS DATE) RETURN N'Просрочен';
    RETURN N'Ожидает оплаты';
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_CounterpartyTypeRu(@Value TINYINT)
RETURNS NVARCHAR(100) WITH SCHEMABINDING AS BEGIN RETURN CASE @Value
    WHEN 0 THEN N'Юридическое лицо' WHEN 1 THEN N'ИП' WHEN 2 THEN N'Физическое лицо'
    ELSE N'Неизвестно' END; END;
GO

CREATE OR ALTER FUNCTION dbo.fn_AssetGroupRu(@Value TINYINT)
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS BEGIN RETURN CASE @Value
    WHEN 0 THEN N'Техника' WHEN 1 THEN N'Оборудование' ELSE N'Неизвестно' END; END;
GO

CREATE OR ALTER FUNCTION dbo.fn_VehicleSubcategoryRu(@Value TINYINT)
RETURNS NVARCHAR(100) WITH SCHEMABINDING AS BEGIN RETURN CASE @Value
    WHEN 0 THEN N'Дорожно-строительная' WHEN 1 THEN N'Промышленный транспорт'
    WHEN 2 THEN N'Прочая самоходная' ELSE N'Неизвестно' END; END;
GO

CREATE OR ALTER FUNCTION dbo.fn_EquipmentSubcategoryRu(@Value TINYINT)
RETURNS NVARCHAR(100) WITH SCHEMABINDING AS BEGIN RETURN CASE @Value
    WHEN 0 THEN N'Прицепы и тралы' WHEN 1 THEN N'Складское' WHEN 2 THEN N'Строительное'
    WHEN 3 THEN N'Навесное оборудование' WHEN 4 THEN N'Прочее промышленное'
    ELSE N'Неизвестно' END; END;
GO

CREATE OR ALTER FUNCTION dbo.fn_AssetConditionRu(@Value TINYINT)
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS BEGIN RETURN CASE @Value
    WHEN 0 THEN N'Новый' WHEN 1 THEN N'Хорошее' WHEN 2 THEN N'Удовлетворительное'
    WHEN 3 THEN N'Требует ремонта' WHEN 4 THEN N'Неисправно' ELSE N'Неизвестно' END; END;
GO

CREATE OR ALTER FUNCTION dbo.fn_PeriodTypeRu(@Value TINYINT)
RETURNS NVARCHAR(50) WITH SCHEMABINDING AS BEGIN RETURN CASE @Value
    WHEN 0 THEN N'Час' WHEN 1 THEN N'Смена' WHEN 2 THEN N'День'
    WHEN 3 THEN N'Неделя' WHEN 4 THEN N'Месяц' ELSE N'Неизвестно' END; END;
GO

CREATE OR ALTER FUNCTION dbo.fn_BooleanRu(@Value BIT)
RETURNS NVARCHAR(10) WITH SCHEMABINDING AS BEGIN RETURN CASE WHEN @Value = 1 THEN N'Да' ELSE N'Нет' END; END;
GO

PRINT 'Функции русской локализации созданы.';
GO

-- ----------------------------------------------------------------------
-- Добавление вычисляемых столбцов с русскими названиями
-- ----------------------------------------------------------------------
PRINT 'Добавление вычисляемых столбцов с русской локализацией...';
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Counterparties') AND name = 'CounterpartyTypeRu')
    ALTER TABLE Counterparties ADD CounterpartyTypeRu AS dbo.fn_CounterpartyTypeRu(CounterpartyType) PERSISTED;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'AssetGroupRu')
    ALTER TABLE Assets ADD AssetGroupRu AS dbo.fn_AssetGroupRu(AssetGroup) PERSISTED;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'AssetConditionRu')
    ALTER TABLE Assets ADD AssetConditionRu AS dbo.fn_AssetConditionRu(AssetCondition) PERSISTED;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'IsAvailableRu')
    ALTER TABLE Assets ADD IsAvailableRu AS dbo.fn_BooleanRu(IsAvailable) PERSISTED;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'ContractTypeRu')
    ALTER TABLE Contracts ADD ContractTypeRu AS dbo.fn_ContractTypeRu(ContractType) PERSISTED;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'ContractStatusRu')
    ALTER TABLE Contracts ADD ContractStatusRu AS dbo.fn_ContractStatusRu(ContractStatus) PERSISTED;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PaymentSchedules') AND name = 'IsPaidRu')
    ALTER TABLE PaymentSchedules ADD IsPaidRu AS dbo.fn_BooleanRu(IsPaid) PERSISTED;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'RoleRu')
    ALTER TABLE Users ADD RoleRu AS CASE Role WHEN 0 THEN N'Администратор' WHEN 1 THEN N'Менеджер' WHEN 2 THEN N'Бухгалтер' WHEN 3 THEN N'Просмотр' ELSE N'Неизвестно' END PERSISTED;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'IsActiveRu')
    ALTER TABLE Users ADD IsActiveRu AS dbo.fn_BooleanRu(IsActive) PERSISTED;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'IsLockedRu')
    ALTER TABLE Users ADD IsLockedRu AS dbo.fn_BooleanRu(IsLocked) PERSISTED;
GO

PRINT 'Вычисляемые столбцы с русской локализацией добавлены.';
GO

PRINT '==============================================';
PRINT 'ВСЕ ТАБЛИЦЫ СОЗДАНЫ УСПЕШНО!';
PRINT '==============================================';
GO
