using System.Data.Entity;
using CmpAzureWorkerWorkerRole.Models.Mapping;

namespace CmpAzureWorkerWorkerRole.Models
{
    public partial class NoopContext : DbContext
    {
        static NoopContext()
        {
            Database.SetInitializer<NoopContext>(null);
        }

        public NoopContext()
            : base("Name=NoopContext")
        {
        }

        public DbSet<Nodat> Nodats { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new NodatMap());
        }
    }
}
