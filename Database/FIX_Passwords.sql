-- ============================================
-- Скрипт для обновления хэшей паролей в существующей базе
-- Хэш вычисляется как SHA256(username + salt + password)
-- ============================================

USE LeasingSystem;
GO

PRINT 'Обновление хэшей паролей пользователей...';
GO

-- Обновляем пароль для admin (admin)
UPDATE Users 
SET 
    PasswordHash = '771B3793E1EBA37D874595917A11D5BC4894550C755742C78064210A35A9B747',
    PasswordSalt = '7777'
WHERE UserName = 'admin';

-- Обновляем пароль для manager (manager)  
UPDATE Users 
SET 
    PasswordHash = '2438799D4B0D76B8C95B7C507AFA810AA112E212A1D2D3EBC894919EA569BC3F',
    PasswordSalt = '7777'
WHERE UserName = 'manager';

-- Обновляем пароль для accountant (accountant)
UPDATE Users 
SET 
    PasswordHash = '98F8E8D00F424DED7BE94409B7D5B79BF1E56011EE01E97BC84385B143AADF19',
    PasswordSalt = '7777'
WHERE UserName = 'accountant';

-- Обновляем пароль для readonly (readonly)
UPDATE Users 
SET 
    PasswordHash = 'B856A5A1A2ECE8C24E831AAB9F65CD3D442ABC111E1577F1A68A8D861DB8F731',
    PasswordSalt = '7777'
WHERE UserName = 'readonly';

PRINT 'Хэши паролей обновлены.';
PRINT 'Пользователи:';
PRINT '  - admin / admin';
PRINT '  - manager / manager';
PRINT '  - accountant / accountant';
PRINT '  - readonly / readonly';
GO

-- Проверяем пользователей
SELECT Id, UserName, Role, IsActive, IsLocked FROM Users;
GO
