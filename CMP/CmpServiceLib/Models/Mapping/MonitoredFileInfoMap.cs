using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class MonitoredFileInfoMap : EntityTypeConfiguration<MonitoredFileInfo>
    {
        public MonitoredFileInfoMap()
        {
            // Primary Key
            this.HasKey(t => new { t.FileInfoID, t.Source, t.Destination, t.LastWriteTime, t.Size });

            // Properties
            this.Property(t => t.FileInfoID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Source)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Destination)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Size)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("MonitoredFileInfo");
            this.Property(t => t.FileInfoID).HasColumnName("FileInfoID");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.Destination).HasColumnName("Destination");
            this.Property(t => t.LastWriteTime).HasColumnName("LastWriteTime");
            this.Property(t => t.Size).HasColumnName("Size");
        }
    }
}
