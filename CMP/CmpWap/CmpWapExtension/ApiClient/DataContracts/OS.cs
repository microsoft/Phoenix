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
    public class OS
    {
        [DataMember(Order = 1)]
        public int VmOsId { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

         [DataMember(Order = 3)]
        public string Description { get; set; }

        [DataMember(Order = 4)]
        public string AzureImageName { get; set; }
         [DataMember(Order = 5)]
        public bool IsActive { get; set; }
    }
}
