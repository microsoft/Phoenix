namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Entitity framework DB contexts to the CMP database
    /// </summary>
    public partial class MicrosoftMgmtSvcCmpContext : DbContext
    {
        public MicrosoftMgmtSvcCmpContext()
            : base("name=MicrosoftMgmtSvcCmpContext")
        {
        }

        public virtual DbSet<AdDomainMap> AdDomainMaps { get; set; }
        public virtual DbSet<AzureRegion> AzureRegions { get; set; }
        public virtual DbSet<CmpRequest> CmpRequests { get; set; }
        public virtual DbSet<VmOs> VmOs { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<WapSubscriptionData> WapSubscriptionDatas { get; set; }
        public DbSet<WapSubscriptionGroupMembership> WapSubscriptionGroupMemberships { get; set; }

        public DbSet<IpakVersionMap> IpakVersionMaps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdDomainMap>()
                .Property(e => e.DomainShortName)
                .IsUnicode(false);

            modelBuilder.Entity<AdDomainMap>()
                .Property(e => e.DomainFullName)
                .IsUnicode(false);

            modelBuilder.Entity<AdDomainMap>()
                .Property(e => e.JoinCredsUserName)
                .IsUnicode(false);

            modelBuilder.Entity<AdDomainMap>()
                .Property(e => e.JoinCredsPasword)
                .IsUnicode(false);

            modelBuilder.Entity<AdDomainMap>()
                .Property(e => e.Config)
                .IsUnicode(false);

            modelBuilder.Entity<AdDomainMap>()
                .Property(e => e.ServerOU)
                .IsUnicode(false);

            modelBuilder.Entity<AdDomainMap>()
                .Property(e => e.WorkstationOU)
                .IsUnicode(false);

            modelBuilder.Entity<AdDomainMap>()
                .Property(e => e.DefaultVmAdminMember)
                .IsUnicode(false);

            modelBuilder.Entity<AdDomainMap>()
              .HasMany(e => e.ResourceProviderAcctGroups)
              .WithRequired(e => e.AdDomainMap)
              .HasForeignKey(e => e.DomainId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<AzureRegion>()
                .Property(e => e.OsImageContainer)
                .IsUnicode(false);

            modelBuilder.Entity<AzureRegion>()
                .Property(e => e.LastUpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EnvironmentType>()
             .Property(e => e.LastUpdatedBy)
             .IsUnicode(false);

            modelBuilder.Entity<EnvironmentType>()
              .HasMany(e => e.ResourceProviderAcctGroups)
              .WithRequired(e => e.EnvironmentType)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<NetworkNIC>()
                .Property(e => e.LastUpdatedBy)
                .IsUnicode(false);


            modelBuilder.Entity<NetworkNIC>()
                .HasMany(e => e.ResourceProviderAcctGroups)
                .WithRequired(e => e.NetworkNIC)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.WapSubscriptionID)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.ParentAppName)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.TargetVmName)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.Domain)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.VmSize)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.TargetLocation)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.StatusCode)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.SourceImageName)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.SourceServerName)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.UserSpec)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.StorageSpec)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.FeatureSpec)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.Config)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.RequestType)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.WhoRequested)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.StatusMessage)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.ExceptionMessage)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.Warnings)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.TagData)
                .IsUnicode(false);

            modelBuilder.Entity<CmpRequest>()
                .Property(e => e.AddressFromVm)
                .IsUnicode(false);

            modelBuilder.Entity<VmOs>()
                .Property(e => e.AzureImageName)
                .IsUnicode(false);

            modelBuilder.Entity<VmOs>()
                .Property(e => e.LastUpdatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<ResourceProviderAcctGroup>()
             .Property(e => e.Name)
             .IsUnicode(false);



            modelBuilder.Entity<ResourceProviderAcctGroup>()
                .Property(e => e.LastUpdatedBy)
                .IsUnicode(false);

            

            modelBuilder.Configurations.Add(new GroupMap());
            modelBuilder.Configurations.Add(new WapSubscriptionDataMap());
            modelBuilder.Configurations.Add(new WapSubscriptionGroupMembershipMap());

            modelBuilder.Entity<IpakVersionMap>()
               .Property(e => e.VersionCode)
               .IsUnicode(false);

            modelBuilder.Entity<IpakVersionMap>()
                .Property(e => e.VersionName)
                .IsUnicode(false);

            modelBuilder.Entity<IpakVersionMap>()
                .Property(e => e.AzureRegion)
                .IsUnicode(false);

            modelBuilder.Entity<IpakVersionMap>()
                .Property(e => e.AdDomain)
                .IsUnicode(false);

            modelBuilder.Entity<IpakVersionMap>()
                .Property(e => e.Config)
                .IsUnicode(false);

            modelBuilder.Entity<IpakVersionMap>()
                .Property(e => e.TagData)
                .IsUnicode(false);

            modelBuilder.Entity<IpakVersionMap>()
                .Property(e => e.IpakDirLocation)
                .IsUnicode(false);

            modelBuilder.Entity<IpakVersionMap>()
                .Property(e => e.IpakFullFileLocation)
                .IsUnicode(false);

            modelBuilder.Entity<IpakVersionMap>()
                .Property(e => e.AdminName)
                .IsUnicode(false);

            modelBuilder.Entity<IpakVersionMap>()
                .Property(e => e.QfeVersion)
                .IsUnicode(false);

        }
    }
}
