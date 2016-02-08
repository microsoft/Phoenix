//*****************************************************************************
// File: Utilities.cs
// Project: Microsoft.WindowsAzurePack.CmpWapExtension.Api
// Purpose: This class contains utilities methids
// Copyright (c) Microsoft Corporation.  All rights reserved.
//*****************************************************************************

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AzureAdminClientLib;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.Api
{
    /// <remarks>
    ///     This class contains utilities methids
    /// </remarks>
    public class Utilities
    {
        //*********************************************************************
        ///
        /// <summary>
        /// This method unwinds exception messages
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns>The unwound messages</returns>
        /// 
        //*********************************************************************

        public static string UnwindExceptionMessages( Exception ex )
        {
            string message = ex.Message;

            if (null != ex.InnerException)
            {
                ex = ex.InnerException;
                message += " - " + ex.Message;

                if (null != ex.InnerException)
                {
                    ex = ex.InnerException;
                    message += " - " + ex.Message;
                }
            }

            return message;
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  This method validates the Azure Active Directory parameters that
        ///  the user inputs as part of a Service Provider Account.
        ///  </summary>
        /// <param name="clientId"></param>
        /// <param name="tenantId"></param>
        /// <param name="clientKey"></param>
        /// <returns>A bool stating whether the credentials are valid (true) or
        /// not (false)</returns>
        ///  
        //*********************************************************************
        public static bool ValidateAadCredentials(string clientId, string tenantId, string clientKey)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientKey))
                return false;

            AuthenticationResult adToken = null;
            
            var task = Task.Run(() =>
            {
                var response = AzureActiveDir.GetAdUserToken(tenantId, clientId, clientKey);
                response.Wait();
                adToken = response.Result;
            });

            task.Wait();

            if (adToken != null)
                return true;

            return false;
        }
    }
}
