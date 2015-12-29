using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class AzureRoleSizeMap : EntityTypeConfiguration<AzureRoleSize>
    {
        public AzureRoleSizeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("AzureRoleSize");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.CoreCount).HasColumnName("CoreCount");
            this.Property(t => t.DiskCount).HasColumnName("DiskCount");
            this.Property(t => t.RamMb).HasColumnName("RamMb");
            this.Property(t => t.DiskSizeRoleOs).HasColumnName("DiskSizeRoleOs");
            this.Property(t => t.DiskSizeRoleApps).HasColumnName("DiskSizeRoleApps");
            this.Property(t => t.DiskSizeVmOs).HasColumnName("DiskSizeVmOs");
            this.Property(t => t.DiskSizeVmTemp).HasColumnName("DiskSizeVmTemp");
            this.Property(t => t.CanBeService).HasColumnName("CanBeService");
            this.Property(t => t.CanBeVm).HasColumnName("CanBeVm");
        }
    }
}
