//*****************************************************************************
// File: CmpRequestMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for CmpRequests
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class CmpRequestMap : EntityTypeConfiguration<CmpRequest>
    {
        public CmpRequestMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.WapSubscriptionID)
                .HasMaxLength(100);

            this.Property(t => t.ParentAppName)
                .HasMaxLength(100);

            this.Property(t => t.TargetVmName)
                .HasMaxLength(256);

            this.Property(t => t.Domain)
                .HasMaxLength(100);

            this.Property(t => t.VmSize)
                .HasMaxLength(50);

            this.Property(t => t.TargetLocation)
                .HasMaxLength(50);

            this.Property(t => t.StatusCode)
                .HasMaxLength(50);

            this.Property(t => t.SourceImageName)
                .HasMaxLength(256);

            this.Property(t => t.SourceServerName)
                .HasMaxLength(256);

            this.Property(t => t.UserSpec)
                .HasMaxLength(1024);

            this.Property(t => t.StorageSpec)
                .HasMaxLength(1024);

            this.Property(t => t.FeatureSpec)
                .HasMaxLength(1024);

            this.Property(t => t.RequestType)
                .HasMaxLength(50);

            this.Property(t => t.WhoRequested)
                .HasMaxLength(50);

            this.Property(t => t.StatusMessage)
                .HasMaxLength(4096);

            this.Property(t => t.AddressFromVm)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CmpRequests");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.WapSubscriptionID).HasColumnName("WapSubscriptionID");
            this.Property(t => t.CmpRequestID).HasColumnName("CmpRequestID");
            this.Property(t => t.ParentAppName).HasColumnName("ParentAppName");
            this.Property(t => t.TargetVmName).HasColumnName("TargetVmName");
            this.Property(t => t.Domain).HasColumnName("Domain");
            this.Property(t => t.VmSize).HasColumnName("VmSize");
            this.Property(t => t.TargetLocation).HasColumnName("TargetLocation");
            this.Property(t => t.StatusCode).HasColumnName("StatusCode");
            this.Property(t => t.SourceImageName).HasColumnName("SourceImageName");
            this.Property(t => t.SourceServerName).HasColumnName("SourceServerName");
            this.Property(t => t.UserSpec).HasColumnName("UserSpec");
            this.Property(t => t.StorageSpec).HasColumnName("StorageSpec");
            this.Property(t => t.FeatureSpec).HasColumnName("FeatureSpec");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.RequestType).HasColumnName("RequestType");
            this.Property(t => t.WhoRequested).HasColumnName("WhoRequested");
            this.Property(t => t.WhenRequested).HasColumnName("WhenRequested");
            this.Property(t => t.StatusMessage).HasColumnName("StatusMessage");
            this.Property(t => t.ExceptionMessage).HasColumnName("ExceptionMessage");
            this.Property(t => t.Warnings).HasColumnName("Warnings");
            this.Property(t => t.LastStatusUpdate).HasColumnName("LastStatusUpdate");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.TagID).HasColumnName("TagID");
            this.Property(t => t.AddressFromVm).HasColumnName("AddressFromVm");
            this.Property(t => t.AccessGroupId).HasColumnName("AccessGroupId");
        }
    }
}
