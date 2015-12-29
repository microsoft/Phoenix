using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class VmMigrationRequestMap : EntityTypeConfiguration<VmMigrationRequest>
    {
        public VmMigrationRequestMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.VmSize)
                .HasMaxLength(50);

            this.Property(t => t.TargetVmName)
                .HasMaxLength(256);

            this.Property(t => t.SourceServerName)
                .HasMaxLength(256);

            this.Property(t => t.StatusCode)
                .HasMaxLength(50);

            this.Property(t => t.StatusMessage)
                .HasMaxLength(4096);

            this.Property(t => t.AgentRegion)
                .HasMaxLength(50);

            this.Property(t => t.AgentName)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("VmMigrationRequests");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.VmDeploymentRequestID).HasColumnName("VmDeploymentRequestID");
            this.Property(t => t.VmSize).HasColumnName("VmSize");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.TagID).HasColumnName("TagID");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.TargetVmName).HasColumnName("TargetVmName");
            this.Property(t => t.SourceServerName).HasColumnName("SourceServerName");
            this.Property(t => t.SourceVhdFilesCSV).HasColumnName("SourceVhdFilesCSV");
            this.Property(t => t.ExceptionMessage).HasColumnName("ExceptionMessage");
            this.Property(t => t.LastStatusUpdate).HasColumnName("LastStatusUpdate");
            this.Property(t => t.StatusCode).HasColumnName("StatusCode");
            this.Property(t => t.StatusMessage).HasColumnName("StatusMessage");
            this.Property(t => t.AgentRegion).HasColumnName("AgentRegion");
            this.Property(t => t.AgentName).HasColumnName("AgentName");
            this.Property(t => t.CurrentStateStartTime).HasColumnName("CurrentStateStartTime");
            this.Property(t => t.CurrentStateTryCount).HasColumnName("CurrentStateTryCount");
            this.Property(t => t.Warnings).HasColumnName("Warnings");
            this.Property(t => t.Active).HasColumnName("Active");

            // Relationships
            this.HasRequired(t => t.VmDeploymentRequest)
                .WithMany(t => t.VmMigrationRequests)
                .HasForeignKey(d => d.VmDeploymentRequestID);

        }
    }
}
