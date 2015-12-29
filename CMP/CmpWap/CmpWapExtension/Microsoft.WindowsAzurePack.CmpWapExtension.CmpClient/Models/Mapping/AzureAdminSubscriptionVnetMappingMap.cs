using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class AzureAdminSubscriptionVnetMappingMap : EntityTypeConfiguration<AzureAdminSubscriptionVnetMapping>
    {
        public AzureAdminSubscriptionVnetMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SubId)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AzureAdminSubscriptionVnetMapping");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SubId).HasColumnName("SubId");
            this.Property(t => t.VnetId).HasColumnName("VnetId");
        }
    }
}
