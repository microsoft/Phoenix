//*****************************************************************************
// File: WapSubscriptionMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for WapSubscriptions
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.MicrosoftMgmtSvcStore;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping.MicrosoftMgmtSvcStore
{
    public class WapSubscriptionMap : EntityTypeConfiguration<WapSubscription>
    {
        public WapSubscriptionMap()
        {
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.SubscriptionId)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.SubscriptionName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.PlanId)
                .IsRequired();

            this.Property(t => t.UserId)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("mp.Subscriptions");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SubscriptionId).HasColumnName("SubscriptionId");
            this.Property(t => t.SubscriptionName).HasColumnName("SubscriptionName");
            this.Property(t => t.PlanId).HasColumnName("PlanId");
            this.Property(t => t.UserId).HasColumnName("UserId");
        }
    }
}
