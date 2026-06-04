using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using ForVlad.Data;
using ForVlad.Models;
using ForVlad.Properties;
using ForVlad.Views;

namespace ForVlad.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISimpleDataService _dataService;
        
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                if (SetField(ref _currentView, value))
                {
                    OnPropertyChanged(nameof(CurrentViewTitle));
                    OnPropertyChanged(nameof(CurrentViewActions));
                    LoadCurrentViewData();
                }
            }
        }
        
        public object CurrentViewActions => CurrentView;
        
        public string AppVersion { get; }
        public string ConnectionStatus => DatabaseConnection.GetDisplayName();
        
        public ICommand NavigateToContractsCommand { get; }
        public ICommand NavigateToAssetsCommand { get; }
        public ICommand NavigateToCounterpartiesCommand { get; }
        public ICommand NavigateToActiveContractsCommand { get; }
        public ICommand NavigateToFinancialReportsCommand { get; }
        public ICommand NavigateToUtilizationReportsCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }
        
        private ObservableCollection<Contract> _contracts;
        public ObservableCollection<Contract> Contracts
        {
            get => _contracts;
            set => SetField(ref _contracts, value);
        }
        
        private ObservableCollection<Counterparty> _counterparties;
        public ObservableCollection<Counterparty> Counterparties
        {
            get => _counterparties;
            set => SetField(ref _counterparties, value);
        }
        
        private ObservableCollection<Asset> _assets;
        public ObservableCollection<Asset> Assets
        {
            get => _assets;
            set => SetField(ref _assets, value);
        }
        
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }
        
        public MainViewModel(ISimpleDataService dataService)
        {
            Contracts = new ObservableCollection<Contract>();
            Counterparties = new ObservableCollection<Counterparty>();
            Assets = new ObservableCollection<Asset>();
            
            AppVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
            
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            Settings.Default.ConnectionStringDisplay = DatabaseConnection.GetDisplayName();
            
            NavigateToContractsCommand = new RelayCommand(_ => NavigateToContracts());
            NavigateToAssetsCommand = new RelayCommand(_ => NavigateToAssets());
            NavigateToCounterpartiesCommand = new RelayCommand(_ => NavigateToCounterparties());
            NavigateToActiveContractsCommand = new RelayCommand(_ => NavigateToActiveContracts());
            NavigateToFinancialReportsCommand = new RelayCommand(_ => NavigateToFinancialReports());
            NavigateToUtilizationReportsCommand = new RelayCommand(_ => NavigateToUtilizationReports());
            NavigateToSettingsCommand = new RelayCommand(_ => NavigateToSettings());
            
            StatusMessage = "Система учёта договоров аренды и лизинга спецтехники";
            NavigateToContracts();
        }
        
        private void NavigateToContracts()
        {
            var view = new ContractsView();
            var vm = new ContractsViewModel(_dataService);
            vm.SetNavigateToContractDetails(NavigateToContractDetails);
            view.DataContext = vm;
            CurrentView = view;
        }

        private void NavigateToContractDetails(int contractId)
        {
            var view = new ContractDetailsView();
            var vm = new ContractDetailsViewModel(_dataService, contractId);
            vm.SetNavigateBack(NavigateToContracts);
            view.DataContext = vm;
            CurrentView = view;
        }
        
        private void NavigateToContractsForCounterparty(int counterpartyId)
        {
            NavigateToContracts();
            if (CurrentView is ContractsView contractsView &&
                contractsView.DataContext is ContractsViewModel vm)
            {
                vm.SetCounterpartyFilter(counterpartyId);
                StatusMessage = $"Договоры контрагента: {_dataService.GetCounterparty(counterpartyId)?.Name}";
            }
        }
        
        private void NavigateToAssets()
        {
            var view = new AssetsView();
            view.DataContext = new AssetsViewModel(_dataService);
            CurrentView = view;
        }
        
        private void NavigateToCounterparties()
        {
            var view = new CounterpartiesView();
            var vm = new CounterpartiesViewModel(_dataService)
            {
                NavigateToCounterpartyContracts = NavigateToContractsForCounterparty
            };
            view.DataContext = vm;
            CurrentView = view;
        }
        
        private void NavigateToActiveContracts()
        {
            var view = new ActiveContractsView();
            view.DataContext = new ActiveContractsViewModel(_dataService);
            CurrentView = view;
        }
        
        private void NavigateToFinancialReports()
        {
            var view = new FinancialReportsView();
            view.DataContext = new FinancialReportsViewModel(_dataService);
            CurrentView = view;
        }
        
        private void NavigateToUtilizationReports()
        {
            var view = new UtilizationReportsView();
            view.DataContext = new UtilizationReportsViewModel(_dataService);
            CurrentView = view;
        }
        
        private void NavigateToSettings()
        {
            var view = new SettingsView();
            view.DataContext = new SettingsViewModel(_dataService);
            CurrentView = view;
        }
        
        public string CurrentViewTitle
        {
            get
            {
                if (CurrentView is ContractsView) return "Договоры";
                if (CurrentView is AssetsView) return "Техника и оборудование";
                if (CurrentView is CounterpartiesView) return "Контрагенты";
                if (CurrentView is ActiveContractsView) return "Действующие договоры";
                if (CurrentView is FinancialReportsView) return "Финансовые отчёты";
                if (CurrentView is UtilizationReportsView) return "Загрузка техники";
                if (CurrentView is SettingsView) return "Настройки";
                return "Раздел";
            }
        }
        
        private void LoadCurrentViewData()
        {
            try
            {
                if (CurrentView is ContractsView contractsView &&
                    contractsView.DataContext is ContractsViewModel contractsVm)
                {
                    contractsVm.LoadContracts();
                    StatusMessage = $"Загружено {contractsVm.Contracts.Count} договоров";
                }
                else if (CurrentView is AssetsView assetsView &&
                         assetsView.DataContext is AssetsViewModel assetsVm)
                {
                    assetsVm.LoadAssets();
                    StatusMessage = $"Загружено {assetsVm.Assets.Count} единиц техники";
                }
                else if (CurrentView is CounterpartiesView cpView &&
                         cpView.DataContext is CounterpartiesViewModel cpVm)
                {
                    cpVm.LoadCounterparties();
                    StatusMessage = $"Загружено {cpVm.Counterparties.Count} контрагентов";
                }
                else if (CurrentView is ActiveContractsView activeView &&
                         activeView.DataContext is ActiveContractsViewModel activeVm)
                {
                    activeVm.LoadActiveContracts();
                    StatusMessage = $"Действующих договоров: {activeVm.ActiveContracts.Count}";
                }
                else if (CurrentView is FinancialReportsView finView &&
                         finView.DataContext is FinancialReportsViewModel finVm)
                {
                    finVm.LoadReport();
                    StatusMessage = $"Финансовый отчёт: {finVm.Payments.Count} платежей";
                }
                else if (CurrentView is UtilizationReportsView utilView &&
                         utilView.DataContext is UtilizationReportsViewModel utilVm)
                {
                    utilVm.LoadReport();
                    StatusMessage = $"Загрузка техники: {utilVm.Rows.Count} активов";
                }
                else if (CurrentView is SettingsView)
                {
                    StatusMessage = "Настройки приложения";
                    OnPropertyChanged(nameof(ConnectionStatus));
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки: {ex.Message}";
            }
        }
    }
}
