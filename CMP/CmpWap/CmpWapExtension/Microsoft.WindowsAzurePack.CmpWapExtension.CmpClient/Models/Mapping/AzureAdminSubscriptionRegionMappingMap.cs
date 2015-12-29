using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class AzureAdminSubscriptionRegionMappingMap : EntityTypeConfiguration<AzureAdminSubscriptionRegionMapping>
    {
        public AzureAdminSubscriptionRegionMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SubId)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AzureAdminSubscriptionRegionMapping");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SubId).HasColumnName("SubId");
            this.Property(t => t.AzureRegionId).HasColumnName("AzureRegionId");
        }
    }
}
