//*****************************************************************************
// File: PlanMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for Plans
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.MicrosoftMgmtSvcStore;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping.MicrosoftMgmtSvcStore
{
    public class PlanMap : EntityTypeConfiguration<Plan>
    {
        public PlanMap()
        {
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("mp.Plans");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
