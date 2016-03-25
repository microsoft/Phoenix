//-----------------------------------------------------------------------
// <copyright file="InstallItemCustomDelegates.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Provides delegate functions that runs before and after calling the msi installation function.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    #region Using directives

    using CMP.Setup.Helpers;
    using CMP.Setup.SetupFramework;
    using KryptoLib;
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;

    #endregion

    /// <summary>
    /// This class contains the buitin custom delegates
    /// </summary>
    public class InstallItemCustomDelegates
    {
        #region Common
        public static bool DummyPreinstallProcessor()
        {
            return true;
        }

        public static bool DummyPostinstallProcessor()
        {
            return true;
        }

        public static bool DummyPrerequsiteProcessor()
        {
            return false;
        }

        public static bool SimplePreinstallProcessor(String controlTitle, bool wipeCmdArg)
        {
            bool returnValue = true;

            try
            {
                // Find the current installdata item in the array
                InstallItemsInstallDataItem simpleInstallItem = null;
                foreach (InstallItemsInstallDataItem itemToInstall in PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall))
                {
                    if (string.Equals(itemToInstall.ControlTitle, controlTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        simpleInstallItem = itemToInstall;
                        break;
                    }
                }

                if (simpleInstallItem == null)
                {
                    // Item not found - throw an exception
                    SetupLogger.LogError(String.Format("SimplePreinstallProcessor:{0} Install Data item not found.", controlTitle));
                    throw new ArgumentException(String.Format("{0} Item To Install not found", controlTitle));
                }
                else
                {
                    PropertyBagDictionary.Instance.SafeAdd("CurrentWorkingInstallItem", simpleInstallItem);
                }

                // Set the Install/Uninstall state
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    simpleInstallItem.ItemWeAreInstallingEnumValue =
                        simpleInstallItem.ItemWeAreInstallingEnumValue | InstallItemsInstallDataItem.InstallDataInputs.Installing;
                }
                else
                {
                    simpleInstallItem.ItemWeAreInstallingEnumValue =
                        simpleInstallItem.ItemWeAreInstallingEnumValue | InstallItemsInstallDataItem.InstallDataInputs.Uninstalling;
                }

                // Set the location of the log file
                simpleInstallItem.LogFile = SetupHelpers.SetLogFilePath(simpleInstallItem.LogFile);

                // If this is an uninstall then wipe the current commandline arguments.
                if (wipeCmdArg && PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    simpleInstallItem.Arguments = "REMOVE=ALL";
                }
            }
            catch (ArgumentException exception)
            {
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason, exception);
                }
                returnValue = false;
            }

            return returnValue;
        }

        public static string ConfigureBasicCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            // Check to see if this is an uninstall
            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                arguments.AppendFormat(CultureInfo.InvariantCulture, "INSTALLDIR=\"{0}\" ", (String)SetupInputs.Instance.FindItem(SetupInputTags.BinaryInstallLocationTag));
            }

            return arguments.ToString();
        }

        public static bool WAPExtensionCommonPreinstallProcessor()
        {
            bool returnValue = true; // used to store the return value for this function

            try
            {
                InstallItemsInstallDataItem wapExtensionCommonInstallItem = null;

                // Find the installdata item in the array
                foreach (InstallItemsInstallDataItem itemToInstall in PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall))
                {
                    if (string.Equals(itemToInstall.ControlTitle, PropertyBagConstants.WAPExtensionCommon, StringComparison.OrdinalIgnoreCase))
                    {
                        wapExtensionCommonInstallItem = itemToInstall;
                        break;
                    }
                }

                if (wapExtensionCommonInstallItem == null)
                {
                    // Item not found - throw an exception
                    SetupLogger.LogError("PageCustomDelegates: WAPExtensionCommonPreinstallProcessor: Install Data item not found.");
                    throw new ArgumentException(WpfResources.WPFResourceDictionary.InvalidArgument);
                }

                // Set the Install/Uninstall state
                wapExtensionCommonInstallItem.ItemWeAreInstallingEnumValue = wapExtensionCommonInstallItem.ItemWeAreInstallingEnumValue |
                    (PropertyBagDictionary.Instance.PropertyExists("uninstall") ? InstallItemsInstallDataItem.InstallDataInputs.Uninstalling : InstallItemsInstallDataItem.InstallDataInputs.Installing);

                // If this is not an uninstall, remove the product code.
                if (0 == (wapExtensionCommonInstallItem.ItemWeAreInstallingEnumValue & InstallItemsInstallDataItem.InstallDataInputs.Uninstalling))
                {
                    wapExtensionCommonInstallItem.ProductCode = string.Empty;
                }

                // Set the location of the log file
                wapExtensionCommonInstallItem.LogFile = SetupHelpers.SetLogFilePath(wapExtensionCommonInstallItem.LogFile);

                // Adjust the command line arguments
                wapExtensionCommonInstallItem.Arguments += ConfigureBasicCommandLineArguments() + ConfigureWAPExtensionCommonCommandLineArguments();

                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    if (SetupInputs.Instance.FindItem(SetupInputTags.WapRemoteDatabaseImpersonationTag))
                    {
                        SetupDatabaseHelper.GrantSetupUserDBAccess(false, false);
                        SetupInputs.Instance.EditItem(SetupInputTags.WapRemoteDatabaseImpersonationTag, false);
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.WapAfterGrantSetupUserDBAccess, true);
                    }

                    bool retainDB = SetupInputs.Instance.FindItem(SetupInputTags.WapRetainSqlDatabaseTag) ||
                        (!SetupInputs.Instance.FindItem(SetupInputTags.WapCreateNewSqlDatabaseTag) &&
                        PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.RollbacksToProcess));

                    if (!retainDB)
                    {
                        SetupDatabaseHelper.RemoveDB(true);
                    }
                }
            }
            catch (ArgumentException exception)
            {
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason, exception);
                }
                returnValue = false;
            }

            // Do any other tasks that are needed.
            return returnValue;
        }

        public static string ConfigureWAPExtensionCommonCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            // Check to see if this is an uninstall
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                arguments.Append("REMOVE=ALL ");
            }
            else
            {
                arguments.Append("ADDLOCAL=ProductFeature,ServiceFeature ");

                // Add the SQL database information so that it can be written to the registry and accessed by the services
                arguments.AppendFormat(CultureInfo.InvariantCulture, "SQLPORT=\"{0}\" ", (int)SetupInputs.Instance.FindItem(SetupInputTags.SqlServerPortTag));
                string sqlInstanceName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(false);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "INSTANCENAME=\"{0}\" ", sqlInstanceName);

                arguments.AppendFormat(CultureInfo.InvariantCulture, "WAPSQLPORT=\"{0}\" ", (int)SetupInputs.Instance.FindItem(SetupInputTags.WapSqlServerPortTag));
                string wapSqlInstanceName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(true);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "WAPINSTANCENAME=\"{0}\" ", wapSqlInstanceName);

                String dbName = (String)SetupInputs.Instance.FindItem(SetupInputTags.SqlDatabaseNameTag);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "DATABASENAME=\"{0}\" ", dbName);
                string partialConnectionString = SetupDatabaseHelper.ConstructWebsiteConnectionString(sqlInstanceName);
                string connectionString = String.Format("{0}database={1}", partialConnectionString, dbName);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "CMPCONNECTIONSTR=\"{0}\" ", connectionString);

                String wapDbName = (String)SetupInputs.Instance.FindItem(SetupInputTags.WapSqlDatabaseNameTag);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "WAPDATABASENAME=\"{0}\" ", wapDbName);
                string wapPartialConnectionString = SetupDatabaseHelper.ConstructWebsiteConnectionString(wapSqlInstanceName);
                string wapConnectionString = String.Format("{0}database={1}", wapPartialConnectionString, wapDbName);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "WAPCONNECTIONSTR=\"{0}\" ", wapConnectionString);

                string sqlMachineName = DnsHelper.GetComputerNameFromFqdnOrNetBios((String)SetupInputs.Instance.FindItem(SetupInputTags.GetSqlMachineNameTag(false)));
                bool onRemoteMachine = String.Compare(sqlMachineName, Environment.MachineName, StringComparison.OrdinalIgnoreCase) != 0;
                arguments.AppendFormat(CultureInfo.InvariantCulture, "ONREMOTESERVER=\"{0}\" ", onRemoteMachine ? 1 : 0);

                string wapSqlMachineName = DnsHelper.GetComputerNameFromFqdnOrNetBios((String)SetupInputs.Instance.FindItem(SetupInputTags.GetSqlMachineNameTag(true)));
                bool wapOnRemoteMachine = String.Compare(wapSqlMachineName, Environment.MachineName, StringComparison.OrdinalIgnoreCase) != 0;
                arguments.AppendFormat(CultureInfo.InvariantCulture, "WAPONREMOTESERVER=\"{0}\" ", wapOnRemoteMachine ? 1 : 0);

                String sqlMachineFqdn = DnsHelper.GetFullyQualifiedName(sqlMachineName);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "SQLMACHINENAME=\"{0}\" ", sqlMachineName);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "SQLMACHINEFQDN=\"{0}\" ", sqlMachineFqdn);

                String wapSqlMachineFqdn = DnsHelper.GetFullyQualifiedName(sqlMachineName);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "WAPSQLMACHINENAME=\"{0}\" ", wapSqlMachineName);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "WAPSQLMACHINEFQDN=\"{0}\" ", wapSqlMachineFqdn);

                String certificateThumbprint = (String)SetupInputs.Instance.FindItem(SetupInputTags.CmpCertificateThumbprintTag);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "CERTIFICATETHUMBPRINT=\"{0}\" ", "LocalMachine,My," + certificateThumbprint);

                // Write the cmp database connection string
                sqlInstanceName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(false);
                wapDbName = (String)SetupInputs.Instance.FindItem(SetupInputTags.SqlDatabaseNameTag);
                wapPartialConnectionString = SetupDatabaseHelper.ConstructWebsiteConnectionString(sqlInstanceName);
                wapConnectionString = String.Format("{0}database={1}", wapPartialConnectionString, wapDbName);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "CMPCONNECTIONSTR=\"{0}\" ", wapConnectionString);

                // Save the user name to use on the cmp service database
                String userName = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminNameTag);
                String domain = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminDomainTag);
                string fullUserName = String.IsNullOrEmpty(domain) ? userName : domain + @"\" + userName;
                arguments.AppendFormat(CultureInfo.InvariantCulture, "CMPDATABASEUSERNAME=\"{0}\" ", fullUserName);

                // Encrypt the password of the worker service so that WAP extensions can use it
                InputParameter pwd = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminPasswordTag);
                string passwordAsText = null;
                if (pwd != null)
                {
                    IntPtr unmanagedString = IntPtr.Zero;
                    try
                    {
                        unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(pwd);
                        passwordAsText = Marshal.PtrToStringUni(unmanagedString);
                    }
                    finally
                    {
                        Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
                    }
                }
                string encryptedPassword = String.Format("{0}{1}{2}", "[KText]", UserAccountHelper.EncryptStringUsingLocalCertificate(passwordAsText, certificateThumbprint), "[KText]");

                arguments.AppendFormat(CultureInfo.InvariantCulture, "CMPCONTEXTPASSWORDSTRING=\"{0}\" ", encryptedPassword);

                //Write the MicrosoftMgmtSvcStoreContext connection string (part of WAP original installation)
                wapDbName = SetupConstants.DefaultWapStoreDBName;
                wapPartialConnectionString = SetupDatabaseHelper.ConstructWebsiteConnectionString(sqlInstanceName);
                wapConnectionString = String.Format("{0}database={1}", wapPartialConnectionString, wapDbName);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "WAPSTORECONNECTIONSTR=\"{0}\" ", wapConnectionString);

                X509Krypto krypto = new X509Krypto("My", "LocalMachine", certificateThumbprint);
                // Encrypt the password of the sql user that will be used by the website to access CMP DB
                //string encryptedCmpDbPassword = String.Format("{0}{1}{2}", "[KText]", UserAccountHelper.EncryptStringUsingLocalCertificate(SetupDatabaseHelper.SqlDbUserPassword, certificateThumbprint), "[KText]");

                string encryptedCmpDbPassword = krypto.EncyptKText(SetupDatabaseHelper.SqlDbUserPassword);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "CMPDBPASSWORDSTRING=\"{0}\" ", encryptedCmpDbPassword);

                // Encrypt the password of the sql user that will be used by the website to access CMP WAP DB
                //string encryptedCmpWapDbPassword = String.Format("{0}{1}{2}", "[KText]", UserAccountHelper.EncryptStringUsingLocalCertificate(SetupDatabaseHelper.SqlDbUserPassword, certificateThumbprint), "[KText]");
                string encryptedCmpWapDbPassword = krypto.EncyptKText(SetupDatabaseHelper.SqlDbUserPassword);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "CMPWAPDBPASSWORDSTRING=\"{0}\" ", encryptedCmpWapDbPassword);

                // Encrypt the password of the sql user that will be used by the website to access WAP's Microsoft.MgmtSvc.Store DB
                //string encryptedMgmtStoreDbPassword = String.Format("{0}{1}{2}", "[KText]", UserAccountHelper.EncryptStringUsingLocalCertificate(SetupDatabaseHelper.SqlDbUserPassword, certificateThumbprint), "[KText]");
                string encryptedMgmtStoreDbPassword = krypto.EncyptKText(SetupDatabaseHelper.SqlDbUserPassword);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "MGMTSTOREPASSWORDSTRING=\"{0}\" ", encryptedMgmtStoreDbPassword);
            }

            return arguments.ToString();
        }

        public static bool WAPExtensionCommonPostIstallProcessor()
        {
            SetupLogger.LogInfo("WAPExtensionPostIstallProcessor: Entered.");
            bool returnValue = true;

            try
            {
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    String currentSetupUser = String.Format(SetupConstants.UserAccountTemplate, Environment.UserDomainName, Environment.UserName);
                    SetupInputs.Instance.EditItem(SetupInputTags.SetupUserAccountTag, currentSetupUser);

                    SetupDatabaseHelper.CheckDatabase(
                        InstallItemCustomDelegates.GetSQLServerInstanceNameStr(true),
                        SetupInputs.Instance.FindItem(SetupInputTags.WapSqlDatabaseNameTag),
                        true);

                    SetupDatabaseHelper.CreateDB(true);

                    if (SetupInputs.Instance.FindItem(SetupInputTags.WapRemoteDatabaseImpersonationTag))
                    {
                        SetupDatabaseHelper.GrantSetupUserDBAccess(true, true);
                        SetupInputs.Instance.EditItem(SetupInputTags.WapRemoteDatabaseImpersonationTag, false);
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.WapAfterGrantSetupUserDBAccess, true);
                    }

                    SetupDatabaseHelper.DeployWAPDacpac();
                }
            }
            catch (ArgumentException exception)
            {
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason, exception);
                }
                returnValue = false;
            }

            return returnValue;
        }

        #endregion Common

        #region Tenant WAP Extension

        private static bool? isTenantWAPExtensionInstalling;
        private static bool IsTenantWAPExtensionInstalling
        {
            get
            {
                if (!isTenantWAPExtensionInstalling.HasValue)
                {
                    // Find the installdata item in the array
                    foreach (InstallItemsInstallDataItem itemToInstall in PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall))
                    {
                        if (string.Equals(itemToInstall.ControlTitle, PropertyBagConstants.TenantWAPExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            isTenantWAPExtensionInstalling = true;
                        }
                    }
                }
                return isTenantWAPExtensionInstalling.Value;
            }
        }

        public static bool TenantWAPExtensionPreinstallProcessor()
        {
            bool returnValue = true; // used to store the return value for this function

            try
            {
                InstallItemsInstallDataItem tenantWapExtensionInstallItem = null;

                // Find the installdata item in the array
                foreach (InstallItemsInstallDataItem itemToInstall in PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall))
                {
                    if (string.Equals(itemToInstall.ControlTitle, PropertyBagConstants.TenantWAPExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        tenantWapExtensionInstallItem = itemToInstall;
                        break;
                    }
                }

                if (tenantWapExtensionInstallItem == null)
                {
                    // Item not found - throw an exception
                    SetupLogger.LogError("PageCustomDelegates: PangaeaClientPreinstallProcessor: Install Data item not found.");
                    throw new ArgumentException(WpfResources.WPFResourceDictionary.InvalidArgument);
                }

                // Set the Install/Uninstall state
                tenantWapExtensionInstallItem.ItemWeAreInstallingEnumValue = tenantWapExtensionInstallItem.ItemWeAreInstallingEnumValue |
                    (PropertyBagDictionary.Instance.PropertyExists("uninstall") ? InstallItemsInstallDataItem.InstallDataInputs.Uninstalling : InstallItemsInstallDataItem.InstallDataInputs.Installing);

                // If this is not an uninstall, remove the product code.
                if (0 == (tenantWapExtensionInstallItem.ItemWeAreInstallingEnumValue & InstallItemsInstallDataItem.InstallDataInputs.Uninstalling))
                {
                    tenantWapExtensionInstallItem.ProductCode = string.Empty;
                }

                // Set the location of the log file
                tenantWapExtensionInstallItem.LogFile = SetupHelpers.SetLogFilePath(tenantWapExtensionInstallItem.LogFile);

                SetupHelpers.CreateEventSources();

                // Adjust the command line arguments
                tenantWapExtensionInstallItem.Arguments += ConfigureBasicCommandLineArguments() + ConfigureTenantWAPCommandLineArguments();
            }
            catch (ArgumentException exception)
            {
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason, exception);
                }
                returnValue = false;
            }

            // Do any other tasks that are needed.
            return returnValue;
        }

        public static string ConfigureTenantWAPCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            // Check to see if this is an uninstall
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                arguments.Append("REMOVE=TenantSiteFeature ");
            }
            else
            {
                arguments.Append("ADDLOCAL=TenantSiteFeature ");
            }

            System.IO.DirectoryInfo inetpubDirectory = System.IO.Directory.GetParent(RegistryUtils.ReadRegistryValue(@"SOFTWARE\Microsoft\InetStp", @"PathWWWRoot", @"%SYSTEMDRIVE%\inetpub", true) as string);
            string tenantSitePath = System.IO.Path.Combine(inetpubDirectory.FullName, SetupConstants.TenantSiteFolderName);
            arguments.Append(String.Format("PATHTENANTSITE={0} ", tenantSitePath));

            return arguments.ToString();
        }

        public static bool TenantWAPExtensionPostIstallProcessor()
        {
            SetupLogger.LogInfo("WAPExtensionPostIstallProcessor: Entered.");
            bool returnValue = true;

            // TODO: Run post-installation required tasks

            return returnValue;
        }
        #endregion // Tenant WAP Extension

        #region Admin WAP Extension
        private static bool? isAdminWAPExtensionInstalling;
        private static bool IsAdminWAPExtensionInstalling
        {
            get
            {
                if (!isAdminWAPExtensionInstalling.HasValue)
                {
                    // Find the installdata item in the array
                    foreach (InstallItemsInstallDataItem itemToInstall in PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall))
                    {
                        if (string.Equals(itemToInstall.ControlTitle, PropertyBagConstants.AdminWAPExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            isAdminWAPExtensionInstalling = true;
                        }
                    }
                }
                return isAdminWAPExtensionInstalling.Value;
            }
        }

        private static string ConfigureAdminWAPCommandlineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            // Check to see if this is an uninstall
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                arguments.Append("REMOVE=AdminSiteFeature ");
            }
            else
            {
                arguments.Append("ADDLOCAL=AdminSiteFeature ");
            }

            System.IO.DirectoryInfo inetpubDirectory = System.IO.Directory.GetParent(RegistryUtils.ReadRegistryValue(@"SOFTWARE\Microsoft\InetStp", @"PathWWWRoot", @"%SYSTEMDRIVE%\inetpub", true) as string);
            string adminSitePath = System.IO.Path.Combine(inetpubDirectory.FullName, SetupConstants.AdminSiteFolderName);
            arguments.Append(String.Format("PATHADMINSITE={0} ", adminSitePath));

            return arguments.ToString();
        }

        public static bool AdminWAPExtensionPreinstallProcessor()
        {
            bool returnValue = true; // used to store the return value for this function

            try
            {
                InstallItemsInstallDataItem adminWapExtensionInstallItem = null;

                // Find the Client installdata item in the array
                foreach (InstallItemsInstallDataItem itemToInstall in PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall))
                {
                    if (string.Equals(itemToInstall.ControlTitle, PropertyBagConstants.AdminWAPExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        adminWapExtensionInstallItem = itemToInstall;
                        break;
                    }
                }

                if (adminWapExtensionInstallItem == null)
                {
                    // Item not found - throw an exception
                    SetupLogger.LogError("PageCustomDelegates: AdminWAPExtensionPreinstallProcessor: Install Data item not found.");
                    throw new ArgumentException(WpfResources.WPFResourceDictionary.InvalidArgument);
                }

                // Set the Install/Uninstall state
                adminWapExtensionInstallItem.ItemWeAreInstallingEnumValue = adminWapExtensionInstallItem.ItemWeAreInstallingEnumValue |
                    (PropertyBagDictionary.Instance.PropertyExists("uninstall") ? InstallItemsInstallDataItem.InstallDataInputs.Uninstalling : InstallItemsInstallDataItem.InstallDataInputs.Installing);

                // If this is not an uninstall, remove the product code.
                if (0 == (adminWapExtensionInstallItem.ItemWeAreInstallingEnumValue & InstallItemsInstallDataItem.InstallDataInputs.Uninstalling))
                {
                    adminWapExtensionInstallItem.ProductCode = string.Empty;
                }

                // Set the location of the log file
                adminWapExtensionInstallItem.LogFile = SetupHelpers.SetLogFilePath(adminWapExtensionInstallItem.LogFile);

                SetupHelpers.CreateEventSources();

                // Adjust the command line arguments
                adminWapExtensionInstallItem.Arguments += ConfigureBasicCommandLineArguments() + ConfigureAdminWAPCommandlineArguments();
            }
            catch (ArgumentException exception)
            {
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason, exception);
                }
                returnValue = false;
            }

            // Do any other tasks that are needed.
            return returnValue;
        }

        public static bool AdminWAPExtensionPostIstallProcessor()
        {
            SetupLogger.LogInfo("WAPExtensionPostIstallProcessor: Entered.");
            bool returnValue = true;

            // TODO: Run post-installation required tasks

            return returnValue;
        }
        #endregion

        #region CMPServer

        private static void WriteConfigSettingsRegistryValue(String regKey, String name, object value)
        {
            RegistryUtils.WriteRegistryValue(Registry.LocalMachine, regKey, name, value);
        }

        private static void WriteSqlRegistryValue(String name, object value)
        {
            WriteConfigSettingsRegistryValue(SetupConstants.SqlSettingsRegKey, name, value);
        }

        private static void WriteWapSqlRegistryValue(String name, object value)
        {
            WriteConfigSettingsRegistryValue(SetupConstants.WapSqlSettingsRegKey, name, value);
        }

        private static void WriteServerRegistrationRegistryValue(String name, object value)
        {
            WriteConfigSettingsRegistryValue(SetupConstants.ServerRegistrationRegKey, name, value);
        }

        private static void DeleteConfigSettingsRegistryValue(String regKey, String name)
        {
            RegistryUtils.DeleteRegistryValue(Registry.LocalMachine, regKey, name);
        }

        private static void DeleteSqlRegistryValue(String name)
        {
            DeleteConfigSettingsRegistryValue(SetupConstants.SqlSettingsRegKey, name);
        }


        /// <summary>
        /// Reset the registry values
        /// </summary>
        private static void ResetRegistry()
        {
            // sql
            string sqlInstanceName = GetSQLServerInstanceNameStr(false);
            string wapSqlInstanceName = GetSQLServerInstanceNameStr(true);
            WriteSqlRegistryValue(SetupConstants.InstanceNameRegistryValueName, sqlInstanceName);
            WriteWapSqlRegistryValue(SetupConstants.WapInstanceNameRegistryValueName, wapSqlInstanceName);
            string dbName = (String)SetupInputs.Instance.FindItem(SetupInputTags.SqlDatabaseNameTag);
            WriteSqlRegistryValue(SetupConstants.DBNameRegistryValueName, dbName);
            string wapDBName = (String)SetupInputs.Instance.FindItem(SetupInputTags.WapSqlDatabaseNameTag);
            WriteWapSqlRegistryValue(SetupConstants.WapDbNameRegistryValueName, wapDBName);
            string partialConnectionString = SetupDatabaseHelper.ConstructConnectionString(sqlInstanceName);
            string wapPartialConnectionString = SetupDatabaseHelper.ConstructConnectionString(wapSqlInstanceName);
            string connectionString = String.Format("{0}database={1}", partialConnectionString, dbName);
            string wapConnectionString = String.Format("{0}database={1}", partialConnectionString, wapDBName);
            WriteSqlRegistryValue(SetupConstants.ConnectionStringRegistryValueName, connectionString);
            WriteWapSqlRegistryValue(SetupConstants.ConnectionStringRegistryValueName, wapConnectionString);

            string sqlMachineName = DnsHelper.GetComputerNameFromFqdnOrNetBios((String)SetupInputs.Instance.FindItem(SetupInputTags.GetSqlMachineNameTag(false)));
            bool onRemoteMachine = String.Compare(sqlMachineName, Environment.MachineName, true) != 0;
            WriteSqlRegistryValue(SetupConstants.OnRemoteRegistryValueName, onRemoteMachine ? 1 : 0);
            WriteSqlRegistryValue(SetupConstants.MachineNameRegistryValueName, sqlMachineName);
            String sqlMachineFqdn = DnsHelper.GetFullyQualifiedName(sqlMachineName);
            WriteSqlRegistryValue(SetupConstants.FqdnRegistryValueName, sqlMachineFqdn);

            sqlMachineName = DnsHelper.GetComputerNameFromFqdnOrNetBios((String)SetupInputs.Instance.FindItem(SetupInputTags.GetSqlMachineNameTag(true)));
            onRemoteMachine = String.Compare(sqlMachineName, Environment.MachineName, true) != 0;
            WriteWapSqlRegistryValue(SetupConstants.OnRemoteRegistryValueName, onRemoteMachine ? 1 : 0);
            WriteWapSqlRegistryValue(SetupConstants.MachineNameRegistryValueName, sqlMachineName);
            sqlMachineFqdn = DnsHelper.GetFullyQualifiedName(sqlMachineName);
            WriteWapSqlRegistryValue(SetupConstants.FqdnRegistryValueName, sqlMachineFqdn);

            // user name, company name under server
            String userName = SetupInputs.Instance.FindItem(SetupInputTags.UserNameTag);
            WriteServerRegistrationRegistryValue(SetupConstants.UserNameRegistryValueName, userName);

            // VmmServiceAccount
            String serviceAccount = UserAccountHelper.GetServiceAccount();
            if (SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceLocalAccountTag))
            {
                serviceAccount = SetupConstants.LocalSystem;
            }

            WriteConfigSettingsRegistryValue(SetupConstants.ServerSetupInfoRegKey, SetupConstants.VmmServiceAccountValueName, serviceAccount);
        }

        public static string GetSQLServerInstanceNameStr(bool isWap)
        {
            string sqlServerInstanceName = String.Empty;
            String instanceName = (String)SetupInputs.Instance.FindItem(SetupInputTags.GetSqlInstanceNameTag(isWap));
            String sqlMachineName = (String)SetupInputs.Instance.FindItem(SetupInputTags.GetSqlMachineNameTag(isWap));
            if (String.IsNullOrEmpty(instanceName) || String.Equals(instanceName, sqlMachineName, StringComparison.OrdinalIgnoreCase))
            {
                sqlServerInstanceName = sqlMachineName;
            }
            else
            {
                sqlServerInstanceName = String.Format(SetupConstants.UserAccountTemplate, sqlMachineName, instanceName);
            }
            int sqlPort = (int)SetupInputs.Instance.FindItem(SetupInputTags.SqlServerPortTag);
            if (sqlPort != 0 && sqlPort != 1433)
            {
                sqlServerInstanceName = String.Format("{0},{1}", sqlServerInstanceName, sqlPort);
            }

            return sqlServerInstanceName;
        }

        public static bool CMPServerPreinstallProcessor()
        {
            bool returnValue = true;

            try
            {
                InstallItemsInstallDataItem virtualMachineManagerInstallItem = null;

                // Find the installdata item in the array
                foreach (InstallItemsInstallDataItem itemToInstall in PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall))
                {
                    if (string.Equals(itemToInstall.ControlTitle, PropertyBagConstants.CMPServer, StringComparison.OrdinalIgnoreCase))
                    {
                        virtualMachineManagerInstallItem = itemToInstall;
                        break;
                    }
                }

                if (virtualMachineManagerInstallItem == null)
                {
                    // Item not found - throw an exception
                    SetupLogger.LogError("PageCustomDelegates: VMMPreinstallProcessor: Install Data item not found.");
                    throw new ArgumentException(WpfResources.WPFResourceDictionary.InvalidArgument);
                }

                // Set the Install/Uninstall state
                virtualMachineManagerInstallItem.ItemWeAreInstallingEnumValue = virtualMachineManagerInstallItem.ItemWeAreInstallingEnumValue |
                    (PropertyBagDictionary.Instance.PropertyExists("uninstall") ? InstallItemsInstallDataItem.InstallDataInputs.Uninstalling : InstallItemsInstallDataItem.InstallDataInputs.Installing);

                // If this is not an uninstall, remove the product code.
                if (0 == (virtualMachineManagerInstallItem.ItemWeAreInstallingEnumValue & InstallItemsInstallDataItem.InstallDataInputs.Uninstalling))
                {
                    virtualMachineManagerInstallItem.ProductCode = string.Empty;
                }

                // Set the location of the log file
                virtualMachineManagerInstallItem.LogFile = SetupHelpers.SetLogFilePath(virtualMachineManagerInstallItem.LogFile);

                SetupHelpers.CreateEventSources();

                // Adjust the command line arguments
                virtualMachineManagerInstallItem.Arguments += InstallItemCustomDelegates.ConfigureBasicCommandLineArguments() + InstallItemCustomDelegates.ConfigureServerCommandLineArguments();


                bool isRollback = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.RollbacksToProcess);

                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    String currentSetupUser = String.Format(SetupConstants.UserAccountTemplate, Environment.UserDomainName, Environment.UserName);
                    SetupInputs.Instance.EditItem(SetupInputTags.SetupUserAccountTag, currentSetupUser);

                    if (SetupInputs.Instance.FindItem(SetupInputTags.RemoteDatabaseImpersonationTag))
                    {
                        SetupDatabaseHelper.GrantSetupUserDBAccess(false, false);
                        SetupInputs.Instance.EditItem(SetupInputTags.RemoteDatabaseImpersonationTag, false);
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AfterGrantSetupUserDBAccess, true);
                    }

                    ServiceConfigurationHandler.RemoveService(SetupConstants.EngineServiceName);

                    // Stopping the service sometimes does not end the processes and this keeps a lock on the process binaries
                    InstallItemCustomDelegates.EndCmpWorkerServiceProcess();

                    bool retainDB = SetupInputs.Instance.FindItem(SetupInputTags.RetainSqlDatabaseTag) ||
                        !SetupInputs.Instance.FindItem(SetupInputTags.CreateNewSqlDatabaseTag);

                    if (!retainDB)
                    {
                        SetupDatabaseHelper.RemoveDB(false);
                    }
                }
            }
            catch (ArgumentException exception)
            {
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason, exception);
                }
                SetupLogger.LogException(exception);
                returnValue = false;
            }

            return returnValue;
        }

        private static void EndCmpWorkerServiceProcess()
        {
            Process[] runningProcesses = Process.GetProcesses();
            foreach (Process process in runningProcesses)
            {
                try
                {
                    foreach (ProcessModule module in process.Modules)
                    {
                        if (module.FileName.Contains(SetupConstants.EngineServiceBinary))
                        {
                            process.Kill();
                        }
                    }
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // Ignore an access denied error since some system processes will throw this.
                }
            }
        }

        public static string ConfigureServerCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            // Check to see if this is an uninstall
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                arguments.Append("REMOVE=ALL ");
                arguments.AppendFormat(CultureInfo.InvariantCulture, "UNINSTALLSERVER = 1 ");
            }
            else
            {
                String serviceAccount = UserAccountHelper.GetServiceAccount();
                arguments.AppendFormat(CultureInfo.InvariantCulture, "SERVERACCOUNTNAME=\"{0}\" ", serviceAccount);  // for agent installation
                if (SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceLocalAccountTag))
                {
                    serviceAccount = SetupConstants.LocalSystem;
                }
                arguments.AppendFormat(CultureInfo.InvariantCulture, "SERVICEACCOUNT=\"{0}\" ", serviceAccount); // for registry
                arguments.AppendFormat(CultureInfo.InvariantCulture, "USERNAME=\"{0}\" ", (String)SetupInputs.Instance.FindItem(SetupInputTags.UserNameTag));

                arguments.AppendFormat(CultureInfo.InvariantCulture, "SQLPORT=\"{0}\" ", (int)SetupInputs.Instance.FindItem(SetupInputTags.SqlServerPortTag));
                string sqlInstanceName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(false);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "INSTANCENAME=\"{0}\" ", sqlInstanceName);

                String dbName = (String)SetupInputs.Instance.FindItem(SetupInputTags.SqlDatabaseNameTag);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "DATABASENAME=\"{0}\" ", dbName);
                string partialConnectionString = SetupDatabaseHelper.ConstructConnectionString(sqlInstanceName);
                string connectionString = String.Format("{0}database={1}", partialConnectionString, dbName);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "CONNECTIONSTR=\"{0}\" ", connectionString);

                string sqlMachineName = DnsHelper.GetComputerNameFromFqdnOrNetBios((String)SetupInputs.Instance.FindItem(SetupInputTags.GetSqlMachineNameTag(false)));
                bool onRemoteMachine = String.Compare(sqlMachineName, Environment.MachineName, true) != 0;
                arguments.AppendFormat(CultureInfo.InvariantCulture, "ONREMOTESERVER=\"{0}\" ", onRemoteMachine ? 1 : 0);

                String sqlMachineFqdn = DnsHelper.GetFullyQualifiedName(sqlMachineName);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "SQLMACHINENAME=\"{0}\" ", sqlMachineName);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "SQLMACHINEFQDN=\"{0}\" ", sqlMachineFqdn);

                arguments.AppendFormat(CultureInfo.InvariantCulture, "SETUPLANGUAGE=\"{0}\" ", CultureInfo.CurrentUICulture.Name);

                String certificateThumbprint = (String)SetupInputs.Instance.FindItem(SetupInputTags.CmpCertificateThumbprintTag);
                arguments.AppendFormat(CultureInfo.InvariantCulture, "CERTIFICATETHUMBPRINT=\"{0}\" ", "LocalMachine,My," + certificateThumbprint);

                // Encrypt the password of the worker service so that WAP extensions can use it
                InputParameter pwd = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminPasswordTag);
                string passwordAsText = null;
                if (pwd != null)
                {
                    IntPtr unmanagedString = IntPtr.Zero;
                    try
                    {
                        unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(pwd);
                        passwordAsText = Marshal.PtrToStringUni(unmanagedString);
                    }
                    finally
                    {
                        Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
                    }
                }
                string encryptedPassword = String.Format("{0}{1}{2}", "[KText]", UserAccountHelper.EncryptStringUsingLocalCertificate(passwordAsText, certificateThumbprint), "[KText]");

                arguments.AppendFormat(CultureInfo.InvariantCulture, "CMPCONTEXTPASSWORDSTRING=\"{0}\" ", encryptedPassword);
            }

            return arguments.ToString();
        }

        public static bool CMPServerPostinstallProcessor()
        {
            bool returnValue = true;

            try
            {
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    String currentSetupUser = String.Format(SetupConstants.UserAccountTemplate, Environment.UserDomainName, Environment.UserName);
                    SetupInputs.Instance.EditItem(SetupInputTags.SetupUserAccountTag, currentSetupUser);

                    SetupDatabaseHelper.CheckDatabase(
                        InstallItemCustomDelegates.GetSQLServerInstanceNameStr(false),
                        SetupInputs.Instance.FindItem(SetupInputTags.SqlDatabaseNameTag),
                        false);

                    SetupDatabaseHelper.CreateDB(false);

                    if (SetupInputs.Instance.FindItem(SetupInputTags.RemoteDatabaseImpersonationTag))
                    {
                        SetupDatabaseHelper.GrantSetupUserDBAccess(true, false);
                        SetupInputs.Instance.EditItem(SetupInputTags.RemoteDatabaseImpersonationTag, false);
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AfterGrantSetupUserDBAccess, true);
                    }

                    ServiceConfigurationHandler serviceConfigurationHandler = new ServiceConfigurationHandler();
                    CmpWorkerServiceHelper.ConfigureCMPWorkerService(serviceConfigurationHandler);

                    serviceConfigurationHandler.StartService(SetupConstants.EngineServiceName);
                }
            }
            catch (Exception exception)
            {
                SetupLogger.LogException(exception, "CMPServerPostinstallProcessor threw an exception");
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.VitalFailure,
                    (PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.VitalFailure)
                    ? (PropertyBagDictionary.Instance.GetProperty<InstallItemsInstallDataItem.InstallDataInputs>
                        (PropertyBagDictionary.VitalFailure) | InstallItemsInstallDataItem.InstallDataInputs.PostInstallItem)
                    : InstallItemsInstallDataItem.InstallDataInputs.PostInstallItem));
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason, exception);
                }
                returnValue = false;
            }

            return returnValue;
        }
        #endregion
    }
}
