//*****************************************************************************
// File: AzureRegionVmSizeMappingMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for Azure Regions
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class AzureRegionVmSizeMappingMap : EntityTypeConfiguration<AzureRegionVmSizeMapping>
    {
        public AzureRegionVmSizeMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.VmSizeId)
                .IsRequired();

            this.Property(t => t.AzureRegionId)
                .IsRequired();

            this.Property(t => t.AzureSubscriptionId)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("AzureRegionVmSizeMapping");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.VmSizeId).HasColumnName("VmSizeId");
            this.Property(t => t.AzureRegionId).HasColumnName("AzureRegionId");
            this.Property(t => t.AzureSubscriptionId).HasColumnName("AzureSubscriptionId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
