# Convert all C# and XAML files to UTF-8 encoding
# This fixes compilation errors with Russian text in Windows-1251 encoding

$files = @(
    "App.xaml",
    "App.xaml.cs",
    "Data\DatabaseConnection.cs",
    "Data\SqlDataService.cs",
    "Converters\BoolToVisibilityConverter.cs",
    "Converters\DaysRemainingConverter.cs",
    "Converters\IntToVisibilityConverter.cs",
    "Converters\StatusConverters.cs",
    "Models\Enums.cs",
    "Services\AssetAvailabilityService.cs",
    "Services\AuthenticationService.cs",
    "Services\CsvExportService.cs",
    "Services\PasswordHasher.cs",
    "Services\PaymentScheduleGenerator.cs",
    "Behaviors\PasswordBoxBehavior.cs",
    "ViewModels\AssetsViewModel.cs",
    "ViewModels\ContractsViewModel.cs",
    "ViewModels\CounterpartiesViewModel.cs",
    "ViewModels\LoginViewModel.cs",
    "ViewModels\MainViewModel.cs",
    "ViewModels\RelayCommand.cs",
    "ViewModels\SettingsViewModel.cs",
    "ViewModels\UserProfileViewModel.cs",
    "ViewModels\ViewModelBase.cs",
    "Views\AssetsView.xaml",
    "Views\AssetsView.xaml.cs",
    "Views\ContractsView.xaml",
    "Views\ContractsView.xaml.cs",
    "Views\CounterpartiesView.xaml",
    "Views\CounterpartiesView.xaml.cs",
    "Views\LoginWindow.xaml",
    "Views\LoginWindow.xaml.cs",
    "Views\MainWindow.xaml",
    "Views\MainWindow.xaml.cs",
    "Views\MainWindowSimple.xaml",
    "Views\MainWindowSimple.xaml.cs",
    "Views\PlaceholderView.xaml",
    "Views\PlaceholderView.xaml.cs",
    "Views\SettingsView.xaml",
    "Views\SettingsView.xaml.cs",
    "Views\UserProfileView.xaml",
    "Views\UserProfileView.xaml.cs",
    "Views\ProfileWindow.xaml",
    "Views\ProfileWindow.xaml.cs"
)

Write-Host "Converting files to UTF-8..." -ForegroundColor Green

foreach ($file in $files) {
    if (Test-Path $file) {
        try {
            # Read as bytes and detect encoding
            $bytes = [System.IO.File]::ReadAllBytes($file)
            
            # Try to detect if it's UTF-8 with BOM
            if ($bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
                Write-Host "  $file - Already UTF-8 with BOM, removing BOM..." -ForegroundColor Yellow
                # Remove BOM and save as UTF-8 without BOM
                $content = [System.Text.Encoding]::UTF8.GetString($bytes, 3, $bytes.Length - 3)
                [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
            }
            else {
                # Try to read as Windows-1251
                $content = [System.Text.Encoding]::GetEncoding(1251).GetString($bytes)
                [System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
                Write-Host "  $file - Converted from CP1251 to UTF-8" -ForegroundColor Green
            }
        }
        catch {
            Write-Host "  $file - Error: $_" -ForegroundColor Red
        }
    } else {
        Write-Host "  $file - Not found" -ForegroundColor Gray
    }
}

Write-Host "Done!" -ForegroundColor Green
