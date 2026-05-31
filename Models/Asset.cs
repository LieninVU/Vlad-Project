using System;
using System.Collections.Generic;

namespace ForVlad.Models
{
    /// <summary>
    /// Модель техники или оборудования
    /// </summary>
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InventoryNumber { get; set; }
        public AssetGroup AssetGroup { get; set; }
        public string Subcategory { get; set; }
        
        // Основные свойства техники
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public int? YearOfManufacture { get; set; }
        
        // Финансовые свойства
        public decimal PurchasePrice { get; set; }
        public decimal ResidualValue { get; set; }
        public decimal MonthlyRentalRate { get; set; }
        
        // Свойства для транспортных средств
        public string RegistrationNumber { get; set; }
        public decimal? EnginePower { get; set; }
        public string VinNumber { get; set; }
        
        // Свойства для оборудования
        public string EquipmentType { get; set; }
        public decimal? Weight { get; set; }
        public string PowerRequirements { get; set; }
        
        // Тарифы аренды
        public decimal HourlyRate { get; set; }
        public decimal DailyRate { get; set; }
        
        // Состояние
        public AssetCondition AssetCondition { get; set; } = AssetCondition.Good;
        public VehicleSubcategory? VehicleSubcategory { get; set; }
        public EquipmentSubcategory? EquipmentSubcategory { get; set; }
        
        public bool IsAvailable { get; set; }
        public string Description { get; set; }
        
        // Метданные
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        
        public virtual ICollection<ContractSpecification> Specifications { get; set; }
        
        public Asset()
        {
            Specifications = new List<ContractSpecification>();
        }
        
        // REFACTOR: Удалены дублирующиеся свойства VehicleBrand, VehicleModel, ManufactureYear
        // Используются Manufacturer, Model, SerialNumber, YearOfManufacture
    }
}
