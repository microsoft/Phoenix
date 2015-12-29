using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class vw_DeploymentRequestExecutionTimesMap : EntityTypeConfiguration<vw_DeploymentRequestExecutionTimes>
    {
        public vw_DeploymentRequestExecutionTimesMap()
        {
            // Primary Key
            this.HasKey(t => t.ExecutionStatus);

            // Properties
            this.Property(t => t.RequestName)
                .HasMaxLength(50);

            this.Property(t => t.ExecutionDuration)
                .HasMaxLength(8);

            this.Property(t => t.ExecutionStatus)
                .IsRequired()
                .HasMaxLength(9);

            this.Property(t => t.LastExecutionStep)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("vw_DeploymentRequestExecutionTimes");
            this.Property(t => t.RequestID).HasColumnName("RequestID");
            this.Property(t => t.RequestName).HasColumnName("RequestName");
            this.Property(t => t.RoleName).HasColumnName("RoleName");
            this.Property(t => t.ExecutionDuration).HasColumnName("ExecutionDuration");
            this.Property(t => t.StartTimePST).HasColumnName("StartTimePST");
            this.Property(t => t.EndTimePST).HasColumnName("EndTimePST");
            this.Property(t => t.ExecutionStatus).HasColumnName("ExecutionStatus");
            this.Property(t => t.LastExecutionStep).HasColumnName("LastExecutionStep");
            this.Property(t => t.Message).HasColumnName("Message");
        }
    }
}
