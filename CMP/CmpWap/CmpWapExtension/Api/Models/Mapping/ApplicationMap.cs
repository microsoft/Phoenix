//*****************************************************************************
// File: ApplicationMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for Applications
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class ApplicationMap : EntityTypeConfiguration<Application>
    {
        public ApplicationMap()
        {
            // Primary Key
            //this.HasKey(t => new { t.ApplicationId, t.Name, t.Code, t.HasService, t.IsActive, t.SubcriptionID, t.CreatedOn, t.CreatedBy, t.LastUpdatedOn, t.LastUpdatedBy });
            this.HasKey(t => t.ApplicationId);

            // Properties
            this.Property(t => t.ApplicationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.SubscriptionId)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.CIOwner)
                .HasMaxLength(150);

            this.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.LastUpdatedBy)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Application");
            this.Property(t => t.ApplicationId).HasColumnName("ApplicationId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.HasService).HasColumnName("HasService");
            this.Property(t => t.SubscriptionId).HasColumnName("SubscriptionId");
            this.Property(t => t.CIOwner).HasColumnName("CIOwner");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastUpdatedOn).HasColumnName("LastUpdatedOn");
            this.Property(t => t.LastUpdatedBy).HasColumnName("LastUpdatedBy");
        }
    }
}
