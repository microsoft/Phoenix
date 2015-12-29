//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Azure.Portal.Configuration;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension
{
    /// <summary>
    /// Factory for generating an interface to the CMP-WAP RP API
    /// </summary>
    public static class ClientFactory
    {
        //Get Service Management API endpoint
        private static Uri tenantApiUri;

        private static BearerMessageProcessingHandler messageHandler;

        //This client is used to communicate with the CmpWapExtension resource provider
        private static Lazy<CmpWapExtensionClient> CmpWapExtensionRestClient = new Lazy<CmpWapExtensionClient>(
           () => new CmpWapExtensionClient(tenantApiUri, messageHandler),
           LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Creates and initializes the static instance
        /// </summary>
        static ClientFactory()
        {
            tenantApiUri = new Uri(AppManagementConfiguration.Instance.RdfeUnifiedManagementServiceUri);
            messageHandler = new BearerMessageProcessingHandler(new WebRequestHandler());
        }

        /// <summary>
        /// Interface to the CMP-WAP extension API
        /// </summary>
        public static CmpWapExtensionClient CmpWapExtensionClient
        {
            get
            {
                return CmpWapExtensionRestClient.Value;
            }
        }
    }
}
