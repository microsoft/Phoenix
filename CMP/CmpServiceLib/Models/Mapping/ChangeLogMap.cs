using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class ChangeLogMap : EntityTypeConfiguration<ChangeLog>
    {
        public ChangeLogMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.StatusCode)
                .HasMaxLength(50);

            this.Property(t => t.Who)
                .HasMaxLength(1024);

            // Table & Column Mappings
            this.ToTable("ChangeLog");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RequestID).HasColumnName("RequestID");
            this.Property(t => t.When).HasColumnName("When");
            this.Property(t => t.StatusCode).HasColumnName("StatusCode");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.ConfigFrom).HasColumnName("ConfigFrom");
            this.Property(t => t.ConfigTo).HasColumnName("ConfigTo");
            this.Property(t => t.Who).HasColumnName("Who");
        }
    }
}
