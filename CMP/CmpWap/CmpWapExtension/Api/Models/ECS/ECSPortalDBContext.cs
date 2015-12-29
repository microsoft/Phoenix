//*****************************************************************************
// File: ECSPortalDBContext.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: DB Context for ECS
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.ECS
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ECSPortalDBContext : DbContext
    {
        public ECSPortalDBContext()
            : base("name=ECSPortalDBContext")
        {
        }

        public virtual DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>()
                .Property(e => e.EnvironmentClass)
                .IsUnicode(false);

            modelBuilder.Entity<Subscription>()
                .Property(e => e.ServiceCategory)
                .IsUnicode(false);
        }
    }
}
