using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class SequenceMap : EntityTypeConfiguration<Sequence>
    {
        public SequenceMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SequenceName)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Sequences");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SequenceName).HasColumnName("SequenceName");
            this.Property(t => t.SequenceOrder).HasColumnName("SequenceOrder");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.TagData).HasColumnName("TagData");
        }
    }
}
