@echo off
echo ========================================
echo ДИАГНОСТИКА ПОДКЛЮЧЕНИЯ К SQL SERVER
echo ========================================
echo.

echo 1. Проверка файла App.config:
if exist "App.config" (
    echo    [OK] App.config найден
    findstr /i "connectionStrings" App.config > nul
    if %errorlevel% equ 0 (
        echo    [OK] Секция connectionStrings найдена
    ) else (
        echo    [ERROR] Секция connectionStrings НЕ найдена!
    )
) else (
    echo    [ERROR] App.config НЕ найден!
)
echo.

echo 2. Проверка скомпилированного файла конфигурации:
if exist "bin\Debug\ForVlad.exe.config" (
    echo    [OK] ForVlad.exe.config найден
    echo    Содержимое connectionStrings:
    findstr /i /c:"connectionStrings" /c:"add name" "bin\Debug\ForVlad.exe.config"
) else (
    echo    [ERROR] ForVlad.exe.config НЕ найден!
    echo    Нужно скомпилировать проект в Visual Studio
)
echo.

echo 3. Проверка доступности SQL Server Express:
echo    Пробуем подключиться через sqlcmd...
sqlcmd -S "(local)\SQLEXPRESS" -E -Q "SELECT @@VERSION" -h -1
if %errorlevel% equ 0 (
    echo    [OK] SQL Server Express доступен
) else (
    echo    [ERROR] SQL Server Express НЕ доступен
    echo    Возможные причины:
    echo    - Служба SQL Server не запущена
    echo    - Неверное имя сервера
    echo    - SQL Server Express не установлен
)
echo.

echo 4. Список баз данных:
sqlcmd -S "(local)\SQLEXPRESS" -E -Q "SELECT name FROM sys.databases" -h -1
echo.

echo 5. Проверка наличия базы LeasingSystem:
sqlcmd -S "(local)\SQLEXPRESS" -E -Q "SELECT name FROM sys.databases WHERE name = 'LeasingSystem'" -h -1
if %errorlevel% equ 0 (
    echo    [OK] База LeasingSystem существует
) else (
    echo    [WARNING] База LeasingSystem НЕ найдена
    echo    Нужно создать базу данных
)
echo.

echo ========================================
echo РЕКОМЕНДАЦИИ:
echo.
echo 1. Убедитесь, что файл bin\Debug\ForVlad.exe.config существует
echo 2. Проверьте, что в нём есть секция connectionStrings
echo 3. Убедитесь, что SQL Server Express запущен
echo 4. Запустите приложение и вызовите ConnectionTester.TestAllConnectionMethods()
echo ========================================

pause