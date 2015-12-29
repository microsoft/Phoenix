using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class AzureRegionMapMap : EntityTypeConfiguration<AzureRegionMap>
    {
        public AzureRegionMapMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("AzureRegionMap");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AzureReqionName).HasColumnName("AzureReqionName");
            this.Property(t => t.WapRegionName).HasColumnName("WapRegionName");
            this.Property(t => t.OsImageContainer).HasColumnName("OsImageContainer");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
