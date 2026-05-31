@echo off
echo Компиляция проекта ForVlad...

REM Устанавливаем путь к компилятору C#
set CSC_PATH=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe

if not exist "%CSC_PATH%" (
    echo Ошибка: Компилятор C# не найден по пути %CSC_PATH%
    echo Убедитесь, что установлен .NET Framework 4.0 или выше
    pause
    exit /b 1
)

echo Используем компилятор: %CSC_PATH%

REM Компилируем проект
"%CSC_PATH%" ^
    /target:winexe ^
    /out:bin\Debug\ForVlad.exe ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\WindowsBase.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\PresentationCore.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\framework\.NETFramework\v4.7.2\PresentationFramework.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\framework\.NETFramework\v4.7.2\System.Xaml.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\framework\.NETFramework\v4.7.2\System.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\framework\.NETFramework\v4.7.2\System.Core.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\framework\.NETFramework\v4.7.2\System.Xml.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\framework\.NETFramework\v4.7.2\System.Data.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\framework\.NETFramework\v4.7.2\System.Deployment.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\framework\.NETFramework\v4.7.2\System.Drawing.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\framework\.NETFramework\v4.7.2\System.Windows.Forms.dll" ^
    /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\framework\.NETFramework\v4.7.2\System.Xml.Linq.dll" ^
    /langversion:8 ^
    /recurse:*.cs ^
    /resource:App.xaml ^
    /resource:Views\MainWindow.xaml ^
    /resource:Views\ContractsView.xaml ^
    /resource:Views\AssetsView.xaml ^
    /resource:Views\CounterpartiesView.xaml ^
    /resource:Views\ActiveContractsView.xaml ^
    /resource:Views\MainWindowSimple.xaml ^
    /resource:Views\FinancialReportsView.xaml ^
    /resource:Views\UtilizationReportsView.xaml ^
    /resource:Views\SettingsView.xaml ^
    /resource:Views\PlaceholderView.xaml

if %errorlevel% equ 0 (
    echo Компиляция успешно завершена!
    echo Исполняемый файл: bin\Debug\ForVlad.exe
) else (
    echo Ошибка компиляции!
)

pause
