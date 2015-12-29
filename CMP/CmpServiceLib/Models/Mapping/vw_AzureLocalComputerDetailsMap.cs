using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class vw_AzureLocalComputerDetailsMap : EntityTypeConfiguration<vw_AzureLocalComputerDetails>
    {
        public vw_AzureLocalComputerDetailsMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.LocalAdminPassword });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ServerName)
                .HasMaxLength(256);

            this.Property(t => t.LocalAdminUsername)
                .HasMaxLength(263);

            this.Property(t => t.LocalAdminPassword)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("vw_AzureLocalComputerDetails");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ServerName).HasColumnName("ServerName");
            this.Property(t => t.LocalAdminUsername).HasColumnName("LocalAdminUsername");
            this.Property(t => t.LocalAdminPassword).HasColumnName("LocalAdminPassword");
        }
    }
}
