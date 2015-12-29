using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class CmpServiceUserAccountMap : EntityTypeConfiguration<CmpServiceUserAccount>
    {
        public CmpServiceUserAccountMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DisplayName)
                .HasMaxLength(100);

            this.Property(t => t.Name)
                .HasMaxLength(50);

            this.Property(t => t.Password)
                .HasMaxLength(1024);

            this.Property(t => t.Domain)
                .HasMaxLength(50);

            this.Property(t => t.AssociatedCorpAccountName)
                .HasMaxLength(100);

            this.Property(t => t.StatusCode)
                .HasMaxLength(50);

            this.Property(t => t.AccountTypeCode)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CmpServiceUserAccounts");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.DisplayName).HasColumnName("DisplayName");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.Domain).HasColumnName("Domain");
            this.Property(t => t.AssociatedCorpAccountName).HasColumnName("AssociatedCorpAccountName");
            this.Property(t => t.StatusCode).HasColumnName("StatusCode");
            this.Property(t => t.AccountTypeCode).HasColumnName("AccountTypeCode");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
