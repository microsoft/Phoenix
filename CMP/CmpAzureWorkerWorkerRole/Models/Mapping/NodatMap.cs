using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpAzureWorkerWorkerRole.Models.Mapping
{
    public class NodatMap : EntityTypeConfiguration<Nodat>
    {
        public NodatMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("Nodat");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TagData).HasColumnName("TagData");
        }
    }
}
