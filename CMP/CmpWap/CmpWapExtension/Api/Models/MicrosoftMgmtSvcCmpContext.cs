//*****************************************************************************
// File: MicrosoftMgmtSvcCmpContext.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for MicrosoftMgmtSvcCmpContext
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System.Data.Entity;
using Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.Mapping;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models
{
    public partial class MicrosoftMgmtSvcCmpContext : DbContext
    {
        static MicrosoftMgmtSvcCmpContext()
        {
            Database.SetInitializer<MicrosoftMgmtSvcCmpContext>(null);
        }

        public MicrosoftMgmtSvcCmpContext()
            : base(GetConnectionString())
        {
        }

        public static string GetConnectionString()
        {
            using (var xk = new KryptoLib.X509Krypto())
            {
                return xk.GetKTextConnectionString("MicrosoftMgmtSvcCmpContext",
                    "MicrosoftMgmtSvcCmpContextPassword");

            }
        }

        public DbSet<AdDomainMap> AdDomainMaps { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<AzureAdminSubscriptionMapping> AzureAdminSubscriptionMappings { get; set; }
        public DbSet<AzureAdminSubscriptionRegionMapping> AzureAdminSubscriptionRegionMappings { get; set; }
        public DbSet<AzureAdminSubscriptionVmOsMapping> AzureAdminSubscriptionVmOsMappings { get; set; }
        public DbSet<AzureAdminSubscriptionVmSizeMapping> AzureAdminSubscriptionVmSizeMappings { get; set; }
        public DbSet<AzureAdminSubscriptionVnet> AzureAdminSubscriptionVnets { get; set; }
        public DbSet<AzureAdminSubscriptionVnetMapping> AzureAdminSubscriptionVnetMappings { get; set; }
        public DbSet<AzureMasterData> AzureMasterDatas { get; set; }
        public DbSet<AzureRegion> AzureRegions { get; set; }
        public DbSet<AzureRegionVmSizeMapping> AzureRegionVmSizeMappings { get; set; }
        public DbSet<AzureRegionVmOsMapping> AzureRegionVmOsMappings { get; set; }
        public DbSet<CmpRequest> CmpRequests { get; set; }
        public DbSet<EnvironmentType> EnvironmentTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<IISRoleService> IISRoleServices { get; set; }
        public DbSet<NetworkNIC> NetworkNICs { get; set; }
        public DbSet<ResourceProviderAcctGroup> ResourceProviderAcctGroups { get; set; }
        public DbSet<SequenceRequest> SequenceRequests { get; set; }
        public DbSet<ServerRole> ServerRoles { get; set; }
        public DbSet<ServerRoleDriveMapping> ServerRoleDriveMappings { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<SQLAnalysisServicesMode> SQLAnalysisServicesModes { get; set; }
        public DbSet<SQLCollation> SQLCollations { get; set; }
        public DbSet<SQLVersion> SQLVersions { get; set; }
        public DbSet<VmOs> VmOs { get; set; }
        public DbSet<VmSize> VmSizes { get; set; }

        public DbSet<WapSubscriptionData> WapSubscriptionDatas { get; set; }
        public DbSet<WapSubscriptionGroupMembership> WapSubscriptionGroupMemberships { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AdDomainMapMap());
            modelBuilder.Configurations.Add(new ApplicationMap());
            modelBuilder.Configurations.Add(new AzureAdminSubscriptionMappingMap());
            modelBuilder.Configurations.Add(new AzureAdminSubscriptionRegionMappingMap());
            modelBuilder.Configurations.Add(new AzureAdminSubscriptionVmOsMappingMap());
            modelBuilder.Configurations.Add(new AzureAdminSubscriptionVmSizeMappingMap());
            modelBuilder.Configurations.Add(new AzureAdminSubscriptionVnetMap());
            modelBuilder.Configurations.Add(new AzureAdminSubscriptionVnetMappingMap());
            modelBuilder.Configurations.Add(new AzureMasterDataMap());
            modelBuilder.Configurations.Add(new AzureRegionMap());
            modelBuilder.Configurations.Add(new AzureRegionVmSizeMappingMap());
            modelBuilder.Configurations.Add(new AzureRegionVmOsMappingMap());
            modelBuilder.Configurations.Add(new CmpRequestMap());
            modelBuilder.Configurations.Add(new EnvironmentTypeMap());
            modelBuilder.Configurations.Add(new GroupMap());
            modelBuilder.Configurations.Add(new IISRoleServiceMap());
            modelBuilder.Configurations.Add(new NetworkNICMap());
            modelBuilder.Configurations.Add(new ResourceProviderAcctGroupMap());
            modelBuilder.Configurations.Add(new SequenceRequestMap());
            modelBuilder.Configurations.Add(new ServerRoleMap());
            modelBuilder.Configurations.Add(new ServerRoleDriveMappingMap());
            modelBuilder.Configurations.Add(new ServiceCategoryMap());
            modelBuilder.Configurations.Add(new SQLAnalysisServicesModeMap());
            modelBuilder.Configurations.Add(new SQLCollationMap());
            modelBuilder.Configurations.Add(new SQLVersionMap());
            modelBuilder.Configurations.Add(new VmOsMap());
            modelBuilder.Configurations.Add(new VmSizeMap());          
            modelBuilder.Configurations.Add(new WapSubscriptionDataMap());
            modelBuilder.Configurations.Add(new WapSubscriptionGroupMembershipMap());
        }
    }
}
