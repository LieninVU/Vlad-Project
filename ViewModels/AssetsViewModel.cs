using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ForVlad.Data;
using ForVlad.Models;

namespace ForVlad.ViewModels
{
    public class AssetsViewModel : ViewModelBase
    {
        private readonly ISimpleDataService _dataService;
        
        private ObservableCollection<Asset> _assets;
        public ObservableCollection<Asset> Assets
        {
            get => _assets;
            set => SetField(ref _assets, value);
        }
        
        private ObservableCollection<Asset> _filteredAssets;
        public ObservableCollection<Asset> FilteredAssets
        {
            get => _filteredAssets;
            set => SetField(ref _filteredAssets, value);
        }
        
        private Asset _selectedAsset;
        public Asset SelectedAsset
        {
            get => _selectedAsset;
            set => SetField(ref _selectedAsset, value);
        }
        
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetField(ref _searchText, value))
                {
                    FilterAssets();
                }
            }
        }
        
        private AssetGroup _filterGroup = AssetGroup.Vehicle;
        public AssetGroup FilterGroup
        {
            get => _filterGroup;
            set
            {
                if (SetField(ref _filterGroup, value))
                {
                    FilterAssets();
                }
            }
        }
        
        private string _filterSubcategory;
        public string FilterSubcategory
        {
            get => _filterSubcategory;
            set
            {
                if (SetField(ref _filterSubcategory, value))
                {
                    FilterAssets();
                }
            }
        }
        
        private bool _isDialogOpen;
        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set => SetField(ref _isDialogOpen, value);
        }
        
        private Asset _editingAsset;
        public Asset EditingAsset
        {
            get => _editingAsset;
            set
            {
                if (SetField(ref _editingAsset, value))
                    OnPropertyChanged(nameof(DialogAssetGroup));
            }
        }
        
        private string _dialogTitle;
        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetField(ref _dialogTitle, value);
        }
        
        public AssetGroup DialogAssetGroup
        {
            get => EditingAsset?.AssetGroup ?? AssetGroup.Vehicle;
            set
            {
                if (EditingAsset == null || EditingAsset.AssetGroup == value)
                    return;
                
                EditingAsset.AssetGroup = value;
                UpdateDialogSubcategories();
                if (DialogSubcategories.Count > 0)
                    EditingAsset.Subcategory = DialogSubcategories[0];
                
                OnPropertyChanged();
                OnPropertyChanged(nameof(EditingAsset));
            }
        }
        
        public ObservableCollection<AssetGroup> Categories { get; }
        public ObservableCollection<string> Subcategories { get; }
        public ObservableCollection<string> DialogSubcategories { get; }
        
        public int TotalAssetsCount => FilteredAssets.Count;
        public int AvailableAssetsCount => FilteredAssets.Count(a => a.IsAvailable);
        public int BusyAssetsCount => FilteredAssets.Count(a => !a.IsAvailable);
        
        public bool HasAssets => FilteredAssets.Count > 0;
        
        public ICommand AddAssetCommand { get; }
        public ICommand EditAssetCommand { get; }
        public ICommand EditAssetFromRowCommand { get; }
        public ICommand DeleteAssetCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleAvailabilityCommand { get; }
        public ICommand SaveAssetCommand { get; }
        public ICommand CancelCommand { get; }
        
        public AssetsViewModel(ISimpleDataService dataService)
        {
            _dataService = dataService;
            Assets = new ObservableCollection<Asset>();
            FilteredAssets = new ObservableCollection<Asset>();
            Subcategories = new ObservableCollection<string>();
            DialogSubcategories = new ObservableCollection<string>();
            Categories = new ObservableCollection<AssetGroup> { AssetGroup.Vehicle, AssetGroup.Equipment };
            AddAssetCommand = new RelayCommand(_ => OpenAddDialog());
            EditAssetCommand = new RelayCommand(_ => OpenEditDialog(), _ => SelectedAsset != null);
            EditAssetFromRowCommand = new RelayCommand(OpenEditFromRow);
            DeleteAssetCommand = new RelayCommand(_ => DeleteAsset(), _ => SelectedAsset != null);
            RefreshCommand = new RelayCommand(_ => LoadAssets());
            ToggleAvailabilityCommand = new RelayCommand(_ => ToggleAvailability(), _ => SelectedAsset != null);
            SaveAssetCommand = new RelayCommand(_ => SaveAsset());
            CancelCommand = new RelayCommand(_ => CloseDialog());
            
            FilterSubcategory = "Все";
            LoadAssets();
        }
        
        public void LoadAssets()
        {
            var assets = _dataService.GetAssets();
            Assets.Clear();
            foreach (var asset in assets)
            {
                Assets.Add(asset);
            }
            
            UpdateSubcategories();
            FilterAssets();
            OnPropertyChanged(nameof(HasAssets));
        }
        
        private void UpdateSubcategories()
        {
            Subcategories.Clear();
            Subcategories.Add("Все");
            
            var uniqueSubcategories = Assets
                .Where(a => !string.IsNullOrEmpty(a.Subcategory))
                .Select(a => a.Subcategory)
                .Distinct()
                .OrderBy(s => s);
            
            foreach (var subcategory in uniqueSubcategories)
            {
                Subcategories.Add(subcategory);
            }
            
            if (string.IsNullOrEmpty(FilterSubcategory) || !Subcategories.Contains(FilterSubcategory))
                FilterSubcategory = "Все";
        }
        
        private void UpdateDialogSubcategories()
        {
            DialogSubcategories.Clear();
            
            if (EditingAsset == null)
                return;
            
            if (EditingAsset.AssetGroup == AssetGroup.Vehicle)
            {
                foreach (VehicleSubcategory item in Enum.GetValues(typeof(VehicleSubcategory)))
                    DialogSubcategories.Add(item.ToString());
            }
            else
            {
                foreach (EquipmentSubcategory item in Enum.GetValues(typeof(EquipmentSubcategory)))
                    DialogSubcategories.Add(item.ToString());
            }
        }
        
        private void FilterAssets()
        {
            FilteredAssets.Clear();
            
            var filtered = Assets.AsEnumerable();
            
            filtered = filtered.Where(a => a.AssetGroup == FilterGroup);
            
            if (!string.IsNullOrEmpty(FilterSubcategory) && FilterSubcategory != "Все")
                filtered = filtered.Where(a => a.Subcategory == FilterSubcategory);
            
            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(a => 
                    (a.Name?.ToLower().Contains(searchLower) ?? false) ||
                    (a.InventoryNumber?.ToLower().Contains(searchLower) ?? false) ||
                    (a.Manufacturer?.ToLower().Contains(searchLower) ?? false) ||
                    (a.Model?.ToLower().Contains(searchLower) ?? false));
            }
            
            foreach (var asset in filtered)
            {
                FilteredAssets.Add(asset);
            }
            
            OnPropertyChanged(nameof(TotalAssetsCount));
            OnPropertyChanged(nameof(AvailableAssetsCount));
            OnPropertyChanged(nameof(BusyAssetsCount));
            OnPropertyChanged(nameof(HasAssets));
        }
        
        private void OpenAddDialog()
        {
            EditingAsset = new Asset
            {
                Name = "",
                InventoryNumber = $"ТЕХ-{DateTime.Now:yyyyMMdd}-{new Random().Next(1, 999):000}",
                AssetGroup = AssetGroup.Vehicle,
                Subcategory = VehicleSubcategory.ConstructionRoad.ToString(),
                Manufacturer = "",
                Model = "",
                YearOfManufacture = DateTime.Now.Year,
                PurchasePrice = 0,
                ResidualValue = 0,
                MonthlyRentalRate = 0,
                HourlyRate = 0,
                DailyRate = 0,
                IsAvailable = true,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };

            UpdateDialogSubcategories();
            DialogTitle = "Новая техника";
            OnPropertyChanged(nameof(EditingAsset));
            IsDialogOpen = true;
        }
        
        private void OpenEditDialog()
        {
            if (SelectedAsset == null)
                return;
            
            EditingAsset = CloneAsset(SelectedAsset);
            UpdateDialogSubcategories();
            DialogTitle = "Редактирование техники";
            OnPropertyChanged(nameof(EditingAsset));
            IsDialogOpen = true;
        }
        
        private void OpenEditFromRow(object parameter)
        {
            if (parameter is Asset asset)
            {
                SelectedAsset = asset;
                OpenEditDialog();
            }
        }
        
        private static Asset CloneAsset(Asset source)
        {
            return new Asset
            {
                Id = source.Id,
                Name = source.Name,
                InventoryNumber = source.InventoryNumber,
                AssetGroup = source.AssetGroup,
                Subcategory = source.Subcategory,
                Manufacturer = source.Manufacturer,
                Model = source.Model,
                SerialNumber = source.SerialNumber,
                YearOfManufacture = source.YearOfManufacture,
                PurchasePrice = source.PurchasePrice,
                ResidualValue = source.ResidualValue,
                MonthlyRentalRate = source.MonthlyRentalRate,
                HourlyRate = source.HourlyRate,
                DailyRate = source.DailyRate,
                IsAvailable = source.IsAvailable,
                Description = source.Description,
                // REFACTOR: Заменено Notes на Description
                CreatedDate = source.CreatedDate,
                ModifiedDate = source.ModifiedDate,
                IsDeleted = source.IsDeleted,
                EnginePower = source.EnginePower,
                RegistrationNumber = source.RegistrationNumber,
                Weight = source.Weight,
                PowerRequirements = source.PowerRequirements
            };
        }
        
        private void SaveAsset()
        {
            if (EditingAsset == null)
                return;
            
            // REFACTOR: UI валидация для полей с ограничениями CHECK в БД
            if (string.IsNullOrWhiteSpace(EditingAsset.InventoryNumber))
            {
                MessageBox.Show("Инвентарный номер должен быть указан. Укажите корректный инвентарный номер техники.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingAsset.Name))
            {
                MessageBox.Show("Введите наименование техники", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingAsset.Subcategory))
            {
                MessageBox.Show("Выберите подкатегорию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (EditingAsset.HourlyRate <= 0 && EditingAsset.DailyRate <= 0 && EditingAsset.MonthlyRentalRate <= 0)
            {
                MessageBox.Show("Хотя бы одно из значений (Почасовая ставка, Дневная ставка или Месячная аренда) должно быть больше 0.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (EditingAsset.HourlyRate < 0)
            {
                MessageBox.Show("Почасовая ставка должна быть больше 0 или равна 0.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (EditingAsset.DailyRate < 0)
            {
                MessageBox.Show("Дневная ставка должна быть больше 0 или равна 0.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (EditingAsset.EnginePower.HasValue && EditingAsset.EnginePower.Value <= 0)
            {
                MessageBox.Show("Мощность двигателя должна быть больше 0 или не указана.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (EditingAsset.Weight.HasValue && EditingAsset.Weight.Value <= 0)
            {
                MessageBox.Show("Вес должен быть больше 0 или не указан.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (EditingAsset.YearOfManufacture.HasValue)
            {
                int currentYear = DateTime.Now.Year;
                if (EditingAsset.YearOfManufacture.Value < 1900 || EditingAsset.YearOfManufacture.Value > currentYear)
                {
                    MessageBox.Show(string.Format("Год выпуска должен быть между 1900 и {0} годом.", currentYear),
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            
            try
            {
                EditingAsset.ModifiedDate = DateTime.Now;
                _dataService.SaveAsset(EditingAsset);
                CloseDialog();
                LoadAssets();
                
                MessageBox.Show("Техника успешно сохранена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                // REFACTOR: Обработка валидационных ошибок из сервиса
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void DeleteAsset()
        {
            if (SelectedAsset == null)
                return;
            
            var result = MessageBox.Show(
                $"Удалить технику «{SelectedAsset.Name}» ({SelectedAsset.InventoryNumber})?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _dataService.DeleteAsset(SelectedAsset.Id);
                LoadAssets();
            }
        }
        
        private void ToggleAvailability()
        {
            if (SelectedAsset == null)
                return;
            
            try
            {
                SelectedAsset.IsAvailable = !SelectedAsset.IsAvailable;
                SelectedAsset.ModifiedDate = DateTime.Now;
                _dataService.SaveAsset(SelectedAsset);
                LoadAssets();
            }
            catch (InvalidOperationException ex)
            {
                // REFACTOR: Обработка валидационных ошибок из сервиса
                SelectedAsset.IsAvailable = !SelectedAsset.IsAvailable; // Откатываем изменение
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void CloseDialog()
        {
            IsDialogOpen = false;
            EditingAsset = null;
            DialogSubcategories.Clear();
        }
    }
}
