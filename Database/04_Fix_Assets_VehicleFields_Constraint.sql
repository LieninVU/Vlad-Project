-- ============================================
-- Исправление ограничения CK_Assets_VehicleFields для проверки VehicleBrand и VehicleModel
-- ============================================
USE LeasingSystem;
GO

PRINT 'Исправляем ограничение CK_Assets_VehicleFields...';
GO

-- Удаляем старое ограничение, если оно существует
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Assets_VehicleFields')
    ALTER TABLE Assets DROP CONSTRAINT CK_Assets_VehicleFields;
GO

-- Создаём новое ограничение, которое проверяет:
-- - Для AssetGroup = 0 (Vehicle): VehicleBrand и VehicleModel должны быть NOT NULL
-- - Для AssetGroup = 1 (Equipment): VehicleBrand и VehicleModel должны быть NULL
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Assets_VehicleFields')
    ALTER TABLE Assets ADD CONSTRAINT CK_Assets_VehicleFields CHECK (
        (AssetGroup = 0 AND VehicleBrand IS NOT NULL AND VehicleModel IS NOT NULL) OR 
        (AssetGroup = 1 AND VehicleBrand IS NULL AND VehicleModel IS NULL)
    );
GO

PRINT 'Ограничение CK_Assets_VehicleFields исправлено.';
GO

-- Дополнительные ограничения для дневной ставки
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Assets_DailyRate_Positive')
    ALTER TABLE Assets ADD CONSTRAINT CK_Assets_DailyRate_Positive CHECK (DailyRate > 0);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Assets_HourlyRate_Positive')
    ALTER TABLE Assets ADD CONSTRAINT CK_Assets_HourlyRate_Positive CHECK (HourlyRate > 0);
GO

PRINT 'Дополнительные ограничения для ставок добавлены.';
GO

-- Проверяем текущие данные на соответствие новому ограничению
PRINT 'Проверка данных на соответствие ограничению...';
GO

-- Для всех записей с AssetGroup = 0 (Vehicle), где VehicleBrand или VehicleModel IS NULL
-- Устанавливаем значения по умолчанию
UPDATE Assets 
SET VehicleBrand = 'Неизвестно', VehicleModel = 'Неизвестно'
WHERE AssetGroup = 0 AND (VehicleBrand IS NULL OR VehicleModel IS NULL);
GO

-- Для всех записей с AssetGroup = 1 (Equipment), где VehicleBrand или VehicleModel NOT NULL
-- Устанавливаем значения в NULL
UPDATE Assets 
SET VehicleBrand = NULL, VehicleModel = NULL
WHERE AssetGroup = 1 AND (VehicleBrand IS NOT NULL OR VehicleModel IS NOT NULL);
GO

PRINT 'Данные приведены в соответствие с новым ограничением.';
GO
PRINT 'Скрипт выполнен успешно!';
GO
