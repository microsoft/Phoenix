// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    /// <summary>
    /// This is a data contract class between extensions and resource provider
    /// FileServer contains data contract of data which shows up in "File Shares" tab inside CmpWapExtension Tenant Extension
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class FileShare
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
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Name of the file server where file share resides
        /// </summary>
        [DataMember(Order = 3)]
        public string FileServerName { get; set; }

        /// <summary>
        /// Size of the file share
        /// </summary>
        [DataMember(Order = 4)]
        public int Size { get; set; }

        /// <summary>
        /// Id of the file share
        /// </summary>
        [DataMember(Order = 5)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
