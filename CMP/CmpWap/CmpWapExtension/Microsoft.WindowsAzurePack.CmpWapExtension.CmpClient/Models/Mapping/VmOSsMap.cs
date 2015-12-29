using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class VmOSsMap : EntityTypeConfiguration<VmOSs>
    {
        public VmOSsMap()
        {
            // Primary Key
            this.HasKey(t => t.VMOsId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.OsFamily)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.AzureImageName)
                .IsRequired();

            this.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.AzureImagePublisher)
                .HasMaxLength(100);

            this.Property(t => t.AzureImageOffer)
                .HasMaxLength(100);

            this.Property(t => t.AzureWindowsOSVersion)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("VmOSs");
            this.Property(t => t.VMOsId).HasColumnName("VMOsId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.OsFamily).HasColumnName("OsFamily");
            this.Property(t => t.AzureImageName).HasColumnName("AzureImageName");
            this.Property(t => t.IsCustomImage).HasColumnName("IsCustomImage");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastUpdatedOn).HasColumnName("LastUpdatedOn");
            this.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
            this.Property(t => t.AzureImagePublisher).HasColumnName("AzureImagePublisher");
            this.Property(t => t.AzureImageOffer).HasColumnName("AzureImageOffer");
            this.Property(t => t.AzureWindowsOSVersion).HasColumnName("AzureWindowsOSVersion");
        }
    }
}
