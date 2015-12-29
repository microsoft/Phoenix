using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class DirectoryMonitorMap : EntityTypeConfiguration<DirectoryMonitor>
    {
        public DirectoryMonitorMap()
        {
            // Primary Key
            this.HasKey(t => new { t.MonitorID, t.Source, t.Destination, t.TransferType, t.SourceType, t.DestinationType, t.Name, t.Notes, t.Config, t.WhoRequested, t.WhenRequested });

            // Properties
            this.Property(t => t.MonitorID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Source)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Destination)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.StorageAccountKey)
                .HasMaxLength(100);

            this.Property(t => t.TransferType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SourceType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DestinationType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Notes)
                .IsRequired()
                .HasMaxLength(4096);

            this.Property(t => t.Config)
                .IsRequired()
                .HasMaxLength(4096);

            this.Property(t => t.WhoRequested)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ResultStatusCode)
                .HasMaxLength(50);

            this.Property(t => t.ResultDescription)
                .IsFixedLength()
                .HasMaxLength(512);

            // Table & Column Mappings
            this.ToTable("DirectoryMonitor");
            this.Property(t => t.MonitorID).HasColumnName("MonitorID");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.Destination).HasColumnName("Destination");
            this.Property(t => t.StorageAccountKey).HasColumnName("StorageAccountKey");
            this.Property(t => t.TransferType).HasColumnName("TransferType");
            this.Property(t => t.SourceType).HasColumnName("SourceType");
            this.Property(t => t.DestinationType).HasColumnName("DestinationType");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.WhoRequested).HasColumnName("WhoRequested");
            this.Property(t => t.WhenRequested).HasColumnName("WhenRequested");
            this.Property(t => t.Enabled).HasColumnName("Enabled");
            this.Property(t => t.ResultStatusCode).HasColumnName("ResultStatusCode");
            this.Property(t => t.ResultDescription).HasColumnName("ResultDescription");
        }
    }
}
