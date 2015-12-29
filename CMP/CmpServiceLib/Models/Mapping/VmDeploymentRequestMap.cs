using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class VmDeploymentRequestMap : EntityTypeConfiguration<VmDeploymentRequest>
    {
        public VmDeploymentRequestMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RequestName)
                .HasMaxLength(50);

            this.Property(t => t.RequestDescription)
                .HasMaxLength(1024);

            this.Property(t => t.ParentAppName)
                .HasMaxLength(100);

            this.Property(t => t.ParentAppID)
                .HasMaxLength(50);

            this.Property(t => t.RequestType)
                .HasMaxLength(50);

            this.Property(t => t.TargetServiceProviderType)
                .HasMaxLength(50);

            this.Property(t => t.TargetLocation)
                .HasMaxLength(50);

            this.Property(t => t.TargetLocationType)
                .HasMaxLength(50);

            this.Property(t => t.TargetAccount)
                .HasMaxLength(100);

            this.Property(t => t.TargetAccountType)
                .HasMaxLength(50);

            this.Property(t => t.TargetAccountCreds)
                .HasMaxLength(4096);

            this.Property(t => t.VmSize)
                .HasMaxLength(50);

            this.Property(t => t.TargetVmName)
                .HasMaxLength(256);

            this.Property(t => t.SourceServerName)
                .HasMaxLength(256);

            this.Property(t => t.WhoRequested)
                .HasMaxLength(100);

            this.Property(t => t.StatusCode)
                .HasMaxLength(50);

            this.Property(t => t.StatusMessage)
                .HasMaxLength(4096);

            this.Property(t => t.TargetServicename)
                .HasMaxLength(100);

            this.Property(t => t.SourceServerRegion)
                .HasMaxLength(100);

            this.Property(t => t.ExceptionTypeCode)
                .HasMaxLength(50);

            this.Property(t => t.LastState)
                .HasMaxLength(50);

            this.Property(t => t.ServiceProviderResourceGroup)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("VmDeploymentRequests");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.RequestName).HasColumnName("RequestName");
            this.Property(t => t.RequestDescription).HasColumnName("RequestDescription");
            this.Property(t => t.ParentAppName).HasColumnName("ParentAppName");
            this.Property(t => t.ParentAppID).HasColumnName("ParentAppID");
            this.Property(t => t.RequestType).HasColumnName("RequestType");
            this.Property(t => t.TargetServiceProviderType).HasColumnName("TargetServiceProviderType");
            this.Property(t => t.TargetLocation).HasColumnName("TargetLocation");
            this.Property(t => t.TargetLocationType).HasColumnName("TargetLocationType");
            this.Property(t => t.TargetAccount).HasColumnName("TargetAccount");
            this.Property(t => t.TargetAccountType).HasColumnName("TargetAccountType");
            this.Property(t => t.TargetAccountCreds).HasColumnName("TargetAccountCreds");
            this.Property(t => t.VmSize).HasColumnName("VmSize");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.TargetVmName).HasColumnName("TargetVmName");
            this.Property(t => t.SourceServerName).HasColumnName("SourceServerName");
            this.Property(t => t.SourceVhdFilesCSV).HasColumnName("SourceVhdFilesCSV");
            this.Property(t => t.WhoRequested).HasColumnName("WhoRequested");
            this.Property(t => t.WhenRequested).HasColumnName("WhenRequested");
            this.Property(t => t.ExceptionMessage).HasColumnName("ExceptionMessage");
            this.Property(t => t.LastStatusUpdate).HasColumnName("LastStatusUpdate");
            this.Property(t => t.StatusCode).HasColumnName("StatusCode");
            this.Property(t => t.StatusMessage).HasColumnName("StatusMessage");
            this.Property(t => t.AftsID).HasColumnName("AftsID");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.TargetServicename).HasColumnName("TargetServicename");
            this.Property(t => t.ServiceProviderStatusCheckTag).HasColumnName("ServiceProviderStatusCheckTag");
            this.Property(t => t.TagID).HasColumnName("TagID");
            this.Property(t => t.ServiceProviderAccountID).HasColumnName("ServiceProviderAccountID");
            this.Property(t => t.OverwriteExisting).HasColumnName("OverwriteExisting");
            this.Property(t => t.CurrentStateStartTime).HasColumnName("CurrentStateStartTime");
            this.Property(t => t.CurrentStateTryCount).HasColumnName("CurrentStateTryCount");
            this.Property(t => t.Warnings).HasColumnName("Warnings");
            this.Property(t => t.ValidationResults).HasColumnName("ValidationResults");
            this.Property(t => t.SourceServerRegion).HasColumnName("SourceServerRegion");
            this.Property(t => t.ExceptionTypeCode).HasColumnName("ExceptionTypeCode");
            this.Property(t => t.LastState).HasColumnName("LastState");
            this.Property(t => t.ServiceProviderResourceGroup).HasColumnName("ServiceProviderResourceGroup");
            this.Property(t => t.ConfigOriginal).HasColumnName("ConfigOriginal");
        }
    }
}
