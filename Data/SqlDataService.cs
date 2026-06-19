using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ForVlad.Models;
using ForVlad.Models.Reports;
using ForVlad.Services;

namespace ForVlad.Data
{
    /// <summary>
    /// Сервис данных с прямым подключением к SQL Server через ADO.NET
    /// Используется как основная реализация ISimpleDataService
    /// </summary>
    public class SqlDataService : ISimpleDataService
    {
        private readonly string _connectionString;
        
        public SqlDataService()
        {
            _connectionString = DatabaseConnection.GetConnectionString();
        }
        
        #region Counterparties
        
        public List<Counterparty> GetCounterparties()
        {
            var counterparties = new List<Counterparty>();
            
            // Используем реальные имена столбцов из БД: IsActive, CreatedAt, UpdatedAt
            // В БД нет OGRN, убрали из запроса
            string sql = @"
                SELECT Id, Name, Inn, Kpp, 
                       LegalAddress, ActualAddress, ContactPerson, 
                       Phone, Email, Notes, [CounterpartyType],
                       IsActive, CreatedAt, UpdatedAt
                FROM Counterparties 
                WHERE IsActive = 1
                ORDER BY Name";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            counterparties.Add(MapCounterparty(reader));
                        }
                    }
                }
            }
            
            return counterparties;
        }
        
        public Counterparty GetCounterparty(int id)
        {
            string sql = @"
                SELECT Id, Name, Inn, Kpp, 
                       LegalAddress, ActualAddress, ContactPerson, 
                       Phone, Email, Notes, [CounterpartyType],
                       IsActive, CreatedAt, UpdatedAt
                FROM Counterparties 
                WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapCounterparty(reader);
                        }
                    }
                }
            }
            
            return null;
        }
        
        public void SaveCounterparty(Counterparty counterparty)
        {
            if (counterparty.Id == 0)
            {
                InsertCounterparty(counterparty);
            }
            else
            {
                UpdateCounterparty(counterparty);
            }
        }
        
        private void InsertCounterparty(Counterparty counterparty)
        {
            // REFACTOR: Валидация ограничений CHECK из БД
            if (string.IsNullOrEmpty(counterparty.INN))
            {
                throw new InvalidOperationException(
                    "ИНН должен быть указан. Укажите корректный ИНН контрагента.");
            }
            
            if (counterparty.INN.Length < 10 || counterparty.INN.Length > 12)
            {
                throw new InvalidOperationException(
                    "ИНН должен содержать от 10 до 12 символов.");
            }
            
            if (!string.IsNullOrEmpty(counterparty.KPP) && counterparty.KPP.Length != 9)
            {
                throw new InvalidOperationException(
                    "КПП должен содержать ровно 9 символов или быть пустым.");
            }
            
            if (!string.IsNullOrEmpty(counterparty.Email))
            {
                if (!counterparty.Email.Contains("@") || !counterparty.Email.Contains("."))
                {
                    throw new InvalidOperationException(
                        "Электронная почта должна содержать символы '@' и '.' или быть пустой.");
                }
            }
            
            // В БД: Inn, Kpp (с маленькой буквы), IsActive, CreatedAt, UpdatedAt
            // В БД нет OGRN, убрали из INSERT
            string sql = @"
                INSERT INTO Counterparties 
                (Name, Inn, Kpp, LegalAddress, ActualAddress, 
                 ContactPerson, Phone, Email, Notes, [CounterpartyType], IsActive, CreatedAt)
                VALUES 
                (@Name, @Inn, @Kpp, @LegalAddress, @ActualAddress, 
                 @ContactPerson, @Phone, @Email, @Notes, @CounterpartyType, @IsActive, @CreatedAt);
                SELECT SCOPE_IDENTITY();";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    AddCounterpartyParameters(command, counterparty);
                    connection.Open();
                    counterparty.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        
        private void UpdateCounterparty(Counterparty counterparty)
        {
            // REFACTOR: Валидация ограничений CHECK из БД
            if (string.IsNullOrEmpty(counterparty.INN))
            {
                throw new InvalidOperationException(
                    "ИНН должен быть указан. Укажите корректный ИНН контрагента.");
            }
            
            if (counterparty.INN.Length < 10 || counterparty.INN.Length > 12)
            {
                throw new InvalidOperationException(
                    "ИНН должен содержать от 10 до 12 символов.");
            }
            
            if (!string.IsNullOrEmpty(counterparty.KPP) && counterparty.KPP.Length != 9)
            {
                throw new InvalidOperationException(
                    "КПП должен содержать ровно 9 символов или быть пустым.");
            }
            
            if (!string.IsNullOrEmpty(counterparty.Email))
            {
                if (!counterparty.Email.Contains("@") || !counterparty.Email.Contains("."))
                {
                    throw new InvalidOperationException(
                        "Электронная почта должна содержать символы '@' и '.' или быть пустой.");
                }
            }
            
            string sql = @"
                UPDATE Counterparties SET
                    Name = @Name,
                    Inn = @Inn,
                    Kpp = @Kpp,
                    LegalAddress = @LegalAddress,
                    ActualAddress = @ActualAddress,
                    ContactPerson = @ContactPerson,
                    Phone = @Phone,
                    Email = @Email,
                    Notes = @Notes,
                    [CounterpartyType] = @CounterpartyType,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    AddCounterpartyParameters(command, counterparty);
                    command.Parameters.AddWithValue("@Id", counterparty.Id);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        public void DeleteCounterparty(int id)
        {
            // В БД используется IsActive (1=активен, 0=неактивен)
            // В модели IsDeleted (true=удален, false=активен)
            // Устанавливаем IsActive = 0 для "удаления"
            string sql = "UPDATE Counterparties SET IsActive = 0, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        private Counterparty MapCounterparty(SqlDataReader reader)
        {
            // Маппинг из структуры БД в модель
            // БД: Inn -> Model: INN, Kpp -> KPP
            return new Counterparty
            {
                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                INN = reader.IsDBNull(2) ? "" : reader.GetString(2),
                KPP = reader.IsDBNull(3) ? null : reader.GetString(3),
                // В БД нет OGRN, пропускаем
                LegalAddress = reader.IsDBNull(4) ? null : reader.GetString(4),
                ActualAddress = reader.IsDBNull(5) ? null : reader.GetString(5),
                ContactPerson = reader.IsDBNull(6) ? null : reader.GetString(6),
                Phone = reader.IsDBNull(7) ? null : reader.GetString(7),
                Email = reader.IsDBNull(8) ? null : reader.GetString(8),
                Notes = reader.IsDBNull(9) ? null : reader.GetString(9),
                CounterpartyType = reader.IsDBNull(10) ? CounterpartyType.LegalEntity : (CounterpartyType)reader.GetByte(10),
                // IsActive в БД: 1=активен, 0=неактивен
                // IsDeleted в модели: false=активен, true=удален
                IsDeleted = reader.IsDBNull(11) ? false : !reader.GetBoolean(11),
                CreatedDate = reader.IsDBNull(12) ? DateTime.Now : reader.GetDateTime(12),
                ModifiedDate = reader.IsDBNull(13) ? (DateTime?)null : reader.GetDateTime(13)
            };
        }
        
        private void AddCounterpartyParameters(SqlCommand command, Counterparty counterparty)
        {
            // БД использует Inn, Kpp (с маленькой буквы)
            // В БД нет OGRN, пропускаем
            // REFACTOR: Исправление для CHECK ограничения - пустые строки преобразуем в NULL
            command.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(counterparty.Name) ? (object)DBNull.Value : counterparty.Name);
            command.Parameters.AddWithValue("@Inn", string.IsNullOrEmpty(counterparty.INN) ? (object)DBNull.Value : counterparty.INN);
            command.Parameters.AddWithValue("@Kpp", string.IsNullOrEmpty(counterparty.KPP) ? (object)DBNull.Value : counterparty.KPP);
            command.Parameters.AddWithValue("@LegalAddress", string.IsNullOrEmpty(counterparty.LegalAddress) ? (object)DBNull.Value : counterparty.LegalAddress);
            command.Parameters.AddWithValue("@ActualAddress", string.IsNullOrEmpty(counterparty.ActualAddress) ? (object)DBNull.Value : counterparty.ActualAddress);
            command.Parameters.AddWithValue("@ContactPerson", string.IsNullOrEmpty(counterparty.ContactPerson) ? (object)DBNull.Value : counterparty.ContactPerson);
            command.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(counterparty.Phone) ? (object)DBNull.Value : counterparty.Phone);
            command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(counterparty.Email) ? (object)DBNull.Value : counterparty.Email);
            command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(counterparty.Notes) ? (object)DBNull.Value : counterparty.Notes);
            command.Parameters.AddWithValue("@CounterpartyType", (int)counterparty.CounterpartyType);
            // IsActive = !IsDeleted
            command.Parameters.AddWithValue("@IsActive", !counterparty.IsDeleted);
            command.Parameters.AddWithValue("@CreatedAt", counterparty.CreatedDate);
        }
        
        #endregion
        
        #region Assets
        
        public List<Asset> GetAssets()
        {
            var assets = new List<Asset>();
            
            // Реальная структура БД: VehicleBrand, VehicleModel, VinNumber, ManufactureYear,
            // HourlyRate, DailyRate, AssetCondition, IsAvailable, Description, CreatedAt, UpdatedAt
            // Добавляем EnginePower, RegistrationNumber, Weight, PowerRequirements
            string sql = @"
                SELECT Id, InventoryNumber, Name, AssetGroup, VehicleBrand, VehicleModel,
                       VinNumber, ManufactureYear, HourlyRate, DailyRate,
                       AssetCondition, IsAvailable, Description,
                       VehicleSubcategory, EquipmentSubcategory,
                       [EquipmentType], EnginePower, RegistrationNumber, Weight, PowerRequirements,
                       CreatedAt, UpdatedAt, MonthlyRate, PurchasePrice, ResidualValue,
                       Manufacturer, Model
                FROM Assets
                ORDER BY Name";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            assets.Add(MapAsset(reader));
                        }
                    }
                }
            }
            
            return assets;
        }
        
        public Asset GetAsset(int id)
        {
            string sql = @"
                SELECT Id, InventoryNumber, Name, AssetGroup, VehicleBrand, VehicleModel,
                       VinNumber, ManufactureYear, HourlyRate, DailyRate,
                       AssetCondition, IsAvailable, Description,
                       VehicleSubcategory, EquipmentSubcategory,
                       [EquipmentType], EnginePower, RegistrationNumber, Weight, PowerRequirements,
                       CreatedAt, UpdatedAt, MonthlyRate, PurchasePrice, ResidualValue,
                       Manufacturer, Model
                FROM Assets
                WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapAsset(reader);
                        }
                    }
                }
            }
            
            return null;
        }
        
        public void SaveAsset(Asset asset)
        {
            SyncSubcategoryToEnum(asset);

            if (asset.Id == 0)
            {
                InsertAsset(asset);
            }
            else
            {
                UpdateAsset(asset);
            }
        }

        private static void SyncSubcategoryToEnum(Asset asset)
        {
            if (string.IsNullOrEmpty(asset.Subcategory))
                return;

            if (asset.AssetGroup == AssetGroup.Vehicle)
            {
                if (Enum.TryParse(asset.Subcategory, out VehicleSubcategory vs))
                    asset.VehicleSubcategory = vs;
            }
            else
            {
                if (Enum.TryParse(asset.Subcategory, out EquipmentSubcategory es))
                    asset.EquipmentSubcategory = es;
            }
        }
        
        private void InsertAsset(Asset asset)
        {
            // REFACTOR: Валидация ограничений CHECK из БД
            if (string.IsNullOrEmpty(asset.InventoryNumber))
            {
                throw new InvalidOperationException(
                    "Инвентарный номер должен быть указан. Укажите корректный инвентарный номер техники.");
            }
            
            // CRITICAL: CK_Assets_VehicleFields constraint validation
            if (asset.AssetGroup == AssetGroup.Vehicle)
            {
                if (string.IsNullOrEmpty(asset.Manufacturer))
                {
                    throw new InvalidOperationException(
                        "Для транспортного средства необходимо указать марку (Manufacturer).");
                }
                
                if (string.IsNullOrEmpty(asset.Model))
                {
                    throw new InvalidOperationException(
                        "Для транспортного средства необходимо указать модель (Model).");
                }
            }
            else if (asset.AssetGroup == AssetGroup.Equipment)
            {
                // Для оборудования VehicleBrand и VehicleModel должны быть NULL
                // Они будут автоматически очищены в AddAssetParameters
            }
            
            // Проверяем, что хотя бы одна ставка больше 0
            if (asset.HourlyRate <= 0 && asset.DailyRate <= 0 && asset.MonthlyRentalRate <= 0)
            {
                throw new InvalidOperationException(
                    "Хотя бы одно из значений (HourlyRate, DailyRate или MonthlyRentalRate) должно быть больше 0.");
            }
            
            // Проверяем HourlyRate
            if (asset.HourlyRate < 0)
            {
                throw new InvalidOperationException(
                    "Почасовая ставка должна быть больше 0 или не указана.");
            }
            
            // Проверяем DailyRate
            if (asset.DailyRate < 0)
            {
                throw new InvalidOperationException(
                    "Дневная ставка должна быть больше 0 или не указана.");
            }
            
            // Проверяем EnginePower
            if (asset.EnginePower.HasValue && asset.EnginePower.Value <= 0)
            {
                throw new InvalidOperationException(
                    "Мощность двигателя должна быть больше 0 или не указана.");
            }
            
            // Проверяем Weight
            if (asset.Weight.HasValue && asset.Weight.Value <= 0)
            {
                throw new InvalidOperationException(
                    "Вес должен быть больше 0 или не указан.");
            }
            
            // Проверяем год выпуска
            if (asset.YearOfManufacture.HasValue)
            {
                int currentYear = DateTime.Now.Year;
                if (asset.YearOfManufacture.Value < 1900 || asset.YearOfManufacture.Value > currentYear)
                {
                    throw new InvalidOperationException(
                        string.Format("Год выпуска должен быть между 1900 и {0} годом.", currentYear));
                }
            }
            
            // В БД: VehicleBrand, VehicleModel, VinNumber, ManufactureYear, HourlyRate, DailyRate, MonthlyRate
            decimal hourlyRate = asset.HourlyRate;
            decimal dailyRate = asset.DailyRate;
            decimal monthlyRate = asset.MonthlyRentalRate;

            // Если месячная ставка не указана, но указана дневная — вычисляем
            if (monthlyRate <= 0 && dailyRate > 0)
                monthlyRate = dailyRate * 30;
            // Если дневная не указана, но указана месячная — вычисляем
            if (dailyRate <= 0 && monthlyRate > 0)
                dailyRate = monthlyRate / 30;
            // Если почасовая не указана, вычисляем из дневной
            if (hourlyRate <= 0 && dailyRate > 0)
                hourlyRate = dailyRate / 8;

string sql = @"
                INSERT INTO Assets 
                (InventoryNumber, Name, AssetGroup, VehicleBrand, VehicleModel, 
                 VinNumber, ManufactureYear, HourlyRate, DailyRate,
                 AssetCondition, IsAvailable, Description, MonthlyRate,
                 PurchasePrice, ResidualValue, Manufacturer, Model,
                 VehicleSubcategory, EquipmentSubcategory,
                 [EquipmentType], EnginePower, RegistrationNumber, Weight, PowerRequirements,
                 CreatedAt)
                VALUES 
                (@InventoryNumber, @Name, @AssetGroup, @VehicleBrand, @VehicleModel,
                 @VinNumber, @ManufactureYear, @HourlyRate, @DailyRate,
                 @AssetCondition, @IsAvailable, @Description, @MonthlyRate,
                 @PurchasePrice, @ResidualValue, @Manufacturer, @Model,
                 @VehicleSubcategory, @EquipmentSubcategory,
                 @EquipmentType, @EnginePower, @RegistrationNumber, @Weight, @PowerRequirements,
                 @CreatedAt);
                SELECT SCOPE_IDENTITY();";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    AddAssetParameters(command, asset, hourlyRate, dailyRate, monthlyRate);
                    connection.Open();
                    asset.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        
        private void UpdateAsset(Asset asset)
        {
            // REFACTOR: Валидация ограничений CHECK из БД
            if (string.IsNullOrEmpty(asset.InventoryNumber))
            {
                throw new InvalidOperationException(
                    "Инвентарный номер должен быть указан. Укажите корректный инвентарный номер техники.");
            }
            
            // CRITICAL: CK_Assets_VehicleFields constraint validation
            if (asset.AssetGroup == AssetGroup.Vehicle)
            {
                if (string.IsNullOrEmpty(asset.Manufacturer))
                {
                    throw new InvalidOperationException(
                        "Для транспортного средства необходимо указать марку (Manufacturer).");
                }
                
                if (string.IsNullOrEmpty(asset.Model))
                {
                    throw new InvalidOperationException(
                        "Для транспортного средства необходимо указать модель (Model).");
                }
            }
            
            // Проверяем, что хотя бы одна ставка больше 0
            if (asset.HourlyRate <= 0 && asset.DailyRate <= 0 && asset.MonthlyRentalRate <= 0)
            {
                throw new InvalidOperationException(
                    "Хотя бы одно из значений (HourlyRate, DailyRate или MonthlyRentalRate) должно быть больше 0.");
            }
            
            // Проверяем HourlyRate
            if (asset.HourlyRate < 0)
            {
                throw new InvalidOperationException(
                    "Почасовая ставка должна быть больше 0 или не указана.");
            }
            
            // Проверяем DailyRate
            if (asset.DailyRate < 0)
            {
                throw new InvalidOperationException(
                    "Дневная ставка должна быть больше 0 или не указана.");
            }
            
            // Проверяем EnginePower
            if (asset.EnginePower.HasValue && asset.EnginePower.Value <= 0)
            {
                throw new InvalidOperationException(
                    "Мощность двигателя должна быть больше 0 или не указана.");
            }
            
            // Проверяем Weight
            if (asset.Weight.HasValue && asset.Weight.Value <= 0)
            {
                throw new InvalidOperationException(
                    "Вес должен быть больше 0 или не указан.");
            }
            
            // Проверяем год выпуска
            if (asset.YearOfManufacture.HasValue)
            {
                int currentYear = DateTime.Now.Year;
                if (asset.YearOfManufacture.Value < 1900 || asset.YearOfManufacture.Value > currentYear)
                {
                    throw new InvalidOperationException(
                        string.Format("Год выпуска должен быть между 1900 и {0} годом.", currentYear));
                }
            }
            
decimal hourlyRate = asset.HourlyRate;
            decimal dailyRate = asset.DailyRate;
            decimal monthlyRate = asset.MonthlyRentalRate;

            if (monthlyRate <= 0 && dailyRate > 0)
                monthlyRate = dailyRate * 30;
            if (dailyRate <= 0 && monthlyRate > 0)
                dailyRate = monthlyRate / 30;
            if (hourlyRate <= 0 && dailyRate > 0)
                hourlyRate = dailyRate / 8;

string sql = @"
                UPDATE Assets SET
                    InventoryNumber = @InventoryNumber,
                    Name = @Name,
                    AssetGroup = @AssetGroup,
                    VehicleBrand = @VehicleBrand,
                    VehicleModel = @VehicleModel,
                    VinNumber = @VinNumber,
                    ManufactureYear = @ManufactureYear,
                    HourlyRate = @HourlyRate,
                    DailyRate = @DailyRate,
                    MonthlyRate = @MonthlyRate,
                    PurchasePrice = @PurchasePrice,
                    ResidualValue = @ResidualValue,
                    Manufacturer = @Manufacturer,
                    Model = @Model,
                    AssetCondition = @AssetCondition,
                    IsAvailable = @IsAvailable,
                    Description = @Description,
                    VehicleSubcategory = @VehicleSubcategory,
                    EquipmentSubcategory = @EquipmentSubcategory,
                    [EquipmentType] = @EquipmentType,
                    EnginePower = @EnginePower,
                    RegistrationNumber = @RegistrationNumber,
                    Weight = @Weight,
                    PowerRequirements = @PowerRequirements,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    AddAssetParameters(command, asset, hourlyRate, dailyRate, monthlyRate);
                    command.Parameters.AddWithValue("@Id", asset.Id);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        public void DeleteAsset(int id)
        {
            // В БД нет IsDeleted, но есть IsAvailable
            // Помечаем как недоступный (альтернатива удалению)
            // Либо можно использоватьsoft delete через дополнительное поле
            // Пока просто устанавливаем IsAvailable = 0
            string sql = "UPDATE Assets SET IsAvailable = 0, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        private Asset MapAsset(SqlDataReader reader)
        {
            // Маппинг из структуры БД в модель
            // SQL: Id(0), InventoryNumber(1), Name(2), AssetGroup(3), VehicleBrand(4), VehicleModel(5),
            //      VinNumber(6), ManufactureYear(7), HourlyRate(8), DailyRate(9),
            //      AssetCondition(10), IsAvailable(11), Description(12),
            //      VehicleSubcategory(13), EquipmentSubcategory(14),
            //      EquipmentType(15), EnginePower(16), RegistrationNumber(17), Weight(18), PowerRequirements(19),
            //      CreatedAt(20), UpdatedAt(21), MonthlyRate(22), PurchasePrice(23), ResidualValue(24),
            //      Manufacturer(25), Model(26)

            decimal hourlyRate = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8);
            decimal dailyRate = reader.IsDBNull(9) ? 0 : reader.GetDecimal(9);
            decimal monthlyRate = reader.FieldCount > 22 && !reader.IsDBNull(22) ? reader.GetDecimal(22) : 0;
            decimal purchasePrice = reader.FieldCount > 23 && !reader.IsDBNull(23) ? reader.GetDecimal(23) : 0;
            decimal residualValue = reader.FieldCount > 24 && !reader.IsDBNull(24) ? reader.GetDecimal(24) : 0;

            string manufacturer = null;
            string model = null;
            if (reader.FieldCount > 25 && !reader.IsDBNull(25))
                manufacturer = reader.GetString(25);
            else if (!reader.IsDBNull(4))
                manufacturer = reader.GetString(4);
            if (reader.FieldCount > 26 && !reader.IsDBNull(26))
                model = reader.GetString(26);
            else if (!reader.IsDBNull(5))
                model = reader.GetString(5);

            var assetGroup = reader.IsDBNull(3) ? AssetGroup.Vehicle : (AssetGroup)reader.GetByte(3);

            string subcategory = null;
            if (assetGroup == AssetGroup.Vehicle && !reader.IsDBNull(13))
                subcategory = ((VehicleSubcategory)reader.GetByte(13)).ToString();
            else if (assetGroup == AssetGroup.Equipment && !reader.IsDBNull(14))
                subcategory = ((EquipmentSubcategory)reader.GetByte(14)).ToString();

            return new Asset
            {
                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                InventoryNumber = reader.IsDBNull(1) ? null : reader.GetString(1),
                Name = reader.IsDBNull(2) ? "" : reader.GetString(2),
                AssetGroup = assetGroup,
                Subcategory = subcategory,
                Manufacturer = manufacturer,
                Model = model,
                SerialNumber = reader.IsDBNull(6) ? null : reader.GetString(6),
                YearOfManufacture = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                MonthlyRentalRate = monthlyRate > 0 ? monthlyRate : dailyRate * 30,
                PurchasePrice = purchasePrice,
                ResidualValue = residualValue,
                IsAvailable = reader.IsDBNull(11) ? true : reader.GetBoolean(11),
                Description = reader.IsDBNull(12) ? null : reader.GetString(12),
                HourlyRate = hourlyRate,
                DailyRate = dailyRate,
                AssetCondition = reader.IsDBNull(10) ? AssetCondition.Good : (AssetCondition)reader.GetByte(10),
                VehicleSubcategory = reader.IsDBNull(13) ? null : (VehicleSubcategory?)reader.GetByte(13),
                EquipmentSubcategory = reader.IsDBNull(14) ? null : (EquipmentSubcategory?)reader.GetByte(14),
                EquipmentType = reader.IsDBNull(15) ? null : reader.GetString(15),
                EnginePower = reader.IsDBNull(16) ? null : (decimal?)reader.GetDecimal(16),
                RegistrationNumber = reader.IsDBNull(17) ? null : reader.GetString(17),
                Weight = reader.IsDBNull(18) ? null : (decimal?)reader.GetDecimal(18),
                PowerRequirements = reader.IsDBNull(19) ? null : reader.GetString(19),
                CreatedDate = reader.IsDBNull(20) ? DateTime.Now : reader.GetDateTime(20),
                ModifiedDate = reader.IsDBNull(21) ? (DateTime?)null : reader.GetDateTime(21),
                IsDeleted = false
            };
        }
        
        private void AddAssetParameters(SqlCommand command, Asset asset, decimal hourlyRate, decimal dailyRate, decimal monthlyRate)
        {
            // БД использует VehicleBrand, VehicleModel, VinNumber, ManufactureYear
            // REFACTOR: Исправление для CHECK ограничений - пустые строки преобразуем в NULL
            command.Parameters.AddWithValue("@InventoryNumber", string.IsNullOrEmpty(asset.InventoryNumber) ? (object)DBNull.Value : asset.InventoryNumber);
            command.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(asset.Name) ? (object)DBNull.Value : asset.Name);
            command.Parameters.AddWithValue("@AssetGroup", (int)asset.AssetGroup);
            
            // CRITICAL: CK_Assets_VehicleFields constraint requires:
            // - Vehicle (AssetGroup = 0): VehicleBrand AND VehicleModel must be NOT NULL
            // - Equipment (AssetGroup = 1): VehicleBrand AND VehicleModel must be NULL
            bool isVehicle = asset.AssetGroup == AssetGroup.Vehicle;
            string vehicleBrand = isVehicle ? asset.Manufacturer : null;
            string vehicleModel = isVehicle ? asset.Model : null;
            
            command.Parameters.AddWithValue("@VehicleBrand", string.IsNullOrEmpty(vehicleBrand) ? (object)DBNull.Value : vehicleBrand);
            command.Parameters.AddWithValue("@VehicleModel", string.IsNullOrEmpty(vehicleModel) ? (object)DBNull.Value : vehicleModel);
            command.Parameters.AddWithValue("@Manufacturer", string.IsNullOrEmpty(asset.Manufacturer) ? (object)DBNull.Value : asset.Manufacturer);
            command.Parameters.AddWithValue("@Model", string.IsNullOrEmpty(asset.Model) ? (object)DBNull.Value : asset.Model);
            command.Parameters.AddWithValue("@VinNumber", string.IsNullOrEmpty(asset.SerialNumber) ? (object)DBNull.Value : asset.SerialNumber);
            if (asset.YearOfManufacture.HasValue)
                command.Parameters.AddWithValue("@ManufactureYear", asset.YearOfManufacture.Value);
            else
                command.Parameters.AddWithValue("@ManufactureYear", DBNull.Value);
            command.Parameters.AddWithValue("@HourlyRate", hourlyRate);
            command.Parameters.AddWithValue("@DailyRate", dailyRate);
            command.Parameters.AddWithValue("@MonthlyRate", monthlyRate);
            command.Parameters.AddWithValue("@PurchasePrice", asset.PurchasePrice);
            command.Parameters.AddWithValue("@ResidualValue", asset.ResidualValue);
            command.Parameters.AddWithValue("@AssetCondition", (int)asset.AssetCondition);
            command.Parameters.AddWithValue("@IsAvailable", asset.IsAvailable);
            command.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(asset.Description) ? (object)DBNull.Value : asset.Description);
            if (asset.VehicleSubcategory.HasValue)
                command.Parameters.AddWithValue("@VehicleSubcategory", (int)asset.VehicleSubcategory.Value);
            else
                command.Parameters.AddWithValue("@VehicleSubcategory", DBNull.Value);
            if (asset.EquipmentSubcategory.HasValue)
                command.Parameters.AddWithValue("@EquipmentSubcategory", (int)asset.EquipmentSubcategory.Value);
            else
                command.Parameters.AddWithValue("@EquipmentSubcategory", DBNull.Value);
            command.Parameters.AddWithValue("@EquipmentType", string.IsNullOrEmpty(asset.EquipmentType) ? (object)DBNull.Value : asset.EquipmentType);
            if (asset.EnginePower.HasValue)
                command.Parameters.AddWithValue("@EnginePower", asset.EnginePower.Value);
            else
                command.Parameters.AddWithValue("@EnginePower", DBNull.Value);
            command.Parameters.AddWithValue("@RegistrationNumber", string.IsNullOrEmpty(asset.RegistrationNumber) ? (object)DBNull.Value : asset.RegistrationNumber);
            if (asset.Weight.HasValue)
                command.Parameters.AddWithValue("@Weight", asset.Weight.Value);
            else
                command.Parameters.AddWithValue("@Weight", DBNull.Value);
            command.Parameters.AddWithValue("@PowerRequirements", string.IsNullOrEmpty(asset.PowerRequirements) ? (object)DBNull.Value : asset.PowerRequirements);
            command.Parameters.AddWithValue("@CreatedAt", asset.CreatedDate);
        }
        
        #endregion
        
        #region Contracts
        
        public List<Contract> GetContracts()
        {
            var contracts = new List<Contract>();
            
            // Реальная структура БД: CreatedAt, UpdatedAt
            string sql = @"
                SELECT Id, ContractNumber, [ContractType], CounterpartyId,
                       SignedDate, StartDate, EndDate, TotalAmount,
                       PaymentTerms, Notes, CreatedAt, UpdatedAt
                FROM Contracts
                ORDER BY SignedDate DESC";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contracts.Add(MapContract(reader));
                        }
                    }
                }
            }
            
            return contracts;
        }
        
        public Contract GetContract(int id)
        {
            string sql = @"
                SELECT Id, ContractNumber, [ContractType], CounterpartyId,
                       SignedDate, StartDate, EndDate, TotalAmount,
                       PaymentTerms, Notes, CreatedAt, UpdatedAt
                FROM Contracts
                WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapContract(reader);
                        }
                    }
                }
            }
            
            return null;
        }
        
        public void SaveContract(Contract contract)
        {
            if (contract.Id == 0)
            {
                InsertContract(contract);
            }
            else
            {
                UpdateContract(contract);
            }
        }
        
        private void InsertContract(Contract contract)
        {
            // В БД: CreatedAt, UpdatedAt
            // Нет: DurationMonths, VATAmount, TotalWithVAT, AdvancePayment, MonthlyPayment, ActivationDate, CompletionDate, IsDeleted
            
            // REFACTOR: Проверка ограничений CHECK из БД
            if (string.IsNullOrEmpty(contract.ContractNumber))
            {
                throw new InvalidOperationException(
                    "Номер договора должен быть указан. Укажите корректный номер договора.");
            }
            
            if (contract.TotalAmount <= 0)
            {
                throw new InvalidOperationException(
                    "Сумма договора должна быть больше 0. Укажите корректную общую сумму договора.");
            }
            
            if (contract.SignedDate > DateTime.Now)
            {
                throw new InvalidOperationException(
                    "Дата подписания договора не может быть в будущем.");
            }
            
            if (contract.StartDate < contract.SignedDate)
            {
                throw new InvalidOperationException(
                    "Дата начала действия договора не может быть раньше даты подписания.");
            }
            
            if (contract.EndDate.HasValue && contract.EndDate.Value <= contract.StartDate)
            {
                throw new InvalidOperationException(
                    "Дата окончания договора должна быть позже даты начала.");
            }
            
            string sql = @"
                INSERT INTO Contracts 
                (ContractNumber, [ContractType], CounterpartyId, 
                 SignedDate, StartDate, EndDate, TotalAmount,
                 PaymentTerms, Notes, CreatedAt)
                VALUES 
                (@ContractNumber, @ContractType, @CounterpartyId, 
                 @SignedDate, @StartDate, @EndDate, @TotalAmount,
                 @PaymentTerms, @Notes, @CreatedAt);
                SELECT SCOPE_IDENTITY();";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    AddContractParameters(command, contract);
                    connection.Open();
                    contract.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        
        private void UpdateContract(Contract contract)
        {
            // REFACTOR: Проверка ограничений CHECK из БД
            if (string.IsNullOrEmpty(contract.ContractNumber))
            {
                throw new InvalidOperationException(
                    "Номер договора должен быть указан. Укажите корректный номер договора.");
            }
            
            if (contract.TotalAmount <= 0)
            {
                throw new InvalidOperationException(
                    "Сумма договора должна быть больше 0. Укажите корректную общую сумму договора.");
            }
            
            if (contract.SignedDate > DateTime.Now)
            {
                throw new InvalidOperationException(
                    "Дата подписания договора не может быть в будущем.");
            }
            
            if (contract.StartDate < contract.SignedDate)
            {
                throw new InvalidOperationException(
                    "Дата начала действия договора не может быть раньше даты подписания.");
            }
            
            if (contract.EndDate.HasValue && contract.EndDate.Value <= contract.StartDate)
            {
                throw new InvalidOperationException(
                    "Дата окончания договора должна быть позже даты начала.");
            }
            
            string sql = @"
                UPDATE Contracts SET
                    ContractNumber = @ContractNumber,
                    [ContractType] = @ContractType,
                    CounterpartyId = @CounterpartyId,
                    SignedDate = @SignedDate,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    TotalAmount = @TotalAmount,
                    PaymentTerms = @PaymentTerms,
                    Notes = @Notes,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    AddContractParameters(command, contract);
                    command.Parameters.AddWithValue("@Id", contract.Id);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        public void DeleteContract(int id)
        {
            // В БД нет IsDeleted
            // Используем мягкое удаление через UpdatedAt
            string sql = "UPDATE Contracts SET UpdatedAt = @UpdatedAt WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        private Contract MapContract(SqlDataReader reader)
        {
            // Маппинг из структуры БД в модель
            // БД: CreatedAt -> Model: CreatedDate
            // БД: UpdatedAt -> Model: ModifiedDate
            // Отсутствующие поля устанавливаем по умолчанию
            
            DateTime startDate = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5);
            DateTime? endDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
            
            int durationMonths = 0;
            if (endDate.HasValue)
            {
                double days = (endDate.Value - startDate).TotalDays;
                durationMonths = (int)Math.Ceiling(days / 30.0);
            }
            
            decimal totalAmount = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7);
            decimal monthlyPayment = durationMonths > 0 ? totalAmount / durationMonths : totalAmount;
            
            return new Contract
            {
                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                ContractNumber = reader.IsDBNull(1) ? "" : reader.GetString(1),
                ContractType = reader.IsDBNull(2) ? ContractType.Rental : (ContractType)reader.GetByte(2),
                CounterpartyId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                SignedDate = reader.IsDBNull(4) ? DateTime.Now : reader.GetDateTime(4),
                StartDate = startDate,
                EndDate = endDate,
                // Рассчитываем DurationMonths
                DurationMonths = durationMonths,
                TotalAmount = totalAmount,
                // Устанавливаем значения по умолчанию для отсутствующих полей
                VATAmount = 0, // Нет в БД
                TotalWithVAT = totalAmount, // Приравниваем к TotalAmount
                AdvancePayment = 0, // Нет в БД
                MonthlyPayment = monthlyPayment,
                PaymentTerms = reader.IsDBNull(8) ? null : reader.GetString(8),
                Notes = reader.IsDBNull(9) ? null : reader.GetString(9),
                CreatedDate = reader.IsDBNull(10) ? DateTime.Now : reader.GetDateTime(10),
                ModifiedDate = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11),
                ActivationDate = null, // Нет в БД
                CompletionDate = null, // Нет в БД
                IsDeleted = false // В БД нет, считаем активным
            };
        }
        
        private void AddContractParameters(SqlCommand command, Contract contract)
        {
            // БД использует CreatedAt, UpdatedAt
            // REFACTOR: Исправление для CHECK ограничений - пустые строки преобразуем в NULL
            command.Parameters.AddWithValue("@ContractNumber", string.IsNullOrEmpty(contract.ContractNumber) ? (object)DBNull.Value : contract.ContractNumber);
            command.Parameters.AddWithValue("@ContractType", (int)contract.ContractType);
            command.Parameters.AddWithValue("@CounterpartyId", contract.CounterpartyId);
            command.Parameters.AddWithValue("@SignedDate", contract.SignedDate);
            command.Parameters.AddWithValue("@StartDate", contract.StartDate);
            if (contract.EndDate.HasValue)
                command.Parameters.AddWithValue("@EndDate", contract.EndDate.Value);
            else
                command.Parameters.AddWithValue("@EndDate", DBNull.Value);
            command.Parameters.AddWithValue("@TotalAmount", contract.TotalAmount);
            command.Parameters.AddWithValue("@PaymentTerms", string.IsNullOrEmpty(contract.PaymentTerms) ? (object)DBNull.Value : contract.PaymentTerms);
            command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(contract.Notes) ? (object)DBNull.Value : contract.Notes);
            command.Parameters.AddWithValue("@CreatedAt", contract.CreatedDate);
        }
        
        #endregion
        
        #region Test Data
        
        public void InitializeTestData()
        {
            // Проверяем, есть ли данные
            var contracts = GetContracts();
            if (contracts.Count > 0)
            {
                Console.WriteLine("[INFO] Тестовые данные уже существуют");
                return;
            }
            
            Console.WriteLine("[INFO] Инициализация тестовых данных...");
            
            // Добавляем тестовых контрагентов
            var counterparty1 = new Counterparty
            {
                Name = "ООО 'СтройТех'",
                INN = "1234567890",
                KPP = "123456789",
                LegalAddress = "г. Москва, ул. Строителей, д. 1",
                ContactPerson = "Иванов Иван Иванович",
                Phone = "+7 (999) 123-45-67",
                Email = "info@stroitech.ru",
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };
            SaveCounterparty(counterparty1);
            
            var counterparty2 = new Counterparty
            {
                Name = "ИП Петров П.П.",
                INN = "0987654321",
                LegalAddress = "г. Санкт-Петербург, пр. Невский, д. 100",
                ContactPerson = "Петров Петр Петрович",
                Phone = "+7 (911) 987-65-43",
                Email = "petrov@mail.ru",
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };
            SaveCounterparty(counterparty2);
            
            // Добавляем тестовую технику
            var asset1 = new Asset
            {
                Name = "Экскаватор-погрузчик JCB 3CX",
                InventoryNumber = "ТЕХ-001",
                AssetGroup = AssetGroup.Vehicle,
                Subcategory = "ConstructionRoad",
                Manufacturer = "JCB",
                Model = "3CX",
                SerialNumber = "JCB3CX2024001",
                YearOfManufacture = 2022,
                PurchasePrice = 5000000,
                ResidualValue = 4500000,
                MonthlyRentalRate = 150000,
                IsAvailable = true,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };
            SaveAsset(asset1);
            
            var asset2 = new Asset
            {
                Name = "Генератор 100 кВт",
                InventoryNumber = "ОБОР-001",
                AssetGroup = AssetGroup.Equipment,
                Subcategory = "Construction",
                Manufacturer = "Cummins",
                Model = "C100D6",
                SerialNumber = "CUM1002023001",
                YearOfManufacture = 2023,
                PurchasePrice = 1000000,
                ResidualValue = 900000,
                MonthlyRentalRate = 30000,
                IsAvailable = true,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };
            SaveAsset(asset2);
            
            // Добавляем тестовый договор
            var contract = new Contract
            {
                ContractNumber = "АР-2024-001",
                ContractType = ContractType.Rental,
                CounterpartyId = counterparty1.Id,
                SignedDate = DateTime.Now.AddDays(-30),
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now.AddDays(60),
                DurationMonths = 3,
                TotalAmount = 450000,
                VATAmount = 90000,
                TotalWithVAT = 540000,
                AdvancePayment = 100000,
                MonthlyPayment = 180000,
                PaymentTerms = "Аванс 30%, остальное ежемесячно",
                CreatedDate = DateTime.Now.AddDays(-30),
                IsDeleted = false
            };
            SaveContract(contract);
            
            Console.WriteLine("[SUCCESS] Тестовые данные успешно добавлены");
        }
        
        #endregion
        
        #region PaymentSchedules
        
        public List<PaymentSchedule> GetPaymentSchedules(int? contractId = null)
        {
            var payments = new List<PaymentSchedule>();
            
            // Реальная структура БД: PaidDate, PaymentMethod (TINYINT), PaymentReference, IsPaid
            string sql = @"
                SELECT Id, ContractId, PaymentNumber, Description,
                       DueDate, Amount, IsPaid, PaidDate, PaymentMethod, PaymentReference
                FROM PaymentSchedules
                ORDER BY DueDate";
            
            if (contractId.HasValue)
            {
                sql = @"
                    SELECT Id, ContractId, PaymentNumber, Description,
                           DueDate, Amount, IsPaid, PaidDate, PaymentMethod, PaymentReference
                    FROM PaymentSchedules
                    WHERE ContractId = @ContractId
                    ORDER BY DueDate";
            }
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    if (contractId.HasValue)
                    {
                        command.Parameters.AddWithValue("@ContractId", contractId.Value);
                    }
                    
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(MapPaymentSchedule(reader));
                        }
                    }
                }
            }
            
            return payments;
        }
        
        public void MarkPaymentPaid(int paymentId, DateTime? paymentDate = null)
        {
            // В БД: IsPaid, PaidDate
            string sql = @"
                UPDATE PaymentSchedules SET
                    IsPaid = @IsPaid,
                    PaidDate = @PaidDate
                WHERE Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", paymentId);
                    command.Parameters.AddWithValue("@IsPaid", true);
                    if (paymentDate.HasValue)
                        command.Parameters.AddWithValue("@PaidDate", paymentDate.Value);
                    else
                        command.Parameters.AddWithValue("@PaidDate", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void MarkPaymentUnpaid(int paymentId)
        {
            // В БД: IsPaid, PaidDate
            string sql = @"
                UPDATE PaymentSchedules SET
                    IsPaid = @IsPaid,
                    PaidDate = NULL
                WHERE Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", paymentId);
                    command.Parameters.AddWithValue("@IsPaid", false);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        private PaymentSchedule MapPaymentSchedule(SqlDataReader reader)
        {
            // Маппинг из структуры БД в модель
            // БД: PaidDate -> Model: PaymentDate
            // БД: PaymentMethod (TINYINT) -> Model: PaymentMethod (string)
            // БД: IsPaid -> Model: IsPaid и Status
            // БД: PaymentReference -> Model: Notes
            // Отсутствующие поля устанавливаем по умолчанию
            
            bool isPaid = reader.IsDBNull(6) ? false : reader.GetBoolean(6);
            DateTime? paidDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7);
            DateTime dueDate = reader.IsDBNull(4) ? DateTime.Today : reader.GetDateTime(4);
            
            // Вычисляем Status на основе IsPaid и DueDate
            PaymentStatus status;
            if (isPaid)
            {
                status = PaymentStatus.Paid;
            }
            else if (paidDate.HasValue && paidDate.Value < DateTime.Today)
            {
                status = PaymentStatus.Overdue;
            }
            else if (dueDate < DateTime.Today)
            {
                status = PaymentStatus.Overdue;
            }
            else
            {
                status = PaymentStatus.Pending;
            }
            
            return new PaymentSchedule
            {
                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                ContractId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                PaymentNumber = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                DueDate = dueDate,
                Amount = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
                IsPaid = isPaid,
                PaymentDate = paidDate,
                // Status вычислен выше
                Status = status,
                // PaymentMethod в БД это TINYINT, в модели string
                PaymentMethod = reader.IsDBNull(8) ? null : reader.GetByte(8).ToString(),
                // PaymentReference -> Notes
                Notes = reader.IsDBNull(9) ? null : reader.GetString(9),
                VATAmount = 0, // Нет в БД
                TotalAmount = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5), // Приравниваем к Amount
                CreatedDate = DateTime.Now, // Нет в БД, устанавливаем текущую дату
                ModifiedDate = null, // Нет в БД
                IsDeleted = false // В БД нет, считаем активным
            };
        }
        
        #endregion
        
        #region ContractSpecifications
        
        public List<ContractSpecification> GetSpecifications(int? contractId = null)
        {
            var specifications = new List<ContractSpecification>();
            
            // Реальная структура БД: нет CreatedDate, ModifiedDate, IsDeleted
            string sql = @"
                SELECT Id, ContractId, AssetId, Quantity, UnitPrice,
                       [PeriodType], AdditionalConditions
                FROM ContractSpecifications
                ORDER BY Id";
            
            if (contractId.HasValue)
            {
                sql = @"
                    SELECT Id, ContractId, AssetId, Quantity, UnitPrice,
                           [PeriodType], AdditionalConditions
                    FROM ContractSpecifications
                    WHERE ContractId = @ContractId
                    ORDER BY Id";
            }
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    if (contractId.HasValue)
                    {
                        command.Parameters.AddWithValue("@ContractId", contractId.Value);
                    }
                    
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            specifications.Add(MapContractSpecification(reader));
                        }
                    }
                }
            }
            
            return specifications;
        }
        
        private ContractSpecification MapContractSpecification(SqlDataReader reader)
        {
            // Маппинг из структуры БД в модель
            // Отсутствующие поля устанавливаем по умолчанию
            return new ContractSpecification
            {
                Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                ContractId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                AssetId = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                Quantity = reader.IsDBNull(3) ? 1 : reader.GetInt32(3),
                UnitPrice = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                PeriodType = reader.IsDBNull(5) ? PeriodType.Month : (PeriodType)reader.GetByte(5),
                AdditionalConditions = reader.IsDBNull(6) ? null : reader.GetString(6),
                CreatedDate = DateTime.Now, // Нет в БД
                ModifiedDate = null, // Нет в БД
                IsDeleted = false // В БД нет, считаем активным
            };
        }
        
        #endregion
        
        #region Reports
        
        public List<PaymentReportRow> GetPaymentReport(DateTime? dueFrom, DateTime? dueTo, bool unpaidOnly)
        {
            var today = DateTime.Today;
            var rows = new List<PaymentReportRow>();
            
            // Фильтрация выполняется в C# для точного сравнения дат
            var payments = GetPaymentSchedules();
            
            foreach (var payment in payments)
            {
                var contract = GetContract(payment.ContractId);
                if (contract == null)
                    continue;
                
                var isPaid = payment.Status == PaymentStatus.Paid || payment.PaymentDate.HasValue;
                if (unpaidOnly && isPaid)
                    continue;
                
                // Фильтрация по датам (учитывает год, месяц и день)
                if (dueFrom.HasValue && payment.DueDate.Date < dueFrom.Value.Date)
                    continue;
                if (dueTo.HasValue && payment.DueDate.Date > dueTo.Value.Date)
                    continue;
                
                var counterparty = GetCounterparty(contract.CounterpartyId);
                var total = payment.TotalAmount ?? payment.Amount;
                
                rows.Add(new PaymentReportRow
                {
                    PaymentId = payment.Id,
                    ContractId = contract.Id,
                    ContractNumber = contract.ContractNumber,
                    CounterpartyName = counterparty?.Name ?? "—",
                    ContactPerson = counterparty?.ContactPerson ?? "",
                    DueDate = payment.DueDate,
                    PaymentDate = payment.PaymentDate,
                    Amount = payment.Amount,
                    TotalAmount = total,
                    Status = payment.Status,
                    IsPaid = isPaid,
                    DaysOverdue = ReportCalculationService.GetDaysOverdue(payment.DueDate, isPaid, today),
                    AgingBucket = ReportCalculationService.GetAgingBucket(payment.DueDate, isPaid, today),
                    Notes = payment.Notes ?? ""
                });
            }
            
            return rows.OrderBy(r => r.DueDate).ToList();
        }
        
        public List<AssetUtilizationRow> GetAssetUtilizationReport(DateTime periodStart, DateTime periodEnd, AssetGroup? assetGroup)
        {
            var rows = new List<AssetUtilizationRow>();
            var daysInPeriod = ReportCalculationService.OverlapDays(periodStart, periodEnd, periodStart, periodEnd);
            if (daysInPeriod <= 0)
                daysInPeriod = 1;

            var assets = GetAssets();
            if (assetGroup.HasValue)
                assets = assets.Where(a => a.AssetGroup == assetGroup.Value).ToList();

            foreach (var asset in assets)
            {
                // Получаем спецификации для этой техники
                var specs = GetSpecifications().Where(s => s.AssetId == asset.Id).ToList();
                int daysRented = 0;
                decimal revenue = 0;
                int activeContractsCount = 0;

                foreach (var spec in specs)
                {
                    var contract = GetContract(spec.ContractId);
                    if (contract == null)
                        continue;

                    // Проверяем пересечение периода договора с отчетным периодом
                    var contractStart = contract.StartDate.Date;
                    var contractEnd = contract.EndDate?.Date ?? DateTime.MaxValue.Date;

                    // Если договор не пересекается с отчетным периодом, пропускаем
                    if (contractEnd < periodStart.Date || contractStart > periodEnd.Date)
                        continue;

                    activeContractsCount++;

                    // Вычисляем пересечение периодов
                    var overlapStart = contractStart > periodStart.Date ? contractStart : periodStart.Date;
                    var overlapEnd = contractEnd < periodEnd.Date ? contractEnd : periodEnd.Date;
                    var overlapDays = Math.Max(0, (overlapEnd - overlapStart).Days + 1);

                    daysRented += overlapDays;

                    // Вычисляем доход на основе спецификации
                    // Если PeriodType = Month, то UnitPrice - это цена за месяц
                    // Если PeriodType = Day, то UnitPrice - это цена за день
                    // Если PeriodType = Hour, то UnitPrice - это цена за час (8 часов в день)
                    decimal dailyRate = 0;
                    switch (spec.PeriodType)
                    {
                        case PeriodType.Month:
                            dailyRate = spec.UnitPrice / 30m; // Приблизительно 30 дней в месяце
                            break;
                        case PeriodType.Week:
                            dailyRate = spec.UnitPrice / 7m; // 7 дней в неделе
                            break;
                        case PeriodType.Hour:
                            dailyRate = spec.UnitPrice * 8m; // 8 часов в день
                            break;
                        case PeriodType.Day:
                        default:
                            dailyRate = spec.UnitPrice;
                            break;
                    }

                    revenue += dailyRate * overlapDays * spec.Quantity;
                }

                daysRented = Math.Min(daysRented, daysInPeriod);
                var rate = daysInPeriod > 0 ? (double)daysRented / daysInPeriod * 100.0 : 0;

                string alert = null;
                if (specs.Count == 0)
                {
                    alert = "Нет договоров (создайте договор для загрузки)";
                }
                else if (activeContractsCount == 0)
                {
                    alert = "Нет активных договоров";
                }
                else if (rate < 30 && daysInPeriod >= 7)
                {
                    alert = "Низкая загрузка (< 30%)";
                }
                if (!asset.IsAvailable && rate > 0)
                {
                    alert = string.IsNullOrEmpty(alert) ? "В аренде" : alert;
                }

                rows.Add(new AssetUtilizationRow
                {
                    AssetId = asset.Id,
                    InventoryNumber = asset.InventoryNumber,
                    AssetName = asset.Name,
                    AssetGroup = asset.AssetGroup,
                    Subcategory = asset.Subcategory,
                    IsAvailable = asset.IsAvailable,
                    DaysInPeriod = daysInPeriod,
                    DaysRented = daysRented,
                    UtilizationRate = Math.Round(rate, 1),
                    Revenue = Math.Round(revenue, 2),
                    MonthlyRate = asset.MonthlyRentalRate,
                    AlertMessage = alert ?? ""
                });
            }

            return rows.OrderByDescending(r => r.UtilizationRate).ToList();
        }

        #endregion
        
        #region ContractSpecifications CRUD
        
        public ContractSpecification GetSpecification(int id)
        {
            string sql = @"
                SELECT Id, ContractId, AssetId, Quantity, UnitPrice,
                       [PeriodType], AdditionalConditions
                FROM ContractSpecifications
                WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapContractSpecification(reader);
                        }
                    }
                }
            }
            
            return null;
        }
        
        public void SaveSpecification(ContractSpecification specification)
        {
            if (specification.Id == 0)
            {
                InsertSpecification(specification);
            }
            else
            {
                UpdateSpecification(specification);
            }
        }
        
        private void InsertSpecification(ContractSpecification specification)
        {
            if (specification.ContractId <= 0)
            {
                throw new InvalidOperationException("Не указан договор для спецификации.");
            }
            
            if (specification.AssetId <= 0)
            {
                throw new InvalidOperationException("Не указана техника для спецификации.");
            }
            
            if (specification.Quantity <= 0)
            {
                throw new InvalidOperationException("Количество должно быть больше 0.");
            }
            
            if (specification.UnitPrice <= 0)
            {
                throw new InvalidOperationException("Цена за единицу должна быть больше 0.");
            }
            
            string sql = @"
                INSERT INTO ContractSpecifications
                (ContractId, AssetId, Quantity, UnitPrice, [PeriodType], AdditionalConditions)
                VALUES
                (@ContractId, @AssetId, @Quantity, @UnitPrice, @PeriodType, @AdditionalConditions);
                SELECT SCOPE_IDENTITY();";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    AddSpecificationParameters(command, specification);
                    connection.Open();
                    specification.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        
        private void UpdateSpecification(ContractSpecification specification)
        {
            if (specification.Quantity <= 0)
            {
                throw new InvalidOperationException("Количество должно быть больше 0.");
            }
            
            if (specification.UnitPrice <= 0)
            {
                throw new InvalidOperationException("Цена за единицу должна быть больше 0.");
            }
            
            string sql = @"
                UPDATE ContractSpecifications SET
                    Quantity = @Quantity,
                    UnitPrice = @UnitPrice,
                    [PeriodType] = @PeriodType,
                    AdditionalConditions = @AdditionalConditions
                WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    AddSpecificationParameters(command, specification);
                    command.Parameters.AddWithValue("@Id", specification.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        public void DeleteSpecification(int id)
        {
            string sql = "DELETE FROM ContractSpecifications WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        private void AddSpecificationParameters(SqlCommand command, ContractSpecification spec)
        {
            command.Parameters.AddWithValue("@ContractId", spec.ContractId);
            command.Parameters.AddWithValue("@AssetId", spec.AssetId);
            command.Parameters.AddWithValue("@Quantity", spec.Quantity);
            command.Parameters.AddWithValue("@UnitPrice", spec.UnitPrice);
            command.Parameters.AddWithValue("@PeriodType", (int)spec.PeriodType);
            command.Parameters.AddWithValue("@AdditionalConditions", 
                string.IsNullOrEmpty(spec.AdditionalConditions) ? (object)DBNull.Value : spec.AdditionalConditions);
        }
        
        #endregion
        
        #region PaymentSchedules CRUD
        
        public PaymentSchedule GetPaymentSchedule(int id)
        {
            string sql = @"
                SELECT Id, ContractId, PaymentNumber, Description,
                       DueDate, Amount, IsPaid, PaidDate, PaymentMethod, PaymentReference
                FROM PaymentSchedules
                WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapPaymentSchedule(reader);
                        }
                    }
                }
            }
            
            return null;
        }
        
        public void SavePaymentSchedule(PaymentSchedule schedule)
        {
            if (schedule.Id == 0)
            {
                InsertPaymentSchedule(schedule);
            }
            else
            {
                UpdatePaymentSchedule(schedule);
            }
        }
        
        private void InsertPaymentSchedule(PaymentSchedule schedule)
        {
            if (schedule.ContractId <= 0)
            {
                throw new InvalidOperationException("Не указан договор для платежа.");
            }
            
            if (schedule.Amount <= 0)
            {
                throw new InvalidOperationException("Сумма платежа должна быть больше 0.");
            }
            
            // Get next payment number for contract
            int paymentNumber = GetNextPaymentNumber(schedule.ContractId);
            
            string sql = @"
                INSERT INTO PaymentSchedules
                (ContractId, PaymentNumber, Description, DueDate, Amount, IsPaid, PaidDate, PaymentMethod, PaymentReference)
                VALUES
                (@ContractId, @PaymentNumber, @Description, @DueDate, @Amount, @IsPaid, @PaidDate, @PaymentMethod, @PaymentReference);
                SELECT SCOPE_IDENTITY();";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    AddPaymentScheduleParameters(command, schedule, paymentNumber);
                    connection.Open();
                    schedule.Id = Convert.ToInt32(command.ExecuteScalar());
                    schedule.PaymentNumber = paymentNumber;
                }
            }
        }
        
        private void UpdatePaymentSchedule(PaymentSchedule schedule)
        {
            if (schedule.Amount <= 0)
            {
                throw new InvalidOperationException("Сумма платежа должна быть больше 0.");
            }
            
            string sql = @"
                UPDATE PaymentSchedules SET
                    Description = @Description,
                    DueDate = @DueDate,
                    Amount = @Amount,
                    IsPaid = @IsPaid,
                    PaidDate = @PaidDate,
                    PaymentMethod = @PaymentMethod,
                    PaymentReference = @PaymentReference
                WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", schedule.Id);
                    command.Parameters.AddWithValue("@Description", 
                        string.IsNullOrEmpty(schedule.Description) ? (object)DBNull.Value : schedule.Description);
                    command.Parameters.AddWithValue("@DueDate", schedule.DueDate);
                    command.Parameters.AddWithValue("@Amount", schedule.Amount);
                    command.Parameters.AddWithValue("@IsPaid", schedule.IsPaid);
                    if (schedule.PaymentDate.HasValue)
                        command.Parameters.AddWithValue("@PaidDate", schedule.PaymentDate.Value);
                    else
                        command.Parameters.AddWithValue("@PaidDate", DBNull.Value);
                    command.Parameters.AddWithValue("@PaymentMethod",
                        string.IsNullOrEmpty(schedule.PaymentMethod) ? (object)DBNull.Value : int.Parse(schedule.PaymentMethod));
                    command.Parameters.AddWithValue("@PaymentReference",
                        string.IsNullOrEmpty(schedule.Notes) ? (object)DBNull.Value : schedule.Notes);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        public void DeletePaymentSchedule(int id)
        {
            string sql = "DELETE FROM PaymentSchedules WHERE Id = @Id";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        
        private int GetNextPaymentNumber(int contractId)
        {
            string sql = "SELECT ISNULL(MAX(PaymentNumber), 0) + 1 FROM PaymentSchedules WHERE ContractId = @ContractId";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ContractId", contractId);
                    connection.Open();
                    return (int)command.ExecuteScalar();
                }
            }
        }
        
        private void AddPaymentScheduleParameters(SqlCommand command, PaymentSchedule schedule, int paymentNumber)
        {
            command.Parameters.AddWithValue("@ContractId", schedule.ContractId);
            command.Parameters.AddWithValue("@PaymentNumber", paymentNumber);
            command.Parameters.AddWithValue("@Description",
                string.IsNullOrEmpty(schedule.Description) ? (object)DBNull.Value : schedule.Description);
            command.Parameters.AddWithValue("@DueDate", schedule.DueDate);
            command.Parameters.AddWithValue("@Amount", schedule.Amount);
            command.Parameters.AddWithValue("@IsPaid", schedule.IsPaid);
            if (schedule.PaymentDate.HasValue)
                command.Parameters.AddWithValue("@PaidDate", schedule.PaymentDate.Value);
            else
                command.Parameters.AddWithValue("@PaidDate", DBNull.Value);
            command.Parameters.AddWithValue("@PaymentMethod",
                string.IsNullOrEmpty(schedule.PaymentMethod) ? (object)DBNull.Value : int.Parse(schedule.PaymentMethod));
            command.Parameters.AddWithValue("@PaymentReference",
                string.IsNullOrEmpty(schedule.Notes) ? (object)DBNull.Value : schedule.Notes);
        }
        
        public void GeneratePaymentSchedule(int contractId, int paymentCount, DateTime startDate, decimal amountPerPayment)
        {
            var contract = GetContract(contractId);
            if (contract == null)
            {
                throw new InvalidOperationException("Договор не найден.");
            }
            
            if (paymentCount <= 0)
            {
                throw new InvalidOperationException("Количество платежей должно быть больше 0.");
            }
            
            if (amountPerPayment <= 0)
            {
                throw new InvalidOperationException("Сумма платежа должна быть больше 0.");
            }
            
            // Delete existing payments for this contract
            var existingPayments = GetPaymentSchedules(contractId);
            foreach (var payment in existingPayments)
            {
                DeletePaymentSchedule(payment.Id);
            }
            
            // Generate new payment schedule
            for (int i = 1; i <= paymentCount; i++)
            {
                var schedule = new PaymentSchedule
                {
                    ContractId = contractId,
                    Description = $"Платёж {i} из {paymentCount}",
                    DueDate = startDate.AddMonths(i - 1),
                    Amount = amountPerPayment,
                    IsPaid = false,
                    Status = PaymentStatus.Pending
                };
                
                SavePaymentSchedule(schedule);
            }
        }
        
        #endregion
        
        #region Utility Methods
        
        public void ResetDemoData()
        {
            // В реальной БД нет столбца IsDeleted
            // Просто удаляем все записи из таблиц
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Удаляем все записи из таблиц (в реальной БД нет IsDeleted)
                        string deleteAllPayments = "DELETE FROM PaymentSchedules";
                        string deleteAllSpecs = "DELETE FROM ContractSpecifications";
                        string deleteAllContracts = "DELETE FROM Contracts";
                        string deleteAllAssets = "DELETE FROM Assets";
                        string deleteAllCounterparties = "DELETE FROM Counterparties";
                        
                        using (var command = new SqlCommand(deleteAllPayments, connection, transaction))
                        { command.ExecuteNonQuery(); }
                        
                        using (var command = new SqlCommand(deleteAllSpecs, connection, transaction))
                        { command.ExecuteNonQuery(); }
                        
                        using (var command = new SqlCommand(deleteAllContracts, connection, transaction))
                        { command.ExecuteNonQuery(); }
                        
                        using (var command = new SqlCommand(deleteAllAssets, connection, transaction))
                        { command.ExecuteNonQuery(); }
                        
                        using (var command = new SqlCommand(deleteAllCounterparties, connection, transaction))
                        { command.ExecuteNonQuery(); }
                        
                        // Сбрасываем IDENTITY
                        string resetIdentity = @"
                            DBCC CHECKIDENT ('Counterparties', RESEED, 0);
                            DBCC CHECKIDENT ('Assets', RESEED, 0);
                            DBCC CHECKIDENT ('Contracts', RESEED, 0);
                            DBCC CHECKIDENT ('ContractSpecifications', RESEED, 0);
                            DBCC CHECKIDENT ('PaymentSchedules', RESEED, 0);";
                        
                        using (var command = new SqlCommand(resetIdentity, connection, transaction))
                        { command.ExecuteNonQuery(); }
                        
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            
            InitializeTestData();
        }
        
        public bool CheckAssetAvailability(int assetId, DateTime startDate, DateTime endDate, int? excludeContractId = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Проверяем, есть ли спецификации для этой техники в указанный период
                string sql = @"
                    SELECT COUNT(*)
                    FROM ContractSpecifications cs
                    INNER JOIN Contracts c ON cs.ContractId = c.Id
                    WHERE cs.AssetId = @AssetId
                        AND (@ExcludeContractId IS NULL OR c.Id != @ExcludeContractId)
                        AND c.StartDate <= @EndDate
                        AND (c.EndDate IS NULL OR c.EndDate >= @StartDate)";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@AssetId", assetId);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    if (excludeContractId.HasValue)
                        command.Parameters.AddWithValue("@ExcludeContractId", excludeContractId.Value);
                    else
                        command.Parameters.AddWithValue("@ExcludeContractId", DBNull.Value);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count == 0; // Доступна, если нет пересечений
                }
            }
        }

        public bool HasActiveContractSpecifications(int assetId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                    SELECT COUNT(*)
                    FROM ContractSpecifications cs
                    INNER JOIN Contracts c ON cs.ContractId = c.Id
                    WHERE cs.AssetId = @AssetId
                        AND (c.EndDate IS NULL OR c.EndDate >= GETDATE())";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@AssetId", assetId);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public bool TestConnection(out string errorMessage)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    using (var command = new SqlCommand("SELECT @@VERSION", connection))
                    {
                        object result = command.ExecuteScalar();
                        string version = result?.ToString() ?? "Unknown";
                        
                        errorMessage = $"Подключение успешно! SQL Server Version: {version.Substring(0, Math.Min(100, version.Length))}...";
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                errorMessage = $"Ошибка SQL Server: {ex.Message} (Error: {ex.Number})";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Ошибка подключения: {ex.Message}";
                return false;
            }
        }
        
        public string GenerateContractNumber(ContractType contractType)
        {
            var prefix = "AR";
            return $"{prefix}-{DateTime.Now:yyyy}-{new Random().Next(1, 999):000}";
        }
        
        #endregion
    }
}