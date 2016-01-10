//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension.Models
{
    /// <summary>
    /// Data model for domain name tenant view
    /// </summary>    
    public class CreateAppModel
    {
        public const string RegisteredStatus = "Registered";

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAppModel" /> class.
        /// </summary>
        public CreateAppModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateApModel" /> class.
        /// </summary>
        /// <param name="createOsFromApi">The domain name from API.</param>
        public CreateAppModel(App createOsFromApi)
        {
            this.Name = createOsFromApi.Name;
            this.ApplicationId = createOsFromApi.ApplicationId;
            this.IsActive = createOsFromApi.IsActive;
            this.AppCode = createOsFromApi.Code;
            this.SubscriptionId = createOsFromApi.SubscriptionId;
            this.Region = createOsFromApi.Region;
        }

        /// <summary>
        /// Convert to the API object.
        /// </summary>
        /// <returns>The API DomainName data contract.</returns>
        public App ToApiObject()
        {
            return new App()
            {
            Name = this.Name,
            ApplicationId = this.ApplicationId,
            IsActive = this.IsActive,
            Code = this.AppCode,
            SubscriptionId = this.SubscriptionId,
            Region = this.Region
            };
        }

        /// <summary>The name of the application</summary>
        public string Name { get; set; }

        /// <summary>The code of the application</summary>
        public string AppCode { get; set; }

      
        /// <summary>The ID of the application</summary>
        public int ApplicationId { get; set; }

        /// <summary>Whether the application is considered active</summary>
        public bool IsActive { get; set; }

        public string SubscriptionId { get; set; }

        public string Region { get; set; }

    }
}