using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ForVlad.Data;
using ForVlad.Models;
using ForVlad.Properties;

namespace ForVlad.ViewModels
{
    public class ContractsViewModel : ViewModelBase
    {
        private readonly ISimpleDataService _dataService;
        
        private ObservableCollection<Contract> _contracts;
        public ObservableCollection<Contract> Contracts
        {
            get => _contracts;
            set => SetField(ref _contracts, value);
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
        
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetField(ref _searchText, value))
                {
                    FilterContracts();
                }
            }
        }
        
        private ContractStatus? _filterStatus;
        public ContractStatus? FilterStatus
        {
            get => _filterStatus;
            set
            {
                if (SetField(ref _filterStatus, value))
                {
                    FilterContracts();
                }
            }
        }
        
        // Для создания/редактирования договора
        private bool _isDialogOpen;
        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set => SetField(ref _isDialogOpen, value);
        }

        private Contract _editingContract;
        public Contract EditingContract
        {
            get => _editingContract;
            set
            {
                if (SetField(ref _editingContract, value))
                {
                    OnPropertyChanged(nameof(DurationDisplay));
                }
            }
        }

        // Справочники для диалога
        public ObservableCollection<Asset> Assets { get; }
        public ObservableCollection<PeriodType> PeriodTypes { get; }

        // Выбранная техника в диалоге
        private Asset _selectedAsset;
        public Asset SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                if (SetField(ref _selectedAsset, value))
                {
                    // Автоматически обновляем цену при выборе техники
                    if (value != null && EditingContract != null)
                    {
                        EditingContract.TotalAmount = value.MonthlyRentalRate > 0 ? value.MonthlyRentalRate : value.DailyRate * 30;
                        EditingContract.MonthlyPayment = EditingContract.TotalAmount;
                        OnPropertyChanged(nameof(EditingContract));
                    }
                }
            }
        }
        
        // Отображение продолжительности
        public string DurationDisplay
        {
            get
            {
                if (EditingContract == null || !EditingContract.EndDate.HasValue)
                    return "";
                
                var duration = EditingContract.EndDate.Value - EditingContract.StartDate;
                
                if (duration.TotalDays < 0)
                    return "Ошибка: дата окончания раньше даты начала";
                
                int months = (int)(duration.TotalDays / 30);
                int days = (int)duration.TotalDays;
                
                if (months >= 1)
                    return $"{months} мес.";
                else
                    return $"{days} дн.";
            }
        }
        
        private string _dialogTitle;
        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetField(ref _dialogTitle, value);
        }
        
        // Списки для ComboBox
        public ObservableCollection<Counterparty> Counterparties { get; }
        public ObservableCollection<ContractStatus> Statuses { get; }
public ObservableCollection<ContractStatus?> StatusFilterOptions { get; }

        // Команды
        public ICommand AddContractCommand { get; }
        public ICommand EditContractCommand { get; }
        public ICommand DeleteContractCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SaveContractCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ClearStatusFilterCommand { get; }
        public ICommand EditContractFromRowCommand { get; }
        public ICommand ViewContractDetailsCommand { get; }

        private Action<int> _navigateToContractDetails;
        
        private int? _filterCounterpartyId;
        
        public ContractsViewModel(ISimpleDataService dataService)
        {
            _dataService = dataService;
            Contracts = new ObservableCollection<Contract>();
            FilteredContracts = new ObservableCollection<Contract>();
            Counterparties = new ObservableCollection<Counterparty>();
            Statuses = new ObservableCollection<ContractStatus>();
            StatusFilterOptions = new ObservableCollection<ContractStatus?>();
            Assets = new ObservableCollection<Asset>();
            PeriodTypes = new ObservableCollection<PeriodType>();

            StatusFilterOptions.Add(null);
            foreach (ContractStatus status in Enum.GetValues(typeof(ContractStatus)))
            {
                Statuses.Add(status);
                StatusFilterOptions.Add(status);
            }

            foreach (PeriodType type in Enum.GetValues(typeof(PeriodType)))
            {
                PeriodTypes.Add(type);
            }

            // Инициализация команд
            AddContractCommand = new RelayCommand(_ => OpenAddDialog());
            EditContractCommand = new RelayCommand(_ => OpenEditDialog(), _ => SelectedContract != null);
            DeleteContractCommand = new RelayCommand(_ => DeleteContract(), _ => SelectedContract != null);
            RefreshCommand = new RelayCommand(_ => LoadContracts());
            SaveContractCommand = new RelayCommand(_ => SaveContract());
            CancelCommand = new RelayCommand(_ => CloseDialog());
            ClearStatusFilterCommand = new RelayCommand(_ => ClearStatusFilter());
            EditContractFromRowCommand = new RelayCommand(OpenEditFromRow);
            ViewContractDetailsCommand = new RelayCommand(_ => ViewContractDetails(), _ => SelectedContract != null);
            
            LoadContracts();
            LoadCounterparties();
        }
        
        public void SetCounterpartyFilter(int counterpartyId)
        {
            _filterCounterpartyId = counterpartyId;
            FilterStatus = null;
            FilterContracts();
        }
        
        public void ClearCounterpartyFilter()
        {
            _filterCounterpartyId = null;
        }

        public void SetNavigateToContractDetails(Action<int> navigateToContractDetails)
        {
            _navigateToContractDetails = navigateToContractDetails;
        }

        private void ViewContractDetails()
        {
            if (SelectedContract != null && _navigateToContractDetails != null)
            {
                _navigateToContractDetails(SelectedContract.Id);
            }
        }

        private void OpenEditFromRow(object parameter)
        {
            if (parameter is Contract contract)
            {
                SelectedContract = contract;
                OpenEditDialog();
            }
        }
        
        private void EnrichContract(Contract contract)
        {
            var counterparty = _dataService.GetCounterparty(contract.CounterpartyId);
            contract.CounterpartyDisplayName = counterparty?.Name ?? "—";
        }
        
        // Метод для обновления продолжительности при изменении дат
        public void UpdateDuration()
        {
            if (EditingContract != null)
            {
                EditingContract.DurationMonths = CalculateDurationMonths();
                OnPropertyChanged(nameof(DurationDisplay));
            }
        }
        
        private int CalculateDurationMonths()
        {
            if (EditingContract == null || !EditingContract.EndDate.HasValue)
                return 0;
            
            var duration = EditingContract.EndDate.Value - EditingContract.StartDate;
            
            if (duration.TotalDays < 0)
                return 0;
            
            // Рассчитываем количество месяцев
            var daysInMonth = Math.Max(28, Settings.Default.DaysInMonth);
            int months = (int)Math.Ceiling(duration.TotalDays / daysInMonth);
            return months;
        }
        
        private void LoadCounterparties()
        {
            var counterparties = _dataService.GetCounterparties();
            Counterparties.Clear();
            foreach (var counterparty in counterparties)
            {
                Counterparties.Add(counterparty);
            }

            var assets = _dataService.GetAssets();
            Assets.Clear();
            foreach (var asset in assets)
            {
                Assets.Add(asset);
            }
        }
        
        public void LoadContracts()
        {
            var contracts = _dataService.GetContracts();
            Contracts.Clear();
            foreach (var contract in contracts)
            {
                EnrichContract(contract);
                Contracts.Add(contract);
            }
            FilterContracts();
        }
        
        private void FilterContracts()
        {
            FilteredContracts.Clear();
            
            var filtered = Contracts.AsEnumerable();
            
            if (_filterCounterpartyId.HasValue)
                filtered = filtered.Where(c => c.CounterpartyId == _filterCounterpartyId.Value);
            
            if (FilterStatus.HasValue)
                filtered = filtered.Where(c => c.Status == FilterStatus.Value);
            
            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(c => 
                    (c.ContractNumber?.ToLower().Contains(searchLower) ?? false) ||
                    (c.Notes?.ToLower().Contains(searchLower) ?? false) ||
                    (c.CounterpartyDisplayName?.ToLower().Contains(searchLower) ?? false));
            }
            
            foreach (var contract in filtered)
            {
                FilteredContracts.Add(contract);
            }
            
            OnPropertyChanged(nameof(HasContracts));
        }
        
        public bool HasContracts => FilteredContracts.Count > 0;
        
        private void OpenAddDialog()
        {
            var defaultCounterpartyId = Counterparties.Count > 0 ? Counterparties[0].Id : 0;
            EditingContract = new Contract
            {
                ContractNumber = _dataService.GenerateContractNumber(ContractType.Rental),
                ContractType = ContractType.Rental,
                Status = ContractStatus.Draft,
                CounterpartyId = defaultCounterpartyId,
                SignedDate = DateTime.Now,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                DurationMonths = 1,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };
            SelectedAsset = null;
            DialogTitle = "Новый договор";
            IsDialogOpen = true;
        }
        
        private void OpenEditDialog()
        {
            if (SelectedContract == null) return;

            // Создаем копию для редактирования
            EditingContract = new Contract
            {
                Id = SelectedContract.Id,
                ContractNumber = SelectedContract.ContractNumber,
                ContractType = SelectedContract.ContractType,
                Status = SelectedContract.Status,
                CounterpartyId = SelectedContract.CounterpartyId,
                SignedDate = SelectedContract.SignedDate,
                StartDate = SelectedContract.StartDate,
                EndDate = SelectedContract.EndDate,
                DurationMonths = SelectedContract.DurationMonths,
                TotalAmount = SelectedContract.TotalAmount,
                VATAmount = SelectedContract.VATAmount,
                TotalWithVAT = SelectedContract.TotalWithVAT,
                AdvancePayment = SelectedContract.AdvancePayment,
                MonthlyPayment = SelectedContract.MonthlyPayment,
                PaymentTerms = SelectedContract.PaymentTerms,
                Notes = SelectedContract.Notes,
                CreatedDate = SelectedContract.CreatedDate,
                IsDeleted = SelectedContract.IsDeleted
            };

            // Загружаем существующую спецификацию для выбранной техники
            var specs = _dataService.GetSpecifications(SelectedContract.Id);
            if (specs.Count > 0)
            {
                var firstSpec = specs[0];
                SelectedAsset = Assets.FirstOrDefault(a => a.Id == firstSpec.AssetId);
            }
            else
            {
                SelectedAsset = null;
            }

            DialogTitle = "Редактирование договора";
            IsDialogOpen = true;
        }
        
        private void SaveContract()
        {
            if (EditingContract == null) return;
            
            // Валидация
            if (string.IsNullOrWhiteSpace(EditingContract.ContractNumber))
            {
                MessageBox.Show("Введите номер договора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (EditingContract.CounterpartyId == 0)
            {
                MessageBox.Show("Выберите контрагента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // REFACTOR: Валидация суммы договора (должна быть > 0)
            if (EditingContract.TotalAmount <= 0)
            {
                MessageBox.Show("Сумма договора должна быть больше 0. Укажите корректную общую сумму.", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Валидация даты подписания
            if (EditingContract.SignedDate > DateTime.Now)
            {
                MessageBox.Show("Дата подписания договора не может быть в будущем.", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Валидация даты начала
            if (EditingContract.StartDate < EditingContract.SignedDate)
            {
                MessageBox.Show("Дата начала действия договора не может быть раньше даты подписания.", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Валидация дат
            if (EditingContract.EndDate.HasValue)
            {
                if (EditingContract.EndDate.Value <= EditingContract.StartDate)
                {
                    MessageBox.Show("Дата окончания должна быть позже даты начала", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            
            // Рассчитываем продолжительность автоматически
            EditingContract.DurationMonths = CalculateDurationMonths();
            
            // Рассчитываем сумму с НДС
            EditingContract.TotalWithVAT = EditingContract.TotalAmount + EditingContract.VATAmount;

            try
            {
                // Если выбрана техника, сначала проверяем доступность
                if (SelectedAsset != null)
                {
                    // При редактировании исключаем текущий договор из проверки
                    var excludeContractId = EditingContract.Id > 0 ? EditingContract.Id : (int?)null;

                    // Проверяем доступность техники на период договора
                    var isAvailable = _dataService.CheckAssetAvailability(
                        SelectedAsset.Id,
                        EditingContract.StartDate,
                        EditingContract.EndDate ?? DateTime.MaxValue,
                        excludeContractId);

                    if (!isAvailable)
                    {
                        MessageBox.Show(
                            "Эта техника недоступна для выбранного периода. Возможно, она уже используется в другом договоре.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Сохраняем договор
                _dataService.SaveContract(EditingContract);

                // Если выбрана техника, создаем спецификацию
                if (SelectedAsset != null)
                {
                    var specification = new ContractSpecification
                    {
                        ContractId = EditingContract.Id,
                        AssetId = SelectedAsset.Id,
                        Quantity = 1,
                        UnitPrice = SelectedAsset.MonthlyRentalRate > 0 ? SelectedAsset.MonthlyRentalRate : SelectedAsset.DailyRate * 30,
                        PeriodType = PeriodType.Month
                    };

                    try
                    {
                        _dataService.SaveSpecification(specification);
                        System.Diagnostics.Debug.WriteLine($"Спецификация сохранена: ContractId={specification.ContractId}, AssetId={specification.AssetId}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении спецификации: {ex.Message}");
                        throw;
                    }

                    // Обновляем доступность техники
                    var asset = _dataService.GetAsset(SelectedAsset.Id);
                    if (asset != null)
                    {
                        asset.IsAvailable = false;
                        asset.ModifiedDate = DateTime.Now;
                        _dataService.SaveAsset(asset);
                    }
                }

                // Создаем график платежей для нового договора
                if (EditingContract.Id > 0 && EditingContract.DurationMonths > 0)
                {
                    try
                    {
                        var paymentCount = EditingContract.DurationMonths;
                        var amountPerPayment = EditingContract.TotalAmount / paymentCount;
                        _dataService.GeneratePaymentSchedule(
                            EditingContract.Id,
                            paymentCount,
                            EditingContract.StartDate,
                            amountPerPayment);
                        System.Diagnostics.Debug.WriteLine($"График платежей создан: ContractId={EditingContract.Id}, Payments={paymentCount}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Ошибка при создании графика платежей: {ex.Message}");
                        // Не прерываем сохранение договора, если график платежей не создан
                    }
                }

                CloseDialog();
                LoadContracts();

                MessageBox.Show("Договор успешно сохранен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                // REFACTOR: Обработка валидационных ошибок из сервиса
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void DeleteContract()
        {
            if (SelectedContract == null) return;
            
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить договор {SelectedContract.ContractNumber}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _dataService.DeleteContract(SelectedContract.Id);
                LoadContracts();
                MessageBox.Show("Договор удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void CloseDialog()
        {
            IsDialogOpen = false;
            EditingContract = null;
            SelectedAsset = null;
        }
        
        private void ClearStatusFilter()
        {
            FilterStatus = null;
            _filterCounterpartyId = null;
            SearchText = string.Empty;
        }
    }
}