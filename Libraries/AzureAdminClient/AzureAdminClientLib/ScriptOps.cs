using System;
using System.Xml.Linq;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading;
using System.Net;
using System.IO;

namespace AzureAdminClientLib
{
    //*************************************************************************
    ///
    /// <summary>
    /// https://msdn.microsoft.com/en-us/library/azure/dn781373.aspx
    /// </summary>
    /// 
    //*************************************************************************

    class ScriptOps
    {
        private static readonly XNamespace wa = "http://schemas.microsoft.com/windowsazure";
        private const string SubscriptionId = "<identifier-of-subscription";
        private const string Version = "2014-06-01";
        private const string ServiceName = "<name-of-cloud-service>";
        private const string DeploymentName = "<name-of-deployment>";
        private const string RoleName = "<name-of-role>";
        private const string StorageName = "<name-of-storage-account>";
        private const string StorageKey = "<storage-account-key>";
        private const string URI = "<uri-of-script-in-storage>";
        private const string ScriptCommand = "<name-of-script>";
        private const string TenantId = "<identifier-of-tenant>";
        private const string Audience = "https://management.core.windows.net/";
        private const string ClientId = "<identifier-of-client>";
        private const string RedirectUri = "<redirect-uri>";

        static void Test()
        {
            var token = GetAToken();
            var account = "{\"storageAccountName\":\"" + StorageName + 
                "\",\"storageAccountKey\": \"" + StorageKey + "\"}";
            var scriptfile = "{\"fileUris\": [\"" + URI + 
                "\"], \"commandToExecute\":\"powershell -ExecutionPolicy Unrestricted -file " + ScriptCommand + "\"}";
            var uriFormat = "{0}/{1}/services/hostedservices/{2}/deployments/{3}/roles/{4}";
            var uri = new Uri(String.Format(uriFormat, Audience, SubscriptionId, ServiceName, DeploymentName, RoleName));

            var bytes = System.Text.Encoding.UTF8.GetBytes(account);
            var scriptPrivateConfig = Convert.ToBase64String(bytes);
            bytes = System.Text.Encoding.UTF8.GetBytes(scriptfile);
            var scriptPublicConfig = Convert.ToBase64String(bytes);

            var requestBody = new XDocument(
             new XDeclaration("1.0", "UTF-8", "no"),
             new XElement(wa + "PersistentVMRole",
               new XElement(wa + "ConfigurationSets",
                 new XElement(wa + "ConfigurationSet",
                   new XElement(wa + "ConfigurationSetType", "NetworkConfiguration"),
                   new XElement(wa + "InputEndpoints",
                     new XElement(wa + "InputEndpoint",
                       new XElement(wa + "LocalPort", "3389"),
                       new XElement(wa + "Name", "Remote Desktop"),
                       new XElement(wa + "Port", "55447"),
                       new XElement(wa + "Protocol", "TCP"))))),
               new XElement(wa + "ResourceExtensionReferences",
                 new XElement(wa + "ResourceExtensionReference",
                   new XElement(wa + "ReferenceName", "MyCustomScriptExtension"),
                   new XElement(wa + "Publisher", "Microsoft.Compute"),
                   new XElement(wa + "Name", "CustomScriptExtension"),
                   new XElement(wa + "Version", "1.0"),
                   new XElement(wa + "ResourceExtensionParameterValues",
                     new XElement(wa + "ResourceExtensionParameterValue",
                       new XElement(wa + "Key", "CustomScriptExtensionPublicConfigParameter"),
                       new XElement(wa + "Value", scriptPublicConfig),
                       new XElement(wa + "Type", "Public")),
                     new XElement(wa + "ResourceExtensionParameterValue",
                       new XElement(wa + "Key", "CustomScriptExtensionPrivateConfigParameter"),
                       new XElement(wa + "Value", scriptPrivateConfig),
                       new XElement(wa + "Type", "Private")))))));

            //XDocument responseBody;
            String responseBody;
            var response = InvokeRequest(uri, "PUT", token, requestBody, out responseBody);

            var statusCode = response.StatusCode;
            Console.WriteLine("The status of the operation: {0}", statusCode.ToString());

            if (responseBody != null)
            {
                //Console.WriteLine(responseBody.ToString(SaveOptions.OmitDuplicateNamespaces));
                Console.WriteLine(responseBody);
            }
            Console.Write("Press any key to continue:");
            Console.ReadKey();
        }

        public static string GetAToken()
        {
            return null;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="token"></param>
        /// <param name="requestBody"></param>
        /// <param name="responseBody"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static HttpWebResponse InvokeRequest(
            Uri uri,
            string method,
            string token,
            XDocument requestBody,
            out string responseBody)
        {
            var request = WebRequest.Create(uri) as HttpWebRequest;

            if(null == request)
                throw new Exception("Unable to create HttpWebRequest for : " + uri);

            request.Method = method;
            request.Headers.Add("x-ms-version", Version);
            request.ContentType = "application/xml";

            if(null != token)
                request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);

            responseBody = null;
            HttpWebResponse response;
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(requestBody);
            }
 
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                responseBody = streamReader.ReadToEnd();
            }

            response.Close();

            return response;
        }
    }
}
