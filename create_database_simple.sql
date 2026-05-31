-- Простой скрипт для создания базы данных
USE master;
GO

-- Создание базы данных
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'LeasingSystemDb')
BEGIN
    CREATE DATABASE LeasingSystemDb;
    PRINT 'База данных LeasingSystemDb создана';
END
ELSE
BEGIN
    PRINT 'База данных LeasingSystemDb уже существует';
END
GO

USE LeasingSystemDb;
GO

-- Таблица контрагентов
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Counterparties')
BEGIN
    CREATE TABLE Counterparties (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(200) NOT NULL,
        INN NVARCHAR(12) NULL,
        KPP NVARCHAR(9) NULL,
        OGRN NVARCHAR(13) NULL,
        LegalAddress NVARCHAR(500) NULL,
        ActualAddress NVARCHAR(500) NULL,
        ContactPerson NVARCHAR(100) NULL,
        Phone NVARCHAR(20) NULL,
        Email NVARCHAR(100) NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        ModifiedDate DATETIME NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    PRINT 'Таблица Counterparties создана';
END
GO

-- Таблица техники/оборудования
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Assets')
BEGIN
    CREATE TABLE Assets (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(200) NOT NULL,
        InventoryNumber NVARCHAR(50) NULL,
        AssetGroup INT NOT NULL, -- 0=Vehicle, 1=Equipment
        Subcategory NVARCHAR(100) NULL,
        Manufacturer NVARCHAR(100) NULL,
        Model NVARCHAR(100) NULL,
        SerialNumber NVARCHAR(50) NULL,
        YearOfManufacture INT NULL,
        PurchasePrice DECIMAL(18,2) NULL,
        ResidualValue DECIMAL(18,2) NULL,
        MonthlyRentalRate DECIMAL(18,2) NULL,
        IsAvailable BIT NOT NULL DEFAULT 1,
        Notes NVARCHAR(MAX) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        ModifiedDate DATETIME NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    PRINT 'Таблица Assets создана';
END
GO

-- Таблица договоров
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Contracts')
BEGIN
    CREATE TABLE Contracts (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ContractNumber NVARCHAR(50) NOT NULL,
        ContractType INT NOT NULL, -- 0=Rental, 1=Leasing
        Status INT NOT NULL, -- 0=Draft, 1=Active, 2=Suspended, 3=Completed, 4=Terminated
        CounterpartyId INT NOT NULL,
        SignedDate DATETIME NOT NULL,
        StartDate DATETIME NOT NULL,
        EndDate DATETIME NULL,
        DurationMonths INT NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        VATAmount DECIMAL(18,2) NOT NULL,
        TotalWithVAT DECIMAL(18,2) NOT NULL,
        AdvancePayment DECIMAL(18,2) NOT NULL DEFAULT 0,
        MonthlyPayment DECIMAL(18,2) NULL,
        PaymentTerms NVARCHAR(500) NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        ModifiedDate DATETIME NULL,
        ActivationDate DATETIME NULL,
        CompletionDate DATETIME NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (CounterpartyId) REFERENCES Counterparties(Id)
    );
    PRINT 'Таблица Contracts создана';
END
GO

-- Таблица спецификаций договоров
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ContractSpecifications')
BEGIN
    CREATE TABLE ContractSpecifications (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ContractId INT NOT NULL,
        AssetId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        TotalPrice DECIMAL(18,2) NOT NULL,
        AdditionalConditions NVARCHAR(MAX) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        ModifiedDate DATETIME NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (ContractId) REFERENCES Contracts(Id),
        FOREIGN KEY (AssetId) REFERENCES Assets(Id)
    );
    PRINT 'Таблица ContractSpecifications создана';
END
GO

-- Таблица графика платежей
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PaymentSchedules')
BEGIN
    CREATE TABLE PaymentSchedules (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ContractId INT NOT NULL,
        DueDate DATETIME NOT NULL,
        PaymentDate DATETIME NULL,
        Amount DECIMAL(18,2) NOT NULL,
        VATAmount DECIMAL(18,2) NOT NULL,
        TotalAmount DECIMAL(18,2) NULL,
        Status INT NOT NULL, -- 0=Pending, 1=Paid, 2=Overdue, 3=Cancelled
        PaymentMethod NVARCHAR(50) NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        ModifiedDate DATETIME NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (ContractId) REFERENCES Contracts(Id)
    );
    PRINT 'Таблица PaymentSchedules создана';
END
GO

-- Создание индексов
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Counterparties_Name' AND object_id = OBJECT_ID('Counterparties'))
BEGIN
    CREATE INDEX IX_Counterparties_Name ON Counterparties(Name);
    PRINT 'Индекс IX_Counterparties_Name создан';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Contracts_ContractNumber' AND object_id = OBJECT_ID('Contracts'))
BEGIN
    CREATE INDEX IX_Contracts_ContractNumber ON Contracts(ContractNumber);
    PRINT 'Индекс IX_Contracts_ContractNumber создан';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Contracts_Status' AND object_id = OBJECT_ID('Contracts'))
BEGIN
    CREATE INDEX IX_Contracts_Status ON Contracts(Status);
    PRINT 'Индекс IX_Contracts_Status создан';
END
GO

PRINT '=== База данных успешно настроена ===';
PRINT 'Таблицы:';
PRINT '  - Counterparties (контрагенты)';
PRINT '  - Assets (техника/оборудование)';
PRINT '  - Contracts (договоры)';
PRINT '  - ContractSpecifications (спецификации)';
PRINT '  - PaymentSchedules (график платежей)';
GO