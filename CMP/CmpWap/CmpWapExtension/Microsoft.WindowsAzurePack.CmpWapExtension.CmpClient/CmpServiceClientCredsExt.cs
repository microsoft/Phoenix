/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient
{
    class CmpServiceClientCredsExt
    {
    }
}*/

using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.CmpWapExtension.CmpClient.CmpService
{
    //*********************************************************************
    ///
    /// <summary>
    /// Service extension to support client creds and client cert
    /// </summary>
    /// 
    //*********************************************************************
    /* to do: rename this class */
    public partial class Container : global::System.Data.Services.Client.DataServiceContext
    {
        private X509Certificate clientCertificate = null;
        private ICredentials clientCredentials = null;

        //*********************************************************************
        ///
        /// <summary>
        /// Client certificate
        /// http://social.msdn.microsoft.com/Forums/en-US/0aa2a875-fd59-4f3e-a459-9f604b374749/how-do-i-use-certificate-based-authentication-with-data-services-client?forum=adodotnetdataservices
        /// </summary>
        /// 
        //*********************************************************************

        public X509Certificate ClientCertificate
        {
            get
            {
                return clientCertificate;
            }
            set
            {
                if (value == null)
                {
                    // if the event has been hooked up before, we should remove it
                    if (clientCertificate != null)
                        this.SendingRequest2 += this.OnSendingRequest_AddCertificate;
                }
                else
                {
                    // hook up the event if its being set to something non-null
                    if (clientCertificate == null)
                        this.SendingRequest2 += this.OnSendingRequest_AddCertificate;
                }

                clientCertificate = value;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Client credentials for the certificate
        /// </summary>
        /// 
        //*********************************************************************

        public ICredentials ClientCredentials
        {
            get
            {
                return clientCredentials;
            }
            set
            {
                if (value == null)
                {
                    // if the event has been hooked up before, we should remove it
                    if (clientCredentials != null)
                        this.SendingRequest2 += this.OnSendingRequest_AddClientCreds;
                }
                else
                {
                    // hook up the event if its being set to something non-null
                    if (clientCredentials == null)
                        this.SendingRequest2 += this.OnSendingRequest_AddClientCreds;
                }

                clientCredentials = value;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Adds a client certificate to a web request
        /// </summary>
        /// <param name="sender">Sender of the web request</param>
        /// <param name="args">Arguments of the request being sent</param>
        /// 
        //*********************************************************************

        private void OnSendingRequest_AddCertificate(object sender, SendingRequest2EventArgs args)
        {
            if (null == ClientCertificate)
                return;

            var ReqM = args.RequestMessage as System.Data.Services.Client.HttpWebRequestMessage;

            if (null == ReqM)
                return;

            //ReqM.Credentials = new System.Net.NetworkCredential("Administrator2", "SecurePassword2");
            //ReqM.HttpWebRequest.Credentials = new System.Net.NetworkCredential("Administrator3", "SecurePassword3");

            ReqM.HttpWebRequest.ClientCertificates.Add(ClientCertificate);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Adds the client credentials to a web request
        /// </summary>
        /// <param name="sender">Sender of the web request</param>
        /// <param name="args">Arguments of the request being sent</param>
        /// 
        //*********************************************************************

        private void OnSendingRequest_AddClientCreds(object sender, SendingRequest2EventArgs args)
        {
            if (null == ClientCredentials)
                return;

            var reqM = args.RequestMessage as System.Data.Services.Client.HttpWebRequestMessage;

            if (null == reqM)
                return;

            if (null == reqM.HttpWebRequest)
                return;

            //ReqM.Credentials = new System.Net.NetworkCredential("Administrator2", "SecurePassword2");
            //ReqM.HttpWebRequest.Credentials = new System.Net.NetworkCredential("Administrator3", "SecurePassword3");

            reqM.HttpWebRequest.Credentials = ClientCredentials;
        }
    }
}