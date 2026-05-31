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

PRINT '==============================================';
PRINT 'ВСЕ ТАБЛИЦЫ СОЗДАНЫ УСПЕШНО!';
PRINT '==============================================';
GO
