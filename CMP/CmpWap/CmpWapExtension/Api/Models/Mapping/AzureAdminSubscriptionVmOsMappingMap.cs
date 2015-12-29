//*****************************************************************************
// File: OsMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for Azure Regions
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class AzureAdminSubscriptionVmOsMappingMap : EntityTypeConfiguration<AzureAdminSubscriptionVmOsMapping>
    {
        public AzureAdminSubscriptionVmOsMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.PlanId)
                .HasMaxLength(50)
                .IsRequired();

            this.Property(t => t.VmOsId)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("AzureAdminSubscriptionVmOsMapping");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.PlanId).HasColumnName("PlanId");
            this.Property(t => t.VmOsId).HasColumnName("VmOsId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
