using System;
using System.Collections.Generic;
using System.Linq;
using ForVlad.Models;
using ForVlad.Models.Reports;

namespace ForVlad.Services
{
    public static class ReportCalculationService
    {
        public static string GetAgingBucket(DateTime dueDate, bool isPaid, DateTime today)
        {
            if (isPaid)
                return "Оплачено";
            
            var days = (today.Date - dueDate.Date).Days;
            if (days < 0)
                return "Текущий";
            if (days <= 7)
                return "1–7 дн.";
            if (days <= 30)
                return "8–30 дн.";
            return "30+ дн.";
        }

        public static int GetDaysOverdue(DateTime dueDate, bool isPaid, DateTime today)
        {
            if (isPaid)
                return 0;
            var days = (today.Date - dueDate.Date).Days;
            return days > 0 ? days : 0;
        }

        public static int OverlapDays(DateTime periodStart, DateTime periodEnd, DateTime rangeStart, DateTime rangeEnd)
        {
            var start = periodStart.Date > rangeStart.Date ? periodStart.Date : rangeStart.Date;
            var end = periodEnd.Date < rangeEnd.Date ? periodEnd.Date : rangeEnd.Date;
            var days = (int)(end - start).TotalDays + 1;
            return days > 0 ? days : 0;
        }

        public static bool ContractOverlapsPeriod(Contract contract, DateTime rangeStart, DateTime rangeEnd)
        {
            if (contract.IsDeleted)
                return false;
            
            var contractEnd = contract.EndDate?.Date ?? rangeEnd.Date;
            return contract.StartDate.Date <= rangeEnd.Date && contractEnd >= rangeStart.Date;
        }

        public static (DateTime start, DateTime end) ResolvePeriod(string periodKind, DateTime? customStart, DateTime? customEnd)
        {
            var today = DateTime.Today;
            switch (periodKind)
            {
                case "Квартал":
                    var quarterStartMonth = ((today.Month - 1) / 3) * 3 + 1;
                    var startQ = new DateTime(today.Year, quarterStartMonth, 1);
                    return (startQ, startQ.AddMonths(3).AddDays(-1));
                case "Год":
                    return (new DateTime(today.Year, 1, 1), new DateTime(today.Year, 12, 31));
                case "Произвольный" when customStart.HasValue && customEnd.HasValue:
                    return (customStart.Value.Date, customEnd.Value.Date);
                default: // Месяц
                    var startM = new DateTime(today.Year, today.Month, 1);
                    return (startM, startM.AddMonths(1).AddDays(-1));
            }
        }

        public static FinancialSummary BuildFinancialSummary(IEnumerable<PaymentReportRow> rows, DateTime periodStart, DateTime periodEnd)
        {
            var list = rows.ToList();
            var today = DateTime.Today;
            return new FinancialSummary
            {
                PaymentCount = list.Count,
                TotalDue = list.Where(r => !r.IsPaid).Sum(r => r.TotalAmount ?? 0),
                TotalOverdue = list.Where(r => !r.IsPaid && r.DueDate.Date < today).Sum(r => r.TotalAmount ?? 0),
                TotalPaidInPeriod = list.Where(r => r.IsPaid && r.PaymentDate.HasValue &&
                    r.PaymentDate.Value.Date >= periodStart.Date && r.PaymentDate.Value.Date <= periodEnd.Date)
                    .Sum(r => r.TotalAmount ?? 0),
                OverdueCount = list.Count(r => !r.IsPaid && r.DueDate.Date < today)
            };
        }

        public static UtilizationSummary BuildUtilizationSummary(IEnumerable<AssetUtilizationRow> rows)
        {
            var list = rows.ToList();
            return new UtilizationSummary
            {
                TotalAssets = list.Count,
                AverageUtilization = list.Count > 0 ? list.Average(r => r.UtilizationRate) : 0,
                TotalRevenue = list.Sum(r => r.Revenue),
                LowUtilizationCount = list.Count(r => r.UtilizationRate < 30 && r.DaysInPeriod > 0)
            };
        }
    }
}
