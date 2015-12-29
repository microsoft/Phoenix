using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class OpRequestMap : EntityTypeConfiguration<OpRequest>
    {
        public OpRequestMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RequestName)
                .HasMaxLength(512);

            this.Property(t => t.RequestDescription)
                .HasMaxLength(1024);

            this.Property(t => t.RequestType)
                .HasMaxLength(50);

            this.Property(t => t.TargetTypeCode)
                .HasMaxLength(50);

            this.Property(t => t.TargetName)
                .HasMaxLength(256);

            this.Property(t => t.WhoRequested)
                .HasMaxLength(100);

            this.Property(t => t.StatusCode)
                .HasMaxLength(50);

            this.Property(t => t.StatusMessage)
                .HasMaxLength(4096);

            // Table & Column Mappings
            this.ToTable("OpRequests");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RequestName).HasColumnName("RequestName");
            this.Property(t => t.RequestDescription).HasColumnName("RequestDescription");
            this.Property(t => t.RequestType).HasColumnName("RequestType");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.TargetTypeCode).HasColumnName("TargetTypeCode");
            this.Property(t => t.TargetName).HasColumnName("TargetName");
            this.Property(t => t.WhoRequested).HasColumnName("WhoRequested");
            this.Property(t => t.WhenRequested).HasColumnName("WhenRequested");
            this.Property(t => t.ExceptionMessage).HasColumnName("ExceptionMessage");
            this.Property(t => t.LastStatusUpdate).HasColumnName("LastStatusUpdate");
            this.Property(t => t.StatusCode).HasColumnName("StatusCode");
            this.Property(t => t.StatusMessage).HasColumnName("StatusMessage");
            this.Property(t => t.Warnings).HasColumnName("Warnings");
            this.Property(t => t.ServiceProviderStatusCheckTag).HasColumnName("ServiceProviderStatusCheckTag");
            this.Property(t => t.CurrentStateStartTime).HasColumnName("CurrentStateStartTime");
            this.Property(t => t.CurrentStateTryCount).HasColumnName("CurrentStateTryCount");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.TagID).HasColumnName("TagID");
            this.Property(t => t.Active).HasColumnName("Active");
        }
    }
}
