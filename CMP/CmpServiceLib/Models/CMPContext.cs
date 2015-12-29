using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CmpServiceLib.Models.Mapping;

namespace CmpServiceLib.Models
{
    public partial class CMPContext : DbContext
    {
        static CMPContext()
        {
            Database.SetInitializer<CMPContext>(null);
        }

        public CMPContext()
            : base("Name=CMPContext")
        {
        }

        public DbSet<AzureRoleSize> AzureRoleSizes { get; set; }
        public DbSet<BadAsset> BadAssets { get; set; }
        public DbSet<ChangeLog> ChangeLogs { get; set; }
        public DbSet<CmpServiceUserAccount> CmpServiceUserAccounts { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<FluRequest> FluRequests { get; set; }
        public DbSet<OpRequest> OpRequests { get; set; }
        public DbSet<Sequence> Sequences { get; set; }
        public DbSet<ServiceProviderAccount> ServiceProviderAccounts { get; set; }
        public DbSet<ServiceProviderSlot> ServiceProviderSlots { get; set; }
        public DbSet<VmDeploymentRequest> VmDeploymentRequests { get; set; }
        public DbSet<VmMigrationRequest> VmMigrationRequests { get; set; }
        public DbSet<WAPMAPPINGDATA> WAPMAPPINGDATAs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AzureRoleSizeMap());
            modelBuilder.Configurations.Add(new BadAssetMap());
            modelBuilder.Configurations.Add(new ChangeLogMap());
            modelBuilder.Configurations.Add(new CmpServiceUserAccountMap());
            modelBuilder.Configurations.Add(new ConfigMap());
            modelBuilder.Configurations.Add(new FluRequestMap());
            modelBuilder.Configurations.Add(new OpRequestMap());
            modelBuilder.Configurations.Add(new SequenceMap());
            modelBuilder.Configurations.Add(new ServiceProviderAccountMap());
            modelBuilder.Configurations.Add(new ServiceProviderSlotMap());
            modelBuilder.Configurations.Add(new VmDeploymentRequestMap());
            modelBuilder.Configurations.Add(new VmMigrationRequestMap());
            modelBuilder.Configurations.Add(new WAPMAPPINGDATAMap());
        }
    }
}
