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
    public class App
    {
        /// <summary>
        /// Name of the file share
        /// </summary>
        [DataMember(Order = 1)]
        public int ApplicationId { get; set; }

        /// <summary>
        /// Name of the file share
        /// </summary>
        [DataMember(Order = 2)]
        public string Name { get; set; }

        /// <summary> /// </summary>
        [DataMember(Order = 3)]
        public string Code { get; set; }

        /// <summary> /// </summary>
        [DataMember(Order = 4)]
        public string HasService { get; set; }
        [DataMember(Order = 4)]
        public string CIOwner { get; set; }
        [DataMember(Order = 5)]
        public bool IsActive { get; set; }
        [DataMember(Order = 6)]
        public string SubscriptionId { get; set; }
    }
}
