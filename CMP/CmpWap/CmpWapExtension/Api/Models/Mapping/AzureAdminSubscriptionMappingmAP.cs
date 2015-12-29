//*****************************************************************************
// File: AzureAdminSubscriptionMappingMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for VM Size
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class AzureAdminSubscriptionMappingMap : EntityTypeConfiguration<AzureAdminSubscriptionMapping>
    {
        public AzureAdminSubscriptionMappingMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id });

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(t => t.SubId)
                .IsRequired();

            this.Property(t => t.PlanId)
                .HasMaxLength(50);

            this.Property(t => t.IsActive);

            // Table & Column Mappings
            this.ToTable("AzureAdminSubscriptionMapping");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SubId).HasColumnName("SubId");
            this.Property(t => t.PlanId).HasColumnName("PlanId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
