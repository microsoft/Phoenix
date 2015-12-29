using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CmpServiceLib.Models.Mapping;

namespace CmpServiceLib.Models
{
    public partial class AzureFileTransferContext : DbContext
    {
        static AzureFileTransferContext()
        {
            Database.SetInitializer<AzureFileTransferContext>(null);
        }

        public AzureFileTransferContext()
            : base("Name=AzureFileTransferContext")
        {
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<DirectoryMonitor> DirectoryMonitors { get; set; }
        public DbSet<MonitoredFileInfo> MonitoredFileInfoes { get; set; }
        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AgentMap());
            modelBuilder.Configurations.Add(new DirectoryMonitorMap());
            modelBuilder.Configurations.Add(new MonitoredFileInfoMap());
            modelBuilder.Configurations.Add(new RequestMap());
        }
    }
}
