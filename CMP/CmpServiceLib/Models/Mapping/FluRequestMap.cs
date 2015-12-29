using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class FluRequestMap : EntityTypeConfiguration<FluRequest>
    {
        public FluRequestMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RequestName)
                .HasMaxLength(100);

            this.Property(t => t.RequestDescription)
                .HasMaxLength(1024);

            this.Property(t => t.ParentAppName)
                .HasMaxLength(50);

            this.Property(t => t.TargetVmName)
                .HasMaxLength(100);

            this.Property(t => t.SourceServerName)
                .HasMaxLength(100);

            this.Property(t => t.SourceVhdFilesCSV)
                .HasMaxLength(1024);

            this.Property(t => t.TargetLocation)
                .HasMaxLength(50);

            this.Property(t => t.WhoRequested)
                .HasMaxLength(100);

            this.Property(t => t.ExceptionMessage)
                .HasMaxLength(1024);

            this.Property(t => t.TagData)
                .HasMaxLength(4096);

            this.Property(t => t.Status)
                .HasMaxLength(50);

            this.Property(t => t.VmSize)
                .HasMaxLength(50);

            this.Property(t => t.TargetLocationType)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FluRequests");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RequestName).HasColumnName("RequestName");
            this.Property(t => t.RequestDescription).HasColumnName("RequestDescription");
            this.Property(t => t.ParentAppName).HasColumnName("ParentAppName");
            this.Property(t => t.TargetVmName).HasColumnName("TargetVmName");
            this.Property(t => t.SourceServerName).HasColumnName("SourceServerName");
            this.Property(t => t.SourceVhdFilesCSV).HasColumnName("SourceVhdFilesCSV");
            this.Property(t => t.TargetLocation).HasColumnName("TargetLocation");
            this.Property(t => t.WhoRequested).HasColumnName("WhoRequested");
            this.Property(t => t.WhenRequested).HasColumnName("WhenRequested");
            this.Property(t => t.ExceptionMessage).HasColumnName("ExceptionMessage");
            this.Property(t => t.LastStatusUpdate).HasColumnName("LastStatusUpdate");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.VmSize).HasColumnName("VmSize");
            this.Property(t => t.TargetLocationType).HasColumnName("TargetLocationType");
            this.Property(t => t.Active).HasColumnName("Active");
        }
    }
}
