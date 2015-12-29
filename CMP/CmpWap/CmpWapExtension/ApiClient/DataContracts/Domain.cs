// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    /// <summary>
    /// This is a data contract class between extensions and resource provider
    /// CreateVM contains data contract of data which shows up in "VMs" tab inside Cmp Tenant Extension
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class Domain
    {
        /// <summary>
        /// Name of the file share
        /// </summary>
        [DataMember(Order = 1)]
        public string Name { get; set; }

        /// <summary>
        /// SubscriptionId of user who created this file share
        /// </summary>
        [DataMember(Order = 2)]
        public string DisplayName { get; set; }


        [DataMember(Order = 3)]
        public int Id { get; set; }
    }
}
