//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;

namespace Microsoft.WindowsAzurePack.Samples.DataContracts
{
    /// <summary>
    /// Certificate Validation state. Setting this to Disable will disable all certificate validation to the remote server. 
    /// Used when the installation has self-signed untrusted certificates.
    /// </summary>
    public enum CertificateValidation
    {
        /// <summary>
        /// The enable
        /// </summary>
        Enable,

        /// <summary>
        /// The disable
        /// </summary>
        Disable,
    }

    /// <summary>
    /// Issues tokens based on the AuthType specified
    /// </summary>
    public static class TokenIssuer
    {
        /// <summary>
        /// Gets the auth token in an Windows Authentication scennario.
        /// </summary>
        /// <param name="windowsAuthSiteEndpoint">The windows auth site endpoint.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="shouldImpersonate">if set to true the token will be obtained as the user specified in the input. if set to <c>false</c> [the request will run as the current user].</param>
        /// <returns>
        /// string token from the windows Auth site
        /// </returns>
        public static string GetWindowsAuthToken(string windowsAuthSiteEndpoint, string domainName, string userName, string password, bool shouldImpersonate = true)
        {
            string token = null;

            ////Impersonate the Administrator and get the Admin Token
            if (shouldImpersonate)
            {
                Impersonation.Impersonate(domainName, userName, password, () => token = GetWindowsTokenHelper(windowsAuthSiteEndpoint));
            }
            else
            {
                token = GetWindowsTokenHelper(windowsAuthSiteEndpoint);
            }

            return token;
        }

        /// <summary>
        /// Gets the auth token in an ADFS scennario.
        /// </summary>
        /// <param name="adfsEndpoint">The adfs endpoint.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// string token from the adfs site
        /// </returns>
        public static string GetAdfsAuthToken(string adfsEndpoint, string domainName, string userName, string password)
        {
            string token = null;
            string completeUserName = domainName + "\\" + userName;
            token = GetADFSTokenHelper(adfsEndpoint, completeUserName, password);
            return token;
        }

        /// <summary>
        /// GGets the auth token in a Membership Provider scennario.
        /// </summary>
        /// <param name="membershipAuthSiteEndpoint">The membership auth site endpoint.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="certificateValidation">Enable/Disable certificate validation.</param>
        /// <returns>
        /// string token from the membership site
        /// </returns>
        public static string GetMembershipAuthToken(string membershipAuthSiteEndpoint, string userName, string password, CertificateValidation certificateValidation)
        {
            string token = null;

            token = GetMembershipTokenHelper(membershipAuthSiteEndpoint, userName, password, certificateValidation);

            return token;
        }

        /// <summary>
        /// Gets the auth token in an ADFS and Membership Provider scennario.
        /// </summary>
        /// <param name="adfsEndpoint">The adfs endpoint.</param>
        /// <param name="membershipAuthSiteEndpoint">The auth site endpoint.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// string token through ADFS from the Membership site
        /// </returns>
        public static string GetMembershipAdfsAuthToken(string adfsEndpoint, string membershipAuthSiteEndpoint, string userName, string password)
        {
            string token = null;
            token = GetADFSMembershipTokenHelper(adfsEndpoint, membershipAuthSiteEndpoint, userName, password);
            return token;
        }

        /// <summary>
        /// Gets the membership token.
        /// </summary>
        /// <param name="authSiteEndPoint">The auth site end point.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="certificateValidation">The certificate validation is enabled by default. users can disable this manually.</param>
        /// <returns>string token from the membership site</returns>
        private static string GetMembershipTokenHelper(string authSiteEndPoint, string userName, string password, CertificateValidation certificateValidation = CertificateValidation.Enable)
        {
            var identityProviderEndpoint = new EndpointAddress(new Uri(authSiteEndPoint + "/wstrust/issue/usernamemixed"));

            var identityProviderBinding = new WS2007HttpBinding(SecurityMode.TransportWithMessageCredential);
            identityProviderBinding.Security.Message.EstablishSecurityContext = false;
            identityProviderBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            identityProviderBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

            var trustChannelFactory = new WSTrustChannelFactory(identityProviderBinding, identityProviderEndpoint)
            {
                TrustVersion = TrustVersion.WSTrust13,
            };

            ////This line is only if we're using self-signed certs in the installation 
            if (certificateValidation == CertificateValidation.Disable)
            {
                trustChannelFactory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication() { CertificateValidationMode = X509CertificateValidationMode.None };
            }

            trustChannelFactory.Credentials.SupportInteractive = false;
            trustChannelFactory.Credentials.UserName.UserName = userName;
            trustChannelFactory.Credentials.UserName.Password = password;

            var channel = trustChannelFactory.CreateChannel();
            var rst = new RequestSecurityToken(RequestTypes.Issue)
            {
                AppliesTo = new EndpointReference("http://azureservices/TenantSite"),
                TokenType = "urn:ietf:params:oauth:token-type:jwt",
                KeyType = KeyTypes.Bearer,
            };

            RequestSecurityTokenResponse rstr = null;
            SecurityToken token = null;

            token = channel.Issue(rst, out rstr);
            var tokenString = (token as GenericXmlSecurityToken).TokenXml.InnerText;
            var jwtString = Encoding.UTF8.GetString(Convert.FromBase64String(tokenString));

            return jwtString;
        }

        private static string GetWindowsTokenHelper(string windowsAuthSiteEndPoint)
        {
            var identityProviderEndpoint = new EndpointAddress(new Uri(windowsAuthSiteEndPoint + "/wstrust/issue/windowstransport"));
            var identityProviderBinding = new WS2007HttpBinding(SecurityMode.Transport);
            identityProviderBinding.Security.Message.EstablishSecurityContext = false;
            identityProviderBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            identityProviderBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            var trustChannelFactory = new WSTrustChannelFactory(identityProviderBinding, identityProviderEndpoint)
            {
                TrustVersion = TrustVersion.WSTrust13,
            };

            trustChannelFactory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication() { CertificateValidationMode = X509CertificateValidationMode.None };
            var channel = trustChannelFactory.CreateChannel();

            var rst = new RequestSecurityToken(RequestTypes.Issue)
            {
                AppliesTo = new EndpointReference("http://azureservices/AdminSite"),
                KeyType = KeyTypes.Bearer,
            };

            RequestSecurityTokenResponse rstr = null;
            SecurityToken token = null;

            token = channel.Issue(rst, out rstr);
            var tokenString = (token as GenericXmlSecurityToken).TokenXml.InnerText;
            var jwtString = Encoding.UTF8.GetString(Convert.FromBase64String(tokenString));

            return jwtString;
        }

        private static string GetADFSTokenHelper(string adfsEndPoint, string userName, string password)
        {
            var identityProviderEndpoint = new EndpointAddress(new Uri(adfsEndPoint + "/adfs/services/trust/13/usernamemixed"));
            var identityProviderBinding = new WS2007HttpBinding(SecurityMode.TransportWithMessageCredential);
            identityProviderBinding.Security.Message.EstablishSecurityContext = false;
            identityProviderBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            identityProviderBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

            var trustChannelFactory = new WSTrustChannelFactory(identityProviderBinding, identityProviderEndpoint)
            {
                TrustVersion = TrustVersion.WSTrust13,
            };

            ////This line is only if we're using self-signed certs in the installation 
            trustChannelFactory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication() { CertificateValidationMode = X509CertificateValidationMode.None };

            trustChannelFactory.Credentials.SupportInteractive = false;
            trustChannelFactory.Credentials.UserName.UserName = userName;
            trustChannelFactory.Credentials.UserName.Password = password;

            var channel = trustChannelFactory.CreateChannel();
            var rst = new RequestSecurityToken(RequestTypes.Issue)
            {
                AppliesTo = new EndpointReference("http://azureservices/AdminSite"),
                TokenType = "urn:ietf:params:oauth:token-type:jwt",
                KeyType = KeyTypes.Bearer,
            };

            RequestSecurityTokenResponse rstr = null;
            SecurityToken token = null;

            token = channel.Issue(rst, out rstr);
            var tokenString = (token as GenericXmlSecurityToken).TokenXml.InnerText;
            var jwtString = Encoding.UTF8.GetString(Convert.FromBase64String(tokenString));

            return jwtString;
        }

        private static string GetADFSMembershipTokenHelper(string adfsEndpoint, string authSiteEndPoint, string userName, string password)
        {
            var identityProviderEndpoint = new EndpointAddress(new Uri(authSiteEndPoint + "/wstrust/issue/usernamemixed"));
            var federationEndpoint = new EndpointAddress(new Uri(adfsEndpoint + "/adfs/services/trust/13/issuedtokenmixedasymmetricbasic256sha256"));

            var identityProviderBinding = new WS2007HttpBinding(SecurityMode.TransportWithMessageCredential);
            identityProviderBinding.Security.Message.EstablishSecurityContext = false;
            identityProviderBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            identityProviderBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

            var xml = new XmlDocument();
            xml.LoadXml(@"<wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy""><wsa:EndpointReference xmlns:wsa=""http://www.w3.org/2005/08/addressing""><wsa:Address>http://kc-adfs.katalcloud.com/adfs/services/trust</wsa:Address></wsa:EndpointReference></wsp:AppliesTo>");
            var federationBinding = new WS2007FederationHttpBinding(WSFederationHttpSecurityMode.TransportWithMessageCredential);
            federationBinding.Security.Message.EstablishSecurityContext = false;
            federationBinding.Security.Message.IssuedKeyType = SecurityKeyType.AsymmetricKey;
            federationBinding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Basic256Sha256;
            federationBinding.Security.Message.NegotiateServiceCredential = false;
            federationBinding.Security.Message.TokenRequestParameters.Add(xml.DocumentElement);
            federationBinding.Security.Message.IssuerAddress = identityProviderEndpoint;
            federationBinding.Security.Message.IssuerBinding = identityProviderBinding;
            federationBinding.Security.Message.IssuedTokenType = "urn:oasis:names:tc:SAML:2.0:assertion";

            var trustChannelFactory = new WSTrustChannelFactory(federationBinding, federationEndpoint)
            {
                TrustVersion = TrustVersion.WSTrust13,
            };

            trustChannelFactory.Credentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication() { CertificateValidationMode = X509CertificateValidationMode.None };
            trustChannelFactory.Credentials.SupportInteractive = false;
            trustChannelFactory.Credentials.UserName.UserName = userName;
            trustChannelFactory.Credentials.UserName.Password = password;

            var channel = trustChannelFactory.CreateChannel();
            var rst = new RequestSecurityToken(RequestTypes.Issue)
            {
                AppliesTo = new EndpointReference("http://azureservices/TenantSite"),
                TokenType = "urn:ietf:params:oauth:token-type:jwt",
                KeyType = KeyTypes.Bearer,
            };

            RequestSecurityTokenResponse rstr = null;

            var token = channel.Issue(rst, out rstr);
            var tokenString = (token as GenericXmlSecurityToken).TokenXml.InnerText;
            var jwtString = Encoding.UTF8.GetString(Convert.FromBase64String(tokenString));

            return jwtString;
        }
    }
}
