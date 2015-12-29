// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    /// <summary>
    /// AdminSettings define data contract of CmpWapExtension resource provider endpoint registration information
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class AdminSettings
    {
        /// <summary>
        /// Address of CmpWapExtension resource provider
        /// </summary>
        [DataMember(Order = 0)]
        public string EndpointAddress { get; set; }

        /// <summary>
        /// Username used by Admin API to connect with CmpWapExtension Resource Provider
        /// </summary>
        [DataMember(Order = 1)]
        public string Username { get; set; }

        /// <summary>
        /// Password used by Admin API to connect with CmpWapExtension Resource Provider
        /// </summary>
        [DataMember(Order = 2)]
        public string Password { get; set; }
    }
}
