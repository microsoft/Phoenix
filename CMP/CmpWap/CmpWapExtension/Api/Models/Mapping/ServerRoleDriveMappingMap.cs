//*****************************************************************************
// File: ServerRoleDriveMappingMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for ServerRoleDriveMapping 
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class ServerRoleDriveMappingMap : EntityTypeConfiguration<ServerRoleDriveMapping>
    {
        public ServerRoleDriveMappingMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id, t.ServerRoleId, t.Drive, t.MemoryInGB, t.CreatedOn, t.CreatedBy, t.LastUpdatedOn, t.LastUpdatedBy });

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ServerRoleId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Drive)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.MemoryInGB)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

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
        }
    }
}
