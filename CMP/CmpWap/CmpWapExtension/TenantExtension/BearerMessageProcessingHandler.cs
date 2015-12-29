// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Microsoft.WindowsAzure.Management;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.TenantExtension
{
    /// <summary>
    /// Bearer Token MessageProcessingHandler
    /// </summary>
    public class BearerMessageProcessingHandler : MessageProcessingHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BearerMessageProcessingHandler" /> class.
        /// </summary>
        /// <param name="innerHandler">The inner handler.</param>
        public BearerMessageProcessingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The request</returns>
        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var bearerToken = AuthenticationHelper.GetPrincipalBearerToken();
                        
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            return request;
        }

        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The response</returns>
        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return response;
        }
    }
}
