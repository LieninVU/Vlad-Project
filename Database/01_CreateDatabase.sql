-- ============================================
-- Скрипт создания базы данных LeasingSystem
-- Версия: 2.0 (с кириллической сортировкой)
-- ============================================

USE master;
GO

-- Проверяем существование базы данных
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'LeasingSystem')
BEGIN
    -- Принудительно закрываем соединения и удаляем базу
    ALTER DATABASE LeasingSystem SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LeasingSystem;
    PRINT 'База данных LeasingSystem удалена.';
END
GO

-- Создаём базу данных с кириллической сортировкой для поддержки русских символов
CREATE DATABASE LeasingSystem 
    COLLATE Cyrillic_General_CI_AS;
GO

PRINT 'База данных LeasingSystem создана успешно.';
GO

-- Устанавливаем базу данных по умолчанию
USE LeasingSystem;
GO

-- Настраиваем параметры базы данных (только необходимые)
ALTER DATABASE LeasingSystem SET RECOVERY SIMPLE;
ALTER DATABASE LeasingSystem SET ANSI_NULLS ON;
ALTER DATABASE LeasingSystem SET ANSI_PADDING ON;
ALTER DATABASE LeasingSystem SET ANSI_WARNINGS ON;
ALTER DATABASE LeasingSystem SET ARITHABORT ON;
ALTER DATABASE LeasingSystem SET CONCAT_NULL_YIELDS_NULL ON;
ALTER DATABASE LeasingSystem SET QUOTED_IDENTIFIER ON;
ALTER DATABASE LeasingSystem SET NUMERIC_ROUNDABORT OFF;
GO

PRINT 'Параметры базы данных настроены.';
GO
