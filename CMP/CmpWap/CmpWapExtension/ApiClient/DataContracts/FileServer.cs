// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    /// <summary>
    /// This is a data contract class between extensions and resource provider
    /// FileServer contains data contract of data which shows up in "File Servers" tab inside CmpWapExtension Admin Extension
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public sealed class FileServer
    {
        [DataMember(Order = 1)]
        public int FileServerId { get; set; }

        /// <summary>
        /// Name of the file server
        /// </summary>
        [DataMember(Order = 2)]
        public string FileServerName { get; set; }

        /// <summary>
        /// Total space in File Server (KB, MB, GB) 
        /// </summary>
        [DataMember(Order = 3)]
        public int TotalSpace { get; set; }

        /// <summary>
        /// Default size of any share in file server (KB, MB, GB) 
        /// </summary>
        [DataMember(Order = 4)]
        public int DefaultSize { get; set; }

        /// <summary>
        /// Total Free Space available in file server (KB, MB, GB)
        /// </summary>
        [DataMember(Order = 5)]
        public int FreeSpace { get; set; }
    }
}
