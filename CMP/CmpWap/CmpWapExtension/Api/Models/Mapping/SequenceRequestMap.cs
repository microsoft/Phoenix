//*****************************************************************************
// File: SequenceRequestMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for Sequence Requests 
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class SequenceRequestMap : EntityTypeConfiguration<SequenceRequest>
    {
        public SequenceRequestMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.WapSubscriptionID)
                .HasMaxLength(100);

            this.Property(t => t.ServiceProviderName)
                .HasMaxLength(256);

            this.Property(t => t.ServiceProviderTypeCode)
                .HasMaxLength(100);

            this.Property(t => t.ServiceProviderJobId)
                .HasMaxLength(100);

            this.Property(t => t.TargetName)
                .HasMaxLength(256);

            this.Property(t => t.TargetTypeCode)
                .HasMaxLength(100);

            this.Property(t => t.StatusCode)
                .HasMaxLength(50);

            this.Property(t => t.WhoRequested)
                .HasMaxLength(50);

            this.Property(t => t.StatusMessage)
                .HasMaxLength(4096);

            // Table & Column Mappings
            this.ToTable("SequenceRequests");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.WapSubscriptionID).HasColumnName("WapSubscriptionID");
            this.Property(t => t.CmpRequestID).HasColumnName("CmpRequestID");
            this.Property(t => t.ServiceProviderName).HasColumnName("ServiceProviderName");
            this.Property(t => t.ServiceProviderTypeCode).HasColumnName("ServiceProviderTypeCode");
            this.Property(t => t.ServiceProviderJobId).HasColumnName("ServiceProviderJobId");
            this.Property(t => t.TargetName).HasColumnName("TargetName");
            this.Property(t => t.TargetTypeCode).HasColumnName("TargetTypeCode");
            this.Property(t => t.StatusCode).HasColumnName("StatusCode");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.WhoRequested).HasColumnName("WhoRequested");
            this.Property(t => t.WhenRequested).HasColumnName("WhenRequested");
            this.Property(t => t.StatusMessage).HasColumnName("StatusMessage");
            this.Property(t => t.ExceptionMessage).HasColumnName("ExceptionMessage");
            this.Property(t => t.Warnings).HasColumnName("Warnings");
            this.Property(t => t.LastStatusUpdate).HasColumnName("LastStatusUpdate");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.TagOpName).HasColumnName("TagOpName");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.TagID).HasColumnName("TagID");
        }
    }
}
