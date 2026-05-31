using System;
using System.Collections.Generic;
using System.Linq;
using ForVlad.Models;
using ForVlad.Models.Reports;
using ForVlad.Services;

namespace ForVlad.Data
{
    // REFACTOR: 31.05.2026 - Класс SimpleDataService не используется в проекте.
    // Все ViewModel используют SqlDataService через DataServiceProvider.
    // Интерфейс ISimpleDataService перенесен в отдельный файл ISimpleDataService.cs
    // если потребуется вернуть встроенное хранилище - раскомментировать этот код
    
    /*
    public interface ISimpleDataService
    {
        List<Counterparty> GetCounterparties();
        Counterparty GetCounterparty(int id);
        void SaveCounterparty(Counterparty counterparty);
        void DeleteCounterparty(int id);
        
        List<Asset> GetAssets();
        Asset GetAsset(int id);
        void SaveAsset(Asset asset);
        void DeleteAsset(int id);
        
        List<Contract> GetContracts();
        Contract GetContract(int id);
        void SaveContract(Contract contract);
        void DeleteContract(int id);
        
        List<PaymentSchedule> GetPaymentSchedules(int? contractId = null);
        List<ContractSpecification> GetSpecifications(int? contractId = null);
        void MarkPaymentPaid(int paymentId, DateTime? paymentDate = null);
        
        List<PaymentReportRow> GetPaymentReport(DateTime? dueFrom, DateTime? dueTo, bool unpaidOnly);
        List<AssetUtilizationRow> GetAssetUtilizationReport(DateTime periodStart, DateTime periodEnd, AssetGroup? assetGroup);
        
        void InitializeTestData();
        void ResetDemoData();
        bool TestConnection(out string errorMessage);
        string GenerateContractNumber(ContractType contractType);
    }

    public class SimpleDataService : ISimpleDataService
    {
        public bool TestConnection(out string errorMessage)
        {
            errorMessage = "Используется встроенное хранилище в памяти (не SQL Server).";
            return true;
        }

        public string GenerateContractNumber(ContractType contractType)
        {
            var prefix = contractType == ContractType.Rental ? "AR" : "LS";
            return $"{prefix}-{DateTime.Now:yyyy}-{new Random().Next(1, 999):000}";
        }

        private static List<Counterparty> _counterparties = new List<Counterparty>();
        private static List<Asset> _assets = new List<Asset>();
        private static List<Contract> _contracts = new List<Contract>();
        private static List<ContractSpecification> _specifications = new List<ContractSpecification>();
        private static List<PaymentSchedule> _payments = new List<PaymentSchedule>();

        private static int _counterpartyId = 1;
        private static int _assetId = 1;
        private static int _contractId = 1;
        private static int _specificationId = 1;
        private static int _paymentId = 1;

        public SimpleDataService()
        {
            if (!_counterparties.Any())
            {
                InitializeTestData();
            }
        }

        // Counterparties
        public List<Counterparty> GetCounterparties()
        {
            return _counterparties.Where(c => !c.IsDeleted).ToList();
        }

        public Counterparty GetCounterparty(int id)
        {
            return _counterparties.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
        }

        public void SaveCounterparty(Counterparty counterparty)
        {
            if (counterparty.Id == 0)
            {
                counterparty.Id = _counterpartyId++;
                counterparty.CreatedDate = DateTime.Now;
                counterparty.IsDeleted = !counterparty.IsActive;
                _counterparties.Add(counterparty);
            }
            else
            {
                var existing = GetCounterparty(counterparty.Id);
                if (existing != null)
                {
                    existing.Name = counterparty.Name;
                    existing.CounterpartyType = counterparty.CounterpartyType;
                    existing.INN = counterparty.INN;
                    existing.KPP = counterparty.KPP;
                    existing.OGRN = counterparty.OGRN;
                    existing.LegalAddress = counterparty.LegalAddress;
                    existing.ActualAddress = counterparty.ActualAddress;
                    existing.ContactPerson = counterparty.ContactPerson;
                    existing.Phone = counterparty.Phone;
                    existing.Email = counterparty.Email;
                    existing.Notes = counterparty.Notes;
                    existing.ModifiedDate = DateTime.Now;
                }
            }
        }

        public void DeleteCounterparty(int id)
        {
            var counterparty = GetCounterparty(id);
            if (counterparty != null)
            {
                counterparty.IsDeleted = true;
                counterparty.ModifiedDate = DateTime.Now;
            }
        }

        // Assets
        public List<Asset> GetAssets()
        {
            return _assets.Where(a => !a.IsDeleted).ToList();
        }

        public Asset GetAsset(int id)
        {
            return _assets.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
        }

        public void SaveAsset(Asset asset)
        {
            if (asset.Id == 0)
            {
                asset.Id = _assetId++;
                asset.CreatedDate = DateTime.Now;
                _assets.Add(asset);
            }
            else
            {
                var existing = GetAsset(asset.Id);
                if (existing != null)
                {
                    existing.Name = asset.Name;
                    existing.InventoryNumber = asset.InventoryNumber;
                    existing.AssetGroup = asset.AssetGroup;
                    existing.Subcategory = asset.Subcategory;
                    existing.Manufacturer = asset.Manufacturer;
                    existing.Model = asset.Model;
                    existing.SerialNumber = asset.SerialNumber;
                    existing.YearOfManufacture = asset.YearOfManufacture;
                    existing.PurchasePrice = asset.PurchasePrice;
                    existing.ResidualValue = asset.ResidualValue;
                    existing.MonthlyRentalRate = asset.MonthlyRentalRate;
                    existing.IsAvailable = asset.IsAvailable;
                    existing.Notes = asset.Notes;
                    existing.ModifiedDate = DateTime.Now;
                }
            }
        }

        public void DeleteAsset(int id)
        {
            var asset = GetAsset(id);
            if (asset != null)
            {
                asset.IsDeleted = true;
                asset.ModifiedDate = DateTime.Now;
            }
        }

        // Contracts
        public List<Contract> GetContracts()
        {
            return _contracts.Where(c => !c.IsDeleted).ToList();
        }

        public Contract GetContract(int id)
        {
            return _contracts.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
        }

        public void SaveContract(Contract contract)
        {
            if (contract.Id == 0)
            {
                contract.Id = _contractId++;
                contract.CreatedDate = DateTime.Now;
                _contracts.Add(contract);
            }
            else
            {
                var existing = GetContract(contract.Id);
                if (existing != null)
                {
                    existing.ContractNumber = contract.ContractNumber;
                    existing.ContractType = contract.ContractType;
                    existing.Status = contract.Status;
                    existing.CounterpartyId = contract.CounterpartyId;
                    existing.SignedDate = contract.SignedDate;
                    existing.StartDate = contract.StartDate;
                    existing.EndDate = contract.EndDate;
                    existing.DurationMonths = contract.DurationMonths;
                    existing.TotalAmount = contract.TotalAmount;
                    existing.VATAmount = contract.VATAmount;
                    existing.TotalWithVAT = contract.TotalWithVAT;
                    existing.AdvancePayment = contract.AdvancePayment;
                    existing.MonthlyPayment = contract.MonthlyPayment;
                    existing.PaymentTerms = contract.PaymentTerms;
                    existing.Notes = contract.Notes;
                    existing.ModifiedDate = DateTime.Now;
                }
            }
        }

        public void DeleteContract(int id)
        {
            var contract = GetContract(id);
            if (contract != null)
            {
                contract.IsDeleted = true;
                contract.ModifiedDate = DateTime.Now;
            }
        }

        public List<PaymentSchedule> GetPaymentSchedules(int? contractId = null)
        {
            var query = _payments.Where(p => !p.IsDeleted);
            if (contractId.HasValue)
                query = query.Where(p => p.ContractId == contractId.Value);
            return query.OrderBy(p => p.DueDate).ToList();
        }

        public List<ContractSpecification> GetSpecifications(int? contractId = null)
        {
            var query = _specifications.Where(s => !s.IsDeleted);
            if (contractId.HasValue)
                query = query.Where(s => s.ContractId == contractId.Value);
            return query.ToList();
        }

        public void MarkPaymentPaid(int paymentId, DateTime? paymentDate = null)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == paymentId && !p.IsDeleted);
            if (payment == null)
                return;
            
            payment.Status = PaymentStatus.Paid;
            payment.PaymentDate = paymentDate ?? DateTime.Now;
            payment.ModifiedDate = DateTime.Now;
            if (!payment.TotalAmount.HasValue)
                payment.TotalAmount = payment.Amount + payment.VATAmount;
        }

        public List<PaymentReportRow> GetPaymentReport(DateTime? dueFrom, DateTime? dueTo, bool unpaidOnly)
        {
            var today = DateTime.Today;
            var rows = new List<PaymentReportRow>();
            
            foreach (var payment in GetPaymentSchedules())
            {
                var contract = GetContract(payment.ContractId);
                if (contract == null)
                    continue;
                
                var isPaid = payment.Status == PaymentStatus.Paid || payment.PaymentDate.HasValue;
                if (unpaidOnly && isPaid)
                    continue;
                
                if (dueFrom.HasValue && payment.DueDate.Date < dueFrom.Value.Date)
                    continue;
                if (dueTo.HasValue && payment.DueDate.Date > dueTo.Value.Date)
                    continue;
                
                var counterparty = GetCounterparty(contract.CounterpartyId);
                var total = payment.TotalAmount ?? (payment.Amount + payment.VATAmount);
                
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
                var specs = GetSpecifications().Where(s => s.AssetId == asset.Id).ToList();
                int daysRented = 0;
                decimal revenue = 0;
                
                foreach (var spec in specs)
                {
                    var contract = GetContract(spec.ContractId);
                    if (contract == null || !ReportCalculationService.IsOperatingContractStatus(contract.Status))
                        continue;
                    if (!ReportCalculationService.ContractOverlapsPeriod(contract, periodStart, periodEnd))
                        continue;
                    
                    var contractEnd = contract.EndDate?.Date ?? periodEnd.Date;
                    var overlap = ReportCalculationService.OverlapDays(
                        contract.StartDate, contractEnd, periodStart, periodEnd);
                    
                    daysRented += overlap;
                    
                    var contractDays = Math.Max(1, (int)(contractEnd - contract.StartDate.Date).TotalDays + 1);
                    revenue += spec.TotalPrice * overlap / (decimal)contractDays;
                }
                
                daysRented = Math.Min(daysRented, daysInPeriod);
                var rate = daysInPeriod > 0 ? (double)daysRented / daysInPeriod * 100.0 : 0;
                
                string alert = null;
                if (rate < 30 && daysInPeriod >= 7)
                    alert = "Низкая загрузка (< 30%)";
                if (asset.IsAvailable && rate > 80)
                    alert = string.IsNullOrEmpty(alert) ? "Высокая загрузка" : alert;
                
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

        public void ResetDemoData()
        {
            _counterparties.Clear();
            _assets.Clear();
            _contracts.Clear();
            _specifications.Clear();
            _payments.Clear();
            _counterpartyId = 1;
            _assetId = 1;
            _contractId = 1;
            _specificationId = 1;
            _paymentId = 1;
            InitializeTestData();
        }

        public void InitializeTestData()
        {
            // Test counterparties
            _counterparties.Add(new Counterparty
            {
                Id = _counterpartyId++,
                Name = "ООО 'СтройТех'",
                CounterpartyType = CounterpartyType.LegalEntity,
                INN = "1234567890",
                KPP = "123456789",
                LegalAddress = "г. Москва, ул. Строителей, д. 1",
                ContactPerson = "Иванов Иван Иванович",
                Phone = "+7 (999) 123-45-67",
                Email = "info@stroitech.ru",
                CreatedDate = DateTime.Now,
                IsDeleted = false
            });

            _counterparties.Add(new Counterparty
            {
                Id = _counterpartyId++,
                Name = "ИП Петров П.П.",
                CounterpartyType = CounterpartyType.IndividualEntrepreneur,
                INN = "0987654321",
                LegalAddress = "г. Санкт-Петербург, пр. Невский, д. 100",
                ContactPerson = "Петров Петр Петрович",
                Phone = "+7 (911) 987-65-43",
                Email = "petrov@mail.ru",
                CreatedDate = DateTime.Now,
                IsDeleted = false
            });

            // Test assets
            _assets.Add(new Asset
            {
                Id = _assetId++,
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
            });

            _assets.Add(new Asset
            {
                Id = _assetId++,
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
            });

            // Test contract
            var contract = new Contract
            {
                Id = _contractId++,
                ContractNumber = "АР-2024-001",
                ContractType = ContractType.Rental,
                Status = ContractStatus.Active,
                CounterpartyId = 1,
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

            _contracts.Add(contract);

            // Test specification
            _specifications.Add(new ContractSpecification
            {
                Id = _specificationId++,
                ContractId = 1,
                AssetId = 1,
                Quantity = 1,
                UnitPrice = 150000,
                CreatedDate = DateTime.Now.AddDays(-30),
                IsDeleted = false
            });

            // Test payment schedule
            _payments.Add(new PaymentSchedule
            {
                Id = _paymentId++,
                ContractId = 1,
                DueDate = DateTime.Now.AddDays(-30),
                PaymentDate = DateTime.Now.AddDays(-30),
                Amount = 100000,
                VATAmount = 20000,
                TotalAmount = 120000,
                Status = PaymentStatus.Paid,
                CreatedDate = DateTime.Now.AddDays(-30),
                IsDeleted = false
            });

            _payments.Add(new PaymentSchedule
            {
                Id = _paymentId++,
                ContractId = 1,
                DueDate = DateTime.Now.AddDays(30),
                Amount = 180000,
                VATAmount = 36000,
                TotalAmount = 216000,
                Status = PaymentStatus.Pending,
                CreatedDate = DateTime.Now.AddDays(-30),
                IsDeleted = false
            });

            _payments.Add(new PaymentSchedule
            {
                Id = _paymentId++,
                ContractId = 1,
                DueDate = DateTime.Now.AddDays(-15),
                Amount = 50000,
                VATAmount = 10000,
                TotalAmount = 60000,
                Status = PaymentStatus.Overdue,
                Notes = "Просроченный платёж",
                CreatedDate = DateTime.Now.AddDays(-45),
                IsDeleted = false
            });

            var contract2 = new Contract
            {
                Id = _contractId++,
                ContractNumber = "ЛЗ-2024-002",
                ContractType = ContractType.Leasing,
                Status = ContractStatus.Signed,
                CounterpartyId = 2,
                SignedDate = DateTime.Now.AddDays(-10),
                StartDate = DateTime.Now.AddDays(-5),
                EndDate = DateTime.Now.AddMonths(12),
                DurationMonths = 12,
                TotalAmount = 360000,
                VATAmount = 72000,
                TotalWithVAT = 432000,
                MonthlyPayment = 30000,
                PaymentTerms = "Ежемесячно",
                CreatedDate = DateTime.Now.AddDays(-10),
                IsDeleted = false
            };
            _contracts.Add(contract2);

            _specifications.Add(new ContractSpecification
            {
                Id = _specificationId++,
                ContractId = 2,
                AssetId = 2,
                Quantity = 1,
                UnitPrice = 30000,
                CreatedDate = DateTime.Now.AddDays(-10),
                IsDeleted = false
            });

            _payments.Add(new PaymentSchedule
            {
                Id = _paymentId++,
                ContractId = 2,
                DueDate = DateTime.Now.AddDays(5),
                Amount = 30000,
                VATAmount = 6000,
                TotalAmount = 36000,
                Status = PaymentStatus.Pending,
                CreatedDate = DateTime.Now.AddDays(-10),
                IsDeleted = false
            });

            var rentedAsset = GetAsset(1);
            if (rentedAsset != null)
                rentedAsset.IsAvailable = false;
        }
    }
    */
}