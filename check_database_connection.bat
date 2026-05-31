@echo off
:: Кодировка: Windows-1251 (Cyrillic)
chcp 1251 > nul
title Проверка подключения к LeasingSystem
color 0A

cls
echo ========================================================
echo     ПРОВЕРКА ПОДКЛЮЧЕНИЯ К БАЗЕ ДАННЫХ LEASINGSYSTEM
echo ========================================================
echo.

echo [1] Проверка файла App.config...
if exist "App.config" (
    echo     OK - App.config найден
    findstr /i "LeasingSystem" App.config > nul
    if %errorlevel% equ 0 (
        echo     OK - Строка подключения найдена
    ) else (
        echo     ERROR - Строка подключения НЕ найдена
    )
) else (
    echo     ERROR - App.config НЕ найден
)
echo.

echo [2] Проверка скомпилированного конфига...
if exist "bin\Debug\ForVlad.exe.config" (
    echo     OK - ForVlad.exe.config найден
) else (
    echo     ERROR - ForVlad.exe.config НЕ найден
    echo     -> Скомпилируйте проект в Visual Studio
)
echo.

echo [3] Проверка SQL Server Express...
sqlcmd -S "(local)\SQLEXPRESS" -E -Q "SELECT @@VERSION" -h -1 > nul 2>&1
if %errorlevel% equ 0 (
    echo     OK - SQL Server доступен
    sqlcmd -S "(local)\SQLEXPRESS" -E -Q "SELECT @@VERSION" -h -1 2> nul
) else (
    echo     ERROR - SQL Server НЕ доступен
    echo     -> Проверьте, запущена ли служба SQL Server
)
echo.

echo [4] Проверка базы LeasingSystem...
sqlcmd -S "(local)\SQLEXPRESS" -E -Q "IF DB_ID('LeasingSystem') IS NOT NULL SELECT 1 ELSE SELECT 0" -h -1 > nul 2>&1
if %errorlevel% equ 0 (
    echo     OK - База LeasingSystem существует
) else (
    echo     ERROR - База LeasingSystem НЕ найдена
    echo     -> Выполните SQL скрипты из папки Database
)
echo.

echo ========================================================
echo                    ИТОГОВЫЙ ОТЧЕТ
echo ========================================================
echo.
echo  Если все проверки OK - подключение настроено правильно!
echo  Если есть ERROR - выполните рекомендации выше.
echo.
echo  Советы:
echo   1. Скомпилируйте проект (Build -> Build Solution)
echo   2. Запустите службу SQL Server: Win+R -> services.msc
   echo      Найдите SQL Server (SQLEXPRESS) и запустите
   echo   3. Выполните скрипты из Database\: 01_ -> 02_ -> 03_ -> 04_ -> 05_
echo.
echo ========================================================
echo.
pause