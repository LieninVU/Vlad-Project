using System;
using System.Collections.Generic;

namespace ForVlad.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InventoryNumber { get; set; }
        public AssetGroup AssetGroup { get; set; }
        public string Subcategory { get; set; }
        
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public int? YearOfManufacture { get; set; }
        
        public decimal PurchasePrice { get; set; }
        public decimal ResidualValue { get; set; }
        public decimal MonthlyRentalRate { get; set; }
        
        public string VehicleBrand { get; set; }
        public string VehicleModel { get; set; }
        public string VinNumber { get; set; }
        public int? ManufactureYear { get; set; }
        public string EquipmentType { get; set; }
        public VehicleSubcategory? VehicleSubcategory { get; set; }
        public EquipmentSubcategory? EquipmentSubcategory { get; set; }
        public decimal? EnginePower { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal? Weight { get; set; }
        public string PowerRequirements { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal DailyRate { get; set; }
        public AssetCondition AssetCondition { get; set; } = AssetCondition.Good;
        
        public bool IsAvailable { get; set; }
        public string Notes { get; set; }
        public string Description { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        
        public virtual ICollection<ContractSpecification> Specifications { get; set; }
        
        public Asset()
        {
            Specifications = new List<ContractSpecification>();
        }
    }
}
