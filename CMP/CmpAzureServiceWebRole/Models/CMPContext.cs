using System.Data.Entity;
using CmpAzureServiceWebRole.Models.Mapping;

namespace CmpAzureServiceWebRole.Models
{
    public partial class CMPContext : DbContext
    {
        static CMPContext()
        {
            Database.SetInitializer<CMPContext>(null);
        }

        public CMPContext()
            : base(GetConnectionString())
        {
        }

        public static string GetConnectionString()
        {
            using (var xk = new KryptoLib.X509Krypto())
            {
                return xk.GetKTextConnectionString("CMPContext",
                    "CMPContextPassword");

            }
        }

        public DbSet<ChangeLog> ChangeLogs { get; set; }
        public DbSet<CmpServiceUserAccount> CmpServiceUserAccounts { get; set; }
        public DbSet<FluRequest> FluRequests { get; set; }
        public DbSet<Sequence> Sequences { get; set; }
        public DbSet<ServiceProviderAccount> ServiceProviderAccounts { get; set; }
        public DbSet<ServiceProviderSlot> ServiceProviderSlots { get; set; }
        public DbSet<VmDeploymentRequest> VmDeploymentRequests { get; set; }
        public DbSet<VmMigrationRequest> VmMigrationRequests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ChangeLogMap());
            modelBuilder.Configurations.Add(new CmpServiceUserAccountMap());
            modelBuilder.Configurations.Add(new FluRequestMap());
            modelBuilder.Configurations.Add(new SequenceMap());
            modelBuilder.Configurations.Add(new ServiceProviderAccountMap());
            modelBuilder.Configurations.Add(new ServiceProviderSlotMap());
            modelBuilder.Configurations.Add(new VmDeploymentRequestMap());
            modelBuilder.Configurations.Add(new VmMigrationRequestMap());
        }
    }
}
