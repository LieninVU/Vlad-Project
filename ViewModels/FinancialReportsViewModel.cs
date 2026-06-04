using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ForVlad.Data;
using ForVlad.Models.Reports;
using ForVlad.Services;
using Microsoft.Win32;

namespace ForVlad.ViewModels
{
    public class FinancialReportsViewModel : ViewModelBase
    {
        private readonly ISimpleDataService _dataService;
        
        private ObservableCollection<PaymentReportRow> _payments;
        public ObservableCollection<PaymentReportRow> Payments
        {
            get => _payments;
            set => SetField(ref _payments, value);
        }
        
        private PaymentReportRow _selectedPayment;
        public PaymentReportRow SelectedPayment
        {
            get => _selectedPayment;
            set => SetField(ref _selectedPayment, value);
        }
        
        private DateTime? _dueDateFrom;
        public DateTime? DueDateFrom
        {
            get => _dueDateFrom;
            set
            {
                if (SetField(ref _dueDateFrom, value))
                {
                    OnPropertyChanged(nameof(DueDateFromDisplay));
                }
            }
        }
        
        private DateTime? _dueDateTo;
        public DateTime? DueDateTo
        {
            get => _dueDateTo;
            set
            {
                if (SetField(ref _dueDateTo, value))
                {
                    OnPropertyChanged(nameof(DueDateToDisplay));
                }
            }
        }
        
        // Вспомогательные свойства для отображения
        public string DueDateFromDisplay => DueDateFrom?.ToString("dd.MM.yyyy") ?? "-";
        public string DueDateToDisplay => DueDateTo?.ToString("dd.MM.yyyy") ?? "-";
        
        private bool _unpaidOnly = true;
        public bool UnpaidOnly
        {
            get => _unpaidOnly;
            set
            {
                if (SetField(ref _unpaidOnly, value))
                    LoadReport();
            }
        }
        
        private FinancialSummary _summary;
        public FinancialSummary Summary
        {
            get => _summary;
            set => SetField(ref _summary, value);
        }
        
        public bool HasPayments => Payments != null && Payments.Count > 0;
        
        public ICommand RefreshCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        public ICommand MarkPaidCommand { get; }
        
        public FinancialReportsViewModel(ISimpleDataService dataService)
        {
            _dataService = dataService;
            Payments = new ObservableCollection<PaymentReportRow>();
            Summary = new FinancialSummary();
            
            var today = DateTime.Today;
            DueDateFrom = today.AddMonths(-3);
            DueDateTo = today.AddMonths(3);
            
            RefreshCommand = new RelayCommand(_ => LoadReport());
            ApplyFiltersCommand = new RelayCommand(_ => LoadReport());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            ExportToCsvCommand = new RelayCommand(_ => ExportCsv(), _ => HasPayments);
            MarkPaidCommand = new RelayCommand(_ => MarkSelectedPaid(), _ => CanMarkPaid());
            
            LoadReport();
        }
        
        public void LoadReport()
        {
            var dueFrom = DueDateFrom?.Date;
            var dueTo = DueDateTo?.Date;
            
            var rows = _dataService.GetPaymentReport(dueFrom, dueTo, UnpaidOnly);
            Payments.Clear();
            foreach (var row in rows)
                Payments.Add(row);
            
            var periodStart = dueFrom ?? DateTime.Today.AddMonths(-1);
            var periodEnd = dueTo ?? DateTime.Today.AddMonths(1);
            Summary = ReportCalculationService.BuildFinancialSummary(rows, periodStart, periodEnd);
            OnPropertyChanged(nameof(Summary));
            OnPropertyChanged(nameof(HasPayments));
            OnPropertyChanged(nameof(DueDateFromDisplay));
            OnPropertyChanged(nameof(DueDateToDisplay));
        }
        
        private void ClearFilters()
        {
            var today = DateTime.Today;
            DueDateFrom = today.AddMonths(-3);
            DueDateTo = today.AddMonths(3);
            UnpaidOnly = false;
            LoadReport();
        }
        
        private bool CanMarkPaid()
        {
            return SelectedPayment != null && !SelectedPayment.IsPaid;
        }
        
        private void MarkSelectedPaid()
        {
            if (SelectedPayment == null || SelectedPayment.IsPaid)
                return;
            
            var result = MessageBox.Show(
                $"Отметить платёж по договору {SelectedPayment.ContractNumber} ({SelectedPayment.TotalAmount:N2} ₽) как оплаченный?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result != MessageBoxResult.Yes)
                return;
            
            _dataService.MarkPaymentPaid(SelectedPayment.PaymentId);
            LoadReport();
            MessageBox.Show("Платёж отмечен как оплаченный", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void ExportCsv()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = $"Платежи_{DateTime.Now:yyyyMMdd}.csv"
            };
            
            if (dialog.ShowDialog() != true)
                return;
            
            var headers = new[]
            {
                "Договор", "Контрагент", "Срок", "Сумма", "Статус", "Просрочка", "Категория", "Оплачен"
            };
            
            var rows = Payments.Select(p => new[]
            {
                p.ContractNumber,
                p.CounterpartyName,
                p.DueDate.ToString("dd.MM.yyyy"),
                (p.TotalAmount ?? 0).ToString("N2"),
                p.Status.ToString(),
                p.DaysOverdue.ToString(),
                p.AgingBucket,
                p.IsPaid ? "Да" : "Нет"
            });
            
            CsvExportService.Export(dialog.FileName, headers, rows);
            MessageBox.Show($"Файл сохранён:\n{dialog.FileName}", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
