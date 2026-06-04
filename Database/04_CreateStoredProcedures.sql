-- ============================================
-- Скрипт создания хранимых процедур (исправленный)
-- ============================================
USE LeasingSystem;
GO

PRINT 'Начинаем создание хранимых процедур...';
GO

-- 1. Проверка доступности актива
CREATE OR ALTER PROCEDURE sp_CheckAssetAvailability
    @AssetId INT,
    @StartDate DATE,
    @EndDate DATE,
    @ExcludeContractId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @IsAvailable BIT = 1;
    
    IF EXISTS (
        SELECT 1 
        FROM ContractSpecifications cs
        INNER JOIN Contracts c ON cs.ContractId = c.Id
        WHERE cs.AssetId = @AssetId
            AND c.Id != ISNULL(@ExcludeContractId, -1)
            AND c.ContractStatus IN (1, 2)
            AND @StartDate < ISNULL(c.EndDate, '9999-12-31')
            AND @EndDate > c.StartDate
    ) SET @IsAvailable = 0;
    
    IF NOT EXISTS (
        SELECT 1 FROM Assets 
        WHERE Id = @AssetId AND IsAvailable = 1 AND AssetCondition IN (0, 1, 2)
    ) SET @IsAvailable = 0;
    
    SELECT @IsAvailable AS IsAvailable;
END
GO
PRINT 'sp_CheckAssetAvailability создана.';
GO

-- 2. Генерация номера договора
CREATE OR ALTER PROCEDURE sp_GenerateContractNumber
    @ContractType TINYINT,
    @Year INT = NULL,
    @ContractNumber NVARCHAR(30) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @Year IS NULL SET @Year = YEAR(GETDATE());
    DECLARE @Prefix NVARCHAR(2) = CASE WHEN @ContractType = 0 THEN 'AR' ELSE 'LS' END;
    DECLARE @NextNumber INT;
    
    SELECT @NextNumber = ISNULL(MAX(CAST(SUBSTRING(ContractNumber, 8, 3) AS INT)), 0) + 1
    FROM Contracts
    WHERE ContractNumber LIKE @Prefix + '-' + CAST(@Year AS NVARCHAR) + '-%';
    
    SET @ContractNumber = @Prefix + '-' + CAST(@Year AS NVARCHAR) + '-' + RIGHT('000' + CAST(@NextNumber AS NVARCHAR), 3);
END
GO
PRINT 'sp_GenerateContractNumber создана.';
GO

-- 3. Создание договора с графиком платежей
CREATE OR ALTER PROCEDURE sp_CreateContractWithPayments
    @ContractType TINYINT,
    @CounterpartyId INT,
    @SignedDate DATE,
    @StartDate DATE,
    @EndDate DATE,
    @TotalAmount DECIMAL(12,2),
    @PaymentTerms NVARCHAR(500) = NULL,
    @Notes NVARCHAR(2000) = NULL,
    @PaymentScheduleType TINYINT = 1,
    @NewContractId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @ContractNumber NVARCHAR(30);
        DECLARE @CurrentYear INT = YEAR(@SignedDate);
        EXEC sp_GenerateContractNumber @ContractType, @CurrentYear, @ContractNumber OUTPUT;
        
        INSERT INTO Contracts (ContractNumber, ContractType, ContractStatus, CounterpartyId, 
                              SignedDate, StartDate, EndDate, TotalAmount, PaymentTerms, Notes)
        VALUES (@ContractNumber, @ContractType, 0, @CounterpartyId,
                @SignedDate, @StartDate, @EndDate, @TotalAmount, @PaymentTerms, @Notes);
        
        SET @NewContractId = SCOPE_IDENTITY();
        
        IF @PaymentScheduleType = 0
        BEGIN
            -- Единовременный платёж
            INSERT INTO PaymentSchedules (ContractId, PaymentNumber, Description, DueDate, Amount)
            VALUES (@NewContractId, 1, 'Полная оплата', @StartDate, @TotalAmount);
        END
        ELSE IF @PaymentScheduleType = 1
        BEGIN
            -- Ежемесячные платежи с корректным учётом года
            -- Рассчитываем количество месяцев с учётом полных периодов
            DECLARE @CurrentDate DATE = @StartDate;
            DECLARE @PaymentCount INT = 0;
            
            -- Сначала считаем количество платежей
            WHILE @CurrentDate <= @EndDate
            BEGIN
                SET @PaymentCount = @PaymentCount + 1;
                SET @CurrentDate = DATEADD(MONTH, 1, @CurrentDate);
            END
            
            -- Если платежей нет, создаём хотя бы один
            IF @PaymentCount = 0 SET @PaymentCount = 1;
            
            DECLARE @BaseAmount DECIMAL(12,2) = FLOOR(@TotalAmount / @PaymentCount);
            DECLARE @Remainder DECIMAL(12,2) = @TotalAmount - (@BaseAmount * @PaymentCount);
            DECLARE @i INT = 1;
            SET @CurrentDate = @StartDate;
            
            WHILE @i <= @PaymentCount
            BEGIN
                DECLARE @Amount DECIMAL(12,2) = @BaseAmount + CASE WHEN @i = @PaymentCount THEN @Remainder ELSE 0 END;
                DECLARE @Description NVARCHAR(200) = 
                    CASE WHEN @i = 1 THEN 'Аванс'
                         WHEN @i = @PaymentCount THEN 'Окончательный платёж'
                         ELSE 'Ежемесячный платёж'
                    END;
                INSERT INTO PaymentSchedules (ContractId, PaymentNumber, Description, DueDate, Amount)
                VALUES (@NewContractId, @i, @Description, @CurrentDate, @Amount);
                SET @i = @i + 1;
                SET @CurrentDate = DATEADD(MONTH, 1, @CurrentDate);
            END
        END
        
        INSERT INTO AuditLogs (TableName, RecordId, ActionType, ChangedBy, ChangedAt)
        VALUES ('Contracts', @NewContractId, 'INSERT', SYSTEM_USER, GETDATE());
        
        COMMIT TRANSACTION;
        PRINT 'Договор ' + @ContractNumber + ' создан. ID: ' + CAST(@NewContractId AS NVARCHAR);
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO
PRINT 'sp_CreateContractWithPayments создана.';
GO

-- 4. Расчет дебиторской задолженности (без несуществующих столбцов)
CREATE OR ALTER PROCEDURE sp_CalculateReceivables
    @AsOfDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @AsOfDate IS NULL SET @AsOfDate = GETDATE();
    
    SELECT 
        cp.Id AS CounterpartyId,
        cp.Name AS CounterpartyName,
        cp.Inn,
        COUNT(DISTINCT c.Id) AS ActiveContractsCount,
        SUM(c.TotalAmount) AS TotalContractAmount,
        SUM(ISNULL(paid.TotalPaid, 0)) AS TotalPaidAmount,
        SUM(c.TotalAmount - ISNULL(paid.TotalPaid, 0)) AS Balance,
        SUM(overdue.OverdueAmount) AS OverdueAmount,
        MAX(overdue.DaysOverdue) AS MaxDaysOverdue
    FROM Counterparties cp
    INNER JOIN Contracts c ON cp.Id = c.CounterpartyId
    OUTER APPLY (
        SELECT SUM(ps.Amount) AS TotalPaid
        FROM PaymentSchedules ps
        WHERE ps.ContractId = c.Id AND ps.IsPaid = 1
    ) paid
    OUTER APPLY (
        SELECT 
            SUM(CASE WHEN ps.IsPaid = 0 AND ps.DueDate < @AsOfDate THEN ps.Amount ELSE 0 END) AS OverdueAmount,
            MAX(CASE WHEN ps.IsPaid = 0 AND ps.DueDate < @AsOfDate THEN DATEDIFF(DAY, ps.DueDate, @AsOfDate) ELSE 0 END) AS DaysOverdue
        FROM PaymentSchedules ps
        WHERE ps.ContractId = c.Id
    ) overdue
    WHERE cp.IsActive = 1 AND c.ContractStatus IN (1,2)
    GROUP BY cp.Id, cp.Name, cp.Inn
    HAVING SUM(c.TotalAmount - ISNULL(paid.TotalPaid, 0)) > 0
    ORDER BY Balance DESC;
END
GO
PRINT 'sp_CalculateReceivables создана.';
GO

-- 5. Отчет по загрузке техники (без изменений, он работал)
CREATE OR ALTER PROCEDURE sp_GetAssetUtilizationReport
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        a.Id, a.InventoryNumber, a.Name, a.AssetGroup,
        CASE a.AssetGroup WHEN 0 THEN 'Транспорт' WHEN 1 THEN 'Оборудование' END AS AssetGroupName,
        a.VehicleBrand, a.VehicleModel, a.IsAvailable,
        (SELECT COUNT(*) FROM ContractSpecifications cs
         INNER JOIN Contracts c ON cs.ContractId = c.Id
         WHERE cs.AssetId = a.Id AND c.StartDate <= @EndDate 
           AND ISNULL(c.EndDate, @EndDate) >= @StartDate AND c.ContractStatus IN (1,2,4)) AS RentalCount,
        ISNULL((SELECT SUM(DATEDIFF(DAY, 
                CASE WHEN c.StartDate < @StartDate THEN @StartDate ELSE c.StartDate END,
                CASE WHEN ISNULL(c.EndDate, @EndDate) > @EndDate THEN @EndDate ELSE ISNULL(c.EndDate, @EndDate) END))
                FROM ContractSpecifications cs
                INNER JOIN Contracts c ON cs.ContractId = c.Id
                WHERE cs.AssetId = a.Id AND c.StartDate <= @EndDate 
                  AND ISNULL(c.EndDate, @EndDate) >= @StartDate AND c.ContractStatus IN (1,2,4)), 0) AS BusyDays,
        DATEDIFF(DAY, @StartDate, @EndDate) + 1 AS TotalDays,
        CAST(ISNULL((SELECT SUM(DATEDIFF(DAY, 
                CASE WHEN c.StartDate < @StartDate THEN @StartDate ELSE c.StartDate END,
                CASE WHEN ISNULL(c.EndDate, @EndDate) > @EndDate THEN @EndDate ELSE ISNULL(c.EndDate, @EndDate) END)) * 100.0
                FROM ContractSpecifications cs
                INNER JOIN Contracts c ON cs.ContractId = c.Id
                WHERE cs.AssetId = a.Id AND c.StartDate <= @EndDate 
                  AND ISNULL(c.EndDate, @EndDate) >= @StartDate AND c.ContractStatus IN (1,2,4)), 0) 
                / (DATEDIFF(DAY, @StartDate, @EndDate) + 1) AS DECIMAL(5,2)) AS UtilizationPercentage,
        ISNULL((SELECT SUM(cs.UnitPrice * cs.Quantity * 
                DATEDIFF(DAY, 
                    CASE WHEN c.StartDate < @StartDate THEN @StartDate ELSE c.StartDate END,
                    CASE WHEN ISNULL(c.EndDate, @EndDate) > @EndDate THEN @EndDate ELSE ISNULL(c.EndDate, @EndDate) END) / 
                CASE cs.PeriodType
                    WHEN 0 THEN 1.0/24
                    WHEN 1 THEN 1.0
                    WHEN 2 THEN 1.0
                    WHEN 3 THEN 7.0
                    WHEN 4 THEN 30.0
                    ELSE 1.0
                END)
                FROM ContractSpecifications cs
                INNER JOIN Contracts c ON cs.ContractId = c.Id
                WHERE cs.AssetId = a.Id AND c.StartDate <= @EndDate 
                  AND ISNULL(c.EndDate, @EndDate) >= @StartDate AND c.ContractStatus IN (1,2,4)), 0) AS Revenue
    FROM Assets a
    WHERE a.IsAvailable = 1
    ORDER BY UtilizationPercentage DESC, Revenue DESC;
END
GO
PRINT 'sp_GetAssetUtilizationReport создана.';
GO

-- 6. Мягкое удаление контрагента
CREATE OR ALTER PROCEDURE sp_SoftDeleteCounterparty
    @CounterpartyId INT,
    @DeletedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM Contracts WHERE CounterpartyId = @CounterpartyId AND ContractStatus IN (1,2))
        BEGIN
            RAISERROR('Невозможно удалить контрагента с активными договорами.', 16, 1);
            RETURN;
        END
        UPDATE Counterparties SET IsActive = 0, UpdatedAt = GETDATE() WHERE Id = @CounterpartyId;
        INSERT INTO AuditLogs (TableName, RecordId, ActionType, ChangedBy, ChangedAt)
        VALUES ('Counterparties', @CounterpartyId, 'SOFT_DELETE', ISNULL(@DeletedBy, SYSTEM_USER), GETDATE());
        COMMIT TRANSACTION;
        PRINT 'Контрагент ID ' + CAST(@CounterpartyId AS NVARCHAR) + ' мягко удален.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO
PRINT 'sp_SoftDeleteCounterparty создана.';
GO

-- 7. Финансовый отчет
CREATE OR ALTER PROCEDURE sp_GetFinancialReport
    @StartDate DATE,
    @EndDate DATE,
    @ReportType TINYINT = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF @ReportType = 0
    BEGIN
        WITH ContractData AS (
            SELECT 
                c.Id, c.TotalAmount, c.ContractStatus,
                ISNULL(paid.TotalPaid, 0) AS PaidAmount,
                c.TotalAmount - ISNULL(paid.TotalPaid, 0) AS Balance
            FROM Contracts c
            OUTER APPLY (SELECT SUM(ps.Amount) AS TotalPaid FROM PaymentSchedules ps WHERE ps.ContractId = c.Id AND ps.IsPaid = 1) paid
            WHERE c.StartDate BETWEEN @StartDate AND @EndDate
        )
        SELECT 'Всего договоров' AS Category,
               COUNT(*) AS Count,
               SUM(TotalAmount) AS TotalAmount,
               SUM(PaidAmount) AS PaidAmount,
               SUM(Balance) AS Balance
        FROM ContractData
        UNION ALL
        SELECT 'Активные договоры',
               COUNT(*),
               SUM(TotalAmount),
               SUM(PaidAmount),
               SUM(Balance)
        FROM ContractData
        WHERE ContractStatus IN (1,2)
        UNION ALL
        SELECT 'Просроченные платежи',
               COUNT(*),
               SUM(Amount),
               0,
               SUM(Amount)
        FROM vw_PaymentOverdue
        WHERE DueDate BETWEEN @StartDate AND @EndDate;
    END
    ELSE IF @ReportType = 1
    BEGIN
        SELECT 
            CASE c.ContractType WHEN 0 THEN 'Аренда' WHEN 1 THEN 'Лизинг' END AS ContractType,
            COUNT(*) AS ContractCount,
            SUM(c.TotalAmount) AS TotalAmount,
            SUM(ISNULL(paid.TotalPaid, 0)) AS PaidAmount,
            SUM(c.TotalAmount - ISNULL(paid.TotalPaid, 0)) AS Balance,
            AVG(DATEDIFF(DAY, c.StartDate, ISNULL(c.EndDate, GETDATE()))) AS AvgDurationDays
        FROM Contracts c
        OUTER APPLY (SELECT SUM(ps.Amount) AS TotalPaid FROM PaymentSchedules ps WHERE ps.ContractId = c.Id AND ps.IsPaid = 1) paid
        WHERE c.StartDate BETWEEN @StartDate AND @EndDate
        GROUP BY c.ContractType
        ORDER BY TotalAmount DESC;
    END
    ELSE IF @ReportType = 2
    BEGIN
        SELECT 
            cp.Name AS CounterpartyName,
            cp.Inn,
            COUNT(c.Id) AS ContractCount,
            SUM(c.TotalAmount) AS TotalAmount,
            SUM(ISNULL(paid.TotalPaid, 0)) AS PaidAmount,
            SUM(c.TotalAmount - ISNULL(paid.TotalPaid, 0)) AS Balance,
            MAX(c.StartDate) AS LastContractDate
        FROM Counterparties cp
        INNER JOIN Contracts c ON cp.Id = c.CounterpartyId
        OUTER APPLY (SELECT SUM(ps.Amount) AS TotalPaid FROM PaymentSchedules ps WHERE ps.ContractId = c.Id AND ps.IsPaid = 1) paid
        WHERE c.StartDate BETWEEN @StartDate AND @EndDate AND cp.IsActive = 1
        GROUP BY cp.Name, cp.Inn, cp.Id
        HAVING SUM(c.TotalAmount) > 0
        ORDER BY TotalAmount DESC;
    END
END
GO
PRINT 'sp_GetFinancialReport создана.';
GO

PRINT '============================================';
PRINT 'ВСЕ ХРАНИМЫЕ ПРОЦЕДУРЫ СОЗДАНЫ УСПЕШНО!';
PRINT '============================================';
GO