using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class RequestMap : EntityTypeConfiguration<Request>
    {
        public RequestMap()
        {
            // Primary Key
            this.HasKey(t => new { t.RequestID, t.Source, t.Destination, t.TransferType, t.SourceType, t.DestinationType, t.Name, t.Notes, t.Config, t.WhoRequested, t.WhenRequested });

            // Properties
            this.Property(t => t.RequestID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Source)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Destination)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.StorageAccountKey)
                .HasMaxLength(100);

            this.Property(t => t.TransferType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SourceType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DestinationType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Notes)
                .IsRequired()
                .HasMaxLength(4096);

            this.Property(t => t.Config)
                .IsRequired()
                .HasMaxLength(4096);

            this.Property(t => t.WhoRequested)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ResultStatusCode)
                .HasMaxLength(50);

            this.Property(t => t.ResultDescription)
                .HasMaxLength(4096);

            this.Property(t => t.AgentRegion)
                .HasMaxLength(100);

            this.Property(t => t.AgentName)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Request");
            this.Property(t => t.RequestID).HasColumnName("RequestID");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.Destination).HasColumnName("Destination");
            this.Property(t => t.StorageAccountKey).HasColumnName("StorageAccountKey");
            this.Property(t => t.TransferType).HasColumnName("TransferType");
            this.Property(t => t.SourceType).HasColumnName("SourceType");
            this.Property(t => t.DestinationType).HasColumnName("DestinationType");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.WhoRequested).HasColumnName("WhoRequested");
            this.Property(t => t.WhenRequested).HasColumnName("WhenRequested");
            this.Property(t => t.TransferStartTime).HasColumnName("TransferStartTime");
            this.Property(t => t.ElapsedTimeMinutes).HasColumnName("ElapsedTimeMinutes");
            this.Property(t => t.MBytesTransferred).HasColumnName("MBytesTransferred");
            this.Property(t => t.ResultStatusCode).HasColumnName("ResultStatusCode");
            this.Property(t => t.ResultDescription).HasColumnName("ResultDescription");
            this.Property(t => t.ResultTime).HasColumnName("ResultTime");
            this.Property(t => t.WillTryAgain).HasColumnName("WillTryAgain");
            this.Property(t => t.SourceMBytes).HasColumnName("SourceMBytes");
            this.Property(t => t.RateBytesSec).HasColumnName("RateBytesSec");
            this.Property(t => t.TagID).HasColumnName("TagID");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.AgentRegion).HasColumnName("AgentRegion");
            this.Property(t => t.AgentName).HasColumnName("AgentName");
            this.Property(t => t.DeleteSourceAfterTransfer).HasColumnName("DeleteSourceAfterTransfer");
            this.Property(t => t.OverwriteDestinationBlob).HasColumnName("OverwriteDestinationBlob");
            this.Property(t => t.Active).HasColumnName("Active");
        }
    }
}
