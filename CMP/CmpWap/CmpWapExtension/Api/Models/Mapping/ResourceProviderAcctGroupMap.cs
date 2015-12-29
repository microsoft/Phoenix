//*****************************************************************************
// File: ResourceProviderAcctGroupMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for ResourceProviderAcct Groups
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class ResourceProviderAcctGroupMap : EntityTypeConfiguration<ResourceProviderAcctGroup>
    {
        public ResourceProviderAcctGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.ResourceProviderAcctGroupId);

            // Properties
            this.Property(t => t.ResourceProviderAcctGroupId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ResourceProviderAcctGroup");
            this.Property(t => t.ResourceProviderAcctGroupId).HasColumnName("ResourceProviderAcctGroupId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.DomainId).HasColumnName("DomainId");
            this.Property(t => t.NetworkNICId).HasColumnName("NetworkNICId");
            this.Property(t => t.EnvironmentTypeId).HasColumnName("EnvironmentTypeId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastUpdatedOn).HasColumnName("LastUpdatedOn");
            this.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");

            // Relationships
            this.HasRequired(t => t.AdDomainMap)
                .WithMany(t => t.ResourceProviderAcctGroups)
                .HasForeignKey(d => d.DomainId);
            //this.HasRequired(t => t.EnvironmentType)
            //    .WithMany(t => t.ResourceProviderAcctGroups)
            //    .HasForeignKey(d => d.EnvironmentTypeId);
            this.HasRequired(t => t.NetworkNIC)
                .WithMany(t => t.ResourceProviderAcctGroups)
                .HasForeignKey(d => d.NetworkNICId);

        }
    }
}
