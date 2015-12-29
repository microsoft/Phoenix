using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CmpServiceLib.Models.Mapping
{
    public class Table1Map : EntityTypeConfiguration<Table1>
    {
        public Table1Map()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.id)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.C__version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Table1", "mobileservicetest2");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.C__createdAt).HasColumnName("__createdAt");
            this.Property(t => t.C__updatedAt).HasColumnName("__updatedAt");
            this.Property(t => t.C__version).HasColumnName("__version");
            this.Property(t => t.thename).HasColumnName("thename");
        }
    }
}
