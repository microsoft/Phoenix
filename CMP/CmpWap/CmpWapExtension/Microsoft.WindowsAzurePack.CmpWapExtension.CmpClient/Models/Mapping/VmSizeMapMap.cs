using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class VmSizeMapMap : EntityTypeConfiguration<VmSizeMap>
    {
        public VmSizeMapMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("VmSizeMap");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.DisplayName).HasColumnName("DisplayName");
            this.Property(t => t.AzureSizeName).HasColumnName("AzureSizeName");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CpuCoreCount).HasColumnName("CpuCoreCount");
            this.Property(t => t.RamMB).HasColumnName("RamMB");
            this.Property(t => t.DiskSizeOS).HasColumnName("DiskSizeOS");
            this.Property(t => t.DiskSizeTemp).HasColumnName("DiskSizeTemp");
            this.Property(t => t.DataDiskCount).HasColumnName("DataDiskCount");
        }
    }
}
