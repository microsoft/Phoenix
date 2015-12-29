//*****************************************************************************
// File: Subscription.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: Model for table vwWAPUpstreamFeed
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api.Models.ECS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vwWAPUpstreamFeed")]
    public partial class Subscription
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PortfolioID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubscriptionID { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid WAPSubscriptionID { get; set; }

        [StringLength(128)]
        public string ServerAdministrator { get; set; }

        [StringLength(128)]
        public string CIOwner { get; set; }

        [StringLength(128)]
        public string ResponsibleOwner { get; set; }

        [StringLength(200)]
        public string L1DomainSupport { get; set; }

        [StringLength(200)]
        public string L2DomainSupport { get; set; }

        [StringLength(200)]
        public string L3DomainSupport { get; set; }

        [StringLength(300)]
        public string ApplicationName { get; set; }

        [StringLength(300)]
        public string ApplicationID { get; set; }

        [StringLength(50)]
        public string FinancialOwner { get; set; }

        public int? OrganizationID { get; set; }

        [StringLength(50)]
        public string ChargeBackGroup { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(4)]
        public string EnvironmentClass { get; set; }

        [StringLength(9)]
        public string ServiceCategory { get; set; }
    }
}
