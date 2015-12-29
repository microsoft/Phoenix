using System;
using System.Collections.Generic;
using CmpInterfaceModel;

namespace AzureAdminClientLib
{
    //*************************************************************************
    //
    //*************************************************************************

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/windowsazure", IsNullable = false)]
    public partial class VirtualNetworkSites
    {

        private VirtualNetworkSitesVirtualNetworkSite virtualNetworkSiteField;

        private string[] textField;

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSite VirtualNetworkSite
        {
            get
            {
                return this.virtualNetworkSiteField;
            }
            set
            {
                this.virtualNetworkSiteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSite
    {

        private string nameField;

        private string idField;

        private string affinityGroupField;

        private string stateField;

        private bool inUseField;

        private VirtualNetworkSitesVirtualNetworkSiteAddressSpace addressSpaceField;

        private VirtualNetworkSitesVirtualNetworkSiteSubnets subnetsField;

        private VirtualNetworkSitesVirtualNetworkSiteDns dnsField;

        private VirtualNetworkSitesVirtualNetworkSiteGateway gatewayField;

        private string[] textField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string AffinityGroup
        {
            get
            {
                return this.affinityGroupField;
            }
            set
            {
                this.affinityGroupField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public bool InUse
        {
            get
            {
                return this.inUseField;
            }
            set
            {
                this.inUseField = value;
            }
        }

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSiteAddressSpace AddressSpace
        {
            get
            {
                return this.addressSpaceField;
            }
            set
            {
                this.addressSpaceField = value;
            }
        }

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSiteSubnets Subnets
        {
            get
            {
                return this.subnetsField;
            }
            set
            {
                this.subnetsField = value;
            }
        }

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSiteDns Dns
        {
            get
            {
                return this.dnsField;
            }
            set
            {
                this.dnsField = value;
            }
        }

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSiteGateway Gateway
        {
            get
            {
                return this.gatewayField;
            }
            set
            {
                this.gatewayField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteAddressSpace
    {

        private string[] addressPrefixesField;

        private string[] textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("AddressPrefix", IsNullable = false)]
        public string[] AddressPrefixes
        {
            get
            {
                return this.addressPrefixesField;
            }
            set
            {
                this.addressPrefixesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteSubnets
    {

        private VirtualNetworkSitesVirtualNetworkSiteSubnetsSubnet[] subnetField;

        private string[] textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Subnet")]
        public VirtualNetworkSitesVirtualNetworkSiteSubnetsSubnet[] Subnet
        {
            get
            {
                return this.subnetField;
            }
            set
            {
                this.subnetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteSubnetsSubnet
    {

        private string nameField;

        private string addressPrefixField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string AddressPrefix
        {
            get
            {
                return this.addressPrefixField;
            }
            set
            {
                this.addressPrefixField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteDns
    {

        private VirtualNetworkSitesVirtualNetworkSiteDnsDnsServers dnsServersField;

        private string[] textField;

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSiteDnsDnsServers DnsServers
        {
            get
            {
                return this.dnsServersField;
            }
            set
            {
                this.dnsServersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteDnsDnsServers
    {

        private VirtualNetworkSitesVirtualNetworkSiteDnsDnsServersDnsServer[] dnsServerField;

        private string[] textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DnsServer")]
        public VirtualNetworkSitesVirtualNetworkSiteDnsDnsServersDnsServer[] DnsServer
        {
            get
            {
                return this.dnsServerField;
            }
            set
            {
                this.dnsServerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteDnsDnsServersDnsServer
    {

        private string nameField;

        private string addressField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteGateway
    {

        private string profileField;

        private VirtualNetworkSitesVirtualNetworkSiteGatewaySites sitesField;

        private VirtualNetworkSitesVirtualNetworkSiteGatewayVPNClientAddressPool vPNClientAddressPoolField;

        private object vnetNamesField;

        private string[] textField;

        /// <remarks/>
        public string Profile
        {
            get
            {
                return this.profileField;
            }
            set
            {
                this.profileField = value;
            }
        }

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSiteGatewaySites Sites
        {
            get
            {
                return this.sitesField;
            }
            set
            {
                this.sitesField = value;
            }
        }

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSiteGatewayVPNClientAddressPool VPNClientAddressPool
        {
            get
            {
                return this.vPNClientAddressPoolField;
            }
            set
            {
                this.vPNClientAddressPoolField = value;
            }
        }

        /// <remarks/>
        public object VnetNames
        {
            get
            {
                return this.vnetNamesField;
            }
            set
            {
                this.vnetNamesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteGatewaySites
    {

        private VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSite localNetworkSiteField;

        private string[] textField;

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSite LocalNetworkSite
        {
            get
            {
                return this.localNetworkSiteField;
            }
            set
            {
                this.localNetworkSiteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSite
    {

        private string nameField;

        private VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSiteAddressSpace addressSpaceField;

        private string vpnGatewayAddressField;

        private VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSiteConnections connectionsField;

        private string[] textField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSiteAddressSpace AddressSpace
        {
            get
            {
                return this.addressSpaceField;
            }
            set
            {
                this.addressSpaceField = value;
            }
        }

        /// <remarks/>
        public string VpnGatewayAddress
        {
            get
            {
                return this.vpnGatewayAddressField;
            }
            set
            {
                this.vpnGatewayAddressField = value;
            }
        }

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSiteConnections Connections
        {
            get
            {
                return this.connectionsField;
            }
            set
            {
                this.connectionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSiteAddressSpace
    {

        private string[] addressPrefixesField;

        private string[] textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("AddressPrefix", IsNullable = false)]
        public string[] AddressPrefixes
        {
            get
            {
                return this.addressPrefixesField;
            }
            set
            {
                this.addressPrefixesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSiteConnections
    {

        private VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSiteConnectionsConnection connectionField;

        private string[] textField;

        /// <remarks/>
        public VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSiteConnectionsConnection Connection
        {
            get
            {
                return this.connectionField;
            }
            set
            {
                this.connectionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteGatewaySitesLocalNetworkSiteConnectionsConnection
    {

        private string typeField;

        /// <remarks/>
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class VirtualNetworkSitesVirtualNetworkSiteGatewayVPNClientAddressPool
    {

        private object addressPrefixesField;

        /// <remarks/>
        public object AddressPrefixes
        {
            get
            {
                return this.addressPrefixesField;
            }
            set
            {
                this.addressPrefixesField = value;
            }
        }
    }

    //*************************************************************************
    //
    //*************************************************************************

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/windowsazure", IsNullable = false)]
    public partial class AddressAvailabilityResponse
    {

        private bool isAvailableField;

        private string[] availableAddressesField;

        /// <remarks/>
        public bool IsAvailable
        {
            get
            {
                return this.isAvailableField;
            }
            set
            {
                this.isAvailableField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("AvailableAddress", IsNullable = false)]
        public string[] AvailableAddresses
        {
            get
            {
                return this.availableAddressesField;
            }
            set
            {
                this.availableAddressesField = value;
            }
        }
    }

    //*************************************************************************
    //
    //*************************************************************************

    public class VnetOps
    {
        const string URLTEMPLATE_GETVNETCONFIG =
            "https://management.core.windows.net/{0}/services/networking/virtualnetwork";

        const string URLTEMPLATE_GETAVAILIPADDRS =
            "https://management.core.windows.net/{0}/services/networking/{1}?op=checkavailability&address={2}";

        const string URLTEMPLATE_GETVNETLISTARM =
            "https://management.azure.com/subscriptions/{0}/providers/Microsoft.Network/virtualnetworks?api-version={1}";

        const string URLTEMPLATE_GETVNETCONFIGARM =
            "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Network/virtualnetworks/{2}?api-version={3}";
        
        private const string ARM_API_VERSION = "2015-06-15";

        private IConnection Connection { get; set; }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// 
        //*********************************************************************

        public VnetOps(IConnection connection)
        {
            Connection = connection;
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vNetName"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public VirtualNetworkSites FetchVnetInfo(string vNetName)
        {
            try
            {
                var url = string.Format(URLTEMPLATE_GETVNETCONFIG, 
                    Connection.SubcriptionID);
                var hi = new HttpInterface(Connection);
                var resp = hi.PerformRequest(HttpInterface.RequestType_Enum.GET, url);

                var vNets = Utilities.DeSerialize(typeof(VirtualNetworkSites), 
                    resp.Body, true) as VirtualNetworkSites;

                if (null == vNets)
                    throw new Exception("Bad or missing VNet config returned from Azure");

                return vNets;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetVnetInfo() :" + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        /// 
        ///  <summary>
        ///  
        ///  </summary>
        /// <param name="resGroupName"></param>
        /// <param name="vNetName"></param>
        ///  <returns></returns>
        ///  
        //*********************************************************************
        public HttpResponse FetchVnetInfoArm(string resGroupName, string vNetName)
        {
            try
            {
                var url = string.Format(URLTEMPLATE_GETVNETCONFIGARM,
                    Connection.SubcriptionID, resGroupName, vNetName, ARM_API_VERSION);

                var hi = new HttpInterface(Connection);

                var ret = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url, null);

                return ret.Body.Contains("(404) Not Found") ? null : ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVnetInfoArm() :" +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public HttpResponse FetchVnetListArm()
        {
            try
            {
                var url = string.Format(URLTEMPLATE_GETVNETLISTARM,
                    Connection.SubcriptionID, ARM_API_VERSION);

                var hi = new HttpInterface(Connection);

                var ret = hi.PerformRequestArm(HttpInterface.RequestType_Enum.GET, url, null);

                return ret.Body.Contains("(404) Not Found") ? null : ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in FetchVnetListArm() :" +
                    Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public string GetAddrFromPrefix(string prefix, int offset)
        {
            var prefixParts = prefix.Split('/');
            var addressParts = prefixParts[0].Split('.');
            var octet = Convert.ToInt32(addressParts[3]);
            addressParts[3] = (octet + offset).ToString();

            return string.Format("{0}.{1}.{2}.{3}", 
                addressParts[0], addressParts[1], addressParts[2], addressParts[3] );
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vNetName"></param>
        /// <param name="subNetName"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public List<string> GetSubnetAddrs(string vNetName, string subNetName, int offset)
        {
            try
            {
                var addrList = new List<string>();
                var vNets = FetchVnetInfo(vNetName);

                if (null == vNets.VirtualNetworkSite)
                    throw new Exception("No VNet found on subscription");

                if (null == vNets.VirtualNetworkSite.Subnets)
                    throw new Exception("No subnets found on subscription");

                if (null == vNets.VirtualNetworkSite.Subnets.Subnet)
                    throw new Exception("No subnet found in subnets on subscription");

                foreach (var subNet in vNets.VirtualNetworkSite.Subnets.Subnet)
                {
                    if (null == subNetName)
                        addrList.Add(GetAddrFromPrefix(subNet.AddressPrefix, offset));
                    else if (subNetName.Equals(subNet.Name))
                        addrList.Add(GetAddrFromPrefix(subNet.AddressPrefix, offset));
                }

                return addrList;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetSubnetAddrs() :" + Utilities.UnwindExceptionMessages(ex));
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// http://msdn.microsoft.com/en-us/library/azure/dn592118.aspx
        /// 
        //*********************************************************************

        public List<string> GetAvailAddrList(string vNetName, string subnetName, string testAddr)
        {
            try
            {
                var subnetAddrs = GetSubnetAddrs(vNetName, subnetName, 0);

                if (0 == subnetAddrs.Count)
                    if (null == subnetName)
                        throw new Exception(string.Format("No subnets found on vnet '{0}'", vNetName));
                    else
                        throw new Exception(string.Format("Requested subnet '{0}' not found on vnet '{1}'",
                            subnetName, vNetName));

                var url = string.Format(URLTEMPLATE_GETAVAILIPADDRS,
                    Connection.SubcriptionID, vNetName, subnetAddrs[0]);
                var hi = new HttpInterface(Connection);
                var resp = hi.PerformRequest(HttpInterface.RequestType_Enum.GET, url);

                var availInfo = Utilities.DeSerialize(typeof(AddressAvailabilityResponse), resp.Body, true)
                    as AddressAvailabilityResponse;

                if (null == availInfo)
                    throw new Exception("Bad or missing AddressAvailabilityResponse returned from Azure");

                var addrList = new List<string>();

                if (availInfo.IsAvailable)
                    addrList.Add(subnetAddrs[0]);
                else
                    addrList.AddRange(availInfo.AvailableAddresses);

                return addrList;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetAvailAddrList() : " + 
                    Utilities.UnwindExceptionMessages(ex));
            }
        }
    }
}
