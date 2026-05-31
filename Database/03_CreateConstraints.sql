-- ============================================
-- Скрипт создания ограничений и дополнительных индексов (идемпотентный)
-- ============================================
USE LeasingSystem;
GO

PRINT 'Начинаем создание ограничений...';
GO

-- Вспомогательная процедура для создания ограничений, если они не существуют
-- (Можно обойтись без неё, но для удобства оставим как есть)

-- Проверка и создание ограничений для Counterparties
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Counterparties_Inn_Length')
    ALTER TABLE Counterparties ADD CONSTRAINT CK_Counterparties_Inn_Length CHECK (LEN(Inn) BETWEEN 10 AND 12);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Counterparties_Kpp_Length')
    ALTER TABLE Counterparties ADD CONSTRAINT CK_Counterparties_Kpp_Length CHECK (Kpp IS NULL OR LEN(Kpp) = 9);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Counterparties_Email')
    ALTER TABLE Counterparties ADD CONSTRAINT CK_Counterparties_Email CHECK (Email IS NULL OR Email LIKE '%@%.%');
GO

PRINT 'Ограничения для Counterparties обработаны.';
GO

-- Проверочные ограничения для Assets
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Assets_InventoryNumber_NotEmpty')
    ALTER TABLE Assets ADD CONSTRAINT CK_Assets_InventoryNumber_NotEmpty CHECK (LEN(InventoryNumber) > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Assets_HourlyRate_Positive')
    ALTER TABLE Assets ADD CONSTRAINT CK_Assets_HourlyRate_Positive CHECK (HourlyRate > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Assets_DailyRate_Positive')
    ALTER TABLE Assets ADD CONSTRAINT CK_Assets_DailyRate_Positive CHECK (DailyRate > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Assets_ManufactureYear')
    ALTER TABLE Assets ADD CONSTRAINT CK_Assets_ManufactureYear CHECK (ManufactureYear IS NULL OR (ManufactureYear BETWEEN 1900 AND YEAR(GETDATE())));
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Assets_EnginePower_Positive')
    ALTER TABLE Assets ADD CONSTRAINT CK_Assets_EnginePower_Positive CHECK (EnginePower IS NULL OR EnginePower > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Assets_Weight_Positive')
    ALTER TABLE Assets ADD CONSTRAINT CK_Assets_Weight_Positive CHECK (Weight IS NULL OR Weight > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Assets_VehicleFields')
    ALTER TABLE Assets ADD CONSTRAINT CK_Assets_VehicleFields CHECK (
        (AssetGroup = 0 AND VehicleBrand IS NOT NULL) OR 
        (AssetGroup = 1 AND VehicleBrand IS NULL)
    );
GO

PRINT 'Ограничения для Assets обработаны.';
GO

-- Проверочные ограничения для Contracts
-- Убрано строгое ограничение на формат номера договора, так как используются разные префиксы (АР, ЛЗ, ЧЕР)
-- IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Contracts_ContractNumber_Format')
--     ALTER TABLE Contracts ADD CONSTRAINT CK_Contracts_ContractNumber_Format CHECK (ContractNumber LIKE 'AR-[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9]' OR ContractNumber LIKE 'LS-[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9]');
-- GO

-- Вместо этого добавляем более гибкое ограничение: номер не должен быть пустым
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Contracts_ContractNumber_NotEmpty')
    ALTER TABLE Contracts ADD CONSTRAINT CK_Contracts_ContractNumber_NotEmpty CHECK (LEN(ContractNumber) > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Contracts_Dates')
    ALTER TABLE Contracts ADD CONSTRAINT CK_Contracts_Dates CHECK (StartDate >= SignedDate AND (EndDate IS NULL OR EndDate > StartDate));
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Contracts_TotalAmount_Positive')
    ALTER TABLE Contracts ADD CONSTRAINT CK_Contracts_TotalAmount_Positive CHECK (TotalAmount > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Contracts_SignedDate_NotFuture')
    ALTER TABLE Contracts ADD CONSTRAINT CK_Contracts_SignedDate_NotFuture CHECK (SignedDate <= GETDATE());
GO

PRINT 'Ограничения для Contracts обработаны.';
GO

-- ContractSpecifications
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_ContractSpecifications_Quantity_Positive')
    ALTER TABLE ContractSpecifications ADD CONSTRAINT CK_ContractSpecifications_Quantity_Positive CHECK (Quantity > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_ContractSpecifications_UnitPrice_Positive')
    ALTER TABLE ContractSpecifications ADD CONSTRAINT CK_ContractSpecifications_UnitPrice_Positive CHECK (UnitPrice > 0);
GO

PRINT 'Ограничения для ContractSpecifications обработаны.';
GO

-- PaymentSchedules
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_PaymentSchedules_Amount_Positive')
    ALTER TABLE PaymentSchedules ADD CONSTRAINT CK_PaymentSchedules_Amount_Positive CHECK (Amount > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_PaymentSchedules_PaymentNumber_Positive')
    ALTER TABLE PaymentSchedules ADD CONSTRAINT CK_PaymentSchedules_PaymentNumber_Positive CHECK (PaymentNumber > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_PaymentSchedules_PaidDate_Logic')
    ALTER TABLE PaymentSchedules ADD CONSTRAINT CK_PaymentSchedules_PaidDate_Logic CHECK ((IsPaid = 1 AND PaidDate IS NOT NULL) OR (IsPaid = 0 AND PaidDate IS NULL));
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_PaymentSchedules_PaidDate_NotFuture')
    ALTER TABLE PaymentSchedules ADD CONSTRAINT CK_PaymentSchedules_PaidDate_NotFuture CHECK (PaidDate IS NULL OR PaidDate <= GETDATE());
GO

PRINT 'Ограничения для PaymentSchedules обработаны.';
GO

PRINT 'Все ограничения добавлены (или уже существовали).';
GO

-- Удаляем старые вычисляемые столбцы, если они были добавлены ранее (чтобы избежать конфликтов)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'PaidAmount')
    ALTER TABLE Contracts DROP COLUMN PaidAmount;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'Balance')
    ALTER TABLE Contracts DROP COLUMN Balance;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PaymentSchedules') AND name = 'IsOverdue')
    ALTER TABLE PaymentSchedules DROP COLUMN IsOverdue;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('PaymentSchedules') AND name = 'DaysOverdue')
    ALTER TABLE PaymentSchedules DROP COLUMN DaysOverdue;
GO

PRINT 'Старые вычисляемые столбцы удалены (если существовали).';
GO

-- Создаём дополнительные индексы (исправленный блок)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Contracts_Status_Dates' AND object_id = OBJECT_ID('Contracts'))
    CREATE INDEX IX_Contracts_Status_Dates ON Contracts(ContractStatus, StartDate, EndDate)
    INCLUDE (ContractNumber, CounterpartyId, TotalAmount);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PaymentSchedules_Overdue' AND object_id = OBJECT_ID('PaymentSchedules'))
    CREATE INDEX IX_PaymentSchedules_Overdue ON PaymentSchedules(IsPaid, DueDate)
    INCLUDE (ContractId, Amount);
-- Убрана проблемная фильтрация с GETDATE()
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Assets_Availability' AND object_id = OBJECT_ID('Assets'))
    CREATE INDEX IX_Assets_Availability ON Assets(AssetGroup, IsAvailable, AssetCondition)
    INCLUDE (Name, HourlyRate, DailyRate);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Counterparties_Active' AND object_id = OBJECT_ID('Counterparties'))
    CREATE INDEX IX_Counterparties_Active ON Counterparties(IsActive, CounterpartyType)
    INCLUDE (Name, Inn, Phone, Email);
GO
-- Обновляем представления (удаляем старые и создаём заново)
DROP VIEW IF EXISTS vw_ActiveContracts;
DROP VIEW IF EXISTS vw_AssetUtilization;
DROP VIEW IF EXISTS vw_PaymentOverdue;
GO

CREATE VIEW vw_ActiveContracts AS
SELECT 
    c.Id,
    c.ContractNumber,
    c.ContractType,
    c.ContractStatus,
    cp.Name AS CounterpartyName,
    cp.Inn AS CounterpartyInn,
    c.SignedDate,
    c.StartDate,
    c.EndDate,
    c.TotalAmount,
    ISNULL((
        SELECT SUM(Amount)
        FROM PaymentSchedules ps
        WHERE ps.ContractId = c.Id AND ps.IsPaid = 1
    ), 0) AS PaidAmount,
    c.TotalAmount - ISNULL((
        SELECT SUM(Amount)
        FROM PaymentSchedules ps
        WHERE ps.ContractId = c.Id AND ps.IsPaid = 1
    ), 0) AS Balance,
    CASE 
        WHEN c.EndDate < GETDATE() THEN 1 
        ELSE 0 
    END AS IsExpired,
    (SELECT COUNT(*) FROM PaymentSchedules ps WHERE ps.ContractId = c.Id AND ps.IsPaid = 0 AND ps.DueDate < GETDATE()) AS OverduePaymentsCount
FROM Contracts c
INNER JOIN Counterparties cp ON c.CounterpartyId = cp.Id
WHERE c.ContractStatus IN (1, 2); -- Signed, Active
GO

CREATE VIEW vw_AssetUtilization AS
SELECT 
    a.Id,
    a.InventoryNumber,
    a.Name,
    a.AssetGroup,
    a.VehicleBrand,
    a.VehicleModel,
    a.IsAvailable,
    a.HourlyRate,
    a.DailyRate,
    (SELECT COUNT(*) FROM ContractSpecifications cs WHERE cs.AssetId = a.Id) AS TotalRentals,
    (SELECT ISNULL(SUM(cs.Quantity), 0) FROM ContractSpecifications cs WHERE cs.AssetId = a.Id) AS TotalUnitsRented,
    (SELECT ISNULL(SUM(cs.Quantity * cs.UnitPrice), 0) FROM ContractSpecifications cs WHERE cs.AssetId = a.Id) AS TotalRevenue, -- Исправлено: TotalPrice не было, считаем как Quantity * UnitPrice
    (SELECT MAX(c.EndDate) FROM Contracts c 
     INNER JOIN ContractSpecifications cs ON c.Id = cs.ContractId 
     WHERE cs.AssetId = a.Id AND c.ContractStatus IN (1, 2)) AS LastRentalDate
FROM Assets a;
GO

CREATE VIEW vw_PaymentOverdue AS
SELECT 
    ps.Id,
    ps.ContractId,
    c.ContractNumber,
    cp.Name AS CounterpartyName,
    ps.PaymentNumber,
    ps.Description,
    ps.DueDate,
    ps.Amount,
    DATEDIFF(DAY, ps.DueDate, GETDATE()) AS DaysOverdue,
    c.TotalAmount - ISNULL((
        SELECT SUM(Amount)
        FROM PaymentSchedules ps2
        WHERE ps2.ContractId = c.Id AND ps2.IsPaid = 1
    ), 0) AS ContractBalance
FROM PaymentSchedules ps
INNER JOIN Contracts c ON ps.ContractId = c.Id
INNER JOIN Counterparties cp ON c.CounterpartyId = cp.Id
WHERE ps.IsPaid = 0 
    AND ps.DueDate < GETDATE()
    AND c.ContractStatus IN (1, 2) -- Signed, Active
-- ORDER BY перенесён в запрос при использовании представления (в представлении ORDER BY не допускается)
-- Но мы можем добавить TOP 100 PERCENT с ORDER BY для совместимости (хоть и не рекомендуется)
-- Однако лучше упорядочивать при SELECT из представления. Оставим без ORDER BY.
;
GO

PRINT 'Представления для отчетности созданы.';
GO

PRINT 'Скрипт выполнен успешно.';