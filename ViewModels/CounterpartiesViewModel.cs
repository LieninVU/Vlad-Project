using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ForVlad.Data;
using ForVlad.Models;

namespace ForVlad.ViewModels
{
    public class CounterpartiesViewModel : ViewModelBase
    {
        private readonly ISimpleDataService _dataService;
        
        public Action<int> NavigateToCounterpartyContracts { get; set; }
        
        private ObservableCollection<Counterparty> _counterparties;
        public ObservableCollection<Counterparty> Counterparties
        {
            get => _counterparties;
            set => SetField(ref _counterparties, value);
        }
        
        private ObservableCollection<Counterparty> _filteredCounterparties;
        public ObservableCollection<Counterparty> FilteredCounterparties
        {
            get => _filteredCounterparties;
            set => SetField(ref _filteredCounterparties, value);
        }
        
        private Counterparty _selectedCounterparty;
        public Counterparty SelectedCounterparty
        {
            get => _selectedCounterparty;
            set => SetField(ref _selectedCounterparty, value);
        }
        
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetField(ref _searchText, value))
                    FilterCounterparties();
            }
        }
        
        private bool _isDialogOpen;
        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set => SetField(ref _isDialogOpen, value);
        }
        
        private Counterparty _editingCounterparty;
        public Counterparty EditingCounterparty
        {
            get => _editingCounterparty;
            set => SetField(ref _editingCounterparty, value);
        }
        
        private string _dialogTitle;
        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetField(ref _dialogTitle, value);
        }
        
        public ObservableCollection<CounterpartyType> CounterpartyTypes { get; }
        
        public bool HasCounterparties => FilteredCounterparties.Count > 0;
        
        public ICommand AddCounterpartyCommand { get; }
        public ICommand EditCounterpartyCommand { get; }
        public ICommand EditCounterpartyFromRowCommand { get; }
        public ICommand DeleteCounterpartyCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewContractsCommand { get; }
        public ICommand SaveCounterpartyCommand { get; }
        public ICommand CancelCommand { get; }
        
        public CounterpartiesViewModel(ISimpleDataService dataService)
        {
            _dataService = dataService;
            Counterparties = new ObservableCollection<Counterparty>();
            FilteredCounterparties = new ObservableCollection<Counterparty>();
            CounterpartyTypes = new ObservableCollection<CounterpartyType>();
            
            foreach (CounterpartyType type in Enum.GetValues(typeof(CounterpartyType)))
                CounterpartyTypes.Add(type);
            
            AddCounterpartyCommand = new RelayCommand(_ => OpenAddDialog());
            EditCounterpartyCommand = new RelayCommand(_ => OpenEditDialog(), _ => SelectedCounterparty != null);
            EditCounterpartyFromRowCommand = new RelayCommand(OpenEditFromRow);
            DeleteCounterpartyCommand = new RelayCommand(_ => DeleteCounterparty(), _ => SelectedCounterparty != null);
            RefreshCommand = new RelayCommand(_ => LoadCounterparties());
            ViewContractsCommand = new RelayCommand(_ => ViewContracts(), _ => SelectedCounterparty != null);
            SaveCounterpartyCommand = new RelayCommand(_ => SaveCounterparty());
            CancelCommand = new RelayCommand(_ => CloseDialog());
            
            LoadCounterparties();
        }
        
        public void LoadCounterparties()
        {
            var counterparties = _dataService.GetCounterparties();
            Counterparties.Clear();
            foreach (var counterparty in counterparties)
                Counterparties.Add(counterparty);
            
            FilterCounterparties();
        }
        
        private void FilterCounterparties()
        {
            FilteredCounterparties.Clear();
            
            var filtered = Counterparties.AsEnumerable();
            
            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(c =>
                    (c.Name?.ToLower().Contains(searchLower) ?? false) ||
                    (c.INN?.ToLower().Contains(searchLower) ?? false) ||
                    (c.ContactPerson?.ToLower().Contains(searchLower) ?? false) ||
                    (c.Phone?.ToLower().Contains(searchLower) ?? false) ||
                    (c.Email?.ToLower().Contains(searchLower) ?? false));
            }
            
            foreach (var counterparty in filtered)
                FilteredCounterparties.Add(counterparty);
            
            OnPropertyChanged(nameof(HasCounterparties));
        }
        
        private void OpenAddDialog()
        {
            EditingCounterparty = new Counterparty
            {
                Name = "",
                CounterpartyType = CounterpartyType.LegalEntity,
                INN = "",
                ContactPerson = "",
                Phone = "",
                Email = "",
                IsActive = true,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };
            DialogTitle = "Новый контрагент";
            IsDialogOpen = true;
        }
        
        private void OpenEditDialog()
        {
            if (SelectedCounterparty == null)
                return;
            
            EditingCounterparty = CloneCounterparty(SelectedCounterparty);
            DialogTitle = "Редактирование контрагента";
            IsDialogOpen = true;
        }
        
        private void OpenEditFromRow(object parameter)
        {
            if (parameter is Counterparty counterparty)
            {
                SelectedCounterparty = counterparty;
                OpenEditDialog();
            }
        }
        
        private static Counterparty CloneCounterparty(Counterparty source)
        {
            return new Counterparty
            {
                Id = source.Id,
                Name = source.Name,
                CounterpartyType = source.CounterpartyType,
                INN = source.INN,
                KPP = source.KPP,
                OGRN = source.OGRN,
                LegalAddress = source.LegalAddress,
                ActualAddress = source.ActualAddress,
                ContactPerson = source.ContactPerson,
                Phone = source.Phone,
                Email = source.Email,
                Notes = source.Notes,
                CreatedDate = source.CreatedDate,
                ModifiedDate = source.ModifiedDate,
                IsDeleted = source.IsDeleted
            };
        }
        
        private void SaveCounterparty()
        {
            if (EditingCounterparty == null)
                return;
            
            if (string.IsNullOrWhiteSpace(EditingCounterparty.Name))
            {
                MessageBox.Show("Введите наименование контрагента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingCounterparty.INN))
            {
                MessageBox.Show("Введите ИНН", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            var duplicateInn = Counterparties.Any(c =>
                c.INN == EditingCounterparty.INN && c.Id != EditingCounterparty.Id);
            if (duplicateInn)
            {
                MessageBox.Show("Контрагент с таким ИНН уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // REFACTOR: UI валидация для полей с ограничениями CHECK в БД
            if (!string.IsNullOrEmpty(EditingCounterparty.KPP) && EditingCounterparty.KPP.Length != 9)
            {
                MessageBox.Show("КПП должен содержать ровно 9 символов или быть пустым.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (!string.IsNullOrEmpty(EditingCounterparty.Email))
            {
                if (!EditingCounterparty.Email.Contains("@") || !EditingCounterparty.Email.Contains("."))
                {
                    MessageBox.Show("Электронная почта должна содержать символы '@' и '.' или быть пустой.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            
            try
            {
                EditingCounterparty.ModifiedDate = DateTime.Now;
                _dataService.SaveCounterparty(EditingCounterparty);
                CloseDialog();
                LoadCounterparties();
                
                MessageBox.Show("Контрагент успешно сохранён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                // REFACTOR: Обработка валидационных ошибок из сервиса
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void DeleteCounterparty()
        {
            if (SelectedCounterparty == null)
                return;
            
            var activeContracts = _dataService.GetContracts()
                .Count(c => c.CounterpartyId == SelectedCounterparty.Id &&
                            !c.IsDeleted);
            
            if (activeContracts > 0)
            {
                MessageBox.Show(
                    "Нельзя удалить контрагента с активными договорами. Завершите или расторгните договоры.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            
            var result = MessageBox.Show(
                $"Деактивировать контрагента «{SelectedCounterparty.Name}»?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dataService.DeleteCounterparty(SelectedCounterparty.Id);
                    LoadCounterparties();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        
        private void ViewContracts()
        {
            if (SelectedCounterparty == null)
                return;
            
            NavigateToCounterpartyContracts?.Invoke(SelectedCounterparty.Id);
        }
        
        private void CloseDialog()
        {
            IsDialogOpen = false;
            EditingCounterparty = null;
        }
    }
}
