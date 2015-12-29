using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpAzureServiceWebRole.Models.Mapping
{
    public class ServiceProviderAccountMap : EntityTypeConfiguration<ServiceProviderAccount>
    {
        public ServiceProviderAccountMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .HasMaxLength(100);

            this.Property(t => t.OwnerNamesCSV)
                .HasMaxLength(1024);

            this.Property(t => t.AccountType)
                .HasMaxLength(50);

            this.Property(t => t.CertificateThumbprint)
                .HasMaxLength(100);

            this.Property(t => t.AccountID)
                .HasMaxLength(100);

            this.Property(t => t.AccountPassword)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("ServiceProviderAccounts");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.OwnerNamesCSV).HasColumnName("OwnerNamesCSV");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.TagID).HasColumnName("TagID");
            this.Property(t => t.AccountType).HasColumnName("AccountType");
            this.Property(t => t.ExpirationDate).HasColumnName("ExpirationDate");
            this.Property(t => t.CertificateBlob).HasColumnName("CertificateBlob");
            this.Property(t => t.CertificateThumbprint).HasColumnName("CertificateThumbprint");
            this.Property(t => t.AccountID).HasColumnName("AccountID");
            this.Property(t => t.AccountPassword).HasColumnName("AccountPassword");
            this.Property(t => t.Active).HasColumnName("Active");
        }
    }
}
