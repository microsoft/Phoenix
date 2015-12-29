// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts
{
    /// <summary>    
    /// This is a data contract class between extensions and resource provider
    /// Product contains data contract of data which shows up in "Products" tab inside CmpWapExtension Admin Extension
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public sealed class Product
    {
        /// <summary>
        /// ProductId is hidden in UI but can be used to identify individual product records in grid
        /// </summary>
        [DataMember(Order = 1)]
        public int ProductId { get; set; }

        /// <summary>
        /// ProductName maps to "Name" column in Products tab
        /// </summary>
        [DataMember(Order = 2)]
        public string ProductName { get; set; }

        /// <summary>
        /// ProductPrice maps to "Price" column in Products tab
        /// </summary>
        [DataMember(Order = 3)]
        public double ProductPrice { get; set; }

        /// <summary>
        /// ExpiryDate maps to "Expires" column in Products tab
        /// </summary>
        [DataMember(Order = 4)]
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// NumberOfUnits maps to "Units" column in Products tab
        /// </summary>
        [DataMember(Order = 5)]
        public int NumberOfUnits { get; set; }
    }
}
