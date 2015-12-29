//*****************************************************************************
// File: AzureRegionMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for Azure Regions
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class AzureRegionMap : EntityTypeConfiguration<AzureRegion>
    {
        public AzureRegionMap()
        {
            // Primary Key
            this.HasKey(t => t.AzureRegionId);

            // Properties
            this.Property(t => t.AzureRegionId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AzureRegion");
            this.Property(t => t.AzureRegionId).HasColumnName("AzureRegionId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.OsImageContainer).HasColumnName("OsImageContainer");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastUpdatedOn).HasColumnName("LastUpdatedOn");
            this.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
        }
    }
}
