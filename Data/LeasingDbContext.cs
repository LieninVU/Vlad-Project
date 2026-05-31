using System.Data.Entity;
using ForVlad.Models;

namespace ForVlad.Data
{
    /// <summary>
    /// Контекст базы данных для Entity Framework 6
    /// Использовать после установки Entity Framework через NuGet:
    /// Install-Package EntityFramework
    /// </summary>
    public class LeasingDbContext : DbContext
    {
        public LeasingDbContext() : base("name=LeasingSystem")
        {
            // Отключаем инициализацию, т.к. база уже существует
            Database.SetInitializer<LeasingDbContext>(null);
        }
        
        // DbSets для каждой таблицы
        public DbSet<Counterparty> Counterparties { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractSpecification> ContractSpecifications { get; set; }
        public DbSet<PaymentSchedule> PaymentSchedules { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Настройка связей между таблицами
            
            // Contract -> Counterparty (многие к одному)
            modelBuilder.Entity<Contract>()
                .HasRequired(c => c.Counterparty)
                .WithMany(c => c.Contracts)
                .HasForeignKey(c => c.CounterpartyId)
                .WillCascadeOnDelete(false);
            
            // ContractSpecification -> Contract (многие к одному)
            modelBuilder.Entity<ContractSpecification>()
                .HasRequired(cs => cs.Contract)
                .WithMany(c => c.Specifications)
                .HasForeignKey(cs => cs.ContractId)
                .WillCascadeOnDelete(true);
            
            // ContractSpecification -> Asset (многие к одному)
            modelBuilder.Entity<ContractSpecification>()
                .HasRequired(cs => cs.Asset)
                .WithMany(a => a.Specifications)
                .HasForeignKey(cs => cs.AssetId)
                .WillCascadeOnDelete(false);
            
            // PaymentSchedule -> Contract (многие к одному)
            modelBuilder.Entity<PaymentSchedule>()
                .HasRequired(ps => ps.Contract)
                .WithMany(c => c.PaymentSchedules)
                .HasForeignKey(ps => ps.ContractId)
                .WillCascadeOnDelete(true);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}