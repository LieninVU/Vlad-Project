using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ForVlad.Data;
using ForVlad.Models;

namespace ForVlad.ViewModels
{
    public class ContractDetailsViewModel : ViewModelBase
    {
        private readonly ISimpleDataService _dataService;
        private readonly int _contractId;
        
        // Спецификации
        private ObservableCollection<ContractSpecification> _specifications;
        public ObservableCollection<ContractSpecification> Specifications
        {
            get => _specifications;
            set => SetField(ref _specifications, value);
        }
        
        private ContractSpecification _selectedSpecification;
        public ContractSpecification SelectedSpecification
        {
            get => _selectedSpecification;
            set => SetField(ref _selectedSpecification, value);
        }
        
        // Платежи
        private ObservableCollection<PaymentSchedule> _payments;
        public ObservableCollection<PaymentSchedule> Payments
        {
            get => _payments;
            set => SetField(ref _payments, value);
        }
        
        private PaymentSchedule _selectedPayment;
        public PaymentSchedule SelectedPayment
        {
            get => _selectedPayment;
            set => SetField(ref _selectedPayment, value);
        }
        
        // Справочники
        public ObservableCollection<Asset> Assets { get; }
        public ObservableCollection<PeriodType> PeriodTypes { get; }
        
        // Для редактирования спецификации
        private bool _isSpecificationDialogOpen;
        public bool IsSpecificationDialogOpen
        {
            get => _isSpecificationDialogOpen;
            set => SetField(ref _isSpecificationDialogOpen, value);
        }
        
        private ContractSpecification _editingSpecification;
        public ContractSpecification EditingSpecification
        {
            get => _editingSpecification;
            set => SetField(ref _editingSpecification, value);
        }
        
        private string _specificationDialogTitle;
        public string SpecificationDialogTitle
        {
            get => _specificationDialogTitle;
            set => SetField(ref _specificationDialogTitle, value);
        }
        
        // Для редактирования платежа
        private bool _isPaymentDialogOpen;
        public bool IsPaymentDialogOpen
        {
            get => _isPaymentDialogOpen;
            set => SetField(ref _isPaymentDialogOpen, value);
        }
        
        private PaymentSchedule _editingPayment;
        public PaymentSchedule EditingPayment
        {
            get => _editingPayment;
            set => SetField(ref _editingPayment, value);
        }
        
        private string _paymentDialogTitle;
        public string PaymentDialogTitle
        {
            get => _paymentDialogTitle;
            set => SetField(ref _paymentDialogTitle, value);
        }
        
        // Для генерации графика
        private bool _isGenerateScheduleDialogOpen;
        public bool IsGenerateScheduleDialogOpen
        {
            get => _isGenerateScheduleDialogOpen;
            set => SetField(ref _isGenerateScheduleDialogOpen, value);
        }
        
        private int _generatedPaymentCount;
        public int GeneratedPaymentCount
        {
            get => _generatedPaymentCount;
            set => SetField(ref _generatedPaymentCount, value);
        }
        
        private DateTime _generatedStartDate;
        public DateTime GeneratedStartDate
        {
            get => _generatedStartDate;
            set => SetField(ref _generatedStartDate, value);
        }
        
        private decimal _generatedAmountPerPayment;
        public decimal GeneratedAmountPerPayment
        {
            get => _generatedAmountPerPayment;
            set => SetField(ref _generatedAmountPerPayment, value);
        }
        
        // Команды спецификаций
        public ICommand AddSpecificationCommand { get; }
        public ICommand EditSpecificationCommand { get; }
        public ICommand DeleteSpecificationCommand { get; }
        public ICommand SaveSpecificationCommand { get; }
        public ICommand CancelSpecificationCommand { get; }
        public ICommand ReturnAssetCommand { get; }
        
        // Команды платежей
        public ICommand AddPaymentCommand { get; }
        public ICommand EditPaymentCommand { get; }
        public ICommand DeletePaymentCommand { get; }
        public ICommand MarkPaidCommand { get; }
        public ICommand SavePaymentCommand { get; }
        public ICommand CancelPaymentCommand { get; }
        
        // Команды генерации
        public ICommand OpenGenerateScheduleCommand { get; }
        public ICommand GenerateScheduleCommand { get; }
        public ICommand CancelGenerateScheduleCommand { get; }
        public ICommand BackCommand { get; }

        private Action _navigateBack;
        
        public bool HasSpecifications => Specifications != null && Specifications.Count > 0;
        public bool HasPayments => Payments != null && Payments.Count > 0;
        public decimal TotalSpecificationsAmount => Specifications?.Sum(s => s.TotalPrice) ?? 0;
        public decimal TotalPaymentsAmount => Payments?.Sum(p => p.Amount) ?? 0;
        public decimal PaidAmount => Payments?.Where(p => p.IsPaid).Sum(p => p.Amount) ?? 0;

        public void SetNavigateBack(Action navigateBack)
        {
            _navigateBack = navigateBack;
        }

        public ContractDetailsViewModel(ISimpleDataService dataService, int contractId)
        {
            _dataService = dataService;
            _contractId = contractId;
            
            Specifications = new ObservableCollection<ContractSpecification>();
            Payments = new ObservableCollection<PaymentSchedule>();
            Assets = new ObservableCollection<Asset>();
            PeriodTypes = new ObservableCollection<PeriodType>();
            
            foreach (PeriodType type in Enum.GetValues(typeof(PeriodType)))
            {
                PeriodTypes.Add(type);
            }
            
            // Инициализация команд спецификаций
            AddSpecificationCommand = new RelayCommand(_ => OpenAddSpecificationDialog());
            EditSpecificationCommand = new RelayCommand(_ => OpenEditSpecificationDialog(), _ => SelectedSpecification != null);
            DeleteSpecificationCommand = new RelayCommand(_ => DeleteSpecification(), _ => SelectedSpecification != null);
            SaveSpecificationCommand = new RelayCommand(_ => SaveSpecification());
            CancelSpecificationCommand = new RelayCommand(_ => CloseSpecificationDialog());
            ReturnAssetCommand = new RelayCommand(_ => ReturnAsset(), _ => SelectedSpecification != null);
            
            // Инициализация команд платежей
            AddPaymentCommand = new RelayCommand(_ => OpenAddPaymentDialog());
            EditPaymentCommand = new RelayCommand(_ => OpenEditPaymentDialog(), _ => SelectedPayment != null);
            DeletePaymentCommand = new RelayCommand(_ => DeletePayment(), _ => SelectedPayment != null);
            MarkPaidCommand = new RelayCommand(_ => MarkPaymentPaid(), _ => SelectedPayment != null && !SelectedPayment.IsPaid);
            SavePaymentCommand = new RelayCommand(_ => SavePayment());
            CancelPaymentCommand = new RelayCommand(_ => ClosePaymentDialog());
            
            // Инициализация команд генерации
            OpenGenerateScheduleCommand = new RelayCommand(_ => OpenGenerateScheduleDialog());
            GenerateScheduleCommand = new RelayCommand(_ => GenerateSchedule());
            CancelGenerateScheduleCommand = new RelayCommand(_ => CloseGenerateScheduleDialog());
            BackCommand = new RelayCommand(_ => _navigateBack?.Invoke());
            
            LoadData();
        }
        
        private void LoadData()
        {
            // Загружаем технику
            var assets = _dataService.GetAssets();
            Assets.Clear();
            foreach (var asset in assets)
            {
                Assets.Add(asset);
            }
            
            // Загружаем спецификации
            var specs = _dataService.GetSpecifications(_contractId);
            Specifications.Clear();
            foreach (var spec in specs)
            {
                spec.Asset = Assets.FirstOrDefault(a => a.Id == spec.AssetId);
                Specifications.Add(spec);
            }
            
            // Загружаем платежи
            var payments = _dataService.GetPaymentSchedules(_contractId);
            Payments.Clear();
            foreach (var payment in payments)
            {
                Payments.Add(payment);
            }
            
            UpdateTotals();
        }
        
        private void UpdateTotals()
        {
            OnPropertyChanged(nameof(HasSpecifications));
            OnPropertyChanged(nameof(HasPayments));
            OnPropertyChanged(nameof(TotalSpecificationsAmount));
            OnPropertyChanged(nameof(TotalPaymentsAmount));
            OnPropertyChanged(nameof(PaidAmount));
        }
        
        #region Specifications CRUD
        
        private void OpenAddSpecificationDialog()
        {
            var defaultAssetId = Assets.Count > 0 ? Assets[0].Id : 0;
            EditingSpecification = new ContractSpecification
            {
                ContractId = _contractId,
                AssetId = defaultAssetId,
                Quantity = 1,
                UnitPrice = 0,
                PeriodType = PeriodType.Month
            };
            SpecificationDialogTitle = "Добавить позицию";
            IsSpecificationDialogOpen = true;
        }
        
        private void OpenEditSpecificationDialog()
        {
            if (SelectedSpecification == null) return;
            
            EditingSpecification = new ContractSpecification
            {
                Id = SelectedSpecification.Id,
                ContractId = SelectedSpecification.ContractId,
                AssetId = SelectedSpecification.AssetId,
                Quantity = SelectedSpecification.Quantity,
                UnitPrice = SelectedSpecification.UnitPrice,
                PeriodType = SelectedSpecification.PeriodType,
                AdditionalConditions = SelectedSpecification.AdditionalConditions
            };
            SpecificationDialogTitle = "Редактировать позицию";
            IsSpecificationDialogOpen = true;
        }
        
        private void SaveSpecification()
        {
            if (EditingSpecification == null) return;
            
            if (EditingSpecification.AssetId <= 0)
            {
                MessageBox.Show("Выберите технику", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (EditingSpecification.Quantity <= 0)
            {
                MessageBox.Show("Количество должно быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (EditingSpecification.UnitPrice <= 0)
            {
                MessageBox.Show("Цена должна быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            try
            {
                _dataService.SaveSpecification(EditingSpecification);
                CloseSpecificationDialog();
                LoadData();
                MessageBox.Show("Позиция сохранена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void DeleteSpecification()
        {
            if (SelectedSpecification == null) return;
            
            var result = MessageBox.Show(
                $"Удалить позицию?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _dataService.DeleteSpecification(SelectedSpecification.Id);
                LoadData();
            }
        }
        
        private void CloseSpecificationDialog()
        {
            IsSpecificationDialogOpen = false;
            EditingSpecification = null;
        }

        private void ReturnAsset()
        {
            if (SelectedSpecification == null) return;

            var asset = _dataService.GetAsset(SelectedSpecification.AssetId);
            if (asset == null) return;

            var result = MessageBox.Show(
                $"Вернуть технику {asset.Name} и сделать её доступной для аренды?\n\n" +
                "Спецификация будет удалена из договора.",
                "Подтверждение возврата",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем спецификацию
                    _dataService.DeleteSpecification(SelectedSpecification.Id);

                    // Делаем технику доступной
                    asset.IsAvailable = true;
                    asset.ModifiedDate = DateTime.Now;
                    _dataService.SaveAsset(asset);

                    LoadData();
                    MessageBox.Show($"Техника {asset.Name} возвращена и теперь доступна для аренды.",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при возврате техники: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion
        
        #region Payments CRUD
        
        private void OpenAddPaymentDialog()
        {
            EditingPayment = new PaymentSchedule
            {
                ContractId = _contractId,
                DueDate = DateTime.Today.AddMonths(1),
                Amount = 0,
                IsPaid = false,
                Status = PaymentStatus.Pending
            };
            PaymentDialogTitle = "Добавить платёж";
            IsPaymentDialogOpen = true;
        }
        
        private void OpenEditPaymentDialog()
        {
            if (SelectedPayment == null) return;
            
            EditingPayment = new PaymentSchedule
            {
                Id = SelectedPayment.Id,
                ContractId = SelectedPayment.ContractId,
                PaymentNumber = SelectedPayment.PaymentNumber,
                Description = SelectedPayment.Description,
                DueDate = SelectedPayment.DueDate,
                Amount = SelectedPayment.Amount,
                IsPaid = SelectedPayment.IsPaid,
                PaymentDate = SelectedPayment.PaymentDate,
                PaymentMethod = SelectedPayment.PaymentMethod,
                Notes = SelectedPayment.Notes,
                Status = SelectedPayment.Status
            };
            PaymentDialogTitle = "Редактировать платёж";
            IsPaymentDialogOpen = true;
        }
        
        private void SavePayment()
        {
            if (EditingPayment == null) return;
            
            if (EditingPayment.Amount <= 0)
            {
                MessageBox.Show("Сумма должна быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            try
            {
                _dataService.SavePaymentSchedule(EditingPayment);
                ClosePaymentDialog();
                LoadData();
                MessageBox.Show("Платёж сохранён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void DeletePayment()
        {
            if (SelectedPayment == null) return;
            
            var result = MessageBox.Show(
                $"Удалить платёж №{SelectedPayment.PaymentNumber}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _dataService.DeletePaymentSchedule(SelectedPayment.Id);
                LoadData();
            }
        }
        
        private void MarkPaymentPaid()
        {
            if (SelectedPayment == null || SelectedPayment.IsPaid) return;
            
            var result = MessageBox.Show(
                $"Отметить платёж №{SelectedPayment.PaymentNumber} как оплаченный?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _dataService.MarkPaymentPaid(SelectedPayment.Id);
                LoadData();
            }
        }
        
        private void ClosePaymentDialog()
        {
            IsPaymentDialogOpen = false;
            EditingPayment = null;
        }
        
        #endregion
        
        #region Generate Schedule
        
        private void OpenGenerateScheduleDialog()
        {
            var contract = _dataService.GetContract(_contractId);
            GeneratedPaymentCount = contract?.DurationMonths ?? 1;
            GeneratedStartDate = contract?.StartDate ?? DateTime.Today;
            GeneratedAmountPerPayment = contract != null && GeneratedPaymentCount > 0 
                ? contract.TotalAmount / GeneratedPaymentCount 
                : 0;
            IsGenerateScheduleDialogOpen = true;
        }
        
        private void GenerateSchedule()
        {
            if (GeneratedPaymentCount <= 0)
            {
                MessageBox.Show("Количество платежей должно быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (GeneratedAmountPerPayment <= 0)
            {
                MessageBox.Show("Сумма платежа должна быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            var result = MessageBox.Show(
                $"Создать график из {GeneratedPaymentCount} платежей по {GeneratedAmountPerPayment:N2} ₽?\n\nСуществующие платежи будут удалены.",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result != MessageBoxResult.Yes) return;
            
            try
            {
                _dataService.GeneratePaymentSchedule(_contractId, GeneratedPaymentCount, GeneratedStartDate, GeneratedAmountPerPayment);
                CloseGenerateScheduleDialog();
                LoadData();
                MessageBox.Show("График платежей создан", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void CloseGenerateScheduleDialog()
        {
            IsGenerateScheduleDialogOpen = false;
        }
        
        #endregion
    }
}
