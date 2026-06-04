@echo off
chcp 65001 > nul
cd /d "C:\Users\STAR BUTTERFLY\source\repos\ForVlad\ForVlad\Data"

:: Convert SqlDataService.cs to UTF-8 using PowerShell
powershell -Command "Get-Content SqlDataService.cs -Encoding Default | Set-Content -Encoding UTF8 -NoNewline SqlDataService.cs"

echo File encoding converted to UTF-8
pause
