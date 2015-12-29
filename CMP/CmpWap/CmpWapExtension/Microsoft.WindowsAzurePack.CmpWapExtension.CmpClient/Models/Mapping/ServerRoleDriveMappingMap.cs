using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models.Mapping
{
    public class ServerRoleDriveMappingMap : EntityTypeConfiguration<ServerRoleDriveMapping>
    {
        public ServerRoleDriveMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Drive)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ServerRoleDriveMapping");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ServerRoleId).HasColumnName("ServerRoleId");
            this.Property(t => t.Drive).HasColumnName("Drive");
            this.Property(t => t.MemoryInGB).HasColumnName("MemoryInGB");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastUpdatedOn).HasColumnName("LastUpdatedOn");
            this.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");

            // Relationships
            this.HasRequired(t => t.ServerRole)
                .WithMany(t => t.ServerRoleDriveMappings)
                .HasForeignKey(d => d.ServerRoleId);

        }
    }
}
