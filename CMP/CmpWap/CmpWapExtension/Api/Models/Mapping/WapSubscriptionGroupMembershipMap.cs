//*****************************************************************************
// File: WapSubscriptionGroupMembershipMap.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model Mapping for Wap Subscription Group Membership
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping
{
    public class WapSubscriptionGroupMembershipMap : EntityTypeConfiguration<WapSubscriptionGroupMembership>
    {
        public WapSubscriptionGroupMembershipMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.WapSubscriptionID)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.GroupName)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("WapSubscriptionGroupMembership");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.WapSubscriptionID).HasColumnName("WapSubscriptionID");
            this.Property(t => t.GroupID).HasColumnName("GroupID");
            this.Property(t => t.GroupName).HasColumnName("GroupName");
            this.Property(t => t.Config).HasColumnName("Config");
            this.Property(t => t.TagData).HasColumnName("TagData");
            this.Property(t => t.TagId).HasColumnName("TagId");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
