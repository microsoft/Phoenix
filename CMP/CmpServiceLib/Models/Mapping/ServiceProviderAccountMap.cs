using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
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

            this.Property(t => t.AzRegion)
                .HasMaxLength(50);

            this.Property(t => t.AzAffinityGroup)
                .HasMaxLength(50);

            this.Property(t => t.AzVNet)
                .HasMaxLength(50);

            this.Property(t => t.AzSubnet)
                .HasMaxLength(50);

            this.Property(t => t.AzStorageContainerUrl)
                .HasMaxLength(100);

            this.Property(t => t.ResourceGroup)
                .HasMaxLength(50);

            this.Property(t => t.AzureADTenantId)
                .HasMaxLength(50);

            this.Property(t => t.AzureADClientId)
                .HasMaxLength(50);

            this.Property(t => t.AzureADClientKey)
                .HasMaxLength(500);

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
            this.Property(t => t.AzRegion).HasColumnName("AzRegion");
            this.Property(t => t.AzAffinityGroup).HasColumnName("AzAffinityGroup");
            this.Property(t => t.AzVNet).HasColumnName("AzVNet");
            this.Property(t => t.AzSubnet).HasColumnName("AzSubnet");
            this.Property(t => t.AzStorageContainerUrl).HasColumnName("AzStorageContainerUrl");
            this.Property(t => t.ResourceGroup).HasColumnName("ResourceGroup");
            this.Property(t => t.CoreCountMax).HasColumnName("CoreCountMax");
            this.Property(t => t.CoreCountCurrent).HasColumnName("CoreCountCurrent");
            this.Property(t => t.VnetCountMax).HasColumnName("VnetCountMax");
            this.Property(t => t.VnetCountCurrent).HasColumnName("VnetCountCurrent");
            this.Property(t => t.StorageAccountCountMax).HasColumnName("StorageAccountCountMax");
            this.Property(t => t.StorageAccountCountCurrent).HasColumnName("StorageAccountCountCurrent");
            this.Property(t => t.VmsPerVnetCountMax).HasColumnName("VmsPerVnetCountMax");
            this.Property(t => t.VmsPerServiceCountMax).HasColumnName("VmsPerServiceCountMax");
            this.Property(t => t.AzureADTenantId).HasColumnName("AzureADTenantId");
            this.Property(t => t.AzureADClientId).HasColumnName("AzureADClientId");
            this.Property(t => t.AzureADClientKey).HasColumnName("AzureADClientKey");
        }
    }
}
