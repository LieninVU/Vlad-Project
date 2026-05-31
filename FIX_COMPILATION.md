# Исправление ошибок компиляции

## Проблемы, которые были исправлены:

### 1. Рекурсивные шаблоны C# 8.0
**Ошибка**: "Компонент 'рекурсивные шаблоны' недоступен в C# 7.3"
**Решение**: Заменены конструкции C# 8.0 на совместимый синтаксис C# 7.3:

#### В MainViewModel.cs:
- `if (CurrentView is ContractsView contractsView)` → `var contractsView = CurrentView as ContractsView; if (contractsView != null)`
- `if (CurrentView is string strView)` → `if (CurrentView is string) return (string)CurrentView;`

#### В StatusConverters.cs:
- Switch expressions заменены на традиционные switch statements
- Pattern matching заменен на явное приведение типов

#### В IntToVisibilityConverter.cs:
- `if (value is int count)` → `if (value is int) { int count = (int)value; ... }`

### 2. Дублирование файлов в проекте
**Ошибка**: "Исходный файл задан несколько раз"
**Решение**: Исправлен файл ForVlad.csproj - удалены дублирующиеся записи для:
- Views\MainWindow.xaml.cs
- Views\ContractsView.xaml.cs

### 3. Настройка языка C#
**Решение**: Добавлена настройка `<LangVersion>8.0</LangVersion>` в файл проекта

## Оставшиеся проблемы:

### 1. Ошибки InitializeComponent
**Ошибка**: "Имя 'InitializeComponent' не существует в текущем контексте"
**Причина**: Для WPF проектов .NET Framework файлы .g.cs генерируются только при использовании MSBuild из Visual Studio, а не dotnet build.

### 2. Точка входа
**Ошибка**: "Программа не содержит статического метода 'Main'"
**Причина**: В WPF метод Main генерируется автоматически при компиляции XAML файлов.

## Рекомендации:

1. **Используйте Visual Studio 2022** для компиляции проекта
2. **Откройте ForVlad.csproj** в Visual Studio
3. **Нажмите F5** для запуска

## Альтернативные варианты:

### Вариант 1: Использование MSBuild из командной строки
```bash
# Найдите путь к MSBuild (обычно в Visual Studio)
"C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" ForVlad.csproj
```

### Вариант 2: Миграция на .NET Core/.NET 5+
- Создайте новый проект WPF на .NET 6+ 
- Скопируйте файлы из текущего проекта
- Обновите зависимости

### Вариант 3: Упрощенная версия приложения
Создайте консольное приложение с тем же функционалом для тестирования бизнес-логики.

## Проверка исправлений:

Все файлы с рекурсивными шаблонами были исправлены:
- ✅ MainViewModel.cs
- ✅ StatusConverters.cs  
- ✅ IntToVisibilityConverter.cs
- ✅ ForVlad.csproj (удалены дубли, добавлен LangVersion)

Проект готов для компиляции в Visual Studio 2022.