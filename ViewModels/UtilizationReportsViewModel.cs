using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ForVlad.Data;
using ForVlad.Models;
using ForVlad.Models.Reports;
using ForVlad.Services;
using Microsoft.Win32;

namespace ForVlad.ViewModels
{
    public class UtilizationReportsViewModel : ViewModelBase
    {
        private readonly ISimpleDataService _dataService;
        
        public ObservableCollection<string> PeriodOptions { get; }
        public ObservableCollection<AssetGroup?> AssetGroupFilters { get; }
        
        private ObservableCollection<AssetUtilizationRow> _rows;
        public ObservableCollection<AssetUtilizationRow> Rows
        {
            get => _rows;
            set => SetField(ref _rows, value);
        }
        
        private ObservableCollection<string> _alerts;
        public ObservableCollection<string> Alerts
        {
            get => _alerts;
            set => SetField(ref _alerts, value);
        }
        
        private string _selectedPeriod = "Месяц";
        public string SelectedPeriod
        {
            get => _selectedPeriod;
            set
            {
                if (SetField(ref _selectedPeriod, value))
                {
                    OnPropertyChanged(nameof(IsCustomPeriod));
                    LoadReport();
                }
            }
        }
        
        public bool IsCustomPeriod => SelectedPeriod == "Произвольный";
        
        private AssetGroup? _selectedAssetGroup;
        public AssetGroup? SelectedAssetGroup
        {
            get => _selectedAssetGroup;
            set
            {
                if (SetField(ref _selectedAssetGroup, value))
                    LoadReport();
            }
        }
        
        private DateTime? _customPeriodStart;
        public DateTime? CustomPeriodStart
        {
            get => _customPeriodStart;
            set => SetField(ref _customPeriodStart, value);
        }
        
        private DateTime? _customPeriodEnd;
        public DateTime? CustomPeriodEnd
        {
            get => _customPeriodEnd;
            set => SetField(ref _customPeriodEnd, value);
        }
        
        private UtilizationSummary _summary;
        public UtilizationSummary Summary
        {
            get => _summary;
            set => SetField(ref _summary, value);
        }
        
        public bool HasRows => Rows != null && Rows.Count > 0;
        public bool HasAlerts => Alerts != null && Alerts.Count > 0;
        
        public ICommand RefreshCommand { get; }
        public ICommand ApplyPeriodCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        
        public UtilizationReportsViewModel(ISimpleDataService dataService)
        {
            _dataService = dataService;
            Rows = new ObservableCollection<AssetUtilizationRow>();
            Alerts = new ObservableCollection<string>();
            Summary = new UtilizationSummary();
            
            PeriodOptions = new ObservableCollection<string> { "Месяц", "Квартал", "Год", "Произвольный" };
            AssetGroupFilters = new ObservableCollection<AssetGroup?> { null, AssetGroup.Vehicle, AssetGroup.Equipment };
            
            CustomPeriodStart = DateTime.Today.AddMonths(-1);
            CustomPeriodEnd = DateTime.Today;
            
            RefreshCommand = new RelayCommand(_ => LoadReport());
            ApplyPeriodCommand = new RelayCommand(_ => LoadReport());
            ExportToCsvCommand = new RelayCommand(_ => ExportCsv(), _ => HasRows);
            
            LoadReport();
        }
        
        public void LoadReport()
        {
            var (start, end) = ReportCalculationService.ResolvePeriod(SelectedPeriod, CustomPeriodStart, CustomPeriodEnd);
            var data = _dataService.GetAssetUtilizationReport(start, end, SelectedAssetGroup);
            
            Rows.Clear();
            Alerts.Clear();
            foreach (var row in data)
            {
                Rows.Add(row);
                if (!string.IsNullOrEmpty(row.AlertMessage))
                    Alerts.Add($"{row.AssetName} ({row.InventoryNumber}): {row.AlertMessage}");
            }
            
            Summary = ReportCalculationService.BuildUtilizationSummary(data);
            OnPropertyChanged(nameof(Summary));
            OnPropertyChanged(nameof(HasRows));
            OnPropertyChanged(nameof(HasAlerts));
        }
        
        private void ExportCsv()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = $"Загрузка_техники_{DateTime.Now:yyyyMMdd}.csv"
            };
            
            if (dialog.ShowDialog() != true)
                return;
            
            var headers = new[]
            {
                "Инв.номер", "Наименование", "Группа", "Дней в периоде", "Дней в аренде",
                "Загрузка %", "Выручка", "Ставка/мес", "Доступен"
            };
            
            var rows = Rows.Select(r => new[]
            {
                r.InventoryNumber,
                r.AssetName,
                EnumLocalization.AssetGroupToRussian(r.AssetGroup),
                r.DaysInPeriod.ToString(),
                r.DaysRented.ToString(),
                r.UtilizationRate.ToString("F1"),
                r.Revenue.ToString("N2"),
                r.MonthlyRate.ToString("N2"),
                r.IsAvailable ? "Да" : "Нет"
            });
            
            CsvExportService.Export(dialog.FileName, headers, rows);
            MessageBox.Show($"Файл сохранён:\n{dialog.FileName}", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
