using System;
using System.Data.SqlClient;
using ForVlad.Models;

namespace ForVlad.Data
{
    internal static class EntityMapper
    {
        public static Counterparty ReadCounterparty(SqlDataReader reader)
        {
            return new Counterparty
            {
                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name")),
                CounterpartyType = reader.IsDBNull(reader.GetOrdinal("CounterpartyType")) ? CounterpartyType.LegalEntity : (CounterpartyType)reader.GetByte(reader.GetOrdinal("CounterpartyType")),
                INN = reader.IsDBNull(reader.GetOrdinal("Inn")) ? "" : reader.GetString(reader.GetOrdinal("Inn")),
                KPP = reader.IsDBNull(reader.GetOrdinal("Kpp")) ? null : reader.GetString(reader.GetOrdinal("Kpp")),
                LegalAddress = reader.IsDBNull(reader.GetOrdinal("LegalAddress")) ? null : reader.GetString(reader.GetOrdinal("LegalAddress")),
                ActualAddress = reader.IsDBNull(reader.GetOrdinal("ActualAddress")) ? null : reader.GetString(reader.GetOrdinal("ActualAddress")),
                ContactPerson = reader.IsDBNull(reader.GetOrdinal("ContactPerson")) ? null : reader.GetString(reader.GetOrdinal("ContactPerson")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                ModifiedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : !reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

        public static Asset ReadAsset(SqlDataReader reader)
        {
            var group = (AssetGroup)reader.GetByte(reader.GetOrdinal("AssetGroup"));
            var hourly = reader.GetDecimal(reader.GetOrdinal("HourlyRate"));
            var daily = reader.GetDecimal(reader.GetOrdinal("DailyRate"));

            var asset = new Asset
            {
                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                InventoryNumber = reader.IsDBNull(reader.GetOrdinal("InventoryNumber")) ? null : reader.GetString(reader.GetOrdinal("InventoryNumber")),
                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name")),
                AssetGroup = group,
                VehicleBrand = reader.IsDBNull(reader.GetOrdinal("VehicleBrand")) ? null : reader.GetString(reader.GetOrdinal("VehicleBrand")),
                VehicleModel = reader.IsDBNull(reader.GetOrdinal("VehicleModel")) ? null : reader.GetString(reader.GetOrdinal("VehicleModel")),
                VinNumber = reader.IsDBNull(reader.GetOrdinal("VinNumber")) ? null : reader.GetString(reader.GetOrdinal("VinNumber")),
                ManufactureYear = reader.IsDBNull(reader.GetOrdinal("ManufactureYear")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ManufactureYear")),
                HourlyRate = hourly,
                DailyRate = daily,
                AssetCondition = reader.IsDBNull(reader.GetOrdinal("AssetCondition")) ? AssetCondition.Good : (AssetCondition)reader.GetByte(reader.GetOrdinal("AssetCondition")),
                IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                ModifiedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                EquipmentType = reader.IsDBNull(reader.GetOrdinal("EquipmentType")) ? null : reader.GetString(reader.GetOrdinal("EquipmentType")),
                EnginePower = reader.IsDBNull(reader.GetOrdinal("EnginePower")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("EnginePower")),
                RegistrationNumber = reader.IsDBNull(reader.GetOrdinal("RegistrationNumber")) ? null : reader.GetString(reader.GetOrdinal("RegistrationNumber")),
                Weight = reader.IsDBNull(reader.GetOrdinal("Weight")) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal("Weight")),
                PowerRequirements = reader.IsDBNull(reader.GetOrdinal("PowerRequirements")) ? null : reader.GetString(reader.GetOrdinal("PowerRequirements"))
            };

            asset.Manufacturer = asset.VehicleBrand ?? "";
            asset.Model = asset.VehicleModel ?? asset.EquipmentType ?? "";
            asset.SerialNumber = asset.VinNumber ?? "";
            asset.YearOfManufacture = asset.ManufactureYear;
            asset.MonthlyRentalRate = daily * 30;
            asset.PurchasePrice = 0;
            asset.ResidualValue = 0;
            asset.Notes = asset.Description;
            asset.IsDeleted = false;

            if (!reader.IsDBNull(reader.GetOrdinal("VehicleSubcategory")))
            {
                asset.VehicleSubcategory = (VehicleSubcategory)reader.GetByte(reader.GetOrdinal("VehicleSubcategory"));
                asset.Subcategory = asset.VehicleSubcategory.ToString();
            }
            else if (!reader.IsDBNull(reader.GetOrdinal("EquipmentSubcategory")))
            {
                asset.EquipmentSubcategory = (EquipmentSubcategory)reader.GetByte(reader.GetOrdinal("EquipmentSubcategory"));
                asset.Subcategory = asset.EquipmentSubcategory.ToString();
            }

            return asset;
        }

        public static Contract ReadContract(SqlDataReader reader)
        {
            var total = reader.IsDBNull(reader.GetOrdinal("TotalAmount")) ? 0m : reader.GetDecimal(reader.GetOrdinal("TotalAmount"));
            var contract = new Contract
            {
                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                ContractNumber = reader.IsDBNull(reader.GetOrdinal("ContractNumber")) ? "" : reader.GetString(reader.GetOrdinal("ContractNumber")),
                ContractType = reader.IsDBNull(reader.GetOrdinal("ContractType")) ? ContractType.Rental : (ContractType)reader.GetByte(reader.GetOrdinal("ContractType")),
                Status = reader.IsDBNull(reader.GetOrdinal("ContractStatus")) ? ContractStatus.Draft : (ContractStatus)reader.GetByte(reader.GetOrdinal("ContractStatus")),
                CounterpartyId = reader.IsDBNull(reader.GetOrdinal("CounterpartyId")) ? 0 : reader.GetInt32(reader.GetOrdinal("CounterpartyId")),
                SignedDate = reader.IsDBNull(reader.GetOrdinal("SignedDate")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("SignedDate")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                TotalAmount = total,
                PaymentTerms = reader.IsDBNull(reader.GetOrdinal("PaymentTerms")) ? null : reader.GetString(reader.GetOrdinal("PaymentTerms")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                ModifiedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = false
            };

            contract.TotalWithVAT = total;
            contract.TotalAmount = Math.Round(total / 1.2m, 2);
            contract.VATAmount = total - contract.TotalAmount;
            contract.DurationMonths = CalculateDurationMonths(contract.StartDate, contract.EndDate);
            return contract;
        }

        public static ContractSpecification ReadSpecification(SqlDataReader reader)
        {
            return new ContractSpecification
            {
                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                ContractId = reader.IsDBNull(reader.GetOrdinal("ContractId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ContractId")),
                AssetId = reader.IsDBNull(reader.GetOrdinal("AssetId")) ? 0 : reader.GetInt32(reader.GetOrdinal("AssetId")),
                Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 1 : reader.GetInt32(reader.GetOrdinal("Quantity")),
                UnitPrice = reader.IsDBNull(reader.GetOrdinal("UnitPrice")) ? 0 : reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                PeriodType = reader.IsDBNull(reader.GetOrdinal("PeriodType")) ? PeriodType.Month : (PeriodType)reader.GetByte(reader.GetOrdinal("PeriodType")),
                AdditionalConditions = reader.IsDBNull(reader.GetOrdinal("AdditionalConditions")) ? null : reader.GetString(reader.GetOrdinal("AdditionalConditions")),
                IsDeleted = false,
                CreatedDate = DateTime.Now
            };
        }

        public static PaymentSchedule ReadPayment(SqlDataReader reader)
        {
            var isPaid = reader.IsDBNull(reader.GetOrdinal("IsPaid")) ? false : reader.GetBoolean(reader.GetOrdinal("IsPaid"));
            var amount = reader.IsDBNull(reader.GetOrdinal("Amount")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Amount"));
            var dueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? DateTime.Today : reader.GetDateTime(reader.GetOrdinal("DueDate"));
            var paymentMethodValue = reader.IsDBNull(reader.GetOrdinal("PaymentMethod")) ? (byte?)null : reader.GetByte(reader.GetOrdinal("PaymentMethod"));
            return new PaymentSchedule
            {
                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                ContractId = reader.IsDBNull(reader.GetOrdinal("ContractId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ContractId")),
                PaymentNumber = reader.IsDBNull(reader.GetOrdinal("PaymentNumber")) ? 0 : reader.GetInt32(reader.GetOrdinal("PaymentNumber")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                DueDate = dueDate,
                Amount = amount,
                VATAmount = Math.Round(amount * 0.2m, 2),
                TotalAmount = amount, // Устанавливаем TotalAmount равным Amount если он не задан в БД
                IsPaid = isPaid,
                PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaidDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PaidDate")),
                Status = isPaid ? PaymentStatus.Paid : (dueDate < DateTime.Today ? PaymentStatus.Overdue : PaymentStatus.Pending),
                PaymentMethod = paymentMethodValue?.ToString(),
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };
        }

        public static int CalculateDurationMonths(DateTime start, DateTime? end)
        {
            if (!end.HasValue)
                return 0;
            var days = (end.Value.Date - start.Date).TotalDays;
            if (days < 0)
                return 0;
            var daysInMonth = Math.Max(28, Properties.Settings.Default.DaysInMonth);
            return Math.Max(1, (int)Math.Ceiling(days / daysInMonth));
        }

        public static void ApplyAssetToParameters(SqlCommand command, Asset asset)
        {
            var daily = asset.DailyRate > 0 ? asset.DailyRate : Math.Max(asset.MonthlyRentalRate / 30m, 1m);
            var hourly = asset.HourlyRate > 0 ? asset.HourlyRate : Math.Max(daily / 8m, 1m);

            command.Parameters.AddWithValue("@InventoryNumber", asset.InventoryNumber ?? "");
            command.Parameters.AddWithValue("@Name", asset.Name ?? "");
            command.Parameters.AddWithValue("@AssetGroup", (byte)asset.AssetGroup);
            command.Parameters.AddWithValue("@VehicleBrand",
                asset.AssetGroup == AssetGroup.Vehicle
                    ? (object)(asset.Manufacturer ?? asset.VehicleBrand ?? "N/A")
                    : DBNull.Value);
            command.Parameters.AddWithValue("@VehicleModel",
                asset.AssetGroup == AssetGroup.Vehicle
                    ? (object)(asset.Model ?? asset.VehicleModel ?? "")
                    : DBNull.Value);
            command.Parameters.AddWithValue("@VinNumber",
                (object)(asset.SerialNumber ?? asset.VinNumber) ?? DBNull.Value);
            command.Parameters.AddWithValue("@ManufactureYear",
                (object)(asset.YearOfManufacture ?? asset.ManufactureYear) ?? DBNull.Value);
            command.Parameters.AddWithValue("@EquipmentType",
                asset.AssetGroup == AssetGroup.Equipment
                    ? (object)(asset.Model ?? asset.EquipmentType ?? asset.Name)
                    : DBNull.Value);
            command.Parameters.AddWithValue("@VehicleSubcategory", ResolveVehicleSubcategory(asset));
            command.Parameters.AddWithValue("@EquipmentSubcategory", ResolveEquipmentSubcategory(asset));
            command.Parameters.AddWithValue("@HourlyRate", hourly);
            command.Parameters.AddWithValue("@DailyRate", daily);
            command.Parameters.AddWithValue("@AssetCondition", (byte)asset.AssetCondition);
            command.Parameters.AddWithValue("@IsAvailable", asset.IsAvailable);
            command.Parameters.AddWithValue("@Description", (object)(asset.Notes ?? asset.Description ?? (object)DBNull.Value));
            // Новые параметры
            command.Parameters.AddWithValue("@EnginePower", asset.EnginePower ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@RegistrationNumber", asset.RegistrationNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Weight", asset.Weight ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PowerRequirements", asset.PowerRequirements ?? (object)DBNull.Value);
        }

        private static object ResolveVehicleSubcategory(Asset asset)
        {
            if (asset.AssetGroup != AssetGroup.Vehicle)
                return DBNull.Value;
            if (Enum.TryParse<VehicleSubcategory>(asset.Subcategory, out var sub))
                return (byte)sub;
            return (byte)VehicleSubcategory.ConstructionRoad;
        }

        private static object ResolveEquipmentSubcategory(Asset asset)
        {
            if (asset.AssetGroup != AssetGroup.Equipment)
                return DBNull.Value;
            if (Enum.TryParse<EquipmentSubcategory>(asset.Subcategory, out var sub))
                return (byte)sub;
            return (byte)EquipmentSubcategory.Construction;
        }
    }
}
