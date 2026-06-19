using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ForVlad.Data;
using ForVlad.Models;
using ForVlad.Services;

namespace ForVlad.ViewModels
{
    public class ActiveContractsViewModel : ViewModelBase
    {
        private readonly ISimpleDataService _dataService;
        
        private ObservableCollection<Contract> _activeContracts;
        public ObservableCollection<Contract> ActiveContracts
        {
            get => _activeContracts;
            set => SetField(ref _activeContracts, value);
        }
        
        private ObservableCollection<Contract> _filteredContracts;
        public ObservableCollection<Contract> FilteredContracts
        {
            get => _filteredContracts;
            set => SetField(ref _filteredContracts, value);
        }
        
        private Contract _selectedContract;
        public Contract SelectedContract
        {
            get => _selectedContract;
            set => SetField(ref _selectedContract, value);
        }
        
        private Contract _viewingContract;
        public Contract ViewingContract
        {
            get => _viewingContract;
            set => SetField(ref _viewingContract, value);
        }
        
        private bool _isDetailsDialogOpen;
        public bool IsDetailsDialogOpen
        {
            get => _isDetailsDialogOpen;
            set => SetField(ref _isDetailsDialogOpen, value);
        }
        
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetField(ref _searchText, value))
                    FilterContracts();
            }
        }
        
        private DateTime? _filterStartDate;
        public DateTime? FilterStartDate
        {
            get => _filterStartDate;
            set
            {
                if (SetField(ref _filterStartDate, value))
                    FilterContracts();
            }
        }
        
        private DateTime? _filterEndDate;
        public DateTime? FilterEndDate
        {
            get => _filterEndDate;
            set
            {
                if (SetField(ref _filterEndDate, value))
                    FilterContracts();
            }
        }
        
        public bool HasContracts => FilteredContracts.Count > 0;
        public int TotalContracts => FilteredContracts.Count;
        public decimal TotalAmount => FilteredContracts.Sum(c => c.TotalAmount);
        public decimal TotalWithVAT => FilteredContracts.Sum(c => c.TotalWithVAT);
        public int ExpiringSoon => FilteredContracts.Count(c =>
            c.EndDate.HasValue && c.EndDate.Value <= DateTime.Now.AddDays(30) && c.EndDate.Value >= DateTime.Now.Date);
        
        public ICommand RefreshCommand { get; }
        public ICommand ExportToExcelCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand ViewDetailsFromRowCommand { get; }
        public ICommand CloseDetailsCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        
        public ActiveContractsViewModel(ISimpleDataService dataService)
        {
            _dataService = dataService;
            ActiveContracts = new ObservableCollection<Contract>();
            FilteredContracts = new ObservableCollection<Contract>();
            
            FilterStartDate = DateTime.Now.AddMonths(-1);
            FilterEndDate = DateTime.Now.AddMonths(1);
            
            RefreshCommand = new RelayCommand(_ => LoadActiveContracts());
            ExportToExcelCommand = new RelayCommand(_ => ExportToExcel(), _ => HasContracts);
            PrintCommand = new RelayCommand(_ => Print(), _ => SelectedContract != null);
            ViewDetailsCommand = new RelayCommand(_ => OpenDetails(), _ => SelectedContract != null);
            ViewDetailsFromRowCommand = new RelayCommand(OpenDetailsFromRow);
            CloseDetailsCommand = new RelayCommand(_ => CloseDetails());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            
            LoadActiveContracts();
        }
        
        public void LoadActiveContracts()
        {
            var contracts = _dataService.GetContracts();
            ActiveContracts.Clear();
            
            foreach (var contract in contracts)
            {
                EnrichContract(contract);
                ActiveContracts.Add(contract);
            }
            
            FilterContracts();
        }
        
        private void EnrichContract(Contract contract)
        {
            var counterparty = _dataService.GetCounterparty(contract.CounterpartyId);
            contract.CounterpartyDisplayName = counterparty?.Name ?? "—";
        }
        
        private void FilterContracts()
        {
            FilteredContracts.Clear();
            
            var filtered = ActiveContracts.AsEnumerable();
            
            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(c =>
                    (c.ContractNumber?.ToLower().Contains(searchLower) ?? false) ||
                    (c.CounterpartyDisplayName?.ToLower().Contains(searchLower) ?? false));
            }
            
            if (FilterStartDate.HasValue && FilterEndDate.HasValue)
            {
                var from = FilterStartDate.Value.Date;
                var to = FilterEndDate.Value.Date;
                filtered = filtered.Where(c =>
                    ReportCalculationService.ContractOverlapsPeriod(c, from, to));
            }
            else
            {
                if (FilterStartDate.HasValue)
                    filtered = filtered.Where(c => !c.EndDate.HasValue || c.EndDate.Value.Date >= FilterStartDate.Value.Date);
                if (FilterEndDate.HasValue)
                    filtered = filtered.Where(c => c.StartDate.Date <= FilterEndDate.Value.Date);
            }
            
            foreach (var contract in filtered)
                FilteredContracts.Add(contract);
            
            UpdateStatistics();
        }
        
        private void UpdateStatistics()
        {
            OnPropertyChanged(nameof(HasContracts));
            OnPropertyChanged(nameof(TotalContracts));
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(TotalWithVAT));
            OnPropertyChanged(nameof(ExpiringSoon));
        }
        
        private void OpenDetails()
        {
            if (SelectedContract == null)
                return;
            ShowDetails(SelectedContract);
        }
        
        private void OpenDetailsFromRow(object parameter)
        {
            if (parameter is Contract contract)
            {
                SelectedContract = contract;
                ShowDetails(contract);
            }
        }
        
        private void ShowDetails(Contract contract)
        {
            ViewingContract = new Contract
            {
                Id = contract.Id,
                ContractNumber = contract.ContractNumber,
                ContractType = contract.ContractType,
                CounterpartyId = contract.CounterpartyId,
                CounterpartyDisplayName = contract.CounterpartyDisplayName,
                SignedDate = contract.SignedDate,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                DurationMonths = contract.DurationMonths,
                TotalAmount = contract.TotalAmount,
                VATAmount = contract.VATAmount,
                TotalWithVAT = contract.TotalWithVAT,
                MonthlyPayment = contract.MonthlyPayment,
                PaymentTerms = contract.PaymentTerms,
                Notes = contract.Notes
            };
            IsDetailsDialogOpen = true;
        }
        
        private void CloseDetails()
        {
            IsDetailsDialogOpen = false;
            ViewingContract = null;
        }
        
        private void ExportToExcel()
        {
            if (!FilteredContracts.Any())
                return;
            
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = $"Действующие_договоры_{DateTime.Now:yyyyMMdd}.csv"
            };
            
            if (dialog.ShowDialog() != true)
                return;
            
            var headers = new[] { "Номер", "Контрагент", "Тип", "Начало", "Окончание", "Сумма", "С НДС" };
            var rows = FilteredContracts.Select(c => new[]
            {
                c.ContractNumber,
                c.CounterpartyDisplayName ?? "",
                EnumLocalization.ContractTypeToRussian(c.ContractType),
                c.StartDate.ToString("dd.MM.yyyy"),
                c.EndDate?.ToString("dd.MM.yyyy") ?? "",
                c.TotalAmount.ToString("N2"),
                c.TotalWithVAT.ToString("N2")
            });
            
            Services.CsvExportService.Export(dialog.FileName, headers, rows);
            MessageBox.Show($"Экспортировано {FilteredContracts.Count} договоров", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void Print()
        {
            if (SelectedContract == null)
                return;
            
            MessageBox.Show(
                $"Печать договора {SelectedContract.ContractNumber}\nКонтрагент: {SelectedContract.CounterpartyDisplayName}\nПериод: {SelectedContract.PeriodDisplay}",
                "Печать",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        
        private void ClearFilters()
        {
            SearchText = string.Empty;
            FilterStartDate = DateTime.Now.AddMonths(-1);
            FilterEndDate = DateTime.Now.AddMonths(1);
        }
    }
}
