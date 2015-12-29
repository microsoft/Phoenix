using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class WAPMAPPINGDATAMap : EntityTypeConfiguration<WAPMAPPINGDATA>
    {
        public WAPMAPPINGDATAMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.WapSubscriptionID)
                .HasMaxLength(100);

            this.Property(t => t.TargetVmName)
                .HasMaxLength(256);

            this.Property(t => t.AdminUser)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("WAPMAPPINGDATA");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.WapSubscriptionID).HasColumnName("WapSubscriptionID");
            this.Property(t => t.TargetVmName).HasColumnName("TargetVmName");
            this.Property(t => t.AdminUser).HasColumnName("AdminUser");
        }
    }
}
