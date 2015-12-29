//*****************************************************************************
// File: AzureAdminSubscriptionRegionMappingMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for CmpRequests
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class AzureAdminSubscriptionRegionMappingMap : EntityTypeConfiguration<AzureAdminSubscriptionRegionMapping>
    {
        public AzureAdminSubscriptionRegionMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.PlanId)
                .HasMaxLength(50);

            this.Property(t => t.AzureRegionId);

            // Table & Column Mappings
            this.ToTable("AzureAdminSubscriptionRegionMapping");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.PlanId).HasColumnName("PlanId");
            this.Property(t => t.AzureRegionId).HasColumnName("AzureRegionId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
