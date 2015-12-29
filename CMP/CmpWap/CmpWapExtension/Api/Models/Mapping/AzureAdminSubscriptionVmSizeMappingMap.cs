//*****************************************************************************
// File: VmSizeMappingMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for Azure Regions
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class AzureAdminSubscriptionVmSizeMappingMap : EntityTypeConfiguration<AzureAdminSubscriptionVmSizeMapping>
    {
        public AzureAdminSubscriptionVmSizeMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.VmSizeId)
                .IsRequired();

            this.Property(t => t.PlanId)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AzureAdminSubscriptionVmSizeMapping");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.VmSizeId).HasColumnName("VmSizeId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.PlanId).HasColumnName("PlanId");
        }
    }
}
