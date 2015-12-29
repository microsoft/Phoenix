using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class vw_CMPDailyMailCompletedBuildsMap : EntityTypeConfiguration<vw_CMPDailyMailCompletedBuilds>
    {
        public vw_CMPDailyMailCompletedBuildsMap()
        {
            // Primary Key
            this.HasKey(t => t.RowOrder);

            // Properties
            this.Property(t => t.RowOrder)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AvgCompleteDuration)
                .HasMaxLength(8);

            this.Property(t => t.MaxCompleteDuration)
                .HasMaxLength(8);

            this.Property(t => t.MinCompleteDuration)
                .HasMaxLength(8);

            // Table & Column Mappings
            this.ToTable("vw_CMPDailyMailCompletedBuilds");
            this.Property(t => t.RowOrder).HasColumnName("RowOrder");
            this.Property(t => t.StartDatePST).HasColumnName("StartDatePST");
            this.Property(t => t.RoleName).HasColumnName("RoleName");
            this.Property(t => t.CompletedBuilds).HasColumnName("CompletedBuilds");
            this.Property(t => t.AvgCompleteDuration).HasColumnName("AvgCompleteDuration");
            this.Property(t => t.MaxCompleteDuration).HasColumnName("MaxCompleteDuration");
            this.Property(t => t.MinCompleteDuration).HasColumnName("MinCompleteDuration");
        }
    }
}
