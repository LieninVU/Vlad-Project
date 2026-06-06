using System;
using System.Collections.Generic;
using ForVlad.Models;
using ForVlad.Models.Reports;

namespace ForVlad.Data
{
    /// <summary>
    /// Интерфейс сервиса данных для работы с сущностями системы
    /// </summary>
    public interface ISimpleDataService
    {
        // Контрагенты
        List<Counterparty> GetCounterparties();
        Counterparty GetCounterparty(int id);
        void SaveCounterparty(Counterparty counterparty);
        void DeleteCounterparty(int id);
        
        // Техника и оборудование
        List<Asset> GetAssets();
        Asset GetAsset(int id);
        void SaveAsset(Asset asset);
        void DeleteAsset(int id);
        
        // Договоры
        List<Contract> GetContracts();
        Contract GetContract(int id);
        void SaveContract(Contract contract);
        void DeleteContract(int id);
        
        // Спецификации договоров
        List<ContractSpecification> GetSpecifications(int? contractId = null);
        ContractSpecification GetSpecification(int id);
        void SaveSpecification(ContractSpecification specification);
        void DeleteSpecification(int id);
        
        // Графики платежей
        List<PaymentSchedule> GetPaymentSchedules(int? contractId = null);
        PaymentSchedule GetPaymentSchedule(int id);
        void SavePaymentSchedule(PaymentSchedule schedule);
        void MarkPaymentPaid(int paymentId, DateTime? paymentDate = null);
        void DeletePaymentSchedule(int id);
        
        // Генерация графика платежей
        void GeneratePaymentSchedule(int contractId, int paymentCount, DateTime startDate, decimal amountPerPayment);
        
        // Отчётность
        List<PaymentReportRow> GetPaymentReport(DateTime? dueFrom, DateTime? dueTo, bool unpaidOnly);
        List<AssetUtilizationRow> GetAssetUtilizationReport(DateTime periodStart, DateTime periodEnd, AssetGroup? assetGroup);

        // Проверка доступности
        bool CheckAssetAvailability(int assetId, DateTime startDate, DateTime endDate, int? excludeContractId = null);
        bool HasActiveContractSpecifications(int assetId);

        // Утилиты
        void InitializeTestData();
        void ResetDemoData();
        bool TestConnection(out string errorMessage);
        string GenerateContractNumber(ContractType contractType);
    }
}
