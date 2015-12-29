// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    /// <summary>    
    /// This is a data contract class between extensions and resource provider
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class Subscription 
    {
        [DataMember(Order = 1)]
        public string WapSubscriptionId { get; set; }

        [DataMember(Order = 2)]
        public string PlanNameId { get; set; }

        [DataMember(Order = 3)]
        public string AzureSubscriptionId { get; set; }
    }
}
