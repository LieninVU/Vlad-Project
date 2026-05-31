using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
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
        
        private string _workingHours;
        public string WorkingHours
        {
            get => _workingHours;
            set => SetField(ref _workingHours, value);
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
            set => SetField(ref _connectionStringDisplay, value);
        }
        
        public string AppVersion { get; }
        
        public ICommand SaveCommand { get; }
        public ICommand ResetDemoDataCommand { get; }
        
        public SettingsViewModel(ISimpleDataService dataService)
        {
            _dataService = dataService;
            ThemeOptions = new ObservableCollection<string> { "Light", "Dark", "System" };
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
            UiTheme = s.UiTheme;
            WorkingHours = s.WorkingHours;
            DefaultCurrency = s.DefaultCurrency;
            ConnectionStringDisplay = DatabaseConnection.GetDisplayName();
        }
        
        private void SaveSettings()
        {
            if (DaysInMonth < 28 || DaysInMonth > 31)
            {
                MessageBox.Show("Дней в месяце для расчётов: от 28 до 31", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var s = Settings.Default;
            s.DaysInMonth = DaysInMonth;
            s.DefaultPageSize = DefaultPageSize;
            s.UiTheme = UiTheme ?? "Light";
            s.WorkingHours = WorkingHours ?? "09:00-18:00";
            s.DefaultCurrency = DefaultCurrency ?? "RUB";
            s.ConnectionStringDisplay = DatabaseConnection.GetDisplayName();
            s.Save();
            OnPropertyChanged(nameof(ConnectionStringDisplay));
            
            MessageBox.Show("Настройки сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
