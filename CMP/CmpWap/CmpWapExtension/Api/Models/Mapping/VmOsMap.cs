//*****************************************************************************
// File: VMOMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for VMO
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class VmOsMap : EntityTypeConfiguration<VmOs>
    {
        public VmOsMap()
        {
            // Primary Key
            this.HasKey(t => t.VmOsId);

            // Properties
            this.Property(t => t.VmOsId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            this.Property(t => t.OsFamily)
                .HasMaxLength(100);

            this.Property(t => t.AzureImageName);

            this.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.AzureImagePublisher)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.AzureImageOffer)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.AzureWindowsOSVersion)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("VmOs");
            this.Property(t => t.VmOsId).HasColumnName("VmOsId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.OsFamily).HasColumnName("OsFamily");
            this.Property(t => t.AzureImageName).HasColumnName("AzureImageName");
            this.Property(t => t.AzureImageName).HasColumnName("AzureImageName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsCustomImage).HasColumnName("IsCustomImage");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastUpdatedOn).HasColumnName("LastUpdatedOn");
            this.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
            this.Property(t => t.AzureImagePublisher).HasColumnName("AzureImagePublisher");
            this.Property(t => t.AzureImageOffer).HasColumnName("AzureImageOffer");
            this.Property(t => t.AzureWindowsOSVersion).HasColumnName("AzureWindowsOSVersion");
        }
    }
}
