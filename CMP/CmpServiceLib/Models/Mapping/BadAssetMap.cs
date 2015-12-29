using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class BadAssetMap : EntityTypeConfiguration<BadAsset>
    {
        public BadAssetMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AssetName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.AssetTypeCode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.WhoReported)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("BadAssets");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AssetName).HasColumnName("AssetName");
            this.Property(t => t.AssetTypeCode).HasColumnName("AssetTypeCode");
            this.Property(t => t.ProblemDescription).HasColumnName("ProblemDescription");
            this.Property(t => t.WhoReported).HasColumnName("WhoReported");
            this.Property(t => t.WhenReported).HasColumnName("WhenReported");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.Active).HasColumnName("Active");
        }
    }
}
