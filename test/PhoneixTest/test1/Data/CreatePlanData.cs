// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreatePlanData.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//   Utilities for generating test data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.Data
{
    public class CreatePlanData
    {
        // Step 1
        public string planName { get; set; }

        public string clientId { get; set; }

        public string clientKey { get; set; }

        public string tenantId { get; set; }

        public string azureSubscription { get; set; }

        public string groupName { get; set; }

        public string region { get; set; }

        // step 2
        public string serverName { get; set; }

        public string azureOsVersion { get; set; }

        public string userName { get; set; }

        public string localAdminPassword { get; set; }

        // step 3
        public bool needAdditionalDrives { get; set; }

    }
}
