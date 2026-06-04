-- ============================================
-- Скрипт для обновления паролей пользователей
-- Используйте этот скрипт, если нужно сменить пароли вручную
-- ============================================

USE LeasingSystem;
GO

PRINT 'Обновление паролей пользователей...';
GO

-- Обновляем пароль для admin
UPDATE Users 
SET 
    PasswordHash = '5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8',
    PasswordSalt = '7777'
WHERE UserName = 'admin';

-- Обновляем пароль для manager  
UPDATE Users 
SET 
    PasswordHash = '5F4DCC3B5AA765D61D8327DEB882CF99',
    PasswordSalt = '7777'
WHERE UserName = 'manager';

-- Обновляем пароль для accountant
UPDATE Users 
SET 
    PasswordHash = '884E1436F44943A0C0C8D85368415822',
    PasswordSalt = '7777'
WHERE UserName = 'accountant';

-- Обновляем пароль для readonly
UPDATE Users 
SET 
    PasswordHash = '6C40404D5735415ABD3678AF670100D6',
    PasswordSalt = '7777'
WHERE UserName = 'readonly';

PRINT 'Пароли обновлены.';
PRINT 'Пользователи:';
PRINT '  - admin / admin';
PRINT '  - manager / manager';
PRINT '  - accountant / accountant';
PRINT '  - readonly / readonly';
GO

-- Проверяем пользователей
SELECT Id, UserName, Role FROM Users;
GO
