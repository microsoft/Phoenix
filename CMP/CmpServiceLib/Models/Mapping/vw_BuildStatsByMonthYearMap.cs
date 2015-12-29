using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class vw_BuildStatsByMonthYearMap : EntityTypeConfiguration<vw_BuildStatsByMonthYear>
    {
        public vw_BuildStatsByMonthYearMap()
        {
            // Primary Key
            this.HasKey(t => t.ExecutionStatus);

            // Properties
            this.Property(t => t.ExecutionStatus)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.MonthYear)
                .HasMaxLength(61);

            // Table & Column Mappings
            this.ToTable("vw_BuildStatsByMonthYear");
            this.Property(t => t.ExecutionStatus).HasColumnName("ExecutionStatus");
            this.Property(t => t.MonthYear).HasColumnName("MonthYear");
            this.Property(t => t.CountOfSvrs).HasColumnName("CountOfSvrs");
        }
    }
}
