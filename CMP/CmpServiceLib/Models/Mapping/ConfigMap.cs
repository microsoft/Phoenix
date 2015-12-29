using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class ConfigMap : EntityTypeConfiguration<Config>
    {
        public ConfigMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(200);

            this.Property(t => t.Region)
                .HasMaxLength(50);

            this.Property(t => t.Instance)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Config");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Region).HasColumnName("Region");
            this.Property(t => t.Instance).HasColumnName("Instance");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
