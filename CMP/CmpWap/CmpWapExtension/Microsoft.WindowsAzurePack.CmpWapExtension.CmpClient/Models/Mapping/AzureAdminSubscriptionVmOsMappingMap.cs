using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class AzureAdminSubscriptionVmOsMappingMap : EntityTypeConfiguration<AzureAdminSubscriptionVmOsMapping>
    {
        public AzureAdminSubscriptionVmOsMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SubscriptionId)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AzureAdminSubscriptionVmOsMapping");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.VmOsId).HasColumnName("VmOsId");
            this.Property(t => t.SubscriptionId).HasColumnName("SubscriptionId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
