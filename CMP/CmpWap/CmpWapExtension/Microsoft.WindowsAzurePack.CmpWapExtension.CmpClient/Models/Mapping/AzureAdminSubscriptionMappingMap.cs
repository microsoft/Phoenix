using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class AzureAdminSubscriptionMappingMap : EntityTypeConfiguration<AzureAdminSubscriptionMapping>
    {
        public AzureAdminSubscriptionMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SubId)
                .HasMaxLength(50);

            this.Property(t => t.PlanId)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AzureAdminSubscriptionMapping");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SubId).HasColumnName("SubId");
            this.Property(t => t.PlanId).HasColumnName("PlanId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
