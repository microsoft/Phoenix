using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class AzureAdminSubscriptionVnetMap : EntityTypeConfiguration<AzureAdminSubscriptionVnet>
    {
        public AzureAdminSubscriptionVnetMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SubId)
                .HasMaxLength(50);

            this.Property(t => t.Subnet)
                .HasMaxLength(50);

            this.Property(t => t.Gateway)
                .HasMaxLength(50);

            this.Property(t => t.CircuitName)
                .HasMaxLength(50);

            this.Property(t => t.VNetType)
                .HasMaxLength(50);

            this.Property(t => t.VNetName)
                .HasMaxLength(50);

            this.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AzureAdminSubscriptionVnet");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SubId).HasColumnName("SubId");
            this.Property(t => t.Subnet).HasColumnName("Subnet");
            this.Property(t => t.Gateway).HasColumnName("Gateway");
            this.Property(t => t.CircuitName).HasColumnName("CircuitName");
            this.Property(t => t.VNetType).HasColumnName("VNetType");
            this.Property(t => t.VNetName).HasColumnName("VNetName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastUpdatedOn).HasColumnName("LastUpdatedOn");
            this.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
        }
    }
}
