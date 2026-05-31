using System;
using System.Collections.Generic;

namespace ForVlad.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; } // Уникальный номер (например, АР-2024-001)
        public ContractType ContractType { get; set; }
        public ContractStatus Status { get; set; }
        
        public int CounterpartyId { get; set; }
        public virtual Counterparty Counterparty { get; set; }
        
        /// <summary>Для отображения в списках (не хранится в БД).</summary>
        public string CounterpartyDisplayName { get; set; }
        
        public string PeriodDisplay
        {
            get
            {
                if (!EndDate.HasValue)
                    return $"{StartDate:dd.MM.yyyy} — бессрочно";
                return $"{StartDate:dd.MM.yyyy} — {EndDate:dd.MM.yyyy}";
            }
        }
        
        public DateTime SignedDate { get; set; } // Дата подписания
        public DateTime StartDate { get; set; } // Начало действия
        public DateTime? EndDate { get; set; } // Окончание (null для бессрочных)
        public int DurationMonths { get; set; } // Продолжительность в месяцах
        
        public decimal TotalAmount { get; set; } // Общая сумма договора
        public decimal VATAmount { get; set; } // Сумма НДС
        public decimal TotalWithVAT { get; set; } // Сумма с НДС
        public decimal AdvancePayment { get; set; } // Авансовый платеж
        public decimal MonthlyPayment { get; set; } // Ежемесячный платеж
        public string PaymentTerms { get; set; } // Условия оплаты
        public string Notes { get; set; } // Примечания
        
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool IsDeleted { get; set; }
        
        public virtual ICollection<ContractSpecification> Specifications { get; set; }
        public virtual ICollection<PaymentSchedule> PaymentSchedules { get; set; }
        
        public Contract()
        {
            Specifications = new List<ContractSpecification>();
            PaymentSchedules = new List<PaymentSchedule>();
        }
    }
}