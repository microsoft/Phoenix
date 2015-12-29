// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension.Models
{
    /// <summary>
    /// This is a model class which contains data contract we send to Controller which then shows up in UI
    /// ProductModel contains data contract of data which shows up in "Products" tab inside CmpWapExtension Admin Extension
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// ProductId is hidden in UI but can be used to identify individual product records in grid
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// ProductName maps to "Name" column in Products tab
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// ProductPrice maps to "Price" column in Products tab
        /// </summary>
        public double ProductPrice { get; set; }

        /// <summary>
        /// ExpiryDate maps to "Expires" column in Products tab
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// NumberOfUnits maps to "Units" column in Products tab
        /// </summary>
        public int NumberOfUnits { get; set; }
        
        public ProductModel(Product product)
        {
            this.ProductId = product.ProductId;
            this.ProductName = product.ProductName;
            this.ProductPrice = product.ProductPrice;
            this.NumberOfUnits = product.NumberOfUnits;            
            this.ExpiryDate = product.ExpiryDate;
        }
    }
}
