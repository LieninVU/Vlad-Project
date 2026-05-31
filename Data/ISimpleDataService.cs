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
        
        // Графики платежей
        List<PaymentSchedule> GetPaymentSchedules(int? contractId = null);
        List<ContractSpecification> GetSpecifications(int? contractId = null);
        void MarkPaymentPaid(int paymentId, DateTime? paymentDate = null);
        
        // Отчётность
        List<PaymentReportRow> GetPaymentReport(DateTime? dueFrom, DateTime? dueTo, bool unpaidOnly);
        List<AssetUtilizationRow> GetAssetUtilizationReport(DateTime periodStart, DateTime periodEnd, AssetGroup? assetGroup);
        
        // Утилиты
        void InitializeTestData();
        void ResetDemoData();
        bool TestConnection(out string errorMessage);
        string GenerateContractNumber(ContractType contractType);
    }
}
