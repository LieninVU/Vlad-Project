using System;

namespace ForVlad.Models
{
    public class PaymentSchedule
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public virtual Contract Contract { get; set; }
        
        public int PaymentNumber { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        
        public decimal Amount { get; set; }
        public decimal VATAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        
        public bool IsPaid { get; set; }
        public PaymentStatus Status { get; set; }
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
