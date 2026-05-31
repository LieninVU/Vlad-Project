using System;

namespace ForVlad.Models
{
    public class ContractSpecification
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public virtual Contract Contract { get; set; }
        
        public int AssetId { get; set; }
        public virtual Asset Asset { get; set; }
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public PeriodType PeriodType { get; set; } = PeriodType.Month;
        public decimal TotalPrice => Quantity * UnitPrice;
        
        public string AdditionalConditions { get; set; } // Доп. условия по позиции
        
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}