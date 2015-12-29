using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class vw_MigrationStatusByDayMap : EntityTypeConfiguration<vw_MigrationStatusByDay>
    {
        public vw_MigrationStatusByDayMap()
        {
            // Primary Key
            this.HasKey(t => t.Status);

            // Properties
            this.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(9);

            this.Property(t => t.AvgCompleteDuration)
                .HasMaxLength(8);

            this.Property(t => t.MaxCompleteDuration)
                .HasMaxLength(8);

            this.Property(t => t.MinCompleteDuration)
                .HasMaxLength(8);

            // Table & Column Mappings
            this.ToTable("vw_MigrationStatusByDay");
            this.Property(t => t.StartDatePST).HasColumnName("StartDatePST");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.NumberOfBuilds).HasColumnName("NumberOfBuilds");
            this.Property(t => t.AvgCompleteDuration).HasColumnName("AvgCompleteDuration");
            this.Property(t => t.MaxCompleteDuration).HasColumnName("MaxCompleteDuration");
            this.Property(t => t.MinCompleteDuration).HasColumnName("MinCompleteDuration");
        }
    }
}
