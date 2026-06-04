@echo off
chcp 65001 >nul

cd /d "%~dp0"

echo Очистка проекта...
msbuild ForVlad.csproj /t:Clean /p:Configuration=Debug /p:Platform="Any CPU" /v:quiet

echo Сборка проекта...
msbuild ForVlad.csproj /t:Build /p:Configuration=Debug /p:Platform="Any CPU"

if %ERRORLEVEL% neq 0 (
    echo Ошибка сборки!
    pause
    exit /b %ERRORLEVEL%
)

echo Сборка завершена успешно!
start "" ForVlad.exe
