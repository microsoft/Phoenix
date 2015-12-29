//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Web;
using Microsoft.WindowsAzurePack.Samples.DataContracts;

namespace Microsoft.WindowsAzurePack.Samples
{
    internal static class QueryExtensions
    {
        /// <summary>
        /// Gets an HTTP query string serialization of this query compatible with FromQueryString
        /// </summary>
        public static string GetQueryString(this Query query)
        {
            List<string> parts = new List<string>();

            if (query.Skip != null)
            {
                parts.Add("skip=" + HttpUtility.UrlEncode(query.Skip.Value.ToString()));
            }

            if (query.Take != null)
            {
                parts.Add("take=" + HttpUtility.UrlEncode(query.Take.Value.ToString()));
            }

            if (!string.IsNullOrEmpty(query.SortProperty))
            {
                parts.Add("sortProperty=" + HttpUtility.UrlEncode(query.SortProperty));
            }

            if (!string.IsNullOrEmpty(query.SortDirection))
            {
                parts.Add("sortDirection=" + HttpUtility.UrlEncode(query.SortDirection));
            }

            if (!string.IsNullOrEmpty(query.Filter))
            {
                parts.Add("filter=" + HttpUtility.UrlEncode(query.Filter));
            }

            return string.Join("&", parts);
        }
    }
}
