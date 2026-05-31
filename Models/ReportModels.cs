using System;
using ForVlad.Models;

namespace ForVlad.Models.Reports
{
    public class PaymentReportRow
    {
        public int PaymentId { get; set; }
        public int ContractId { get; set; }
        public string ContractNumber { get; set; }
        public string CounterpartyName { get; set; }
        public string ContactPerson { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public decimal? TotalAmount { get; set; }
        public PaymentStatus Status { get; set; }
        public bool IsPaid { get; set; }
        public int DaysOverdue { get; set; }
        public string AgingBucket { get; set; }
        public string Notes { get; set; }
    }

    public class AssetUtilizationRow
    {
        public int AssetId { get; set; }
        public string InventoryNumber { get; set; }
        public string AssetName { get; set; }
        public AssetGroup AssetGroup { get; set; }
        public string Subcategory { get; set; }
        public bool IsAvailable { get; set; }
        public int DaysInPeriod { get; set; }
        public int DaysRented { get; set; }
        public double UtilizationRate { get; set; }
        public decimal Revenue { get; set; }
        public decimal MonthlyRate { get; set; }
        public string AlertMessage { get; set; }
    }

    public class FinancialSummary
    {
        public decimal TotalDue { get; set; }
        public decimal TotalOverdue { get; set; }
        public decimal TotalPaidInPeriod { get; set; }
        public int PaymentCount { get; set; }
        public int OverdueCount { get; set; }
    }

    public class UtilizationSummary
    {
        public int TotalAssets { get; set; }
        public double AverageUtilization { get; set; }
        public decimal TotalRevenue { get; set; }
        public int LowUtilizationCount { get; set; }
    }
}
