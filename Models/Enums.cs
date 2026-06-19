namespace ForVlad.Models
{
    public enum CounterpartyType { LegalEntity, IndividualEntrepreneur, Individual }
    
    public enum AssetGroup { Vehicle, Equipment }
    
    public enum VehicleSubcategory 
    { 
        ConstructionRoad,    // экскаваторы, бульдозеры, автогрейдеры...
        IndustrialTransport, // погрузчики, ричтраки, карьерные самосвалы...
        OtherSelfPropelled   // мотоциклы, квадроциклы, снегоходы...
    }

    public enum EquipmentSubcategory
    {
        Trailers,           // прицепы, полуприцепы, тралы
        Warehouse,          // тележки, штабелеры, конвейеры
        Construction,       // бетоносмесители, генераторы, сварочные аппараты
        Attachments,        // ковши, гидромолоты, отвалы, щётки
        IndustrialOther     // насосы, станки, окрасочные камеры
    }

    public enum ContractType { Rental, Leasing }
    
    public enum PaymentStatus { Pending, Paid, Overdue, Cancelled }
    
    public enum AssetCondition { New, Good, Satisfactory, NeedsRepair, OutOfOrder }
    
    public enum PeriodType { Hour, Shift, Day, Week, Month }
}