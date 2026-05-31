@echo off
echo Установка пакетов NuGet для проекта ForVlad...
echo.

echo Установка EntityFramework 6.4.4...
nuget install EntityFramework -Version 6.4.4 -OutputDirectory packages

echo.
echo Установка завершена.
echo.
echo Для добавления ссылок в проект выполните:
echo 1. Откройте проект в Visual Studio
echo 2. В Обозревателе решений щелкните правой кнопкой по References
echo 3. Выберите "Add Reference..."
echo 4. Перейдите на вкладку "Browse"
echo 5. Нажмите "Browse..." и выберите файлы из папки packages:
echo    - EntityFramework.6.4.4\lib\net45\EntityFramework.dll
echo    - EntityFramework.SqlServer.6.4.4\lib\net45\EntityFramework.SqlServer.dll
echo 6. Нажмите OK

pause