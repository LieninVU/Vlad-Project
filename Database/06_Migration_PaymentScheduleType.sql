-- ============================================
-- Скрипт миграции: Добавление поля PaymentScheduleType в Contracts
-- и MonthlyRate в Assets
-- Версия: 1.2
-- ============================================

USE LeasingSystem;
GO

PRINT 'Начинаем миграцию...';
GO

-- Добавляем поле MonthlyRate, если его нет
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Assets') AND name = 'MonthlyRate')
BEGIN
    ALTER TABLE Assets ADD MonthlyRate DECIMAL(12,2) NULL;
    PRINT 'Поле MonthlyRate добавлено в таблицу Assets.';
END
ELSE
BEGIN
    PRINT 'Поле MonthlyRate уже существует.';
END
GO

-- Добавляем поле PaymentScheduleType в Contracts, если его нет
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'PaymentScheduleType')
BEGIN
    ALTER TABLE Contracts ADD PaymentScheduleType TINYINT NULL DEFAULT 1;
    PRINT 'Поле PaymentScheduleType добавлено в таблицу Contracts.';
END
ELSE
BEGIN
    PRINT 'Поле PaymentScheduleType уже существует.';
END
GO

-- Обновляем существующие записи: устанавливаем PaymentScheduleType = 1 (Monthly) по умолчанию
UPDATE Contracts SET PaymentScheduleType = 1 WHERE PaymentScheduleType IS NULL;
GO

-- Добавляем CHECK-ограничение для PaymentScheduleType
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Contracts_PaymentScheduleType')
BEGIN
    ALTER TABLE Contracts ADD CONSTRAINT CK_Contracts_PaymentScheduleType 
        CHECK (PaymentScheduleType BETWEEN 0 AND 2);
    PRINT 'Ограничение CK_Contracts_PaymentScheduleType добавлено.';
END
ELSE
BEGIN
    PRINT 'Ограничение CK_Contracts_PaymentScheduleType уже существует.';
END
GO

-- Изменяем столбец на NOT NULL, если он ещё nullable
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Contracts') AND name = 'PaymentScheduleType' AND is_nullable = 1)
BEGIN
    ALTER TABLE Contracts ALTER COLUMN PaymentScheduleType TINYINT NOT NULL;
    PRINT 'Поле PaymentScheduleType изменено на NOT NULL.';
END
GO

PRINT '============================================';
PRINT 'МИГРАЦИЯ PaymentScheduleType ЗАВЕРШЕНА УСПЕШНО!';
PRINT '============================================';
GO