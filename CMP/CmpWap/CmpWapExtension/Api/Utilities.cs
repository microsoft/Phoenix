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
                try
                {
                    response.Wait();
                    adToken = response.Result;
                }
                catch (Exception) {} //This catches the exception thrown and allows the calling method to manage in a validation manner, not as a fatal error.            
            });

            /*TO-DO: Research the execution of the AAD method above. Why does it hang intermittently with invalid creds? */ 
            task.Wait(5000); //Added 5 sec limit, since sometimes either Azure or the call leaves the process hanging with invalid creds. Works perfectly if it's second time or if creds are valid and token is issued.
            

            if (adToken != null)
                return true;

            return false;
        }
    }
}
