@echo off
echo Тестирование подключения к базе данных...
echo.

echo 1. Проверка наличия SQL Server...
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT @@VERSION" > nul 2>&1
if %errorlevel% equ 0 (
    echo ✓ SQL Server LocalDB доступен
) else (
    echo ✗ SQL Server LocalDB не доступен
    echo Установите SQL Server Express или LocalDB
    pause
    exit /b 1
)

echo.
echo 2. Проверка базы данных LeasingSystemDb...
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "USE LeasingSystemDb; SELECT DB_NAME()" > nul 2>&1
if %errorlevel% equ 0 (
    echo ✓ База данных LeasingSystemDb существует
) else (
    echo ✗ База данных LeasingSystemDb не существует
    echo Создайте базу данных с помощью скриптов в папке Scripts
)

echo.
echo 3. Проверка таблиц...
sqlcmd -S "(localdb)\MSSQLLocalDB" -d LeasingSystemDb -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" 2> nul
if %errorlevel% equ 0 (
    echo ✓ Таблицы существуют
) else (
    echo ✗ Ошибка при проверке таблиц
)

echo.
echo 4. Проверка Entity Framework...
echo Откройте проект в Visual Studio и выполните:
echo   1. Build -> Build Solution (Ctrl+Shift+B)
echo   2. Если есть ошибки, установите пакет EntityFramework через NuGet
echo   3. Запустите приложение (F5)
echo.

echo 5. Проверка строки подключения...
echo Откройте файл App.config и убедитесь, что строка подключения:
echo   Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LeasingSystemDb;...
echo.

pause