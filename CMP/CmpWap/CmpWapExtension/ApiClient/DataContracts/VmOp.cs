// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    /// <summary>
    /// This is a data contract class between extensions and resource provider
    /// VmOp contains data contract of data which shows up in "VMs" tab inside Cmp Tenant Extension
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class VmOp
    {
        /// <summary>
        /// Name of the file share
        /// </summary>
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public int Id { get; set; }

        [DataMember(Order = 3)]
        public string Opcode { get; set; }

        [DataMember(Order = 4)]
        public int VmId { get; set; }

        [DataMember(Order = 5)]
        public string sData { get; set; }

        [DataMember(Order = 6)]
        public int iData { get; set; }

        [DataMember(Order = 7)]
        public string Config { get; set; }

        [DataMember(Order = 8)]
        public string StatusCode { get; set; }

        [DataMember(Order = 9)]
        public string StatusMessage { get; set; }

        [DataMember(Order = 10)]
        public string Vmsize { get; set; }

        [DataMember(Order = 11)]
        public List<object> disks { get; set; }

        [DataMember(Order = 12)]
        public bool IsMultiOp { get; set; }

    }
}
