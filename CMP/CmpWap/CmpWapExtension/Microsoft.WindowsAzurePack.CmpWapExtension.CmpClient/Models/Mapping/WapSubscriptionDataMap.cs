using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    /// <summary>
    /// Entity framework class to map a WapSubscriptionData object to the appropriate table
    /// </summary>
    public class WapSubscriptionDataMap : EntityTypeConfiguration<WapSubscriptionData>
    {
        public WapSubscriptionDataMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.WapSubscriptionID)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("WapSubscriptionData");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.WapSubscriptionID).HasColumnName("WapSubscriptionID");
            this.Property(t => t.DefaultObjectCreationGroupID).HasColumnName("DefaultObjectCreationGroupID");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.TagId).HasColumnName("TagId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
