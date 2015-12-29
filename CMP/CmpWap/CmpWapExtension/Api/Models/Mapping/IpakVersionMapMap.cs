using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class IpakVersionMapMap : EntityTypeConfiguration<IpakVersionMap>
    {
        public IpakVersionMapMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.VersionCode)
                .HasMaxLength(50);

            this.Property(t => t.VersionName)
                .HasMaxLength(200);

            this.Property(t => t.AzureRegion)
                .HasMaxLength(50);

            this.Property(t => t.AdDomain)
                .HasMaxLength(200);

            this.Property(t => t.IpakDirLocation)
                .HasMaxLength(250);

            this.Property(t => t.IpakFullFileLocation)
                .HasMaxLength(250);

            this.Property(t => t.AdminName)
                .HasMaxLength(100);

            this.Property(t => t.QfeVersion)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("IpakVersionMap");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.VersionCode).HasColumnName("VersionCode");
            this.Property(t => t.VersionName).HasColumnName("VersionName");
            this.Property(t => t.AzureRegion).HasColumnName("AzureRegion");
            this.Property(t => t.AdDomain).HasColumnName("AdDomain");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.IpakDirLocation).HasColumnName("IpakDirLocation");
            this.Property(t => t.IpakFullFileLocation).HasColumnName("IpakFullFileLocation");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.AdminName).HasColumnName("AdminName");
            this.Property(t => t.QfeVersion).HasColumnName("QfeVersion");
        }
    }
}
