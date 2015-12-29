using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class vw_MigrationFailedJobStatusDetailsMap : EntityTypeConfiguration<vw_MigrationFailedJobStatusDetails>
    {
        public vw_MigrationFailedJobStatusDetailsMap()
        {
            // Primary Key
            this.HasKey(t => new { t.CMPRequestID, t.ValidationResult, t.Warnings });

            // Properties
            this.Property(t => t.CMPRequestID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.VMName)
                .HasMaxLength(256);

            this.Property(t => t.StatusCode)
                .HasMaxLength(50);

            this.Property(t => t.VMSourceName)
                .HasMaxLength(256);

            this.Property(t => t.Status)
                .HasMaxLength(50);

            this.Property(t => t.Application_Name)
                .HasMaxLength(100);

            this.Property(t => t.Application_ID)
                .HasMaxLength(50);

            this.Property(t => t.Requestor)
                .HasMaxLength(100);

            this.Property(t => t.Status_Message)
                .HasMaxLength(8000);

            this.Property(t => t.ValidationResult)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Warnings)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("vw_MigrationFailedJobStatusDetails");
            this.Property(t => t.CMPRequestID).HasColumnName("CMPRequestID");
            this.Property(t => t.VMName).HasColumnName("VMName");
            this.Property(t => t.StatusCode).HasColumnName("StatusCode");
            this.Property(t => t.VMSourceName).HasColumnName("VMSourceName");
            this.Property(t => t.AD_Domain).HasColumnName("AD Domain");
            this.Property(t => t.Org_Division).HasColumnName("Org Division");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Application_Name).HasColumnName("Application Name");
            this.Property(t => t.Application_ID).HasColumnName("Application ID");
            this.Property(t => t.Requestor).HasColumnName("Requestor");
            this.Property(t => t.Status_Message).HasColumnName("Status Message");
            this.Property(t => t.ValidationResult).HasColumnName("ValidationResult");
            this.Property(t => t.Azure_IP_Address).HasColumnName("Azure IP Address");
            this.Property(t => t.StartTimePST).HasColumnName("StartTimePST");
            this.Property(t => t.Submitted_Elapsed_Time).HasColumnName("Submitted Elapsed Time");
            this.Property(t => t.Last_Update).HasColumnName("Last Update");
            this.Property(t => t.Last_Update_Elapsed_Time).HasColumnName("Last Update Elapsed Time");
            this.Property(t => t.Exception).HasColumnName("Exception");
            this.Property(t => t.Warnings).HasColumnName("Warnings");
        }
    }
}
