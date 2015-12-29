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
    public class VmSize
    {

        [DataMember(Order = 1)]
        public int VmSizeId { get; set; }

       [DataMember(Order = 2)]
        public string Name { get; set; }

       [DataMember(Order = 3)]
        public string Description { get; set; }
        [DataMember(Order = 4)]
        public int Cores { get; set; }
        [DataMember(Order = 5)]
        public int Memory { get; set; }
        [DataMember(Order = 6)]
        public int MaxDataDiskCount { get; set; }
        [DataMember(Order = 7)]
        public bool IsActive { get; set; }
        ///// <summary>
        ///// Name of the file share
        ///// </summary>
        //[DataMember(Order = 1)]
        //public int ID { get; set; }

        ///// <summary>
        ///// Name of the file share
        ///// </summary>
        //[DataMember(Order = 2)]
        //public string Name { get; set; }

        ///// <summary>
        ///// SubscriptionId of user who created this file share
        ///// </summary>
        //[DataMember(Order = 3)]
        //public string DisplayName { get; set; }

        ///// <summary>
        ///// SubscriptionId of user who created this file share
        ///// </summary>
        //[DataMember(Order = 4)]
        //public string Description { get; set; }

        ///// <summary>
        ///// SubscriptionId of user who created this file share
        ///// </summary>
        //[DataMember(Order = 5)]
        //public string Config { get; set; }

        ///// <summary>
        ///// SubscriptionId of user who created this file share
        ///// </summary>
        //[DataMember(Order = 6)]
        //public string TagData { get; set; }

        ///// <summary> </summary>
        //[DataMember(Order = 7)]
        //public bool IsActive { get; set; }

        ///// <summary> </summary>
        //[DataMember(Order = 8)]
        //public int CpuCoreCount { get; set; }

        ///// <summary> </summary>
        //[DataMember(Order = 9)]
        //public int RamMB { get; set; }

        ///// <summary> </summary>
        //[DataMember(Order = 10)]
        //public int DiskSizeOS { get; set; }

        ///// <summary> </summary>
        //[DataMember(Order = 11)]
        //public int DiskSizeTemp { get; set; }
        ///// <summary> </summary>
        ///// 
        //[DataMember(Order = 12)]
        //public int DataDiskCount { get; set; }
    }
}
