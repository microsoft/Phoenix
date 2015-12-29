//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Data model for file share tenant view
    /// </summary>    
    public class FileShareModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileShareModel" /> class.
        /// </summary>
        public FileShareModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileShareModel" /> class.
        /// </summary>
        /// <param name="fileShareFromApi">The file share from API.</param>
        public FileShareModel(FileShare fileShareFromApi)
        {
            this.Name = fileShareFromApi.Name;
            this.SubscriptionId = fileShareFromApi.SubscriptionId;
            this.FileServerName = fileShareFromApi.FileServerName;
            this.Size = fileShareFromApi.Size;            
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API file share data contract.</returns>
        public FileShare ToApiObject()
        {
            return new FileShare()
            {
                Name = this.Name,
                FileServerName = this.FileServerName,
                Size = this.Size,
                SubscriptionId = this.SubscriptionId
            };
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }              

        /// <summary>
        /// Gets or sets the value of the display name of the file server 
        /// </summary>
        public string FileServerName { get; set; }

        /// <summary>
        /// Gets or sets the value of the subscription id
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the value of the file share size
        /// </summary>
        public int Size { get; set; }

       
    }
}