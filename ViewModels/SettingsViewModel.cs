using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ForVlad.Data;
using ForVlad.Properties;

namespace ForVlad.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISimpleDataService _dataService;

        public ObservableCollection<string> ThemeOptions { get; }
        public ObservableCollection<int> PageSizeOptions { get; }

        private int _daysInMonth;
        public int DaysInMonth
        {
            get => _daysInMonth;
            set => SetField(ref _daysInMonth, value);
        }

        private int _defaultPageSize;
        public int DefaultPageSize
        {
            get => _defaultPageSize;
            set => SetField(ref _defaultPageSize, value);
        }

        private string _uiTheme;
        public string UiTheme
        {
            get => _uiTheme;
            set => SetField(ref _uiTheme, value);
        }

        private string _defaultCurrency;
        public string DefaultCurrency
        {
            get => _defaultCurrency;
            set => SetField(ref _defaultCurrency, value);
        }

        private string _connectionStringDisplay;
        public string ConnectionStringDisplay
        {
            get => _connectionStringDisplay;
            private set => SetField(ref _connectionStringDisplay, value);
        }

        public string AppVersion { get; }

        public ICommand SaveCommand { get; }
        public ICommand ResetDemoDataCommand { get; }

        public SettingsViewModel(ISimpleDataService dataService)
        {
            _dataService = dataService;
            ThemeOptions = new ObservableCollection<string> { "Светлая", "Тёмная" };
            PageSizeOptions = new ObservableCollection<int> { 25, 50, 100 };
            AppVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";

            SaveCommand = new RelayCommand(_ => SaveSettings());
            ResetDemoDataCommand = new RelayCommand(_ => ResetDemoData());

            LoadSettings();
        }

        private void LoadSettings()
        {
            var s = Settings.Default;
            DaysInMonth = s.DaysInMonth;
            DefaultPageSize = s.DefaultPageSize;

            string savedTheme = s.UiTheme;
            if (savedTheme == "Light" || savedTheme == "Светлая")
                UiTheme = "Светлая";
            else if (savedTheme == "Dark" || savedTheme == "Тёмная")
                UiTheme = "Тёмная";
            else
                UiTheme = "Светлая";

            DefaultCurrency = string.IsNullOrEmpty(s.DefaultCurrency) ? "RUB" : s.DefaultCurrency;
            ConnectionStringDisplay = DatabaseConnection.GetDisplayName();
        }

        private void SaveSettings()
        {
            if (DaysInMonth < 28 || DaysInMonth > 31)
            {
                MessageBox.Show("Дней в месяце для расчётов: от 28 до 31", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DefaultPageSize <= 0)
            {
                MessageBox.Show("Размер страницы должен быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(DefaultCurrency))
            {
                MessageBox.Show("Укажите валюту по умолчанию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var s = Settings.Default;
            s.DaysInMonth = DaysInMonth;
            s.DefaultPageSize = DefaultPageSize;

            string themeToSave = UiTheme == "Тёмная" ? "Dark" : "Light";
            s.UiTheme = themeToSave;

            s.DefaultCurrency = DefaultCurrency?.Trim().ToUpperInvariant() ?? "RUB";
            s.ConnectionStringDisplay = DatabaseConnection.GetDisplayName();
            s.Save();

            ConnectionStringDisplay = DatabaseConnection.GetDisplayName();
            ApplyTheme();

            MessageBox.Show("Настройки сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static void ApplyTheme()
        {
            string savedTheme = Settings.Default.UiTheme;
            bool isDark = savedTheme == "Dark" || savedTheme == "Тёмная";

            var app = Application.Current;
            if (app == null) return;

            var resources = app.Resources;

            if (isDark)
            {
                resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(44, 62, 80));
                resources["PanelBackground"] = new SolidColorBrush(Color.FromRgb(52, 73, 94));
                resources["TextForeground"] = new SolidColorBrush(Color.FromRgb(236, 240, 241));
                resources["DataGridRowBackground"] = new SolidColorBrush(Color.FromRgb(44, 62, 80));
                resources["DataGridAlternatingRowBackground"] = new SolidColorBrush(Color.FromRgb(52, 73, 94));
                resources["HeaderBackground"] = new SolidColorBrush(Color.FromRgb(41, 128, 185));
            }
            else
            {
                resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(236, 240, 241));
                resources["PanelBackground"] = new SolidColorBrush(Color.FromRgb(248, 249, 250));
                resources["TextForeground"] = new SolidColorBrush(Color.FromRgb(44, 62, 80));
                resources["DataGridRowBackground"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                resources["DataGridAlternatingRowBackground"] = new SolidColorBrush(Color.FromRgb(248, 249, 250));
                resources["HeaderBackground"] = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            }
        }

        private void ResetDemoData()
        {
            var result = MessageBox.Show(
                "Очистить таблицы и загрузить демонстрационные данные заново? Все текущие записи в БД будут удалены.",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            _dataService.ResetDemoData();
            MessageBox.Show("Демонстрационные данные восстановлены", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
