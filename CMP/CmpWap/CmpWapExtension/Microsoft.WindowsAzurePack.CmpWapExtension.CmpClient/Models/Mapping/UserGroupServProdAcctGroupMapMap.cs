using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class UserGroupServProdAcctGroupMapMap : EntityTypeConfiguration<UserGroupServProdAcctGroupMap>
    {
        public UserGroupServProdAcctGroupMapMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UserGroupName)
                .HasMaxLength(50);

            this.Property(t => t.ServProvAcctGroupName)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("UserGroupServProdAcctGroupMap");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserGroupName).HasColumnName("UserGroupName");
            this.Property(t => t.ServProvAcctGroupName).HasColumnName("ServProvAcctGroupName");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.IsEnabled).HasColumnName("IsEnabled");
        }
    }
}
