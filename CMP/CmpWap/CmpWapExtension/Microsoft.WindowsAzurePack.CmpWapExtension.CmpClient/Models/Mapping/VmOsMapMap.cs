using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class VmOsMapMap : EntityTypeConfiguration<VmOsMap>
    {
        public VmOsMapMap()
        {
            // Primary Key
            this.HasKey(t => t.VMOsId);

            // Properties
            this.Property(t => t.VMOsId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("VMOs");
            this.Property(t => t.VMOsId).HasColumnName("VMOsId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.AzureImageName).HasColumnName("AzureImageName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
          
        }
    }
}
