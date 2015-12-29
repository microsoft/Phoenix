using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.Xml;
using System.Net;  // For HttpWebRequest and HttpWebResponse classes.
using System.Security.Cryptography.X509Certificates;  // For certificate-related classes.
using System.Security.Cryptography;
using System.Globalization; // For Stream classes.
using CmpInterfaceModel;

namespace AzureAdminClientLib
{
    public enum virtualNetworkFilter
    {
        Region = 1,
        HostedService = 2
    }

    //*************************************************************************
    //
    // Note: The bulk of this class was generated via the Paste Special menu option, hence the private
    // backing fields vs. auto implemented fields. Not as pretty but it was free. There is a bit of inconsistency
    // here because the response returned from Azure didn't include all of the possible elements as defined
    // by the schema in the reference below so the missing ones were added by hand and consequently used
    // auto implemented fields.
    // Also, not all fields that have a Max value have a current value. At the time of this writing the 
    // schema was defined as such. Reference: http://msdn.microsoft.com/en-us/library/windowsazure/hh403995.aspx
    //
    //*************************************************************************

    //*** NOTE * Compute
    //*** NOTE * Network
    //*** NOTE * Refresh

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/windowsazure", IsNullable = false)]
    public class AzureSubscription
    {
        //const string AzureServiceManagementVersionString = "2013-06-01";
        const string AzureServiceManagementVersionString = "2014-05-01";

        private XNamespace AzureNameSpace = "http://schemas.microsoft.com/windowsazure";

        private string subscriptionIDField;

        private string subscriptionNameField;

        private string subscriptionStatusField;

        private string accountAdminLiveEmailIdField;

        private string serviceAdminLiveEmailIdField;

        private uint maxCoreCountField;

        private uint maxStorageAccountsField;

        private uint maxHostedServicesField;

        private uint currentCoreCountField;

        private uint currentHostedServicesField;

        private uint currentStorageAccountsField;

        // Internal instance variables.
        private X509Certificate2 managementCertificate;

        bool _vnetsMustBeActive = true; //*** vnets must be active by default, we can override this with 'VnetsMustBeActive = false' in the config file

        public AzureSubscription()
        {
            string vnetsMustBeActive_t = Microsoft.Azure.CloudConfigurationManager.GetSetting("VnetsMustBeActive");

            if (null != vnetsMustBeActive_t)
                if (vnetsMustBeActive_t.ToLower().Contains("f"))
                    _vnetsMustBeActive = false;
        }


        /// <summary>
        /// Attempts to populate the AzureSubScription class. This class is a hybrid between the static class returned
        /// from the Azure service management API directly and other sub objects pulled into this class such as the 
        /// virtual networks associated with the subscription and the virtual machines associated with each virtual network.
        /// </summary>
        /// <param name="subscriptionId">This is the GUID assigned by Azure and displayed on the Manage Subscriptions page.</param>
        /// <param name="managementCertificateThumbPrint">This is the thumbprint for one of the management certificates associated with the subscription. The certificate
        /// that this thumbprint references needs to be in your local store or the Azure PAAS store WITH the private key available.</param>
        public void Load(string subscriptionId, string managementCertificateThumbPrint)
        {
            Load(subscriptionId, managementCertificateThumbPrint, null, 1);
        }


        /// <summary>
        /// This method loads only the values that are directly obtainable from the AzureSubscription XML
        /// vs. the properties that are members of XML objects that are children of the subscription object.
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="managementCertificateThumbPrint"></param>
        public void LoadOnlyStaticValues(string subscriptionId, string managementCertificateThumbPrint)
        {

            // Get the static subscription information

            subscriptionIDField = subscriptionId;
            managementCertificate = GetCertificate(managementCertificateThumbPrint);

            // Create the request.
            var requestUri = new Uri("https://management.core.windows.net/" + subscriptionId);

            var httpWebRequest = WebRequest.Create(requestUri) as HttpWebRequest;

            if(null == httpWebRequest)
                throw new Exception("Unable to create httpWebRequest for : " + requestUri);

            // Add the certificate to the request.
            httpWebRequest.ClientCertificates.Add(managementCertificate);

            // Specify the version information in the header.
            httpWebRequest.Headers.Add("x-ms-version", AzureServiceManagementVersionString);

            // Make the call using the web request.
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            if (null == httpWebResponse)
                throw new Exception("httpWebResponse == null");

            var xmlDoc = new XmlDocument();

            xmlDoc.Load(httpWebResponse.GetResponseStream());

            PopulateClassFromXMLDocument(xmlDoc);

            // Close the resources no longer needed.
            httpWebResponse.Close();
        }

        /// <summary>
        /// Attempts to populate the AzureSubScription class. This class is a hybrid between the static class returned
        /// from the Azure service management API directly and other sub objects pulled into this class such as the 
        /// virtual networks associated with the subscription and the virtual machines associated with each virtual network.
        /// </summary>
        /// <param name="SubscriptionID">This is the GUID assigned by Azure and displayed on the Manage Subscriptions page.</param>
        /// <param name="managementCertificateThumbPrint">This is the thumbprint for one of the management certificates associated with the subscription. The certificate
        /// that this thumbprint references needs to be in your local store or the Azure PAAS store WITH the private key available.</param>
        /// <param name="filter">Supplying a value for region will have the affect of filtering the list of virtual networks that are in the spedified region.</param>
        /// <param name="filterType">Indicates if the filter is a region or cloud service.</param>
        public void Load(string SubscriptionID, string managementCertificateThumbPrint, string filter, int filterType)
        {
            // In order to fully populate our hybrid subscription class, it is necessary to query many of the REST endpoints.
            // The flow is as follows:
            // Get the static subscription information
            // Get the list of virtual networks
            // Get the list of cloud services (a.k.a. hosted services)
            // For each cloud service get the staging and production deployments which details the virtual machines and the 
            // virtual network that they are connected to. With this information tie the VMs to the virtual networks identified above.
            
            // Get the static subscription information

            subscriptionIDField = SubscriptionID;
            managementCertificate = GetCertificate(managementCertificateThumbPrint);

            // Create the request.
            var requestUri = new Uri("https://management.core.windows.net/" + SubscriptionID);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);

            // Add the certificate to the request.
            httpWebRequest.ClientCertificates.Add(managementCertificate);

            // Specify the version information in the header.
            httpWebRequest.Headers.Add("x-ms-version", AzureServiceManagementVersionString);

            // Make the call using the web request.
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            var xmlDoc = new XmlDocument();

            xmlDoc.Load(httpWebResponse.GetResponseStream());

            PopulateClassFromXMLDocument(xmlDoc);

            // Close the resources no longer needed.
            if (httpWebResponse != null) httpWebResponse.Close();

            // Load the dynamic parts of the subscription that are not part of the static subscription XML from Azure.

            // If a region was passed in, populate the AffinityGroups that belong to that region so filtering can be 
            // done when populating the virtual networks in GetVirtualNetworks.
            if (!string.IsNullOrEmpty(filter))
            {
                switch (filterType)
                {
                    case (int)virtualNetworkFilter.Region:
                        AffinityGroups = GetAffinityGroupsForRegion(filter);
                        break;

                    case (int)virtualNetworkFilter.HostedService:
                        AffinityGroups = GetAffinityGroupsForHostedService(filter);
                        break;
                }
            }            
            
            // Get the list of virtual networks
            VirtualNetworks = GetVirtualNetworks();

            // Get the list of cloud services (a.k.a. hosted services)
            VirtualMachines = GetVirtualMachines();

            // Populate the VirtualNetwork.VMCount attribute now that we have the VMs
            foreach (var aVirtualNetwork in VirtualNetworks)
            {
                foreach (var aVirtualMachine in VirtualMachines)
                {
                    if (string.Compare(aVirtualNetwork.Name, aVirtualMachine.VirtualNetworkName, true) == 0)
                    {
                        aVirtualNetwork.VirtualMachineCount++;
                    }
                }
            }

            StorageAccounts = GetStorageAccounts();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SubscriptionID"></param>
        /// <param name="ManagementCertificateThumbPrint"></param>
        /// 
        //*********************************************************************

        public void LoadStorageAccounts(string SubscriptionID, string ManagementCertificateThumbPrint)
        {
            // In order to fully populate our hybrid subscription class, it is necessary to query many of the REST endpoints.
            // The flow is as follows:
            // Get the static subscription information
            // Get the list of virtual networks
            // Get the list of cloud services (a.k.a. hosted services)
            // For each cloud service get the staging and production deployments which details the virtual machines and the 
            // virtual network that they are connected to. With this information tie the VMs to the virtual networks identified above.

            // Get the static subscription information

            subscriptionIDField = SubscriptionID;
            managementCertificate = GetCertificate(ManagementCertificateThumbPrint);

            // Create the request.
            var requestUri = new Uri("https://management.core.windows.net/" + SubscriptionID);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);

            // Add the certificate to the request.
            httpWebRequest.ClientCertificates.Add(managementCertificate);

            // Specify the version information in the header.
            httpWebRequest.Headers.Add("x-ms-version", AzureServiceManagementVersionString);

            // Make the call using the web request.
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            var xmlDoc = new XmlDocument();

            xmlDoc.Load(httpWebResponse.GetResponseStream());

            PopulateClassFromXMLDocument(xmlDoc);

            // Close the resources no longer needed.
            if (httpWebResponse != null) httpWebResponse.Close();

            // Load the dynamic parts of the subscription that are not part of the static subscription XML from Azure.
            StorageAccounts = GetStorageAccounts();
        }

        public AzureStorageAccount GetStorageAccount(string SubscriptionID, string ManagementCertificateThumbPrint, string url)
        {
            // In order to fully populate our hybrid subscription class, it is necessary to query many of the REST endpoints.
            // The flow is as follows:
            // Get the static subscription information
            // Get the list of virtual networks
            // Get the list of cloud services (a.k.a. hosted services)
            // For each cloud service get the staging and production deployments which details the virtual machines and the 
            // virtual network that they are connected to. With this information tie the VMs to the virtual networks identified above.

            // Get the static subscription information

            subscriptionIDField = SubscriptionID;
            managementCertificate = GetCertificate(ManagementCertificateThumbPrint);

            // Create the request.
            var requestUri = new Uri("https://management.core.windows.net/" + SubscriptionID);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);

            // Add the certificate to the request.
            httpWebRequest.ClientCertificates.Add(managementCertificate);

            // Specify the version information in the header.
            httpWebRequest.Headers.Add("x-ms-version", AzureServiceManagementVersionString);

            // Make the call using the web request.
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            var xmlDoc = new XmlDocument();

            xmlDoc.Load(httpWebResponse.GetResponseStream());

            PopulateClassFromXMLDocument(xmlDoc);

            // Close the resources no longer needed.
            if (httpWebResponse != null) httpWebResponse.Close();

            // Load the dynamic parts of the subscription that are not part of the static subscription XML from Azure.
            var saList = GetStorageAccounts();

            foreach (var sa in saList)
                foreach(var cont in sa.Containers)
                    if (cont.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase))
                        return sa;

            return null;
        }

        private List<string> GetAffinityGroupsForHostedService(string Filter)
        {
            // Need to get the Location or Affinity Group for the cloud service passed in.

            var hostedServicesDoc = QueryAzureServiceManagementWebService("/services/hostedservices");

            var hostedServicesFound = from item in hostedServicesDoc.Descendants(AzureNameSpace + "HostedService")
                                      where item.Element(AzureNameSpace + "ServiceName").Value == Filter
                                      select new
                                      {
                                          ServiceName = item.Element(AzureNameSpace + "ServiceName"),
                                          AffinityGroup = item.Element(AzureNameSpace + "HostedServiceProperties").Element(AzureNameSpace + "AffinityGroup"),
                                          Location = item.Element(AzureNameSpace + "HostedServiceProperties").Element(AzureNameSpace + "Location")
                                      };

            var affinityGroups = new List<string>();

            foreach (var hostedService in hostedServicesFound)
            {
                if (hostedService.AffinityGroup != null)
                {
                    affinityGroups.Add(hostedService.AffinityGroup.Value);
                }

                if (hostedService.Location != null)
                {
                    affinityGroups = GetAffinityGroupsForRegion(hostedService.Location.Value);                    
                }   
            }

            return affinityGroups;

        }

        /// <summary>
        /// Gets the affinity groups for the specified region.
        /// </summary>
        /// <param name="Region"></param>
        /// <returns></returns>
        private List<string> GetAffinityGroupsForRegion(string Region)
        {
            var affinityGroups = new List<string>();

            var affinityGroupDoc = QueryAzureServiceManagementWebService("/affinitygroups");

            var affinityGroupsFound = from item in affinityGroupDoc.Descendants(AzureNameSpace + "AffinityGroup")
                                      select new
                                      {
                                          affinityGroupName = item.Element(AzureNameSpace + "Name"),
                                          region = item.Element(AzureNameSpace + "Location")
                                      };

            foreach (var affinityGroup in affinityGroupsFound)
            {
                if (string.Compare(affinityGroup.region.Value, Region, true) == 0)
                {
                    affinityGroups.Add(affinityGroup.affinityGroupName.Value);
                }
            }

            return affinityGroups;
        }
        
        private static int GetVNetsInLocation_VisitCount = 0;
        
        private List<AzureVirtualMachine> GetVirtualMachines()
        {
                    GetVNetsInLocation_VisitCount++;

            // Need to get the cloud services and for each, get the deployments per deployment slot.
            var virtualMachines = new List<AzureVirtualMachine>();

            var hostedServicesDoc = QueryAzureServiceManagementWebService("/services/hostedservices");

            var hostedServicesFound = from item in hostedServicesDoc.Descendants(AzureNameSpace + "HostedService")
                                      select new
                                      {
                                          ServiceName = item.Element(AzureNameSpace + "ServiceName")
                                      };

            foreach (var hostedService in hostedServicesFound)
            {
                try
                {
                    var productionVirtualMachines = GetVirtualMachines(hostedService.ServiceName.Value, "production");

                    foreach (var oneVirtualMachine in productionVirtualMachines)
                    {
                        virtualMachines.Add(oneVirtualMachine);
                    }
                }
                catch (Exception ex)
                {
                    if(!ex.Message.Contains("404"))
                        throw;
                }

                try
                {
                    var stagingVirtualMachines = GetVirtualMachines(hostedService.ServiceName.Value, "staging");

                    foreach (var oneVirtualMachine in stagingVirtualMachines)
                    {
                        virtualMachines.Add(oneVirtualMachine);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("404"))
                        throw;
                }

            }

            return virtualMachines;
        }


        private List<AzureVirtualMachine> GetVirtualMachines(string HostedService, string DeploymentSlot)
        {
            // Get the VMs for this deployment slot
            var virtualNetworkName = "NotDefined"; // Per the deployment schema, there can only be one virtual network name per deployment...
            XDocument deploymentDoc;
            var azureVirtualMachines = new List<AzureVirtualMachine>();

            try
            {
                deploymentDoc = QueryAzureServiceManagementWebService("/services/hostedservices/" + HostedService + "/deploymentslots/" + DeploymentSlot);

                var virtualNetworksFound = from item in deploymentDoc.Descendants(AzureNameSpace + "Deployment")
                                      select new
                                      {
                                          virtualNetwork = item.Element(AzureNameSpace + "VirtualNetworkName")
                                      };

                foreach (var oneVirtualNetwork in virtualNetworksFound)
                {
                    if (oneVirtualNetwork.virtualNetwork != null)
                    {
                        virtualNetworkName = oneVirtualNetwork.virtualNetwork.Value;
                    }                    
                }


                var virtualMachinesFound = from item in deploymentDoc.Descendants(AzureNameSpace + "RoleInstance")
                            select new
                            {
                                RoleName = item.Element(AzureNameSpace + "RoleName"),
                                InstanceName = item.Element(AzureNameSpace + "InstanceName"),
                                HostName = item.Element(AzureNameSpace + "HostName"),
                                InstanceStatus = item.Element(AzureNameSpace + "InstanceStatus")
                            };

                foreach (var virtualMachine in virtualMachinesFound)
                {
                    //*** TODO: Mark West : Added the checks because HostName was null and threw an exception in the new below
                    var roleNameValue = "";
                    var instanceNameValue = "";
                    var hostNameValue = "";
                    var instanceStatusValue = "";

                    if (null != virtualMachine.RoleName)
                        roleNameValue = virtualMachine.RoleName.Value;

                    if (null != virtualMachine.InstanceName)
                        instanceNameValue = virtualMachine.InstanceName.Value;

                    if (null != virtualMachine.HostName)
                        hostNameValue = virtualMachine.HostName.Value;

                    if (null != virtualMachine.InstanceStatus)
                        instanceStatusValue = virtualMachine.InstanceStatus.Value;

                    var newVirtualMachine = new AzureVirtualMachine(roleNameValue,
                                                                                    instanceNameValue,
                                                                                    hostNameValue,
                                                                                    instanceStatusValue,
                                                                                    DeploymentSlot,
                                                                                    virtualNetworkName);
                    azureVirtualMachines.Add(newVirtualMachine);
                }

            }
            catch (WebException e)
            {
                if (e.HResult == -2146233079)
                {
                    // There aren't any deployments so just move on.
                }
                else
                {
                    throw;
                }
            }


            return azureVirtualMachines;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="managementCertificateThumbPrint"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<AzureVirtualNetwork> GetVirtualNetworks(string subscriptionId,
            string managementCertificateThumbPrint)
        {
            subscriptionIDField = subscriptionId;
            managementCertificate = GetCertificate(managementCertificateThumbPrint);

            return GetVirtualNetworks();
        }

        public List<AzureVirtualMachine> GetAllVirtualMachines(string subscriptionId,
            string managementCertificateThumbPrint)
        {
            subscriptionIDField = subscriptionId;
            managementCertificate = GetCertificate(managementCertificateThumbPrint);

            return GetVirtualMachines();
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private List<AzureVirtualNetwork> GetVirtualNetworks()
        {
            XDocument virtualNetworkDoc;

            try
            {
                virtualNetworkDoc = QueryAzureServiceManagementWebService("/services/networking/virtualnetwork");
            }
            catch (Exception)
            {                
                throw;
            }

            var virtualNetworksFound = from item in virtualNetworkDoc.Descendants(AzureNameSpace + "VirtualNetworkSite")
                                  select new
                                  {
                                      Name = item.Element(AzureNameSpace + "Name"),
                                      InUse = item.Element(AzureNameSpace + "InUse"),
                                      AffinityGroup = item.Element(AzureNameSpace + "AffinityGroup"),
                                      Location = item.Element(AzureNameSpace + "Location"),
                                      //Subnets = item.Descendants(this.AzureNameSpace + "Subnet").Elements(this.AzureNameSpace + "Name")
                                      Subnets = item.Descendants(AzureNameSpace + "Subnet")
                                  };

            var azureVirtualNetworks = new List<AzureVirtualNetwork>();

            foreach (var virtualNetwork in virtualNetworksFound)
            {
                // If there is not a value for InUse, set it to False.
                // Changed to only return InUse virtual networks since I couldn't figure out how to filter them out of the CompareTo method.
                if ((!_vnetsMustBeActive) | (virtualNetwork.InUse != null && bool.Parse(virtualNetwork.InUse.Value) == true))
                {
                    var subnets = new List<string>();
                    var possibleAddresses = 0;

                    foreach (var subnet in virtualNetwork.Subnets)
                    {
                        subnets.Add(subnet.Element(AzureNameSpace + "Name").Value);

                        // Don't add the address counts for the GatewaySubnet as VMs can't actually be put in this subnet.
                        if (string.Compare(subnet.Element(AzureNameSpace + "Name").Value, "GatewaySubnet", true) != 0)
                        {

                            var subnetMask = 0;

                            // If the value in the AddressPrefix isn't parsable as an int it will probably throw an exception. Just ignore it and consider the 
                            // available address range to be zero.
                            try 
	                        {
                                subnetMask = Int32.Parse(subnet.Element(AzureNameSpace + "AddressPrefix").Value.Substring(subnet.Element(AzureNameSpace + "AddressPrefix").Value.IndexOf('/') + 1));		
	                        }
	                        catch (Exception)
	                        {
	                        }

                            // Realistic numbers for subnet mask will be between 25 and 29. If out of this range, don't do the calculation, just return zero addresses.
                            if (subnetMask > 24 && subnetMask < 30)
                            {
                                possibleAddresses += (int) ((Math.Pow(2, (32 - subnetMask))) - 5);
                                // The 5 above is kind of a magic number that accounts for addresses reserved by Azure + two general networking addresses.
                            }
                        }
                    }

                    AzureVirtualNetwork newVirtualNetwork = null;

                    if (null == virtualNetwork.AffinityGroup)
                        newVirtualNetwork = new AzureVirtualNetwork(virtualNetwork.Name.Value, true, null, null, subnets, possibleAddresses);
                    else
                        newVirtualNetwork = new AzureVirtualNetwork(virtualNetwork.Name.Value, true, virtualNetwork.AffinityGroup.Value, null, subnets, possibleAddresses);

                    if (null != virtualNetwork.Location)
                        newVirtualNetwork.Location = virtualNetwork.Location.Value;

                    // 12-17-2013 Added restriction to filter out virtual networks if a region was specified.
                    if (AffinityGroups == null)
                    {
                        azureVirtualNetworks.Add(newVirtualNetwork);
                    }
                    else if (AffinityGroups.Contains(virtualNetwork.AffinityGroup.Value))
                    {
                        azureVirtualNetworks.Add(newVirtualNetwork);   
                    }
                }
            }

            return azureVirtualNetworks;

        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private List<AzureStorageAccount> GetStorageAccounts()
        {
            var azureStorageAccounts = new List<AzureStorageAccount>();

            var storageAccountDoc = QueryAzureServiceManagementWebService("/services/storageservices");

            var storageAccountsFound = from item in storageAccountDoc.Descendants(AzureNameSpace + "StorageService")
                                       select new
                                       {
                                           Name = item.Element(AzureNameSpace + "ServiceName"),
                                           StorageServiceProperties = item.Element(AzureNameSpace + "StorageServiceProperties"),
                                           Status = item.Descendants(AzureNameSpace + "StorageServiceProperties").Elements(AzureNameSpace + "Status").First()
                                       };

            foreach (var storageAccount in storageAccountsFound)
            {
                string affinityGroup = null;
                string location = null;

                if (storageAccount.StorageServiceProperties.Element(AzureNameSpace + "AffinityGroup") != null)
                {
                    affinityGroup = storageAccount.StorageServiceProperties.Element(AzureNameSpace + "AffinityGroup").Value;
                }

                if (storageAccount.StorageServiceProperties.Element(AzureNameSpace + "Location") != null)
                {
                    location = storageAccount.StorageServiceProperties.Element(AzureNameSpace + "Location").Value;
                }

                // Get the Storage Access Keys
                string primaryAccessKey = null;
                string secondaryAccessKey = null;

                var retryCount = 1;

                while (true)
                {
                    try
                    {
                        var accessKeys = GetStorageAccountKeys(storageAccount.Name.Value);
                        primaryAccessKey = accessKeys[0];
                        secondaryAccessKey = accessKeys[1];
                        break;
                    }
                    catch (Exception ex)
                    {
                            throw new Exception("Exception in GetStorageAccounts() : " +
                                Utilities.UnwindExceptionMessages(ex));
                    }
                }

                // Get the containers
                List<AzureStorageAccountContainer> containers = null;

                var newStorageAccount = 
                    new AzureStorageAccount(storageAccount.Name.Value, affinityGroup, 
                        location, primaryAccessKey, secondaryAccessKey, containers);

                azureStorageAccounts.Add(newStorageAccount);
            }

            return azureStorageAccounts;
        }

        private static string QueryAccountKeyLock = ".";

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageServiceName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private List<string> GetStorageAccountKeys(string storageServiceName)
        {
            try
            {
                var azureStorageAccountKeys = new List<string>();
                XDocument storageAccountKeysDoc = null;
                var index = 0;

                while (true)
                {
                    try
                    {
                        //lock (QueryAccountKeyLock)
                        //
                            storageAccountKeysDoc =
                                QueryAzureServiceManagementWebService(
                                    "/services/storageservices/" + storageServiceName + "/keys");
                        //}

                        break;
                    }
                    catch (Exception)
                    {
                        if(index++ > 2)
                            throw;
                    }
                }

                var storageAccountKeysFound =
                    from item in storageAccountKeysDoc.Descendants(AzureNameSpace + "StorageServiceKeys")
                    select new
                    {
                        Primary = item.Element(AzureNameSpace + "Primary"),
                        Secondary = item.Element(AzureNameSpace + "Secondary")
                    };

                foreach (var storageAccountKey in storageAccountKeysFound)
                {
                    azureStorageAccountKeys.Add(storageAccountKey.Primary.Value);
                    azureStorageAccountKeys.Add(storageAccountKey.Secondary.Value);
                }

                return azureStorageAccountKeys;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetStorageAccountKeys(" + storageServiceName + 
                    ") : " + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canonicalizedString"></param>
        /// <param name="storageAccountName"></param>
        /// <param name="storageAccountAccessKey"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static String CreateAuthorizationHeader(string canonicalizedString, 
            string storageAccountName, string storageAccountAccessKey)
        {
            string signature;

            using (var hmacSha256 = new HMACSHA256(Convert.FromBase64String(storageAccountAccessKey)))
            {
                var dataToHmac = System.Text.Encoding.UTF8.GetBytes(canonicalizedString);
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
            }

            var authorizationHeader = String.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}:{2}",
                "SharedKey", //AzureStorageConstants.SharedKeyAuthorizationScheme,
                storageAccountName, //AzureStorageConstants.Account,
                signature
            );

            return authorizationHeader;
        }

        /// ********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        ///  <param name="storageAccountName"></param>
        ///  <param name="storageAccountAccessKey"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        ///  
        //*********************************************************************

        public static List<AzureStorageAccountContainer> GetAzureStorageAccountContainers(
            string storageAccountName, string storageAccountAccessKey, string containerName)
        {
            var containers = new List<AzureStorageAccountContainer>();

            const string requestMethod = "GET";

            var urlPath = String.Format("?comp=list");
            var modifiedUrlPath = String.Format("\ncomp:list");

            const string storageServiceVersion = "2009-09-19"; // Oh so important this value is... I've found others listed online but don't work.
            var dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            var canonicalizedHeaders = String.Format(
                    "x-ms-date:{0}\nx-ms-version:{1}",
                    dateInRfc1123Format,
                    storageServiceVersion);
            var canonicalizedResource = String.Format("/{0}/{1}", storageAccountName, modifiedUrlPath);
            var stringToSign = String.Format(
                    "{0}\n\n\n\n\n\n\n\n\n\n\n\n{1}\n{2}",
                    requestMethod,
                    canonicalizedHeaders,
                    canonicalizedResource);
            var authorizationHeader = CreateAuthorizationHeader(
                stringToSign, storageAccountName, storageAccountAccessKey);

            var uri = new Uri("http://" + storageAccountName + ".blob.core.windows.net/" + urlPath);
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = requestMethod;
            request.Headers.Add("x-ms-date", dateInRfc1123Format);
            request.Headers.Add("x-ms-version", storageServiceVersion);
            request.Headers.Add("Authorization", authorizationHeader);


            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                var xdoc = XDocument.Load(response.GetResponseStream());

                var containersFound = from item in xdoc.Descendants("Container")
                                      select new
                                      {
                                          Name = item.Element("Name"),
                                          Url = item.Element("Url")
                                      };

                foreach (var oneContainer in containersFound)
                {
                    if(null != containerName)
                        if (!containerName.Equals(oneContainer.Name.Value, StringComparison.InvariantCultureIgnoreCase))
                            continue;

                    //*** Fetch number of objects in container ***
                    var numberOfObjects = 0;

                    var newContainer = new AzureStorageAccountContainer(
                        oneContainer.Name.Value, oneContainer.Url.Value, numberOfObjects, null);

                    containers.Add(newContainer);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            return containers;
        }

        private X509Certificate2 GetCertificate(string ManagementCertificateThumbPrint)
        {
            // X.509 certificate variables.
            X509Store certStore = null;
            X509Certificate2Collection certCollection = null;

            // Open the certificate store for the current user.
            //certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadOnly);

            // Find the certificate with the specified thumbprint.
            certCollection = certStore.Certificates.Find(
                                 X509FindType.FindByThumbprint,
                                 ManagementCertificateThumbPrint,
                                 false);

            // Close the certificate store.
            certStore.Close();

            // Check to see if a matching certificate was found.
            if (0 == certCollection.Count)
            {
                throw new Exception("No certificate found containing thumbprint " + ManagementCertificateThumbPrint);
            }

            // A matching certificate was found.
            return(certCollection[0]);
        }

        private void PopulateClassFromXMLDocument(XmlDocument subscriptionDocument)
        {
            var subscriptionElement = subscriptionDocument.DocumentElement;

            subscriptionIDField = GetNodeValueOrEmptyString(subscriptionElement["SubscriptionId"]);
            subscriptionIDField = (subscriptionIDField == string.Empty) ? GetNodeValueOrEmptyString(subscriptionElement["SubscriptionID"]) : subscriptionIDField;
            subscriptionNameField = GetNodeValueOrEmptyString(subscriptionElement["SubscriptionName"]);
            subscriptionStatusField = GetNodeValueOrEmptyString(subscriptionElement["SubscriptionStatus"]);
            accountAdminLiveEmailIdField = GetNodeValueOrEmptyString(subscriptionElement["AccountAdminLiveEmailId"]);
            serviceAdminLiveEmailIdField = GetNodeValueOrEmptyString(subscriptionElement["ServiceAdminLiveEmailId"]);
            maxCoreCountField = GetNodeValueOrZero(subscriptionElement["MaxCoreCount"]);
            maxStorageAccountsField = GetNodeValueOrZero(subscriptionElement["MaxStorageAccounts"]);
            maxHostedServicesField = GetNodeValueOrZero(subscriptionElement["MaxHostedServices"]);
            currentCoreCountField = GetNodeValueOrZero(subscriptionElement["CurrentCoreCount"]);
            currentStorageAccountsField = GetNodeValueOrZero(subscriptionElement["CurrentStorageAccounts"]);
            currentHostedServicesField = GetNodeValueOrZero(subscriptionElement["CurrentHostedServices"]);
            // Properties below were added by hand cuz weren't in the response object returned in the "x-ms-version", "2011-10-01"
            MaxVirtualNetworkSites = GetNodeValueOrZero(subscriptionElement["MaxVirtualNetworkSites"]);
            CurrentVirtualNetworkSites = GetNodeValueOrZero(subscriptionElement["CurrentVirtualNetworkSites"]);
            MaxLocalNetworkSites = GetNodeValueOrZero(subscriptionElement["MaxLocalNetworkSites"]);
            MaxDnsServers = GetNodeValueOrZero(subscriptionElement["MaxDnsServers"]);
            // Properties below were added by hand cuz were in the response object returned in the "x-ms-version", "2012-06-01"
            // <OfferCategories>Azure_Platform_All;Azure_Paid;Azure_Consumption;Azure_Internal;Azure_MS-AZR-0015P</OfferCategories> 
            OfferCategories = GetNodeValueOrEmptyString(subscriptionElement["OfferCategories"]);
            // Properties below were added by hand cuz weren't in the response object returned in the "x-ms-version", "2012-06-01" but were in online documentation
            AADTenantID = GetNodeValueOrEmptyString(subscriptionElement["AADTenantID"]);
            CreatedTime = GetNodeValueOrEmptyString(subscriptionElement["CreatedTime"]);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceManagementOperation"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        private XDocument QueryAzureServiceManagementWebService(string serviceManagementOperation)
        {
            var retryCount = 1;

            while (true)
            {
                try
                {
                    // Request and response variables.
                    HttpWebRequest httpWebRequest = null;
                    HttpWebResponse httpWebResponse = null;

                    // URI variable.
                    Uri requestUri = null;

                    // Create the request.
                    requestUri =
                        new Uri("https://management.core.windows.net/" + subscriptionIDField +
                                serviceManagementOperation);

                    httpWebRequest = (HttpWebRequest) WebRequest.Create(requestUri);

                    // Add the certificate to the request.
                    httpWebRequest.ClientCertificates.Add(managementCertificate);

                    // Specify the version information in the header.
                    // TODO: Move the version string into a constant
                    httpWebRequest.Headers.Add("x-ms-version", AzureServiceManagementVersionString);

                    // Make the call using the web request.
                    httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                    var xDoc = XDocument.Load(httpWebResponse.GetResponseStream());

                    if (httpWebResponse != null) httpWebResponse.Close();

                    return xDoc;
                }
                catch (Exception ex)
                {
                    if ((ex.Message.Contains("503") | ex.Message.Contains("timed out")) & (5 > retryCount++))
                        Thread.Sleep((int)(1000 * Math.Pow(4.17, retryCount)));
                    else
                        throw new Exception("Exception in QueryAzureServiceManagementWebService() : " +
                            Utilities.UnwindExceptionMessages(ex));
                }
            }
        }



        /// <remarks/>
        public string SubscriptionID
        {
            get
            {
                return subscriptionIDField;
            }
        }

        /// <remarks/>
        public string SubscriptionName
        {
            get
            {
                return subscriptionNameField;
            }
        }

        /// <remarks/>
        public string SubscriptionStatus
        {
            get
            {
                return subscriptionStatusField;
            }
        }

        /// <remarks/>
        public string AccountAdminLiveEmailId
        {
            get
            {
                return accountAdminLiveEmailIdField;
            }
        }

        /// <remarks/>
        public string ServiceAdminLiveEmailId
        {
            get
            {
                return serviceAdminLiveEmailIdField;
            }
        }

        /// <remarks/>
        public uint MaxCoreCount
        {
            get
            {
                return maxCoreCountField;
            }
        }

        /// <remarks/>
        public uint MaxStorageAccounts
        {
            get
            {
                return maxStorageAccountsField;
            }
        }

        /// <remarks/>
        public uint MaxHostedServices
        {
            get
            {
                return maxHostedServicesField;
            }
        }

        /// <remarks/>
        public uint CurrentCoreCount
        {
            get
            {
                return currentCoreCountField;
            }
        }

        /// <remarks/>
        public uint CurrentHostedServices
        {
            get
            {
                return currentHostedServicesField;
            }
        }

        /// <remarks/>
        public uint CurrentStorageAccounts
        {
            get
            {
                return currentStorageAccountsField;
            }
        }

        public uint MaxVirtualNetworkSites { get; protected set; }

        public uint CurrentVirtualNetworkSites { get; protected set; }

        public uint MaxLocalNetworkSites { get; protected set; }

        public uint MaxDnsServers { get; protected set; }

        public string OfferCategories { get; protected set; }

        public string AADTenantID { get; protected set; }

        public string CreatedTime { get; protected set; }

        public List<AzureVirtualNetwork> VirtualNetworks { get; protected set; }

        public List<AzureVirtualMachine> VirtualMachines { get; protected set; }

        public List<AzureStorageAccount> StorageAccounts { get; protected set; }

        public List<string> AffinityGroups { get; protected set; }

        private static string GetNodeValueOrEmptyString(XmlNode possiblyNullNode)
        {
            return (possiblyNullNode != null) ? possiblyNullNode.InnerText : string.Empty;
        }
        private static uint GetNodeValueOrZero(XmlNode possiblyNullNode)
        {
            uint returnValue = 0;

            if (possiblyNullNode != null)
            {
                uint result;

                if (uint.TryParse(possiblyNullNode.InnerText, out result))
                {
                    returnValue = uint.Parse(possiblyNullNode.InnerText);
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Walks the storage containers associated with the subscription and tries to find one that matches the affinity group.
        /// If there are multiple, it will favor one named 'vhds'.
        /// 
        /// In the future it might of interest to randomize the container returned or add logic to identify the least used.
        /// </summary>
        /// <param name="AffinityGroup"></param>
        /// <returns>A container that matches the name filter(s) passed in.</returns>
        public AzureStorageAccountContainer GetStorageAccountContainer(string AffinityGroup)
        {
            AzureStorageAccountContainer theBestMatchingAzureStorageAccountContainer = null;

            foreach (var oneStorageAccount in StorageAccounts)
            {
                if (string.Compare(oneStorageAccount.AffinityGroup, AffinityGroup, true) == 0)
                {
                    foreach (var oneContainer in oneStorageAccount.Containers)
                    {
                        theBestMatchingAzureStorageAccountContainer = oneContainer;

                        // If we happen to find the 'vhds' container, don't both looking further.
                        if (string.Compare(oneContainer.Name, "vhds", true) == 0)
                        {
                            break;
                        }
                    }
                }
            }

            return theBestMatchingAzureStorageAccountContainer;
        }

    }
}
