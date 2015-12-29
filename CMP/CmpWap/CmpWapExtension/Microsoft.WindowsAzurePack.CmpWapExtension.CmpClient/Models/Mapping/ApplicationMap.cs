using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class ApplicationMap : EntityTypeConfiguration<Application>
    {
        public ApplicationMap()
        {
            // Primary Key
            this.HasKey(t => t.ApplicationId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.CIOwner)
                .HasMaxLength(150);

            this.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SubscriptionId)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("Application");
            this.Property(t => t.ApplicationId).HasColumnName("ApplicationId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.HasService).HasColumnName("HasService");
            this.Property(t => t.CIOwner).HasColumnName("CIOwner");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastUpdatedOn).HasColumnName("LastUpdatedOn");
            this.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
            this.Property(t => t.SubscriptionId).HasColumnName("SubscriptionId");
        }
    }
}
