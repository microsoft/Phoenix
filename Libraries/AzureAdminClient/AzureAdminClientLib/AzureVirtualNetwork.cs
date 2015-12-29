using System;
using System.Collections.Generic;

namespace AzureAdminClientLib
{
    public class AzureVirtualNetwork : IComparable<AzureVirtualNetwork>
    {
        public AzureVirtualNetwork(string name, bool inUse, string affinityGroup, 
            string location, List<string> subnets, int maximumSupportedIpAddressCount)
        {
            Name = name;
            InUse = inUse;
            AffinityGroup = affinityGroup;
            Location = location;
            Subnets = subnets;
            _maximumSupportedIpAddressCount = maximumSupportedIpAddressCount;

        }

        private readonly int _maximumSupportedIpAddressCount;

        public string Name { get; protected set; }

        public bool InUse { get; protected set; }

        public string AffinityGroup { get; protected set; }

        public string Location { get; set; }

        public List<string> Subnets { get; protected set; }

        public int VirtualMachineCount { get; set; }

        //*********************************************************************
        ///
        /// <summary>
        /// Returns the Min() of the number of possible IP addresses as determined 
        /// by the address range allotted in Azure or the AppSettings config entry 
        /// "MaxPossibleIPAddressesOverride" if one is defined.
        /// </summary>
        /// 
        //*********************************************************************

        public int MaximumSupportedIpAddressCount 
        { 
            get
            {
                int configOverride;

                // See if there is a value for the override in the configuration file and if so use it to calculate
                // the max supported IPAddressCount.
                if (int.TryParse(System.Configuration.ConfigurationManager.AppSettings["MaxPossibleIpAddressesOverride"], out configOverride))
                {
                    return Math.Min(_maximumSupportedIpAddressCount, configOverride);
                }

                return _maximumSupportedIpAddressCount;
            } 
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="virtualNetworkParameter"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public int CompareTo(AzureVirtualNetwork virtualNetworkParameter)
        {
            // The comparison right now is being done only on the VirtualMachineCount column.
            return VirtualMachineCount.CompareTo(virtualNetworkParameter.VirtualMachineCount);
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vNetList"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static AzureVirtualNetwork FindLeastUsed(List<AzureVirtualNetwork> vNetList)
        {
            try
            {
                if (vNetList == null)
                    throw new Exception("NULL vNet List");

                if (vNetList.Count < 1)
                    throw new Exception("Empty vNet List");

                var vNetOut = vNetList[0];

                foreach (var vNet in vNetList)
                    if (vNet.VirtualMachineCount < vNetOut.VirtualMachineCount)
                        vNetOut = vNet;

                return vNetOut;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in AzureVirtualNetwork.FindLeastUsed() : " + ex.Message);
            }
        }
    }
}
