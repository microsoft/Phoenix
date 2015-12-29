//*****************************************************************************
// File: MicrosoftMgmtSvcStoreContext.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for MicrosoftMgmtSvcStoreContext
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.Data.Entity;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping.MicrosoftMgmtSvcStore;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.MicrosoftMgmtSvcStore
{
    public partial class MicrosoftMgmtSvcStoreContext : DbContext
    {
        static MicrosoftMgmtSvcStoreContext()
        {
            Database.SetInitializer<MicrosoftMgmtSvcStoreContext>(null);
        }

        public MicrosoftMgmtSvcStoreContext()
            : base("Name=MicrosoftMgmtSvcStoreContext")
        {
        }

        public DbSet<Plan> Plans { get; set; }
        public DbSet<WapSubscription> Subscriptions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new PlanMap());
            modelBuilder.Configurations.Add(new WapSubscriptionMap());
        }
    }
}
