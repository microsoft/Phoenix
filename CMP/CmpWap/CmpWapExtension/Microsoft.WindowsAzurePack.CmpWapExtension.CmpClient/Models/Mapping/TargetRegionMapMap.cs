using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class TargetRegionMapMap : EntityTypeConfiguration<TargetRegionMap>
    {
        public TargetRegionMapMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DisplayName)
                .HasMaxLength(50);

            this.Property(t => t.AzureReqionName)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TargetRegionMap");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.DisplayName).HasColumnName("DisplayName");
            this.Property(t => t.AzureReqionName).HasColumnName("AzureReqionName");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
