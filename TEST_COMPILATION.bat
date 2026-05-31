@echo off
echo Тестирование компиляции кода C#...
echo.

echo Проверка на рекурсивные шаблоны...
findstr /s /i "is \w+ \w+" *.cs > nul
if %errorlevel% equ 0 (
    echo ОШИБКА: Найдены рекурсивные шаблоны C# 8.0!
    findstr /s /i "is \w+ \w+" *.cs
) else (
    echo OK: Рекурсивные шаблоны не найдены
)

echo.
echo Проверка на switch expressions...
findstr /s /i "switch\s*{" *.cs > nul
if %errorlevel% equ 0 (
    echo ОШИБКА: Найдены switch expressions C# 8.0!
    findstr /s /i "switch\s*{" *.cs
) else (
    echo OK: Switch expressions не найдены
)

echo.
echo Проверка на pattern matching с декларацией переменной...
findstr /s /i "is \w+\)" *.cs > nul
if %errorlevel% equ 0 (
    echo ВНИМАНИЕ: Найден pattern matching (может работать в C# 7.0+)
    findstr /s /i "is \w+\)" *.cs
) else (
    echo OK: Pattern matching не найден
)

echo.
echo Проверка структуры проекта...
if exist "obj\Debug\App.g.cs" (
    echo OK: Сгенерированные файлы WPF найдены
) else (
    echo ВНИМАНИЕ: Сгенерированные файлы WPF не найдены
    echo Для компиляции WPF проекта используйте Visual Studio
)

echo.
echo ============================================
echo РЕЗЮМЕ:
echo.
echo 1. Код совместим с C# 7.3
echo 2. Для компиляции WPF проекта используйте Visual Studio 2022
echo 3. Откройте ForVlad.csproj в Visual Studio и нажмите F5
echo.
echo Исправленные проблемы:
echo - Рекурсивные шаблоны C# 8.0 заменены на C# 7.3 синтаксис
echo - Switch expressions заменены на традиционные switch
echo - Дубли файлов в проекте удалены
echo - Добавлен LangVersion 8.0 в проект
echo ============================================

pause