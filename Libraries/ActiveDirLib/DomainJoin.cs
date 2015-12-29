using System;
using System.DirectoryServices;
using System.Management;

namespace ActiveDirLib
{
    public class DomainJoin
    {

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        /// 
        //*********************************************************************

        public static bool IsDomainJoined(out string domainName)
        {
            try
            {
                var query = new SelectQuery("Win32_ComputerSystem");
            
                using (var searcher = new
                    ManagementObjectSearcher(query))
                    {
                        foreach (var o in searcher.Get())
                        {
                            var mo = (ManagementObject) o;
                            if ((bool)mo["partofdomain"] != true)
                            {
                                domainName = mo["workgroup"] as string;
                                return false;
                            }
                            else
                            {
                                domainName = mo["domain"] as string;
                                return true;
                            }
                        }
                    }

                domainName = null;
                return false;
            }
            catch(Exception ex)
            {
                throw new Exception("Exception in IsDomainJoined() : " + ex.Message);
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// http://juanchif.wordpress.com/2009/12/09/joining-a-computer-to-a-domain-programatically-from-c/
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="password"></param>
        /// <param name="username"></param>
        /// <param name="destinationOU"></param> "OU=ITManaged,OU=ITServices,DC=redmond,DC=corp,DC=microsoft,DC=com"
        /// 
        //*********************************************************************

        public static void Join(string domain, string password, string username, string destinationOU )
        {
            // Define constants used in the method.
            const int JOIN_DOMAIN = 1;
            const int ACCT_CREATE = 2;
            //const int ACCT_DELETE = 4;
            //const int WIN9X_UPGRADE = 16;
            //const int DOMAIN_JOIN_IF_JOINED = 32;
            //const int JOIN_UNSECURE = 64;
            //const int MACHINE_PASSWORD_PASSED = 128;
            //const int DEFERRED_SPN_SET = 256;
            //const int INSTALL_INVOCATION = 262144;

            // Here we will set the parameters that we like using the logical OR operator.
            // If you want to create the account if it doesn't exist you should add " | ACCT_CREATE "
            // For more information see: http://msdn.microsoft.com/en-us/library/aa392154%28VS.85%29.aspx
            //int parameters = JOIN_DOMAIN | DOMAIN_JOIN_IF_JOINED;
            var parameters = JOIN_DOMAIN | ACCT_CREATE;

            // The arguments are passed as an array of string objects in a specific order
            object[] methodArgs = { domain, password, username + "@" + domain, destinationOU, parameters };

            // Here we construct the ManagementObject and set Options
            var computerSystem = new ManagementObject("Win32_ComputerSystem.Name='" + Environment.MachineName + "'");
            computerSystem.Scope.Options.Authentication = System.Management.AuthenticationLevel.PacketPrivacy;
            computerSystem.Scope.Options.Impersonation = ImpersonationLevel.Impersonate;
            computerSystem.Scope.Options.EnablePrivileges = true;

            // Here we invoke the method JoinDomainOrWorkgroup passing the parameters as the second parameter
            var Oresult = computerSystem.InvokeMethod("JoinDomainOrWorkgroup", methodArgs);

            // The result is returned as an object of type int, so we need to cast.
            var result = (int)Convert.ToInt32(Oresult);

            // If the result is 0 then the computer is joined.
            if (result == 0)
            {
                Console.WriteLine("Joined Successfully!");
                return;
            }
            else
            {
                // Here are the list of possible errors
                var strErrorDescription = "Unrecognized Error";
                switch (result)
                {
                    case 5: strErrorDescription = "Access is denied";
                        break;
                    case 87: strErrorDescription = "The parameter is incorrect";
                        break;
                    case 110: strErrorDescription = "The system cannot open the specified object";
                        break;
                    case 1323: strErrorDescription = "Unable to update the password";
                        break;
                    case 1326: strErrorDescription = "Logon failure: unknown username or bad password";
                        break;
                    case 1355: strErrorDescription = "The specified domain either does not exist or could not be contacted";
                        break;
                    case 2224: strErrorDescription = "The account already exists";
                        break;
                    case 2691: strErrorDescription = "The machine is already joined to the domain";
                        break;
                    case 2692: strErrorDescription = "The machine is not currently joined to a domain";
                        break;
                }
                Console.WriteLine(strErrorDescription);
                return;
            }
        }

        //*********************************************************************
        ///
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strComputerName"></param>"JustinTest29"
        /// <param name="strLDAPOrgUnitPath"></param>"OU=ITManaged,OU=ITServices,DC=redmond,DC=corp,DC=microsoft,DC=com"
        /// 
        //*********************************************************************

        public static void Join2(string strComputerName, string strLDAPOrgUnitPath)
        {
            try
            {
                // Bind to the Users container, add a new group and a new contact
                var de = new DirectoryEntry("LDAP://" + strLDAPOrgUnitPath);

                // Create a new group object in the local cache
                var newComputer = de.Children.Add("CN=" + strComputerName.ToUpper(), "computer");

                // Set sAMAccountName property - Required for all security principal objects beginning with Windows Server 2003
                newComputer.Properties["sAMAccountName"].Value = strComputerName.ToUpper() + "$";

                // Create the new object on the server
                newComputer.CommitChanges();

                // Set the new computer object's properties - 4096 is default for workstations
                // Full property list available via http://support.microsoft.com/kb/305144
                newComputer.Properties["userAccountControl"][0] = 4096;
                newComputer.CommitChanges();

                // Close connection
                de.Close();
                newComputer.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in ActiveDirLib.DomainJoin.Join2() " + ex.Message);
            }

            // Check if object was successfully created
            if (!DirectoryEntry.Exists("LDAP://CN=" + strComputerName + "," + strLDAPOrgUnitPath))
                throw new Exception("Unable to create domain account in ActiveDirLib.DomainJoin.Join2() : ");
        }
    }
}