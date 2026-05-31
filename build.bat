@echo off
echo Проверка структуры проекта...
echo.

echo Файлы проекта:
dir /b *.csproj
echo.

echo Модели:
dir /b Models\*.cs
echo.

echo ViewModels:
dir /b ViewModels\*.cs
echo.

echo Views:
dir /b Views\*.xaml
echo.

echo App.xaml:
type App.xaml
echo.

echo Для компиляции откройте проект в Visual Studio 2022
echo или используйте команду:
echo MSBuild ForVlad.csproj /p:Configuration=Debug /p:Platform="Any CPU"
echo.
pause