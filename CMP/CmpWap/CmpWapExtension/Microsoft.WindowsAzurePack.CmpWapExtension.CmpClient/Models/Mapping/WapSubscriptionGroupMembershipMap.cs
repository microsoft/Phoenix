using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    /// <summary>
    /// Entity framework class to map a WapSubscriptionGroupMembership object to the appropriate table
    /// </summary>
    public class WapSubscriptionGroupMembershipMap : EntityTypeConfiguration<WapSubscriptionGroupMembership>
    {
        public WapSubscriptionGroupMembershipMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.WapSubscriptionID)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.GroupName)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("WapSubscriptionGroupMembership");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.WapSubscriptionID).HasColumnName("WapSubscriptionID");
            this.Property(t => t.GroupID).HasColumnName("GroupID");
            this.Property(t => t.GroupName).HasColumnName("GroupName");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.TagId).HasColumnName("TagId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}

