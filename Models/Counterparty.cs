using System;
using System.Collections.Generic;

namespace ForVlad.Models
{
    public class Counterparty
    {
        public int Id { get; set; }
        public string Name { get; set; } // Наименование организации / ФИО ИП
        public CounterpartyType CounterpartyType { get; set; } = CounterpartyType.LegalEntity;
        public string INN { get; set; }
        public string KPP { get; set; } // Опционально для юрлиц
        public string OGRN { get; set; } // ОГРН/ОГРНИП
        public string LegalAddress { get; set; }
        public string ActualAddress { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        
        /// <summary>Активен (не деактивирован) — соответствует IsActive в спецификации.</summary>
        public bool IsActive
        {
            get => !IsDeleted;
            set => IsDeleted = !value;
        }
        
        public virtual ICollection<Contract> Contracts { get; set; }
        
        public Counterparty()
        {
            Contracts = new List<Contract>();
        }
    }
}