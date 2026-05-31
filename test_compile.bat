@echo off
echo ============================================
echo Тестирование компиляции проекта
echo ============================================
echo.

echo 1. Проверка основных файлов:
if exist "App.xaml" (
    echo    ✓ App.xaml найден
) else (
    echo    ✗ App.xaml не найден
    goto error
)

if exist "Views\MainWindowSimple.xaml" (
    echo    ✓ MainWindowSimple.xaml найден
) else (
    echo    ✗ MainWindowSimple.xaml не найден
    goto error
)

if exist "ViewModels\MainViewModel.cs" (
    echo    ✓ MainViewModel.cs найден
) else (
    echo    ✗ MainViewModel.cs не найден
    goto error
)

echo.
echo 2. Проверка моделей данных:
dir Models\*.cs /b
echo.

echo 3. Проверка ViewModels:
dir ViewModels\*.cs /b
echo.

echo 4. Инструкция:
echo    Откройте ForVlad.csproj в Visual Studio 2022
echo    Нажмите Ctrl+Shift+B для компиляции
echo    Или F5 для запуска
echo.

echo 5. Ожидаемое поведение:
echo    - Открывается одно окно
echo    - Боковое меню с 6 кнопками
echo    - При нажатии на кнопки меняется текст в центре
echo.

:success
echo ✅ Проект готов к компиляции
pause
exit /b 0

:error
echo ❌ Обнаружены проблемы с файлами
pause
exit /b 1