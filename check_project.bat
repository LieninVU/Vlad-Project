@echo off
echo ============================================
echo Проверка проекта WPF для учёта договоров
echo ============================================
echo.

echo 1. Проверка структуры проекта:
echo    - Модели данных: %cd%\Models\
echo    - ViewModels: %cd%\ViewModels\
echo    - Views: %cd%\Views\
echo.

echo 2. Основные файлы:
if exist "App.xaml" (
    echo    ✓ App.xaml найден
) else (
    echo    ✗ App.xaml не найден
)

if exist "Views\MainWindowSimple.xaml" (
    echo    ✓ MainWindowSimple.xaml найден
) else (
    echo    ✗ MainWindowSimple.xaml не найден
)

echo.
echo 3. Модели данных:
dir Models\*.cs /b
echo.

echo 4. Инструкция по запуску:
echo    Откройте ForVlad.csproj в Visual Studio 2022
echo    Убедитесь, что выбран .NET Framework 4.7.2
echo    Нажмите F5 для запуска
echo.

echo 5. Для устранения ошибок:
echo    - Используйте MainWindowSimple.xaml (упрощенная версия)
echo    - Убедитесь, что нет конфликта точек входа
echo.

pause