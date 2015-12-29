using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class AgentMap : EntityTypeConfiguration<Agent>
    {
        public AgentMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.HostName)
                .HasMaxLength(100);

            this.Property(t => t.Region)
                .HasMaxLength(100);

            this.Property(t => t.StatusCode)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Agents");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.HostName).HasColumnName("HostName");
            this.Property(t => t.Region).HasColumnName("Region");
            this.Property(t => t.CheckInTime).HasColumnName("CheckInTime");
            this.Property(t => t.StatusCode).HasColumnName("StatusCode");
            this.Property(t => t.StatusMessage).HasColumnName("StatusMessage");
            this.Property(t => t.TransferLoad).HasColumnName("TransferLoad");
            this.Property(t => t.MaxSupportedLoad).HasColumnName("MaxSupportedLoad");
            this.Property(t => t.AgentConfig).HasColumnName("AgentConfig");
            this.Property(t => t.SystemConfig).HasColumnName("SystemConfig");
            this.Property(t => t.Enabled).HasColumnName("Enabled");
        }
    }
}
