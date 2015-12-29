using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpAzureServiceWebRole.Models.Mapping
{
    public class ServiceProviderSlotMap : EntityTypeConfiguration<ServiceProviderSlot>
    {
        public ServiceProviderSlotMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TypeCode)
                .HasMaxLength(50);

            this.Property(t => t.ServiceProviderSlotName)
                .HasMaxLength(250);

            this.Property(t => t.Description)
                .HasMaxLength(1024);

            // Table & Column Mappings
            this.ToTable("ServiceProviderSlots");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ServiceProviderAccountId).HasColumnName("ServiceProviderAccountId");
            this.Property(t => t.TypeCode).HasColumnName("TypeCode");
            this.Property(t => t.ServiceProviderSlotName).HasColumnName("ServiceProviderSlotName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.TagInt).HasColumnName("TagInt");
            this.Property(t => t.Active).HasColumnName("Active");
        }
    }
}
