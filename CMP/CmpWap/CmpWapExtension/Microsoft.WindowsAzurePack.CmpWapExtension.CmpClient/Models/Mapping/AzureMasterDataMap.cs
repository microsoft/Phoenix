using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class AzureMasterDataMap : EntityTypeConfiguration<AzureMasterData>
    {
        public AzureMasterDataMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.OsName)
                .HasMaxLength(100);

            this.Property(t => t.RegionName)
                .HasMaxLength(100);

            this.Property(t => t.VnetName)
                .HasMaxLength(100);

            this.Property(t => t.VmSize)
                .HasMaxLength(100);

            this.Property(t => t.StorageType)
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            this.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AzureMasterData");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.OsName).HasColumnName("OsName");
            this.Property(t => t.RegionName).HasColumnName("RegionName");
            this.Property(t => t.VnetName).HasColumnName("VnetName");
            this.Property(t => t.VmSize).HasColumnName("VmSize");
            this.Property(t => t.StorageType).HasColumnName("StorageType");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastUpdatedOn).HasColumnName("LastUpdatedOn");
            this.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
        }
    }
}
