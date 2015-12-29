//*****************************************************************************
//
// File:
// Author: Mark west (mark.west@microsoft.com)
//
//*****************************************************************************

using CmpInterfaceModel;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Diagnostics;

namespace AzureAdminClientLib
{
    public interface IConnection
    {
        Microsoft.Azure.SubscriptionCloudCredentials CertCloudCreds { get; }
        string CertThumbprint { get; }
        X509Certificate2 Certificate { get; }
        string SubcriptionID { get; }
        AuthenticationResult AdToken { get; }
    }

    //*****************************************************************************
    ///
    /// <summary>
    /// 
    /// </summary>
    /// 
    //*****************************************************************************

    public class Connection : IConnection
    {
        readonly string _subscriptionId;
        readonly string _certThumbprint;
        readonly X509Certificate2 _cert;
        private AuthenticationResult _adToken;
        private readonly Microsoft.Azure.SubscriptionCloudCredentials _certCloudCreds;
        private string _tenantId;
        private string _clientId;
        private string _clientKey;
        private EventLog eLog;

        public string SubcriptionID { get { return _subscriptionId; } }
        public string CertThumbprint { get { return _certThumbprint; } }
        public X509Certificate2 Certificate { get { return _cert; } }
        public Microsoft.Azure.SubscriptionCloudCredentials CertCloudCreds { get { return _certCloudCreds; } }
        public AuthenticationResult AdToken {get{ return _adToken; } }

        //*****************************************************************************
        ///
        /// <summary>
        /// Initializes a Connection object to be used with ARM API
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="certThumbprint"></param>
        /// <param name="tenantId"></param>
        /// <param name="clientId"></param>
        /// <param name="clientKey"></param>
        /// 
        //*****************************************************************************

        public Connection(string subscriptionId, string certThumbprint, 
            string tenantId, string clientId, string clientKey)
        {
            _subscriptionId = subscriptionId;
            _certThumbprint = certThumbprint;
            _certCloudCreds = null;
            _adToken = null;

            if(null != certThumbprint) if (0 != certThumbprint.Length)
            {
                _cert = FetchCertFromStore(certThumbprint);

                _certCloudCreds = new Microsoft.Azure.CertificateCloudCredentials(
                    _subscriptionId, _cert);
            }

            if (null != tenantId & null != clientId & null != clientKey)
                if (0 != tenantId.Length & 0 != clientId.Length & 0 != clientKey.Length)
                {

                    _tenantId = tenantId;
                    _clientId = clientId;
                    _clientKey = clientKey;

                    var tisk = Task.Run(() =>
                    {
                        var tr = FetchAdUserToken();
                        tr.Wait();
                        _adToken = tr.Result;
                    });

                    tisk.Wait();
                }

            if (null == _certCloudCreds & null == _adToken)
                throw new Exception("Exception in AzureAdminClientLib:Connection:Connection() : No valid AAD credentials or certificate thumbprint provided in request");
        }

        //*****************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*****************************************************************************

        public async Task< AuthenticationResult> FetchAdUserToken()
        {
            if (null != _tenantId & null != _clientId & null != _clientKey)
                if (0 != _tenantId.Length & 0 != _clientId.Length & 0 != _clientKey.Length)
                {
                    var res = await AzureActiveDir.GetAdUserToken(_tenantId, _clientId, _clientKey);
                    return res;
                }

            return null;
        }

        //*****************************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="certThumbprint"></param>
        /// <returns></returns>
        /// 
        //*****************************************************************************

        private X509Certificate2 FetchCertFromStore(string certThumbprint)
        {
            try
            {
                if (String.IsNullOrEmpty(certThumbprint))
                    throw new Exception("Certificate Thumbprint is null or empty");
                
                //X509Store certificateStore = new X509Store(StoreName.CertificateAuthority, StoreLocation.LocalMachine);

                var certificateStore = new X509Store(StoreName.My, StoreLocation.LocalMachine); //*** Works in Azure Service ***
                certificateStore.Open(OpenFlags.ReadOnly);
                //X509Certificate2Collection certs = certificateStore.Certificates;

                var certs = certificateStore.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false);

                if (certs.Count != 1)
                {
                    certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser); //*** Works on local dev computer ***
                    certificateStore.Open(OpenFlags.ReadOnly);
                    certs = certificateStore.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false);

                    if (certs.Count != 1)
                        throw new Exception("Certificate not found in store");
                }

                return certs[0];
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Connection.FetchCertFromStore() : "
                    + Utilities.UnwindExceptionMessages(ex));
            }
        }

        #region --- Helper methods --------------------------------------------

        private void LogThis(Exception ex, EventLogEntryType type, string prefix,
            int id, short category)
        {
            if (null == eLog)
                return;

            if (null == ex)
                eLog.WriteEntry(prefix, type, id, category);
            else
                eLog.WriteEntry(prefix + " : " +
                    Utilities.UnwindExceptionMessages(ex),
                    type, id, category);
        }

        #endregion
    }
}
