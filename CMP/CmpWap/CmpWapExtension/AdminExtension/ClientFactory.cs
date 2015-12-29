//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Azure.Portal.Configuration;
using Microsoft.WindowsAzurePack.CmpWapExtension.ApiClient;
using Microsoft.WindowsAzurePack.Samples;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.AdminExtension
{
    public static class ClientFactory
    {
        //Get Service Management API endpoint
        private static Uri adminApiUri;

        private static BearerMessageProcessingHandler messageHandler;

        //This client is used to communicate with the CmpWapExtension resource provider
        private static Lazy<CmpWapExtensionClient> CmpWapExtensionRestClient = new Lazy<CmpWapExtensionClient>(
           () => new CmpWapExtensionClient(adminApiUri, messageHandler),
           LazyThreadSafetyMode.ExecutionAndPublication);

        //This client is used to communicate with the Admin API
        private static Lazy<AdminManagementClient> adminApiRestClient = new Lazy<AdminManagementClient>(
            () => new AdminManagementClient(adminApiUri, messageHandler),
            LazyThreadSafetyMode.ExecutionAndPublication);

        static ClientFactory()
        {
            adminApiUri = new Uri(OnPremPortalConfiguration.Instance.RdfeAdminUri);
            messageHandler = new BearerMessageProcessingHandler(new WebRequestHandler());
        }

        public static CmpWapExtensionClient CmpWapExtensionClient
        {
            get
            {
                return CmpWapExtensionRestClient.Value;
            }
        }

        public static AdminManagementClient AdminManagementClient
        {
            get
            {
                return adminApiRestClient.Value;
            }
        }
    }
}
