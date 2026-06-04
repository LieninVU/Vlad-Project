# Replace Russian text with English equivalents in C# files

$files = @(
    "Data\SqlDataService.cs",
    "Data\DatabaseConnection.cs", 
    "Services\AssetAvailabilityService.cs",
    "ViewModels\CounterpartiesViewModel.cs"
)

$replacements = @{
    # Russian error messages to English
    "Электронная почта должна содержать символы '@' и '.' или быть пустой" = "Email must contain '@' and '.' or be empty"
    "КПП должен содержать ровно 9 символов или быть пустым" = "KPP must be exactly 9 characters or empty"
    "ИНН должен быть указан" = "INN must be specified"
    "ИНН должен содержать от 10 до 12 символов" = "INN must be 10-12 characters"
    "Ошибки валидации:" = "Validation errors:"
    "Инвентарный номер должен быть указан" = "Inventory number must be specified"
    "Наименование техники должно быть указано" = "Asset name must be specified"
    "Подкатегория должна быть выбрана" = "Subcategory must be selected"
    "Почасовая ставка должна быть >= 0" = "Hourly rate must be >= 0"
    "Дневная ставка должна быть >= 0" = "Daily rate must be >= 0"
    "Месячная арендная ставка должна быть >= 0" = "Monthly rental rate must be >= 0"
    "Хотя бы одна из ставок" = "At least one rate"
    "Для транспортного средства должен быть указан производитель" = "For vehicle, manufacturer must be specified"
    "Для транспортного средства должна быть указана модель" = "For vehicle, model must be specified"
    "Для оборудования поле 'Производитель' должно быть пустым" = "For equipment, Manufacturer field must be empty"
    "Для оборудования поле 'Модель' должно быть пустым" = "For equipment, Model field must be empty"
    "Мощность двигателя должна быть > 0" = "Engine power must be > 0"
    "Вес должен быть > 0" = "Weight must be > 0"
    "Год выпуска должен быть между 1900 и" = "Year of manufacture must be between 1900 and"
    "Текущий пароль неверный" = "Current password is incorrect"
    "Пароли не совпадают" = "Passwords do not match"
    "Пароль должен содержать не менее 6 символов" = "Password must be at least 6 characters"
}

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "Processing $file..."
        $content = Get-Content $file -Raw -Encoding UTF8
        
        foreach ($key in $replacements.Keys) {
            $content = $content -replace [regex]::Escape($key), $replacements[$key]
        }
        
        # Remove all remaining Russian comments (replace with empty)
        $content = $content -replace '<summary>.*?</summary>', ''
        $content = $content -replace '//.*?$', ''
        
        Set-Content $file $content -Encoding UTF8 -NoNewline
        Write-Host "  Done"
    }
}

Write-Host "Replacement complete!"
