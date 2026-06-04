# PowerShell script to convert files from Windows-1251 to UTF-8
# Run this script to fix compilation errors with Russian text

$files = @(
    "Data\SqlDataService.cs",
    "Data\DatabaseConnection.cs",
    "Services\AssetAvailabilityService.cs",
    "ViewModels\CounterpartiesViewModel.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "Converting $file to UTF-8..."
        $content = Get-Content $file -Encoding Default -Raw
        $content | Set-Content $file -Encoding UTF8 -NoNewline
        Write-Host "  Done"
    } else {
        Write-Host "File not found: $file"
    }
}

Write-Host "Encoding conversion complete!"
