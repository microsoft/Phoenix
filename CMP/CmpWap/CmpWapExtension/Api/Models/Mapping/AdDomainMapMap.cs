//*****************************************************************************
// File: AdDomainMapMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for AdDomain
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class AdDomainMapMap : EntityTypeConfiguration<AdDomainMap>
    {
        public AdDomainMapMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ServerOU)
                .HasMaxLength(200);

            this.Property(t => t.WorkstationOU)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("AdDomainMap");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.DomainShortName).HasColumnName("DomainShortName");
            this.Property(t => t.DomainFullName).HasColumnName("DomainFullName");
            this.Property(t => t.JoinCredsUserName).HasColumnName("JoinCredsUserName");
            this.Property(t => t.JoinCredsPasword).HasColumnName("JoinCredsPasword");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.ServerOU).HasColumnName("ServerOU");
            this.Property(t => t.WorkstationOU).HasColumnName("WorkstationOU");
            this.Property(t => t.DefaultVmAdminMember).HasColumnName("DefaultVmAdminMember");
        }
    }
}
