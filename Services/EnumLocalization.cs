using System;
using ForVlad.Models;

namespace ForVlad.Services
{
    public static class EnumLocalization
    {
        public static string ToRussian(object value)
        {
            if (value == null)
                return string.Empty;

            if (value is bool boolean)
                return boolean ? "Да" : "Нет";

            if (value is Enum enumValue)
                return ToRussian(enumValue);

            if (value is string text)
                return SubcategoryToRussian(text);

            return value.ToString();
        }

        public static string ToRussian(Enum value)
        {
            switch (value)
            {
                case ContractStatus status:
                    return ContractStatusToRussian(status);
                case ContractType type:
                    return ContractTypeToRussian(type);
                case PaymentStatus paymentStatus:
                    return PaymentStatusToRussian(paymentStatus);
                case CounterpartyType counterpartyType:
                    return CounterpartyTypeToRussian(counterpartyType);
                case AssetGroup assetGroup:
                    return AssetGroupToRussian(assetGroup);
                case VehicleSubcategory vehicleSubcategory:
                    return VehicleSubcategoryToRussian(vehicleSubcategory);
                case EquipmentSubcategory equipmentSubcategory:
                    return EquipmentSubcategoryToRussian(equipmentSubcategory);
                case AssetCondition condition:
                    return AssetConditionToRussian(condition);
                case PeriodType periodType:
                    return PeriodTypeToRussian(periodType);
                default:
                    return value.ToString();
            }
        }

        public static string SubcategoryToRussian(string subcategory)
        {
            if (string.IsNullOrWhiteSpace(subcategory))
                return subcategory;

            if (Enum.TryParse(subcategory, out VehicleSubcategory vehicleSubcategory))
                return VehicleSubcategoryToRussian(vehicleSubcategory);

            if (Enum.TryParse(subcategory, out EquipmentSubcategory equipmentSubcategory))
                return EquipmentSubcategoryToRussian(equipmentSubcategory);

            return subcategory;
        }

        public static string ContractStatusToRussian(ContractStatus status)
        {
            switch (status)
            {
                case ContractStatus.Draft: return "Черновик";
                case ContractStatus.Signed: return "Подписан";
                case ContractStatus.Active: return "Действующий";
                case ContractStatus.Suspended: return "Приостановлен";
                case ContractStatus.Completed: return "Завершён";
                case ContractStatus.Terminated: return "Расторгнут";
                default: return status.ToString();
            }
        }

        public static string ContractTypeToRussian(ContractType type)
        {
            switch (type)
            {
                case ContractType.Rental: return "Аренда";
                case ContractType.Leasing: return "Лизинг";
                default: return type.ToString();
            }
        }

        public static string PaymentStatusToRussian(PaymentStatus status)
        {
            switch (status)
            {
                case PaymentStatus.Pending: return "Ожидает оплаты";
                case PaymentStatus.Paid: return "Оплачен";
                case PaymentStatus.Overdue: return "Просрочен";
                case PaymentStatus.Cancelled: return "Отменён";
                default: return status.ToString();
            }
        }

        public static string CounterpartyTypeToRussian(CounterpartyType type)
        {
            switch (type)
            {
                case CounterpartyType.LegalEntity: return "Юридическое лицо";
                case CounterpartyType.IndividualEntrepreneur: return "ИП";
                case CounterpartyType.Individual: return "Физическое лицо";
                default: return type.ToString();
            }
        }

        public static string AssetGroupToRussian(AssetGroup group)
        {
            switch (group)
            {
                case AssetGroup.Vehicle: return "Техника";
                case AssetGroup.Equipment: return "Оборудование";
                default: return group.ToString();
            }
        }

        public static string VehicleSubcategoryToRussian(VehicleSubcategory subcategory)
        {
            switch (subcategory)
            {
                case VehicleSubcategory.ConstructionRoad: return "Дорожно-строительная";
                case VehicleSubcategory.IndustrialTransport: return "Промышленный транспорт";
                case VehicleSubcategory.OtherSelfPropelled: return "Прочая самоходная";
                default: return subcategory.ToString();
            }
        }

        public static string EquipmentSubcategoryToRussian(EquipmentSubcategory subcategory)
        {
            switch (subcategory)
            {
                case EquipmentSubcategory.Trailers: return "Прицепы и тралы";
                case EquipmentSubcategory.Warehouse: return "Складское";
                case EquipmentSubcategory.Construction: return "Строительное";
                case EquipmentSubcategory.Attachments: return "Навесное оборудование";
                case EquipmentSubcategory.IndustrialOther: return "Прочее промышленное";
                default: return subcategory.ToString();
            }
        }

        public static string AssetConditionToRussian(AssetCondition condition)
        {
            switch (condition)
            {
                case AssetCondition.New: return "Новый";
                case AssetCondition.Good: return "Хорошее";
                case AssetCondition.Satisfactory: return "Удовлетворительное";
                case AssetCondition.NeedsRepair: return "Требует ремонта";
                case AssetCondition.OutOfOrder: return "Неисправно";
                default: return condition.ToString();
            }
        }

        public static string PeriodTypeToRussian(PeriodType periodType)
        {
            switch (periodType)
            {
                case PeriodType.Hour: return "Час";
                case PeriodType.Shift: return "Смена";
                case PeriodType.Day: return "День";
                case PeriodType.Week: return "Неделя";
                case PeriodType.Month: return "Месяц";
                default: return periodType.ToString();
            }
        }
    }
}
