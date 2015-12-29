using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class AzureRegionVmSizeMappingMap : EntityTypeConfiguration<AzureRegionVmSizeMapping>
    {
        public AzureRegionVmSizeMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("AzureRegionVmSizeMapping");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.VmSizeId).HasColumnName("VmSizeId");
            this.Property(t => t.AzureRegionId).HasColumnName("AzureRegionId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
