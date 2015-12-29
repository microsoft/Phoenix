using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace CMP.Setup.Helpers
{
    class DnsHelper
    {
        public enum DnsParseOption
        {
            RequireFQDN,
            AllowDotlessName,
        }

        /// <summary>
        /// Gets the computer name from either FQDN or NetBIOS(no-op). Note that the computer name
        /// part of the FQDN is not always the same as the NetBIOS name, particularly if the computer
        /// name is longer than 15 characters (the limit for NetBIOS names).
        /// </summary>
        /// <param name="name">The FQDN or NetBIOS name.</param>
        /// <returns>The extracted computer name.</returns>
        public static string GetComputerNameFromFqdnOrNetBios(string name)
        {
            return DnsHelper.GetComputerName(name, DnsParseOption.AllowDotlessName);
        }

        public static string GetFullyQualifiedName(string computerName)
        {
            string result = null;
            AppAssert.AssertNotNull(computerName);

            try
            {
                IPAddress ipAddress = null;
                if (IPAddress.TryParse(computerName, out ipAddress))
                {
                    result = Dns.GetHostEntry(ipAddress).HostName;
                }
                else
                {
                    result = Dns.GetHostEntry(computerName).HostName;
                }
            }
            catch (ArgumentException e)
            {
                throw new Exception("Compute name is invalid");
            }
            catch (System.Net.Sockets.SocketException e)
            {
                throw new Exception("Cannot contact the machine with name " + computerName);
            }

            return result;
        }

        /// <summary>
        /// Extracts the computer name from the passed in dnsName. Note that if the dnsName is an
        /// IP address, no conversion is performed. Also note that if the dnsName is a FQDN name, the
        /// extracted computer name is not the same as the NetBIOS name, particularly in the case
        /// that the computer name is longer than 15 characters (the limit for NetBios names).
        /// </summary>
        /// <param name="dnsName">The dnsName.</param>
        /// <param name="parseOption">Used to specify whether it is ok if the dnsName is not the FQDN name.</param>
        /// <returns>The extracted computer name.</returns>
        public static string GetComputerName(string dnsName, DnsParseOption parseOption)
        {
            string computerName;
            if (dnsName.IndexOf('.') == -1)
            {
                // No dot in the dnsName - assume computername is same as dnsName
                computerName = dnsName;

                if (parseOption == DnsParseOption.RequireFQDN)
                {
                    throw new Exception(String.Format("Compute name {0} is invalid", dnsName));
                }
            }
            else
            {
                IPAddress ipAddr;
                if (IPAddress.TryParse(dnsName, out ipAddr))
                {
                    // don't try to parse the IP
                    computerName = dnsName;
                }
                else
                {
                    computerName = dnsName.Substring(0, dnsName.IndexOf('.'));
                }
            }
            return computerName;
        }

        public static string GetLocalMachineAccount()
        {
            // Gets the NetBIOS name of this local computer
            string computerName = Environment.MachineName;
            string domainName = GetPrimaryDnsDomainName(computerName);
            if (String.IsNullOrEmpty(domainName))
            {
                domainName = DnsHelper.GetDomainName(GetLocalMachineFullyQualifiedName(), DnsParseOption.AllowDotlessName);
            }

            return String.Format("{0}\\{1}$", domainName, computerName);
        }

        public static string GetDomainName(string dnsName, DnsParseOption parseOption)
        {
            string domainName;
            if (dnsName.IndexOf('.') == -1)
            {
                // No dot in the dnsName - assume domain name is the same as the dnsName
                domainName = dnsName;

                if (parseOption == DnsParseOption.RequireFQDN)
                {
                    throw new Exception(String.Format("Computer name {0} is invalid", dnsName));
                }
            }
            else
            {
                IPAddress ipAddr;
                if (IPAddress.TryParse(dnsName, out ipAddr))
                {
                    // there's no domain in case of IP
                    domainName = string.Empty;
                }
                else
                {
                    domainName = dnsName.Substring(dnsName.IndexOf('.') + 1);
                }
            }
            return domainName;
        }

        public static string GetLocalMachineFullyQualifiedName()
        {
            return GetFullyQualifiedName(GetLocalMachineHostName());
        }

        public static string GetLocalMachineHostName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// Gets the name of the DNS domain assigned to the local computer
        /// Using Lsa functions
        /// </summary>
        public static string GetPrimaryDnsDomainName(string computerName)
        {
            string dName = string.Empty;

            IntPtr polHandle = IntPtr.Zero;
            NativeMethods.LSA_OBJECT_ATTRIBUTES objAttr = new NativeMethods.LSA_OBJECT_ATTRIBUTES();
            objAttr.Length = 0;
            objAttr.RootDirectory = IntPtr.Zero;
            objAttr.Attributes = 0;
            objAttr.SecurityDescriptor = IntPtr.Zero;
            objAttr.SecurityQualityOfService = IntPtr.Zero;

            NativeMethods.LSA_UNICODE_STRING localSysName = new NativeMethods.LSA_UNICODE_STRING();
            localSysName.Buffer = Marshal.StringToHGlobalUni(computerName);
            localSysName.Length = (ushort)(computerName.Length * UnicodeEncoding.CharSize);
            localSysName.MaximumLength = localSysName.Length;

            // LsaOpenPolicy
            UInt32 retcode = NativeMethods.LsaOpenPolicy(ref localSysName, ref objAttr,
                (UInt32)(NativeMethods.LsaPolicies.GENERIC_READ |
                NativeMethods.LsaPolicies.POLICY_VIEW_LOCAL_INFORMATION), out polHandle);
            Int32 win32ErrorCode = NativeMethods.LsaNtStatusToWinError(retcode);

            if (win32ErrorCode == 0)
            {
                NativeMethods.POLICY_INFORMATION_CLASS policyInfo = NativeMethods.POLICY_INFORMATION_CLASS.PolicyPrimaryDomainInformation;
                IntPtr pData = IntPtr.Zero;

                // LsaQueryInformationPolicy
                retcode = NativeMethods.LsaQueryInformationPolicy(polHandle,
                    policyInfo,
                    out pData);
                win32ErrorCode = NativeMethods.LsaNtStatusToWinError(retcode);

                if (win32ErrorCode == 0)
                {
                    NativeMethods.POLICY_PRIMARY_DOMAIN_INFO primaryDomainInfo = (NativeMethods.POLICY_PRIMARY_DOMAIN_INFO)Marshal.PtrToStructure(pData, typeof(NativeMethods.POLICY_PRIMARY_DOMAIN_INFO));
                    dName = Marshal.PtrToStringUni(primaryDomainInfo.DomainName.Buffer);

                    NativeMethods.LsaFreeMemory(pData);

                }

                NativeMethods.LsaClose(polHandle);

            }

            return dName;
        }

        /// <summary>
        /// Get the full username (domain\username) from a partial one.
        /// </summary>
        /// <param name="username">input user name</param>
        /// <param name="fullUserName">resulting full user name</param>
        /// <returns>invalid user error, or Error.Success</returns>
        public static void CheckAndGetFullUserName(string userName, out string fullUserName)
        {
            fullUserName = string.Empty;
            if (string.IsNullOrEmpty(userName))
            {
                throw new Exception(String.Format("Unable to find a domain account for owner {0}", userName));
            }
            try
            {
                NTAccount account = new NTAccount(userName);
                SecurityIdentifier id = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
                // now for the fun part, set the textbox to the correct name...
                fullUserName = id.Translate(typeof(NTAccount)).Value;
            }
            catch (IdentityNotMappedException e)
            {
                throw new Exception(String.Format("Unable to find a domain account for owner {0}", userName));
            }
            catch (ArgumentException e)
            {
                throw new Exception(String.Format("Unable to find a domain account for owner {0}", userName));
            }
            catch (SystemException e)
            {
                throw new Exception(String.Format("Unable to find a domain account for owner {0}", userName));
            }
        }
    }
}
