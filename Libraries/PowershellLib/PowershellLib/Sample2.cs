using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using System.Net.Security;
using System.Collections.Generic;
using System.Management.Automation.Internal;
using System.Net.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

//*********************************************************************
//
// Weekend Scripter: Getting Started with Windows Azure and PowerShell
// http://blogs.technet.com/b/heyscriptingguy/archive/2013/06/22/weekend-scripter-getting-started-with-windows-azure-and-powershell.aspx
//
// Weekend Scripter: Remoting the Cloud with Windows Azure and PowerShell
// http://blogs.technet.com/b/heyscriptingguy/archive/2013/09/07/weekend-scripter-remoting-the-cloud-with-windows-azure-and-powershell.aspx
//
// Using Remote Powershell with Windows Azure Virtual Machines
// http://fabriccontroller.net/blog/posts/using-remote-powershell-with-windows-azure-virtual-machines/
//
// Script to automatically configuring Remote PowerShell for Windows Azure Virtual Machines on your machine
// http://fabriccontroller.net/blog/posts/automatically-configuring-remote-powershell-for-windows-azure-virtual-machines-on-your-machine/
//
//*********************************************************************

namespace PowershellLib
{
    public static class VirtualMachineRemotePowerShell
    {
        private static readonly XNamespace XmlnsWindowsAzure = "http://schemas.microsoft.com/windowsazure";
        private static readonly XNamespace XmlnsInstance = "http://www.w3.org/2001/XMLSchema-instance";

        const int PrivateRpcPortHttps = 5986;
        const int PrivateRpcPort = 5985;
        private const int PublicRpcPortHttp = 5985;

        public enum RpcPortVisibility { PublicHttps, PrivateHttps, PublicHttp, Private, None };

        //*********************************************************************
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="aadToken"></param>
        /// <param name="resourceGroupName"></param>
        /// <param name="virtualMachineName"></param>
        /// <param name="rpcVisibility"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        //*********************************************************************
        public static string GetPowerShellUrl(string subscriptionId,
            AuthenticationResult aadToken, string resourceGroupName,
            string virtualMachineName, RpcPortVisibility rpcVisibility,
            out string ipAddress)
        {
            ipAddress = null;

            var nicNameList = FetchArmVmNicIds(aadToken, subscriptionId, resourceGroupName, virtualMachineName);

            if (null == nicNameList)
                throw new Exception("VM contains no network interfaces");

            if (0 == nicNameList.Count)
                throw new Exception("VM contains no network interfaces");

            var nicAddrName = FetchArmNicAddrId(aadToken, subscriptionId, resourceGroupName, nicNameList[0]);

            if (null == nicNameList)
                throw new Exception("VM contains no network interfaces");

            ipAddress = FetchArmIpAddr(aadToken, subscriptionId, resourceGroupName, nicAddrName);
            var publicFqdn = FetchArmPublicFqdn(aadToken, subscriptionId, resourceGroupName, nicAddrName);





            //var certificate = new X509Certificate2();

            //using (var cloudServiceDescription = GetCloudService(certificate, subscriptionId, resourceGroupName))
            //{
                //if (null == cloudServiceDescription)
                 //   return null;

                string powerShellUrl = null;

                switch (rpcVisibility)
                {
                    case RpcPortVisibility.Private:

                        powerShellUrl = String.Format("http://{0}:{1}", virtualMachineName, PrivateRpcPort);

                        //ipAddress = GetPrivateIpAddress(cloudServiceDescription, virtualMachineName);
                        ipAddress = String.Format("http://{0}:{1}", ipAddress, PrivateRpcPort);

                        //var privateIpAddress = GetPrivateIpAddress(cloudServiceDescription, virtualMachineName);
                        //if (null != privateIpAddress)
                        //    powerShellUrl = String.Format("http://{0}:{1}", privateIpAddress, PrivateRpcPortHttps);

                        break;

                    case RpcPortVisibility.PrivateHttps:

                        //ipAddress = GetPrivateIpAddress(cloudServiceDescription, virtualMachineName);

                        if (null != ipAddress)
                            powerShellUrl = String.Format("https://{0}:{1}", ipAddress, PrivateRpcPortHttps);

                        ipAddress = powerShellUrl;

                        break;

                    case RpcPortVisibility.PublicHttps:
                        //var remotePowerShellPort = GetPublicPort(cloudServiceDescription, virtualMachineName,
                        //   PrivateRpcPortHttps);
                        //if (null != remotePowerShellPort)
                        //    powerShellUrl = String.Format("https://{0}.cloudapp.net:{1}", resourceGroupName,
                        //        remotePowerShellPort);

                        powerShellUrl = String.Format("https://{0}:{1}",
                            publicFqdn, PrivateRpcPortHttps);

                        break;

                    case RpcPortVisibility.PublicHttp:
                        //var remotePowerShellPort = GetPublicPort(cloudServiceDescription, virtualMachineName,
                        //   PrivateRpcPortHttps);
                        //if (null != remotePowerShellPort)
                        //    powerShellUrl = String.Format("https://{0}.cloudapp.net:{1}", resourceGroupName,
                        //        remotePowerShellPort);

                        powerShellUrl = String.Format("http://{0}:{1}",
                            ipAddress, PublicRpcPortHttp);

                        break;
                }

                //DownloadAndInstallCertificate(remotePowerShellUrl);

                //*** You can now use one of the following commands to connect to your session
                //*** Enter-PSSession -ComputerName {0}.cloudapp.net -Port {1} -Credential <myUsername> -UseSSL", cloudServiceName, remotePowerShellPort);
                //*** Invoke-Command -ConnectionUri https://{0}.cloudapp.net:{1} -Credential <myUsername> -ScriptBlock {{ dir c:\\ }}\r\n", cloudServiceName, remotePowerShellPort);

                return powerShellUrl;
            //}
        }

        private static List<string> FetchArmVmNicIds(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string vmName)
        {
            var nicListOut = new List<string>();
            try
            {
                var vmNameparts = vmName.Split(new char[] { '.' });

                var vm = FetchArmVmInfo(aadToken, subscriptionId, resourceGroupName, vmNameparts[0]);

                if (null == vm)
                    throw new Exception("Given VM: '" + vmName + "' not found");

                var jvList = FetchJsonValue(vm, "properties");

                if (null == jvList)
                    return null;

                jvList = FetchJsonValue(jvList.ToString(), "networkProfile");

                if (null == jvList)
                    return null;

                jvList = FetchJsonValue(jvList.ToString(), "networkInterfaces");

                if (null == jvList)
                    return null;

                var nicList = jvList as Newtonsoft.Json.Linq.JArray;

                if (null == nicList)
                    return null;

                foreach (var nic in nicList)
                {
                    var id = FetchJsonValue(nic.ToString(), "id");
                    if (null != id)
                    {
                        var idParts = id.ToString().Split(new char[] { '/' });
                        nicListOut.Add(idParts[idParts.Count() - 1]);
                    }
                }

                return nicListOut;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmVmNicIds() :" + ex.Message);
            }
        }

        private static string FetchArmVmInfo(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string vmName)
        {
            try
            {
                var url =
                    string.Format(@"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Compute/virtualMachines/{2}?api-version={3}",
                        subscriptionId, resourceGroupName, vmName, "2015-06-15");

                var res = PerformRequestArm(RequestType_Enum.GET, url, aadToken.AccessToken, null);

                if (!res.HadError)
                    return res.Body;

                throw new Exception(res.Body);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmVmInfo() :" + ex.Message);
            }
        }

        private static string FetchArmNicAddrId(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string nicName)
        {
            var nicListOut = new List<string>();
            try
            {
                var vm = FetchArmNicInfo(aadToken, subscriptionId, resourceGroupName, nicName);

                if (null == vm)
                    throw new Exception("Given NIC: '" + nicName + "' not found");

                var jvList = FetchJsonValue(vm, "properties");

                if (null == jvList)
                    return null;

                jvList = FetchJsonValue(jvList.ToString(), "ipConfigurations");

                var ipConfigList = jvList as Newtonsoft.Json.Linq.JArray;

                if (null == ipConfigList)
                    return null;

                if (0 == ipConfigList.Count)
                    return null;

                jvList = FetchJsonValue(ipConfigList[0].ToString(), "properties");

                if (null == jvList)
                    return null;

                jvList = FetchJsonValue(jvList.ToString(), "publicIPAddress");

                if (null == jvList)
                    return null;

                var id = FetchJsonValue(jvList.ToString(), "id");

                if (null == id)
                    return null;

                var idParts = id.ToString().Split(new char[] { '/' });
                return idParts[idParts.Count() - 1];
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmNicAddrId() :" + ex.Message);
            }
        }

        private static string FetchArmNicInfo(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string nicName)
        {
            try
            {
                var url =
                    string.Format(@"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Network/networkInterfaces/{2}?api-version={3}",
                        subscriptionId, resourceGroupName, nicName, "2015-06-15");

                var res = PerformRequestArm(RequestType_Enum.GET, url, aadToken.AccessToken, null);

                if (!res.HadError)
                    return res.Body;

                throw new Exception(res.Body);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmNicInfo() :" + ex.Message);
            }
        }

        private static string FetchArmIpAddr(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string ipAddrName)
        {
            var nicListOut = new List<string>();
            try
            {
                var vm = FetchArmIpAddrInfo(aadToken, subscriptionId, resourceGroupName, ipAddrName);

                if (null == vm)
                    throw new Exception("Given Public IP Address: '" + ipAddrName + "' not found");

                var jvList = FetchJsonValue(vm, "properties");

                if (null == jvList)
                    return null;

                jvList = FetchJsonValue(jvList.ToString(), "ipAddress");

                if (null == jvList)
                    return null;

                return jvList.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmIpAddr() :" + ex.Message);
            }
        }

        private static string FetchArmPublicFqdn(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string ipAddrName)
        {
            var nicListOut = new List<string>();
            try
            {
                var vm = FetchArmIpAddrInfo(aadToken, subscriptionId, resourceGroupName, ipAddrName);

                if (null == vm)
                    throw new Exception("Given Public IP Address: '" + ipAddrName + "' not found");

                var jvList = FetchJsonValue(vm, "properties");

                if (null == jvList)
                    return null;

                jvList = FetchJsonValue(jvList.ToString(), "dnsSettings");

                if (null == jvList)
                    return null;

                jvList = FetchJsonValue(jvList.ToString(), "fqdn");

                if (null == jvList)
                    return null;

                return jvList.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmPublicFqdn() :" + ex.Message);
            }
        }

        private static string FetchArmIpAddrInfo(AuthenticationResult aadToken,
            string subscriptionId, string resourceGroupName, string ipAddrName)
        {
            try
            {
                var url =
                    string.Format(@"https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Network/publicIPAddresses/{2}?api-version={3}",
                        subscriptionId, resourceGroupName, ipAddrName, "2015-06-15");

                var res = PerformRequestArm(RequestType_Enum.GET, url, aadToken.AccessToken, null);

                if (!res.HadError)
                    return res.Body;

                throw new Exception(res.Body);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchArmIpAddrInfo() :" + ex.Message);
            }
        }









        public enum RequestType_Enum { UNDEF, GET, POST, DELETE, PUT };

        public class HttpResponse
        {
            public bool HadError = false;
            public bool Retry = false;
            public string Operation;
            public string Body;
            public string HTTP;
            public string StatusCheckUrl;
            public long SourceSizeMBytes = 0;
            public long MBytesTransferred = 0;
            public int ElapsedTimeMinutes = 0;
        }

        public static HttpResponse PerformRequestArm(RequestType_Enum requestType,
            string requestUrl, string bearerToken, string bodyContents = null)
        {
            var responseOut = new HttpResponse();

            try
            {
                var operationsResponse = string.Empty;
                var requestUri = new Uri(requestUrl);
                var request = WebRequest.Create(requestUri) as HttpWebRequest;

                if (null == request)
                    throw new Exception("Unable to create a HttpWebRequest for : " + requestUri);

                request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + bearerToken);
                request.Method = requestType.ToString();
                request.ContentType = "application/json";
                request.Accept = "application/json";

                if (requestType == RequestType_Enum.POST | requestType == RequestType_Enum.PUT)
                {
                    var requestStream = request.GetRequestStream();
                    var streamWriter = new StreamWriter(requestStream, System.Text.Encoding.UTF8);

                    streamWriter.Write(bodyContents);

                    streamWriter.Close();
                    requestStream.Close();
                }

                var response = (HttpWebResponse)request.GetResponse();
                var responseStatus = response.StatusCode;

                if (response.ContentLength > 0)
                {
                    var responseStream = response.GetResponseStream();

                    if (null == responseStream)
                        throw new Exception("Unable to create Stream from HttpWebResponse");

                    /*var reader = new StreamReader(responseStream);
                    var xDocResp = new XmlDocument();
                    xDocResp.Load(reader);

                    if (requestUrl.Contains("/operations/"))
                    {
                        var xn = xDocResp.SelectSingleNode("/*[local-name()='Operation' and namespace-uri()='http://schemas.microsoft.com/windowsazure']/*[local-name()='Status' and namespace-uri()='http://schemas.microsoft.com/windowsazure']");

                        if (null != xn)
                        {
                            operationsResponse = xn.InnerXml;
                            responseOut.Operation = operationsResponse;
                        }
                    }

                    responseOut.Body = Util.PrettyPrintXml(xDocResp);*/

                    var reader = new StreamReader(responseStream, Encoding.ASCII);
                    responseOut.Body = reader.ReadToEnd();
                    //responseOut.Body = new StreamReader(responseStream, Encoding.Unicode).ReadToEnd();

                    var err = FetchJsonValue(responseOut.Body, "error") as Dictionary<string, dynamic>;

                    if (null != err)
                    {
                        responseOut.HadError = true;
                        responseOut.Body += string.Format("Web Error making REST API call.\r\nCode: {0}, Message: {1}",
                            err["code"], err["code"]);
                    }

                    var props = FetchJsonValue(responseOut.Body, "properties") as Newtonsoft.Json.Linq.JObject;

                    if (null != props)
                    {
                        var state = props["provisioningState"].ToString();

                        if (null != props["correlationId"])
                            responseOut.StatusCheckUrl = props["correlationId"].ToString();

                        responseOut.HadError = false;
                    }

                    reader.Close();
                    responseStream.Close();
                }
                else
                {
                    if (!requestUrl.Contains("/operations/"))
                        responseOut.Body = "No HTTP Response Body Returned.";
                }

                //*** https://management.core.windows.net/26661eda-81ca-4307-b331-698118ad668e/operations/7ee88cae5e15489789c829b28e7759f9

                /*if (!requestUrl.Contains("/operations/") && responseStatus == HttpStatusCode.Accepted)
                {
                    var responseHeaders = response.Headers;
                    var requestId = responseHeaders.Get("x-ms-request-id");
                    responseOut.StatusCheckUrl = "https://management.core.windows.net/" +
                        Connection.SubcriptionID + "/operations/" + requestId;
                    responseOut.Body = "202 - Accepted. Asynch processing. Check status at: " +
                        responseOut.StatusCheckUrl;
                }*/

                response.Close();
            }
            catch (WebException exWeb)
            {
                var responseStream = exWeb.Response.GetResponseStream();

                if (null == responseStream)
                {
                    responseOut.HadError = true;
                    responseOut.Body += "Error making REST API call.\r\n" + exWeb.Message + "\r\n";
                    return responseOut;
                }

                var reader = new StreamReader(responseStream, Encoding.ASCII);

                var responseStatus = (HttpWebResponse)exWeb.Response;
                responseOut.HTTP = (int)responseStatus.StatusCode + " - " +
                    responseStatus.StatusCode.ToString();
                responseOut.HadError = true;
                responseOut.Body += "Web Error making REST API call.\r\nMessage: " +
                    exWeb.Message + "\r\nResponse:\r\n" + reader.ReadToEnd();

                responseStream.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                responseOut.HadError = true;
                responseOut.Body += "Error making REST API call.\r\n" + ex.Message + "\r\n";
            }

            return responseOut;
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="subscriptionId"></param>
        ///  <param name="certificate"></param>
        /// <param name="aadToken"></param>
        /// <param name="cloudServiceName"></param>
        ///  <param name="virtualMachineName"></param>
        /// <param name="rpcVisibility"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************
        public static string GetPowerShellUrl(string subscriptionId, 
            X509Certificate2 certificate, AuthenticationResult aadToken,
            string cloudServiceName, string virtualMachineName, 
            RpcPortVisibility rpcVisibility, out string ipAddress)
        {
            ipAddress = null;

            //*** ARM *****************************************************

            if (null != aadToken)
                return GetPowerShellUrl(subscriptionId, aadToken, cloudServiceName,
                    virtualMachineName, rpcVisibility,out ipAddress);

            //*** ARM *****************************************************

            using (var cloudServiceDescription = GetCloudService(
                certificate, subscriptionId, cloudServiceName))
            {
                if (null == cloudServiceDescription)
                    return null;

                string powerShellUrl = null;

                switch(rpcVisibility)
                {
                    case RpcPortVisibility.Private:

                        powerShellUrl = String.Format("http://{0}:{1}", virtualMachineName, PrivateRpcPort);

                        ipAddress = GetPrivateIpAddress(cloudServiceDescription, virtualMachineName);
                        ipAddress = String.Format("http://{0}:{1}", ipAddress, PrivateRpcPort);

                        //var privateIpAddress = GetPrivateIpAddress(cloudServiceDescription, virtualMachineName);
                        //if (null != privateIpAddress)
                        //    powerShellUrl = String.Format("http://{0}:{1}", privateIpAddress, PrivateRpcPortHttps);

                        break;

                    case RpcPortVisibility.PrivateHttps:

                        ipAddress = GetPrivateIpAddress(cloudServiceDescription, virtualMachineName);

                        if (null != ipAddress)
                            powerShellUrl = String.Format("https://{0}:{1}", ipAddress, PrivateRpcPortHttps);

                        ipAddress = powerShellUrl;

                        break;

                    case RpcPortVisibility.PublicHttps:
                        var remotePowerShellPort = GetPublicPort(cloudServiceDescription, virtualMachineName, PrivateRpcPortHttps);
                        if (null != remotePowerShellPort)
                            powerShellUrl = String.Format("https://{0}.cloudapp.net:{1}", cloudServiceName, remotePowerShellPort);
                    break;
                }

                //DownloadAndInstallCertificate(remotePowerShellUrl);

                //*** You can now use one of the following commands to connect to your session
                //*** Enter-PSSession -ComputerName {0}.cloudapp.net -Port {1} -Credential <myUsername> -UseSSL", cloudServiceName, remotePowerShellPort);
                //*** Invoke-Command -ConnectionUri https://{0}.cloudapp.net:{1} -Credential <myUsername> -ScriptBlock {{ dir c:\\ }}\r\n", cloudServiceName, remotePowerShellPort);

                return powerShellUrl;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Enable remote PowerShell by finding the public port for Remote 
        /// PowerShell, downloading the HTTPS certificate and installing it 
        /// locally in the trusted root.
        /// </summary>
        /// <param name="publishSettingsPath"></param>
        /// <param name="subscriptionName"></param>
        /// <param name="cloudServiceName"></param>
        /// <param name="virtualMachineName"></param>
        /// 
        //*********************************************************************

        public static void Enable(string publishSettingsPath, string subscriptionName, 
            string cloudServiceName, string virtualMachineName)
        {
            Console.WriteLine("\r\n  Enabling Remote PowerShell on {0} for {1} (in {2}.cloudapp.net)\r\n", Environment.MachineName, virtualMachineName, cloudServiceName);

            var certificate = GetCertificateFromPublishProfile(publishSettingsPath);
            var subscriptionId = GetSubscriptionId(publishSettingsPath, subscriptionName);
            using (var cloudServiceDescription = GetCloudService(certificate, subscriptionId, cloudServiceName))
            {
                if (null == cloudServiceDescription)
                {
                    Console.WriteLine("Not Found");
                    return;
                }

                var remotePowerShellPort = GetPublicPort(cloudServiceDescription, virtualMachineName, 5986);
                Console.WriteLine("   > Found remote port: " + remotePowerShellPort);

                var remotePowerShellUrl = String.Format("https://{0}.cloudapp.net:{1}", cloudServiceName, remotePowerShellPort);
                Console.WriteLine("   > Fetching certificate from: {0}", remotePowerShellUrl);

                DownloadAndInstallCertificate(remotePowerShellUrl);

                Console.WriteLine("\r\n  You can now use one of the following commands to connect to your session\r\n");
                Console.WriteLine("     Enter-PSSession -ComputerName {0}.cloudapp.net -Port {1} -Credential <myUsername> -UseSSL", cloudServiceName, remotePowerShellPort);
                Console.WriteLine("     Invoke-Command -ConnectionUri https://{0}.cloudapp.net:{1} -Credential <myUsername> -ScriptBlock {{ dir c:\\ }}\r\n", cloudServiceName, remotePowerShellPort);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Parse the publish profile to get the certificate.
        /// </summary>
        /// <param name="publishProfilePath"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static X509Certificate2 GetCertificateFromPublishProfile(string publishProfilePath)
        {
            Console.WriteLine("   > Loading certificate from publish profile: {0}", publishProfilePath);

            //return new X509Certificate2(Convert.FromBase64String(
            //    XDocument.Load(publishProfilePath).Descendants("PublishProfile").Single().Attribute("ManagementCertificate").Value));

            var CertB64 = "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";
            return new X509Certificate2(Convert.FromBase64String(CertB64));
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Get the subscription ID.
        /// </summary>
        /// <param name="publishProfilePath"></param>
        /// <param name="subscriptionName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string GetSubscriptionId(string publishProfilePath, string subscriptionName)
        {
            Console.WriteLine("   > Loading subscription ID from publish profile: {0}", publishProfilePath);

            var subscription = XDocument.Load(publishProfilePath).Descendants("PublishProfile").Single().Elements("Subscription").Where(s => s.HasAttributes && s.Attribute("Name").Value == subscriptionName).SingleOrDefault();
            if (subscription == null)
                throw new InvalidOperationException("Unable to find subscription: " + subscriptionName);
            return subscription.Attribute("Id").Value;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Get the description of the Cloud Service from the management API.
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="subscriptionId"></param>
        /// <param name="cloudServiceName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static Stream GetCloudService(X509Certificate2 certificate, 
            string subscriptionId, string cloudServiceName)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(
                    String.Format("https://management.core.windows.net/{0}/services/hostedservices/{1}/deploymentslots/{2}",
                    subscriptionId, cloudServiceName, "Production"));

                request.Headers["x-ms-version"] = "2014-05-01";
                request.ClientCertificates.Add(certificate);

                var response = request.GetResponse();

                var stream = response.GetResponseStream();

                return stream;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return null;
            }
        }

        /*<InputEndpoints>
- <InputEndpoint>
  <LocalPort>3389</LocalPort> 
  <Name>RDP</Name> 
  <Port>59306</Port> 
  <Protocol>tcp</Protocol> 
  <Vip>157.56.165.202</Vip> 
  </InputEndpoint>
- <InputEndpoint>
  <LocalPort>5986</LocalPort> 
  <Name>WinRmHTTPs</Name> 
  <Port>59235</Port> 
  <Protocol>tcp</Protocol> 
  <Vip>157.56.165.202</Vip> 
  </InputEndpoint>
  </InputEndpoints>*/

        private static string GetPrivateIpAddress(Stream cloudServiceXml, string virtualMachineName)
        {
            using (var response = cloudServiceXml)
            {
                string VmName = null;

                var Index = virtualMachineName.IndexOf(".");

                if (-1 == Index)
                    VmName = virtualMachineName;
                else
                    VmName = virtualMachineName.Substring(0, Index);

                var document = XDocument.Load(response);

                // Get the role instance
                var roleInstance = document.Root.GetElement("RoleInstanceList").GetElements("RoleInstance")
                    .Where(r => r.GetElement("InstanceName").Value == VmName)
                    .SingleOrDefault();
                if (roleInstance == null)
                    throw new InvalidOperationException("Unable to find Virtual Machine: " + virtualMachineName);

                // Get the private IP address.
                var IpAddress = GetElement(roleInstance, "IpAddress").Value;
                return IpAddress;
            }
        }

        private static string GetShortName(string longName)
        {
            var index = longName.IndexOf('.');

            if (0 > index)
                return longName;

            return longName.Remove(index);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cloudServiceXml"></param>
        /// <param name="virtualMachine"></param>
        /// <param name="internalPort"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string GetPublicPort(Stream cloudServiceXml,
            string virtualMachine, int internalPort)
        {
            using (var response = cloudServiceXml)
            {
                try
                {
                    var document = XDocument.Load(response);

                    //string IE = GetXmlInnerText(document.Root.ToString(), "InputEndpoints");

                    //var Role = document.Root.GetElement("RoleList").GetElements("Role").SingleOrDefault(); ;
                    //var ConfigSets = Role.GetElement("ConfigurationSets").GetElements("ConfigurationSet").SingleOrDefault();

                    //*****************************

                    var roleList = document.Root.GetElement("RoleList").GetElements("Role")
                        .Where(r => r.GetElement("RoleName") != null && r.IsOfType("PersistentVMRole"));

                    if (roleList == null)
                        throw new InvalidOperationException("Unable to find any virtual machines, NULL");

                    if (roleList.Count() == 0)
                        throw new InvalidOperationException("Unable to find any virtual machines, 0");

                    foreach (XElement role in roleList)
                    {
                        if (role == null)
                            throw new InvalidOperationException("Unable to find Virtual Machine: " + virtualMachine);

                        var roleName = role.GetElement("RoleName");

                        if (!roleName.Value.Contains(GetShortName(virtualMachine)))
                            continue;

                        // Get the network configuration.
                        var networkConfigurationSet =
                            role.GetElement("ConfigurationSets")
                                .GetElements("ConfigurationSet")
                                .Where(c => c.IsOfType("NetworkConfigurationSet"))
                                .SingleOrDefault();
                        if (networkConfigurationSet == null)
                            throw new InvalidOperationException(
                                "Could not find NetworkConfigurationSet for Virtual Machine: " + virtualMachine);

                        // Exit if there are no public endpoints
                        var inputEndpoints = networkConfigurationSet.GetElement("InputEndpoints");

                        if (null == inputEndpoints)
                            return null;

                        // Get the endpoints.
                        /*var endpoint = networkConfigurationSet.GetElement("InputEndpoints").GetElements("InputEndpoint")
                        .Where(e => GetElement(e, "LocalPort") != null && GetElement(e, "LocalPort").Value == internalPort.ToString()).SingleOrDefault();
                        if (endpoint == null)
                        throw new InvalidOperationException("Could not find the a public endpoint matching the internal port " + internalPort + " for Virtual Machine: " + virtualMachine);*/

                        var endpoint = inputEndpoints.GetElements("InputEndpoint")
                            .Where(
                                e =>
                                    GetElement(e, "LocalPort") != null &&
                                    GetElement(e, "LocalPort").Value == internalPort.ToString()).SingleOrDefault();

                        if (endpoint == null)
                            return null;

                        var portElement = GetElement(endpoint, "Port");

                        if (portElement == null)
                            return null;

                        // Get the remote port.
                        var remotePort = portElement.Value;
                        return remotePort;
                    }

                    throw new InvalidOperationException("Virtual Machine: '" + virtualMachine + "' not found in config");
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception in VirtualMachineRemotePowerShell.GetPublicPort() : " + UnwindExceptionMessages(ex));
                }
            }
        }

        private static string GetPublicPortOld(Stream cloudServiceXml,
            string virtualMachine, int internalPort)
        {
            using (var response = cloudServiceXml)
            {
                try
                {
                    var document = XDocument.Load(response);

                    //string IE = GetXmlInnerText(document.Root.ToString(), "InputEndpoints");

                    //var Role = document.Root.GetElement("RoleList").GetElements("Role").SingleOrDefault(); ;
                    //var ConfigSets = Role.GetElement("ConfigurationSets").GetElements("ConfigurationSet").SingleOrDefault();

                    //*****************************

                    var roleList = document.Root.GetElement("RoleList").GetElements("Role")
                        .Where(r => r.GetElement("RoleName") != null && r.IsOfType("PersistentVMRole"));

                    //*****************************

                    // Get the role.
                    var role = document.Root.GetElement("RoleList").GetElements("Role")
                        .Where(r => r.GetElement("RoleName") != null && r.IsOfType("PersistentVMRole"))
                        .SingleOrDefault();
                    if (role == null)
                        throw new InvalidOperationException("Unable to find Virtual Machine: " + virtualMachine);

                    Console.WriteLine("   > Found Virtual Machine: {0}", virtualMachine);

                    // Get the network configuration.
                    var networkConfigurationSet =
                        role.GetElement("ConfigurationSets")
                            .GetElements("ConfigurationSet")
                            .Where(c => c.IsOfType("NetworkConfigurationSet"))
                            .SingleOrDefault();
                    if (networkConfigurationSet == null)
                        throw new InvalidOperationException(
                            "Could not find NetworkConfigurationSet for Virtual Machine: " + virtualMachine);

                    // Exit if there are no public endpoints
                    var inputEndpoints = networkConfigurationSet.GetElement("InputEndpoints");

                    if (null == inputEndpoints)
                        return null;

                    // Get the endpoints.
                    /*var endpoint = networkConfigurationSet.GetElement("InputEndpoints").GetElements("InputEndpoint")
                    .Where(e => GetElement(e, "LocalPort") != null && GetElement(e, "LocalPort").Value == internalPort.ToString()).SingleOrDefault();
                    if (endpoint == null)
                    throw new InvalidOperationException("Could not find the a public endpoint matching the internal port " + internalPort + " for Virtual Machine: " + virtualMachine);*/

                    var endpoint = inputEndpoints.GetElements("InputEndpoint")
                        .Where(
                            e =>
                                GetElement(e, "LocalPort") != null &&
                                GetElement(e, "LocalPort").Value == internalPort.ToString()).SingleOrDefault();

                    if (endpoint == null)
                        return null;

                    var portElement = GetElement(endpoint, "Port");

                    if (portElement == null)
                        return null;

                    // Get the remote port.
                    var remotePort = portElement.Value;
                    return remotePort;
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception in VirtualMachineRemotePowerShell.GetPublicPort() : " + UnwindExceptionMessages(ex));
                }
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Get the public port for a specific 
        /// </summary>
        /// <param name="cloudServiceXml"></param>
        /// <param name="virtualMachine"></param>
        /// <param name="internalPort"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static string GetPublicPortOldOld(Stream cloudServiceXml, 
            string virtualMachine, int internalPort)
        {
            using (var response = cloudServiceXml)
            {
                var document = XDocument.Load(response);

                // Get the role.
                var role = document.Root.GetElement("RoleList").GetElements("Role")
                    .Where(r => r.GetElement("RoleName") != null && r.GetElement("RoleName").Value == virtualMachine && r.IsOfType("PersistentVMRole"))
                    .SingleOrDefault();
                if (role == null)
                    throw new InvalidOperationException("Unable to find Virtual Machine: " + virtualMachine);

                Console.WriteLine("   > Found Virtual Machine: {0}", virtualMachine);

                // Get the network configuration.
                var networkConfigurationSet = role.GetElement("ConfigurationSets").GetElements("ConfigurationSet").Where(c => c.IsOfType("NetworkConfigurationSet"))
                    .SingleOrDefault();
                if (networkConfigurationSet == null)
                    throw new InvalidOperationException("Could not find NetworkConfigurationSet for Virtual Machine: " + virtualMachine);

                // Get the endpoints.
                var endpoint = networkConfigurationSet.GetElement("InputEndpoints").GetElements("InputEndpoint")
                    .Where(e => GetElement(e, "LocalPort") != null && GetElement(e, "LocalPort").Value == internalPort.ToString()).SingleOrDefault();
                if (endpoint == null)
                    throw new InvalidOperationException("Could not find the a public endpoint matching the internal port " + internalPort + " for Virtual Machine: " + virtualMachine);

                // Get the remote port.
                var remotePort = GetElement(endpoint, "Port").Value;
                return remotePort;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Install the certificate.
        /// </summary>
        /// <param name="url"></param>
        /// 
        //*********************************************************************

        private static void DownloadAndInstallCertificate(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback += OnServerCertificateValidationCallback;

            // Build the request and initialize the response to get the certificate.
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = null;

            try
            {
                // This will return a 404, which is normal.
                response = request.GetResponse() as HttpWebResponse;
            }
            catch
            {

            }

            var file = Path.GetTempFileName();

            if (null == request.ServicePoint.Certificate)
                throw new InvalidOperationException("!!! No Certificate found at ServicePoint !!!");

            // Download the certificate.
            var certificate = request.ServicePoint.Certificate.Export(X509ContentType.Cert);
            File.WriteAllBytes(file, certificate);

            Console.WriteLine("   > Downloaded certificate: {0}", request.ServicePoint.Certificate.Subject);

            // Install the certificate.
            //InstallCertificateInTrustedRoot(file);
     InstallCertificateInTrustedRoot(new X509Certificate2(request.ServicePoint.Certificate));

            // Clean up the file.
            //File.Delete(file);

            // Reset the certificate validation.
            ServicePointManager.ServerCertificateValidationCallback -= OnServerCertificateValidationCallback;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Install the certificate in the trusted root store.
        /// </summary>
        /// <param name="filePath"></param>
        /// 
        //*********************************************************************

        //private static void InstallCertificateInTrustedRoot(string filePath)
        private static void InstallCertificateInTrustedRoot(X509Certificate2 cert)
        {
            // Install it.
            //var rootStore = new X509Store(StoreName.CertificateAuthority, StoreLocation.LocalMachine);
            var rootStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser);

            try
            {
                rootStore.Open(OpenFlags.ReadWrite);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VirtualMachineRemotePowerShell.InstallCertificateInTrustedRoot() : Unable to open store : " + UnwindExceptionMessages(ex));
            }

            try
            {
                //rootStore.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(filePath)));
                rootStore.Add(cert);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in VirtualMachineRemotePowerShell.InstallCertificateInTrustedRoot() : Unable add certificate to store : " + UnwindExceptionMessages(ex));
            }

            finally
            {
                rootStore.Close();
            }

            Console.WriteLine("   > Certificate has been imported, You can now connect from this machine!");
        }

        //*********************************************************************
        ///
        /// <summary>
        /// The certificate is self signed, so skip the validation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="policyErrors"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static bool OnServerCertificateValidationCallback(object sender, 
            X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Helper method to get an element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static XElement GetElement(this XElement element, string name)
        {
            return element.Element(XmlnsWindowsAzure + name);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Helper method to get a list of elements.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static IEnumerable<XElement> GetElements(this XElement element, string name)
        {
            return element.Elements(XmlnsWindowsAzure + name);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Check the type of a specific element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static bool IsOfType(this XElement element, string type)
        {
            return element.HasAttributes && element.Attributes().Any(a => a.Name == XmlnsInstance + "type" && a.Value == type);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// Unwinds exception messages
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns>The unwound messages</returns>
        /// 
        //*********************************************************************

        public static string UnwindExceptionMessages(Exception ex)
        {
            var Message = ex.Message;

            if (null != ex.InnerException)
            {
                ex = ex.InnerException;
                Message += " - " + ex.Message;

                if (null != ex.InnerException)
                {
                    ex = ex.InnerException;
                    Message += " - " + ex.Message;
                }
            }

            return Message;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        static public string GetXmlInnerText(string body, string tag)
        {
            string Out;

            try
            {
                var Index = body.IndexOf("<" + tag + ">");

                if (-1 == Index)
                    return null;

                Out = body.Substring(Index + tag.Length + 2);
                Index = Out.IndexOf("</" + tag + ">");
                Out = Out.Substring(0, Index);
            }
            catch (Exception)
            {
                return null;
            }

            return Out;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonIn"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private static object FetchJsonValue(string jsonIn, string keyName)
        {
            try
            {
                var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonIn);

                if (null == keyName)
                    return jsonResult;

                return !jsonResult.ContainsKey(keyName) ? null : jsonResult[keyName];
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchJsonValue() : "
                    + UnwindExceptionMessages(ex));
            }
        }
    }
}



