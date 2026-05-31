-- ============================================
-- Скрипт заполнения базы данных LeasingSystem тестовыми данными
-- Версия: 7.0 (с явно указанными ID, Unicode и полной очисткой)
-- ============================================

USE LeasingSystem;
GO

PRINT 'Начинаем заполнение тестовыми данными...';
GO

-- ============================================
-- Полная очистка таблиц
-- ============================================

PRINT 'Очистка таблиц от старых данных...';
GO

-- Удаляем данные в обратном порядке из-за внешних ключей
DELETE FROM PaymentSchedules;
GO

DELETE FROM ContractSpecifications;
GO

DELETE FROM Contracts;
GO

DELETE FROM Assets;
GO

DELETE FROM Counterparties;
GO

-- Сбрасываем идентификаторы на 0
DBCC CHECKIDENT ('Counterparties', RESEED, 0);
DBCC CHECKIDENT ('Assets', RESEED, 0);
DBCC CHECKIDENT ('Contracts', RESEED, 0);
DBCC CHECKIDENT ('ContractSpecifications', RESEED, 0);
DBCC CHECKIDENT ('PaymentSchedules', RESEED, 0);
GO

PRINT 'Таблицы очищены.';
GO

-- ============================================
-- Тестовые контрагенты (с явным указанием ID)
-- ============================================

SET IDENTITY_INSERT Counterparties ON;
GO

-- Контрагент 1: Юридическое лицо
INSERT INTO Counterparties (Id, Name, CounterpartyType, Inn, Kpp, LegalAddress, ActualAddress, ContactPerson, Phone, Email, Notes, IsActive, CreatedAt)
VALUES 
    (1, N'ООО "СтройТех"', 0, N'1234567890', N'123456789', 
     N'г. Москва, ул. Строителей, д. 1', N'г. Москва, ул. Строителей, д. 1', 
     N'Иванов Иван Иванович', N'+7 (999) 123-45-67', N'info@stroitech.ru', 
     N'Крупный застройщик', 1, GETDATE());
GO

-- Контрагент 2: Индивидуальный предприниматель
INSERT INTO Counterparties (Id, Name, CounterpartyType, Inn, Kpp, LegalAddress, ActualAddress, ContactPerson, Phone, Email, Notes, IsActive, CreatedAt)
VALUES 
    (2, N'ИП Петров П.П.', 1, N'0987654321', NULL, 
     N'г. Санкт-Петербург, пр. Невский, д. 100', N'г. Санкт-Петербург, пр. Невский, д. 100', 
     N'Петров Петр Петрович', N'+7 (911) 987-65-43', N'petrov@mail.ru', 
     N'Частный предприниматель', 1, GETDATE());
GO

-- Контрагент 3: Физическое лицо
INSERT INTO Counterparties (Id, Name, CounterpartyType, Inn, Kpp, LegalAddress, ActualAddress, ContactPerson, Phone, Email, Notes, IsActive, CreatedAt)
VALUES 
    (3, N'Сидоров Сидор Сидорович', 2, N'1122334455', NULL, 
     N'г. Казань, ул. Баумана, д. 50', N'г. Казань, ул. Баумана, д. 50', 
     N'Сидоров Сидор Сидорович', N'+7 (843) 222-33-44', N'sidorov@email.ru', 
     N'Физическое лицо', 1, GETDATE());
GO

SET IDENTITY_INSERT Counterparties OFF;
GO

PRINT 'Тестовые контрагенты добавлены.';
GO

-- ============================================
-- Тестовая техника (с явным указанием ID)
-- ============================================

SET IDENTITY_INSERT Assets ON;
GO

-- Техника 1: Экскаватор (Vehicle, AssetGroup = 0)
INSERT INTO Assets (Id, InventoryNumber, Name, AssetGroup, VehicleBrand, VehicleModel, VinNumber, ManufactureYear, 
                   EnginePower, RegistrationNumber, EquipmentType, Weight, PowerRequirements, 
                   VehicleSubcategory, EquipmentSubcategory, HourlyRate, DailyRate, AssetCondition, 
                   IsAvailable, Description, CreatedAt)
VALUES 
    (1, N'ТЕХ-001', N'Экскаватор-погрузчик JCB 3CX', 0, N'JCB', N'3CX', N'JCB3CX2024001', 2022,
     150.50, N'А123БВ45', NULL, 8500.00, N'Дизель', 
     0, NULL, 1500.00, 12000.00, 0, 1, 
     N'Экскаватор-погрузчик для строительных работ', GETDATE());
GO

-- Техника 2: Генератор (Equipment, AssetGroup = 1)
INSERT INTO Assets (Id, InventoryNumber, Name, AssetGroup, VehicleBrand, VehicleModel, VinNumber, ManufactureYear, 
                   EnginePower, RegistrationNumber, EquipmentType, Weight, PowerRequirements, 
                   VehicleSubcategory, EquipmentSubcategory, HourlyRate, DailyRate, AssetCondition, 
                   IsAvailable, Description, CreatedAt)
VALUES 
    (2, N'ОБОР-001', N'Генератор 100 кВт', 1, NULL, NULL, N'CUM1002023001', 2023,
     NULL, N'Б123ВН77', N'Генератор', 250.00, N'220В', 
     NULL, 2, 100.00, 3000.00, 0, 1, 
     N'Дизельный генератор мощностью 100 кВт', GETDATE());
GO

-- Техника 3: Бульдозер (Vehicle, AssetGroup = 0)
INSERT INTO Assets (Id, InventoryNumber, Name, AssetGroup, VehicleBrand, VehicleModel, VinNumber, ManufactureYear, 
                   EnginePower, RegistrationNumber, EquipmentType, Weight, PowerRequirements, 
                   VehicleSubcategory, EquipmentSubcategory, HourlyRate, DailyRate, AssetCondition, 
                   IsAvailable, Description, CreatedAt)
VALUES 
    (3, N'ТЕХ-002', N'Бульдозер D6', 0, N'Caterpillar', N'D6', N'CATD62020001', 2021,
     200.00, N'В234ГН56', NULL, 12000.00, N'Дизель', 
     0, NULL, 2000.00, 18000.00, 0, 1, 
     N'Тяжёлый бульдозер для земляных работ', GETDATE());
GO

SET IDENTITY_INSERT Assets OFF;
GO

PRINT 'Тестовая техника добавлена.';
GO

-- ============================================
-- Тестовые договоры (с явным указанием ID и латинскими префиксами)
-- ============================================

SET IDENTITY_INSERT Contracts ON;
GO

-- Договор 1: Аренда (AR = Rental, ContractType = 0, Status = 2 = Active)
INSERT INTO Contracts (Id, ContractNumber, ContractType, ContractStatus, CounterpartyId, 
                       SignedDate, StartDate, EndDate, TotalAmount, PaymentTerms, Notes, CreatedAt)
VALUES 
    (1, N'AR-2024-001', 0, 2, 1, 
     CAST(GETDATE() - 30 AS DATE), CAST(GETDATE() - 30 AS DATE), CAST(GETDATE() + 60 AS DATE),
     450000.00, N'Аванс 30%, остальное ежемесячно', N'Договор аренды экскаватора', GETDATE());
GO

-- Договор 2: Лизинг (LS = Leasing, ContractType = 1, Status = 1 = Signed)
INSERT INTO Contracts (Id, ContractNumber, ContractType, ContractStatus, CounterpartyId, 
                       SignedDate, StartDate, EndDate, TotalAmount, PaymentTerms, Notes, CreatedAt)
VALUES 
    (2, N'LS-2024-002', 1, 1, 2,
     CAST(GETDATE() - 10 AS DATE), CAST(GETDATE() - 5 AS DATE), CAST(GETDATE() + 355 AS DATE),
     360000.00, N'Ежемесячные платежи', N'Договор лизинга генератора', GETDATE());
GO

-- Договор 3: Черновик (DR = Draft, ContractType = 0, Status = 0 = Draft)
INSERT INTO Contracts (Id, ContractNumber, ContractType, ContractStatus, CounterpartyId, 
                       SignedDate, StartDate, EndDate, TotalAmount, PaymentTerms, Notes, CreatedAt)
VALUES 
    (3, N'DR-2024-003', 0, 0, 3,
     CAST(GETDATE() AS DATE), CAST(GETDATE() + 30 AS DATE), CAST(GETDATE() + 180 AS DATE),
     250000.00, N'Предварительный договор', N'Черновик договора аренды бульдозера', GETDATE());
GO

SET IDENTITY_INSERT Contracts OFF;
GO

PRINT 'Тестовые договоры добавлены.';
GO

-- ============================================
-- Тестовые спецификации договоров (с явным указанием ID)
-- ============================================

SET IDENTITY_INSERT ContractSpecifications ON;
GO

-- Спецификация для договора 1 (Аренда экскаватора, ContractId = 1, AssetId = 1)
INSERT INTO ContractSpecifications (Id, ContractId, AssetId, Quantity, UnitPrice, PeriodType, AdditionalConditions)
VALUES 
    (1, 1, 1, 1, 150000.00, 3, N'Аренда экскаватора на 3 месяца');
GO

-- Спецификация для договора 2 (Лизинг генератора, ContractId = 2, AssetId = 2)
INSERT INTO ContractSpecifications (Id, ContractId, AssetId, Quantity, UnitPrice, PeriodType, AdditionalConditions)
VALUES 
    (2, 2, 2, 1, 30000.00, 3, N'Лизинг генератора на 12 месяцев');
GO

-- Спецификация для договора 3 (Аренда бульдозера, ContractId = 3, AssetId = 3)
INSERT INTO ContractSpecifications (Id, ContractId, AssetId, Quantity, UnitPrice, PeriodType, AdditionalConditions)
VALUES 
    (3, 3, 3, 1, 80000.00, 3, N'Предварительная аренда бульдозера');
GO

SET IDENTITY_INSERT ContractSpecifications OFF;
GO

PRINT 'Тестовые спецификации договоров добавлены.';
GO

-- ============================================
-- Тестовые графики платежей (с явным указанием ID)
-- ============================================

SET IDENTITY_INSERT PaymentSchedules ON;
GO

-- Платежи для договора 1 (ContractId = 1)
INSERT INTO PaymentSchedules (Id, ContractId, PaymentNumber, Description, DueDate, Amount, IsPaid, PaidDate, PaymentMethod, PaymentReference)
VALUES 
    (1, 1, 1, N'Авансовый платёж', CAST(GETDATE() - 30 AS DATE), 100000.00, 1, CAST(GETDATE() - 30 AS DATE), 0, N'Оплата аванса');
GO

INSERT INTO PaymentSchedules (Id, ContractId, PaymentNumber, Description, DueDate, Amount, IsPaid, PaidDate, PaymentMethod, PaymentReference)
VALUES 
    (2, 1, 2, N'Ежемесячный платёж за июль', CAST(GETDATE() + 30 AS DATE), 180000.00, 0, NULL, 1, N'Ежемесячный платёж');
GO

INSERT INTO PaymentSchedules (Id, ContractId, PaymentNumber, Description, DueDate, Amount, IsPaid, PaidDate, PaymentMethod, PaymentReference)
VALUES 
    (3, 1, 3, N'Ежемесячный платёж за август', CAST(GETDATE() + 60 AS DATE), 180000.00, 0, NULL, 1, N'Ежемесячный платёж');
GO

-- Платежи для договора 2 (ContractId = 2)
INSERT INTO PaymentSchedules (Id, ContractId, PaymentNumber, Description, DueDate, Amount, IsPaid, PaidDate, PaymentMethod, PaymentReference)
VALUES 
    (4, 2, 1, N'Первый платёж', CAST(GETDATE() + 5 AS DATE), 30000.00, 0, NULL, 2, N'Безналичный расчёт');
GO

INSERT INTO PaymentSchedules (Id, ContractId, PaymentNumber, Description, DueDate, Amount, IsPaid, PaidDate, PaymentMethod, PaymentReference)
VALUES 
    (5, 2, 2, N'Второй платёж', CAST(GETDATE() + 35 AS DATE), 30000.00, 0, NULL, 2, N'Безналичный расчёт');
GO

-- Платежи для договора 3 (ContractId = 3)
INSERT INTO PaymentSchedules (Id, ContractId, PaymentNumber, Description, DueDate, Amount, IsPaid, PaidDate, PaymentMethod, PaymentReference)
VALUES 
    (6, 3, 1, N'Первый платёж по черновику', CAST(GETDATE() + 10 AS DATE), 50000.00, 0, NULL, 0, N'Предварительный платёж');
GO

SET IDENTITY_INSERT PaymentSchedules OFF;
GO

PRINT 'Тестовые графики платежей добавлены.';
GO

-- ============================================
-- Проверка данных
-- ============================================

PRINT '==============================================';
PRINT 'Проверка загруженных тестовых данных:';
PRINT '==============================================';
GO

SELECT COUNT(*) AS CounterpartiesCount FROM Counterparties;
GO

SELECT COUNT(*) AS AssetsCount FROM Assets;
GO

SELECT COUNT(*) AS ContractsCount FROM Contracts;
GO

SELECT COUNT(*) AS SpecificationsCount FROM ContractSpecifications;
GO

SELECT COUNT(*) AS PaymentsCount FROM PaymentSchedules;
GO

-- Выводим сами данные для проверки
PRINT '------------------------------------------------';
PRINT 'Данные контрагентов:';
PRINT '------------------------------------------------';
SELECT Id, Name, Inn FROM Counterparties;
GO

PRINT '------------------------------------------------';
PRINT 'Данные техники:';
PRINT '------------------------------------------------';
SELECT Id, Name, InventoryNumber FROM Assets;
GO

PRINT '------------------------------------------------';
PRINT 'Данные договоров:';
PRINT '------------------------------------------------';
SELECT Id, ContractNumber, ContractType, ContractStatus, CounterpartyId FROM Contracts;
GO

PRINT '------------------------------------------------';
PRINT 'Данные спецификаций:';
PRINT '------------------------------------------------';
SELECT Id, ContractId, AssetId, Quantity, UnitPrice FROM ContractSpecifications;
GO

PRINT '------------------------------------------------';
PRINT 'Данные платежей:';
PRINT '------------------------------------------------';
SELECT Id, ContractId, PaymentNumber, DueDate, Amount, IsPaid FROM PaymentSchedules;
GO

PRINT '==============================================';
PRINT 'ВСЕ ТЕСТОВЫЕ ДАННЫЕ ЗАГРУЖЕНЫ УСПЕШНО!';
PRINT '==============================================';
GO
