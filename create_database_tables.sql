-- Скрипт создания таблиц для системы учёта договоров аренды
-- Выполнить в SQL Server Management Studio или через sqlcmd

USE LeasingSystem;
GO

-- Таблица контрагентов
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Counterparties')
BEGIN
    CREATE TABLE Counterparties (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        INN NVARCHAR(12) NOT NULL,
        KPP NVARCHAR(9) NULL,
        OGRN NVARCHAR(15) NULL,
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

-- Таблица техники и оборудования
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Assets')
BEGIN
    CREATE TABLE Assets (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        InventoryNumber NVARCHAR(50) NOT NULL UNIQUE,
        AssetGroup INT NOT NULL,  -- 0 = Vehicle, 1 = Equipment
        Subcategory NVARCHAR(50) NULL,
        Manufacturer NVARCHAR(100) NULL,
        Model NVARCHAR(100) NULL,
        SerialNumber NVARCHAR(100) NULL,
        YearOfManufacture INT NULL,
        PurchasePrice DECIMAL(18, 2) NOT NULL,
        ResidualValue DECIMAL(18, 2) NOT NULL,
        MonthlyRentalRate DECIMAL(18, 2) NOT NULL,
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
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Contracts')
BEGIN
    CREATE TABLE Contracts (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ContractNumber NVARCHAR(50) NOT NULL UNIQUE,
        ContractType INT NOT NULL,  -- 0 = Rental, 1 = Leasing
        Status INT NOT NULL,  -- 0 = Draft, 1 = Active, 2 = Suspended, 3 = Completed, 4 = Terminated
        CounterpartyId INT NOT NULL,
        SignedDate DATETIME NOT NULL,
        StartDate DATETIME NOT NULL,
        EndDate DATETIME NULL,
        DurationMonths INT NOT NULL,
        TotalAmount DECIMAL(18, 2) NOT NULL,
        VATAmount DECIMAL(18, 2) NOT NULL,
        TotalWithVAT DECIMAL(18, 2) NOT NULL,
        AdvancePayment DECIMAL(18, 2) NOT NULL DEFAULT 0,
        MonthlyPayment DECIMAL(18, 2) NOT NULL DEFAULT 0,
        PaymentTerms NVARCHAR(500) NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        ModifiedDate DATETIME NULL,
        ActivationDate DATETIME NULL,
        CompletionDate DATETIME NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_Contracts_Counterparties FOREIGN KEY (CounterpartyId) 
            REFERENCES Counterparties(Id) ON DELETE NO ACTION
    );
    
    PRINT 'Таблица Contracts создана';
END
GO

-- Таблица спецификаций договоров
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ContractSpecifications')
BEGIN
    CREATE TABLE ContractSpecifications (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ContractId INT NOT NULL,
        AssetId INT NOT NULL,
        Quantity INT NOT NULL DEFAULT 1,
        UnitPrice DECIMAL(18, 2) NOT NULL,
        TotalPrice DECIMAL(18, 2) NOT NULL,
        AdditionalConditions NVARCHAR(MAX) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        ModifiedDate DATETIME NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_ContractSpecifications_Contracts FOREIGN KEY (ContractId) 
            REFERENCES Contracts(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ContractSpecifications_Assets FOREIGN KEY (AssetId) 
            REFERENCES Assets(Id) ON DELETE NO ACTION
    );
    
    PRINT 'Таблица ContractSpecifications создана';
END
GO

-- Таблица графиков платежей
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PaymentSchedules')
BEGIN
    CREATE TABLE PaymentSchedules (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ContractId INT NOT NULL,
        DueDate DATETIME NOT NULL,
        PaymentDate DATETIME NULL,
        Amount DECIMAL(18, 2) NOT NULL,
        VATAmount DECIMAL(18, 2) NOT NULL,
        TotalAmount DECIMAL(18, 2) NULL,
        Status INT NOT NULL DEFAULT 0,  -- 0 = Pending, 1 = Paid, 2 = Overdue, 3 = Cancelled
        PaymentMethod NVARCHAR(50) NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        ModifiedDate DATETIME NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_PaymentSchedules_Contracts FOREIGN KEY (ContractId) 
            REFERENCES Contracts(Id) ON DELETE CASCADE
    );
    
    PRINT 'Таблица PaymentSchedules создана';
END
GO

-- Создание индексов для оптимизации
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Contracts_CounterpartyId')
BEGIN
    CREATE INDEX IX_Contracts_CounterpartyId ON Contracts(CounterpartyId);
    PRINT 'Индекс IX_Contracts_CounterpartyId создан';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Contracts_Status')
BEGIN
    CREATE INDEX IX_Contracts_Status ON Contracts(Status);
    PRINT 'Индекс IX_Contracts_Status создан';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assets_AssetGroup')
BEGIN
    CREATE INDEX IX_Assets_AssetGroup ON Assets(AssetGroup);
    PRINT 'Индекс IX_Assets_AssetGroup создан';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ContractSpecifications_ContractId')
BEGIN
    CREATE INDEX IX_ContractSpecifications_ContractId ON ContractSpecifications(ContractId);
    PRINT 'Индекс IX_ContractSpecifications_ContractId создан';
END
GO

PRINT '========================================';
PRINT 'Создание таблиц завершено успешно!';
PRINT '========================================';
GO

-- Проверка созданных таблиц
SELECT t.name AS TableName, 
       c.name AS ColumnName,
       ty.name AS DataType,
       c.is_nullable AS IsNullable
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE t.name IN ('Counterparties', 'Assets', 'Contracts', 'ContractSpecifications', 'PaymentSchedules')
ORDER BY t.name, c.column_id;
GO