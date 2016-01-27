using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class ContainerMap : EntityTypeConfiguration<Container>
    {
        public ContainerMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.Code)
                .HasMaxLength(300);

            this.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CIOwner)
                .HasMaxLength(150);

            this.Property(t => t.SubscriptionId)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Region)
                .HasMaxLength(50);

            this.Property(t => t.Path)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Containers");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.HasService).HasColumnName("HasService");
            this.Property(t => t.CIOwner).HasColumnName("CIOwner");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.SubscriptionId).HasColumnName("SubscriptionId");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastUpdatedOn).HasColumnName("LastUpdatedOn");
            this.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
            this.Property(t => t.Region).HasColumnName("Region");
            this.Property(t => t.Path).HasColumnName("Path");
            this.Property(t => t.Config).HasColumnName("Config");
        }
    }
}
