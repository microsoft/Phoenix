using System;
using System.Collections.Generic;

namespace CmpServiceLib
{
    public class ServProvAccount
    {
        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="subscriptionId"></param>
        ///  <param name="certThumbprint"></param>
        /// <param name="azureAdTenantId"></param>
        /// <param name="azureAdClientId"></param>
        /// <param name="azureAdClientKey"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************
        public static AzureAdminClientLib.Connection GetAzureServiceAccountConnection(
            string subscriptionId, string certThumbprint, 
            string azureAdTenantId, string azureAdClientId, string azureAdClientKey)
        {
            try
            {
                var Out =
                    new AzureAdminClientLib.Connection(subscriptionId, certThumbprint, 
                        azureAdTenantId, azureAdClientId, azureAdClientKey);
                return Out;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ServProvAccount.GetAzureServiceAccountConnection() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="accountId"></param>
        /// <param name="cmpDbConnectionString"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public static AzureAdminClientLib.Connection GetAzureServiceAccountConnection(
            int accountId, string cmpDbConnectionString)
        {
            var cdb = new CmpDb(cmpDbConnectionString);

            var accountInfo = cdb.FetchServiceProviderAccount(accountId);
            var Out =
                GetAzureServiceAccountConnection(accountInfo.AccountID, accountInfo.CertificateThumbprint,
                accountInfo.AzureADTenantId, accountInfo.AzureADClientId, accountInfo.AzureADClientKey);

            return Out;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="cmpDbConnectionString"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static Models.ServiceProviderAccount GetAzureServiceAccount(
            int accountId, string cmpDbConnectionString)
        {
            var cdb = new CmpDb(cmpDbConnectionString);
            return cdb.FetchServiceProviderAccount(accountId);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="cmpDbConnectionString"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static List<Models.ServiceProviderAccount> GetAzureServiceAccountList(
            string groupName, string cmpDbConnectionString)
        {
            var cdb = new CmpDb(cmpDbConnectionString);
            return cdb.FetchServiceProviderAccountList(groupName);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmpDbConnectionString"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static List<Models.ServiceProviderAccount> GetAzureServiceAccountList(
            string cmpDbConnectionString)
        {
            var cdb = new CmpDb(cmpDbConnectionString);
            return cdb.FetchServiceProviderAccountList(string.Empty);
        }

    }
}
