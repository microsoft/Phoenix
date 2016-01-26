//-----------------------------------------------------------------------
// <copyright file="SetupConstants.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Provides constants for use in the setup.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using CMP.Setup.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    using System.Text;
    using System.Threading;
    using System.IO;

    /// <summary>
    /// Setup features that can be specified from command line
    /// </summary>
    [Flags]
    public enum SetupFeatures
    {
        /// <summary>
        /// Server components
        /// </summary>
        Server                  = 0x00000001,

        /// <summary>
        /// Extension Comon components
        /// </summary>
        ExtensionCommon         = 0x00000010,

        /// <summary>
        /// Tenant WAP Extension components
        /// </summary>
        TenantExtension         = 0x00000100,

        /// <summary>
        /// Admin WAP Extension components
        /// </summary>
        AdminExtension          = 0x00001000,
    };

    public enum AgreementType
    {
        Notice
    }

    /// <summary>
    /// Contains general constants to use in the setup
    /// </summary>
    public static class SetupConstants
    {

        // Installation settings
        public const String DefaultInstallDirectory = @"Compute Management Pack";
        public const String TenantSiteFolderName = @"MgmtSvc-TenantSite";
        public const String AdminSiteFolderName = @"MgmtSvc-AdminSite";
        /// <summary>
        /// maximum version string length (required for unmanaged call)
        /// </summary>
        internal const int MaximumVersionStringLength = 512;

        /// <summary>
        /// Upgrade code is of type "{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}" + 1 for null char
        /// </summary>
        internal const int MaximumUpgradeCodeLength = 39;

        internal const string ServerProductCode = "{FAC2B832-6869-4F2B-9625-0EDE1644CB09}";
        public static string GetUpgradeCode(SetupFeatures feature)
        {
            string guid = null;

            switch (feature)
            {
                case SetupFeatures.ExtensionCommon:
                    guid = "092688F9-8EF4-4D99-AEBF-CBDEB5276D2C";
                    break;
                case SetupFeatures.TenantExtension:
                    guid = "092688F9-8EF4-4D99-AEBF-CBDEB5276D2B";
                    break;
                case SetupFeatures.AdminExtension:
                    guid = "092688F9-8EF4-4D99-AEBF-CBDEB5276D2A";
                    break;
                case SetupFeatures.Server:
                    guid = "93CAADEF-B45F-4343-B824-B90245A80500";
                    break;
            }

            return "{" + guid + "}";
        }

        // SQL server settings
        public const string DefaultDBName = @"CMP_DB";
        public const string DefaultWapDBName = @"CMPWAP_DB";
        public const string DefaultWapStoreDBName = @"Microsoft.MgmtSvc.Store";
        public const string DBNameRegistryValueName = @"DatabaseName";
        public const string OnRemoteRegistryValueName = @"OnRemoteServer"; 
        public const string InstanceNameRegistryValueName = @"InstanceName";
        public const string MachineNameRegistryValueName = @"MachineName";
        public const string SqlSettingsRegKey = SetupConstants.ServerSettingsRegKey + @"\" + SetupConstants.SqlSettingsKeyName;
        public const string WapSqlSettingsRegKey = @"SOFTWARE\Microsoft\MgmtSvc\CmpWapExtension";
        public const string SettingsKeyName = @"";
        public const string SqlSettingsKeyName = @"";
        public const string ServerSettingsRegKey = SetupConstants.ServerRegKey + @"\" + SetupConstants.SettingsKeyName;
        public const string ConnectionStringRegistryValueName = @"CMPContext";
        public const string FqdnRegistryValueName = @"MachineFQDN";
        public const string UserNameRegistryValueName = @"DatabaseName";
        public const string VmmServiceAccountValueName = @"VmmServiceAccount";

        // Server settings
        private static string serverInstallationPath = null;
        private static string tenantExtensionInstallationPath = null;
        private static string extensionCommonInstallationPath = null;
        private static string adminExtensionInstallationPath = null;

        public const string RootRegKey = @"SOFTWARE\Microsoft\";
        public const string BeanstalkRegSubKey = @"MgmtSvc\";
        public const string ServerRegSubKey = SetupConstants.BeanstalkRegSubKey + @"CmpWapExtension";
        public const string ServerRegKey = SetupConstants.RootRegKey + SetupConstants.ServerRegSubKey;
        public const string SetupKeyName = @"";
        public const string ServerSetupInfoRegKey = SetupConstants.ServerRegKey + @"\" + SetupConstants.SetupKeyName;

        //public const string TenantExtensionRegSubKey = SetupConstants.BeanstalkRegSubKey + @"TenantExtension";
        //public const string TenantExtensionRegKey = SetupConstants.RootRegKey + SetupConstants.TenantExtensionRegSubKey;
        //public const string TenantExtensionSetupInfoRegKey = SetupConstants.TenantExtensionRegKey + @"\" + SetupConstants.SetupKeyName;

        //public const string AdminExtensionRegSubKey = SetupConstants.BeanstalkRegSubKey + @"AdminExtension";
        //public const string AdminExtensionRegKey = SetupConstants.RootRegKey + SetupConstants.AdminExtensionRegSubKey;
        //public const string AdminExtensionSetupInfoRegKey = SetupConstants.AdminExtensionRegKey + @"\" + SetupConstants.SetupKeyName;

        public const string RegistrationKeyName = @"";
        public const string ServerRegistrationRegKey = SetupConstants.ServerSetupInfoRegKey + @"\" + SetupConstants.RegistrationKeyName;

        public const string InstallPath = "InstallPath";
        private const string SetupDirectory = "Setup";
        public const string EngineServiceName = "CmpWorkerService";
        public const string EngineServiceBinary = "CmpWorkerService.exe";
        internal const int ServiceActionsCount = 2;
        internal const int ServiceRestartDelay = 100;
        internal const int ServiceResetPeriod = 60 * 5;

        /// <summary>
        /// Time out for starting the service
        /// </summary>
        internal static readonly TimeSpan ServiceStartTimeout = new TimeSpan(0, 2, 0);

        public const char AccountDomainUserSeparator = '\\';
        public const char IPAddressSeparator = ',';
        public const string UserAccountTemplate = @"{0}\{1}";

        public const string MsiFolder = "msi";
        public const string LicensesFolder = "Licenses";
        public const String LogFolder = "CMPLogs";

        public const string VerboseLog = "VerboseLog";

        public const string LocalSystem = "LocalSystem";

        public const string BaseLanguageID = "1033";
        public const string UnknownLCID = "????";

        public const string MUAuthCab = "MUAuth.cab";
        public const string ConsoleExeFileName = "VmmAdminUI.exe";
        public const string SetupFolder = "setup";
        public const string ConfigIcon = "ServerConfig.ico";
        public const string ShortcutNameStringFormat = "{0}.lnk";
        public const string MiniSetupCommand = "SetupCMP.exe";
        public const string MiniSetupArguments = @"/server /config /spawned /runui";

        public const String MSDeployUpgradeCode = "{F63293CE-9EAC-4F8D-A261-2A280DFEADE8}";
        public static Version MSDeployVersionToInstall = new Version(3, 1236, 1631);
        public const String SQLSysClrTypesUpgradeCode = "{34CE963B-6DCD-437E-B75C-CF71A8E2FE77}";
        public static Version SQLSysClrTypesVersionToInstall = new Version(12, 0, 2000, 8);
        public const String SqlDomUpgradeCode = "{B43C8B4F-2CB5-4DC2-A7A3-E6579D509AD7}";
        public static Version SqlDomVersionToInstall = new Version(12, 0, 2000, 8);
        public const String SharedManagementObjectsUpgradeCode = "{A85FACB4-80AB-414C-BC73-619E9F8783E5}";
        public static Version SharedManagementObjectsVersionToInstall = new Version(12, 0, 2000, 8);
        public const String DACFrameworkUpgradeCode = "{08E406E2-FEB6-48B9-8286-4C6D742EB067}";
        public static Version DACFrameworkVersionToInstall = new Version(12, 0, 2603, 2);

        public const string PSModulePathEnvVarName = "PSModulePath";
        public const string VMMModuleSubDirectory = @"psModules";

        public const String Root = "Root";
        public const String DisplayItem = "DisplayItem";
        public const String Image = "image";
        public const String Parent = "Parent";
        public const String DisplayText = "displayText";

        /// <summary>
        /// Const for Error reporting opt in
        /// </summary>
        public const string EnableErrorReporting = "ErrorRepOptIn";

        /// <summary>
        /// Constant value for one gb
        /// </summary>
        public const long ONEGB = 1073741824;

        public const string BlankVHD1FileName = @"blankdisk16gb.vhd";
        public const string BlankVHD2FileName = @"blankdisk60gb.vhd";
        /// <summary>
        /// user friendly file name for blank vhd disk1
        /// </summary>
        public static string BlankVHD1UserFriendlyFileName
        {
            get
            {
                return String.Concat(WpfResources.WPFResourceDictionary.BlankDisk1Name, @".vhd");
            }
        }

        /// <summary>
        /// user friendly file name for blank vhd disk2
        /// </summary>
        public static string BlankVHD2UserFriendlyFileName
        {
            get
            {
                return String.Concat(WpfResources.WPFResourceDictionary.BlankDisk2Name, @".vhd");
            }
        }

        public const string BlankVHDX1FileName = @"blankdisk16gb.vhdx";
        public const string BlankVHDX2FileName = @"blankdisk60gb.vhdx";
        /// <summary>
        /// user friendly file name for blank vhdx disk1
        /// </summary>
        public static string BlankVHDX1UserFriendlyFileName
        {
            get
            {
                return String.Concat(WpfResources.WPFResourceDictionary.BlankDisk1Name, @".vhdx");
            }
        }

        /// <summary>
        /// user friendly file name for blank vhdx disk2
        /// </summary>
        public static string BlankVHDX2UserFriendlyFileName
        {
            get
            {
                return String.Concat(WpfResources.WPFResourceDictionary.BlankDisk2Name, @".vhdx");
            }
        }

        public static string DBName
        {
            get
            {
                return RegistryUtils.ReadRegistryValue(SetupConstants.SqlSettingsRegKey, SetupConstants.DBNameRegistryValueName, null, false) as string;
            }
        }

        public static string WapDBName
        {
            get
            {
                return RegistryUtils.ReadRegistryValue(SetupConstants.WapSqlSettingsRegKey, SetupConstants.DBNameRegistryValueName, null, false, ignoreArchitecture: true) as string;
            }
        }


        public static bool DBOnRemoteServer
        {
            get
            {
                int? returnValue = RegistryUtils.ReadRegistryValue(SetupConstants.SqlSettingsRegKey, SetupConstants.OnRemoteRegistryValueName, 0, false) as int?;

                return returnValue.HasValue ? (returnValue.Value == 1) : false;
            }
        }
        public static bool WapDBOnRemoteServer
        {
            get
            {
                int? returnValue = RegistryUtils.ReadRegistryValue(SetupConstants.WapSqlSettingsRegKey, SetupConstants.OnRemoteRegistryValueName, 0, false, ignoreArchitecture: true) as int?;

                return returnValue.HasValue ? (returnValue.Value == 1) : false;
            }
        }


        public static string SqlInstanceName
        {
            get
            {
                return RegistryUtils.ReadRegistryValue(SetupConstants.SqlSettingsRegKey, SetupConstants.InstanceNameRegistryValueName, Environment.MachineName, false) as string;
            }
        }

        public static string WapSqlInstanceName
        {
            get
            {
                return RegistryUtils.ReadRegistryValue(SetupConstants.WapSqlSettingsRegKey, SetupConstants.InstanceNameRegistryValueName, null, false, ignoreArchitecture:true) as string;
            }
        }

        public static string SqlMachineName
        {
            get
            {
                return RegistryUtils.ReadRegistryValue(SetupConstants.SqlSettingsRegKey, SetupConstants.MachineNameRegistryValueName, Environment.MachineName, false) as string;
            }
        }

        public static string WapSqlMachineName
        {
            get
            {
                return RegistryUtils.ReadRegistryValue(SetupConstants.WapSqlSettingsRegKey, SetupConstants.MachineNameRegistryValueName, null, false, ignoreArchitecture: true) as string;
            }
        }

        /// <summary>
        /// Get service account from registry
        /// </summary>
        public static string VmmServiceAccount
        {
            get
            {
                return RegistryUtils.ReadRegistryValue(SetupConstants.ServerSetupInfoRegKey, SetupConstants.VmmServiceAccountValueName, null, false) as string;
            }
        }

        public static bool VmmServiceRunningAsLocalSystem
        {
            get
            {
                return String.Equals(SetupConstants.VmmServiceAccount, SetupConstants.LocalSystem);
            }
        }

        public static string GetServerSetupPath()
        {
            return Path.Combine(SetupConstants.GetServerInstallPath(), SetupConstants.SetupDirectory);
        }

        /// <summary>
        /// Get the install path for the server
        /// </summary>
        /// <returns></returns>
        public static string GetServerInstallPath()
        {
            if (SetupConstants.serverInstallationPath == null)
            {
                SetupConstants.serverInstallationPath = SetupConstants.GetInstallPathFromRegistry(SetupConstants.ServerSetupInfoRegKey);
            }

            return SetupConstants.serverInstallationPath;
        }

        /// <summary>
        /// Get the install path for the tenant extension
        /// </summary>
        /// <returns></returns>
        public static string GetTenantExtensionInstallPath()
        {
            if (SetupConstants.tenantExtensionInstallationPath == null)
            {
                SetupConstants.tenantExtensionInstallationPath = SetupConstants.GetInstallPathFromRegistry(SetupConstants.ServerSetupInfoRegKey);
            }

            return SetupConstants.tenantExtensionInstallationPath;
        }

        /// <summary>
        /// Get the install path for the extension common components
        /// </summary>
        /// <returns></returns>
        public static string GetExtensionCommonInstallPath()
        {
            if (SetupConstants.extensionCommonInstallationPath == null)
            {
                SetupConstants.extensionCommonInstallationPath = SetupConstants.GetInstallPathFromRegistry(SetupConstants.ServerSetupInfoRegKey);
            }

            return SetupConstants.extensionCommonInstallationPath;
        }

        /// <summary>
        /// Get the install path for the admin extension
        /// </summary>
        /// <returns></returns>
        public static string GetAdminExtensionInstallPath()
        {
            if (SetupConstants.adminExtensionInstallationPath == null)
            {
                SetupConstants.adminExtensionInstallationPath = SetupConstants.GetInstallPathFromRegistry(SetupConstants.ServerSetupInfoRegKey);
            }

            return SetupConstants.adminExtensionInstallationPath;
        }

        /// <summary>
        /// Get the path for the currently executing application.
        /// </summary>
        /// <returns>The path for the currently executing application.</returns>
        public static string GetExecutingApplicationLocation()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private static string GetInstallPathFromRegistry(string regKeyPath)
        {
            string defaultInstallPath = InputDefaults.InstallLocation;
            return RegistryUtils.ReadRegistryValue(regKeyPath, SetupConstants.InstallPath, defaultInstallPath, false) as string;
        }

        #region Forward Links
        /// <summary>
        /// Microsoft Update FAQ Link
        /// </summary>
        public const String MicrosoftUpdateFaqLink = @"http://go.microsoft.com/fwlink/?linkid=57191";

        /// <summary>
        /// Microsoft Update Privacy Link
        /// </summary>
        public const String MicrosoftUpdatePrivacyLink = @"http://go.microsoft.com/fwlink/?linkid=57190";

        /// <summary>
        /// "How do I configure distributed key management?" link
        /// </summary>
        public const String ConfigureDKMLink = @"http://go.microsoft.com/fwlink/?linkid=209609";

        /// <summary>
        /// "Which type of account should I use?" link
        /// </summary>
        public const String TypeOfAccountLink = @"http://go.microsoft.com/fwlink/?linkid=212847";

        public const String ComputerSystemRequirementsLink = @"http://go.microsoft.com/fwlink/?LinkID=614935&clcid=0x409";

        public const String ReleaseNotesLink = @"http://go.microsoft.com/fwlink/?LinkID=601052&clcid=0x409";

        public const String InstallationGuideLink = @"http://go.microsoft.com/fwlink/?linkid=209798";

        public const String ReadDocumentationLink = @"http://go.microsoft.com/fwlink/?linkid=209605";

        public const String SystemCenterOnlineLink = @"http://go.microsoft.com/fwlink/?linkid=209800";

        #endregion Forward Links

        #region Install Iis
        // Estimated time in seconds
        public const double InstallIisEstimatedTimeInSeconds = 180;

        // Success values 
        public const int InstallIisSuccessValue = 0;
        public const int InstallIisSuccessRebootValue = 0;
        #endregion

        #region Install Windows Installer 4.5
        // Estimated time in seconds
        public const double InstallInstallerEstimatedTimeInSeconds = 60;

        // Success values for MSI install
        public const int InstallInstallerSuccessValue =
            (int)NativeMethods.InstallErrorLevel.Error_Success; // ERROR_SUCCESS;
        public const int InstallInstallerSuccessRebootValue =
            (int)NativeMethods.InstallErrorLevel.Error_Success_Reboot_Required; // ERROR_SUCCESS_REBOOT_REQUIRED;
        #endregion

        #region Database Names
        public const string SqlServerDefaultInstanceName = "MSSQLSERVER";
        public const string WapSqlServerDefaultInstanceName = "MSSQLSERVER";
        #endregion

        #region SQL Version Numbers
        public const string RequiredSqlVersion = "9.0.4053.0";
        public const string RequiredSqlOSVersion = "5.2";
        #endregion
    }

    public enum InstallItem
    {
        CMPServer,
        WAPExtensionCommon,
        TenantWAPExtension,
        AdminWAPExtension,
        FinalConfiguration
    }

    public interface IEnumHelper<EnumType>
    {
        string GetName(EnumType value);
    }

    /// <summary>
    /// Helper class to look up enums in a resource table provided by the passed in
    /// resource manager.
    /// NOTE: Any resources for enums using this helper class must be saved in the format:
    /// "EnumType"."EnumValue" - e.g. CarmineObjectType.VM must have a resource ID of
    /// "CarmineObjectTypeVM".  These will be checked at static initialization time, providing
    /// more testable resource handling than using a developer-provided string table.
    /// </summary>
    /// <typeparam name="EnumType"></typeparam>
    public class LocalizableEnumHelper<EnumType> : IEnumHelper<EnumType>
    {
        //For each langauge, there is a dictionary which stores each enum's type with its string representation in that language.
        private Dictionary<CultureInfo, Dictionary<EnumType, string>> cultureToEnumTableLookup;
        private Dictionary<EnumType, EnumType[]> additionalCombinedValues;
        protected ResourceManager localResourceManager;
        private Object syncObject = new object();
        private string suffix;

        /// <summary>
        /// Look up text by "{Enum type name}{Enum value name}"
        /// </summary>
        public LocalizableEnumHelper(ResourceManager resourceManager)
            : this(resourceManager, String.Empty)
        {
        }

        /// <summary>
        /// Look up text by "{Enum type name}{Enum value name}{Suffix}"
        /// </summary>
        public LocalizableEnumHelper(ResourceManager resourceManager, string suffix)
        {
            this.suffix = suffix;
            this.localResourceManager = resourceManager;
            this.additionalCombinedValues = new Dictionary<EnumType, EnumType[]>();
            this.cultureToEnumTableLookup = new Dictionary<CultureInfo, Dictionary<EnumType, string>>();
            this.LoadEnumValuesForCurrentCulture();
        }

        protected void LoadEnumValuesForCurrentCulture()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;
            Dictionary<EnumType, string> enumStringTable = new Dictionary<EnumType, string>();

            string[] enumNames = Enum.GetNames(typeof(EnumType));
            EnumType[] enumValues = (EnumType[])Enum.GetValues(typeof(EnumType));

            for (int i = 0; i < enumNames.Length; ++i)
            {
                //as long as the value has a name, we're okay.
                //(we can't tell the difference between two enums with the same value)
                if (!enumStringTable.ContainsKey(enumValues[i]))
                {
                    string key = typeof(EnumType).Name + enumNames[i] + this.suffix;
                    string localizedName = this.localResourceManager.GetString(key);
                    AppAssert.AssertNotNull(localizedName,
                        string.Format("Each enum in {0} must have a localizable string name. Resource {1} in {2} for value {3} undefined",
                        typeof(EnumType).FullName, key, this.localResourceManager.BaseName, enumNames[i]));

                    enumStringTable.Add(enumValues[i], localizedName);
                }
            }

            this.PopulateAdditionalCombinedValues(enumStringTable);

            lock (this.syncObject)
            {
                this.cultureToEnumTableLookup[currentCulture] = enumStringTable;
            }
        }

        protected void PopulateAdditionalCombinedValues(Dictionary<EnumType, string> enumStringTable)
        {
            //Lock so additionalCombinedValues keys collection does not change.
            lock (this.syncObject)
            {
                foreach (EnumType key in this.additionalCombinedValues.Keys)
                {
                    if (!enumStringTable.ContainsKey(key))
                    {
                        List<string> combinedLocalizedEnums = new List<string>();
                        EnumType[] listOfEnumsToCombine = this.additionalCombinedValues[key];

                        foreach (EnumType enumType in listOfEnumsToCombine)
                        {
                            //Read each enum in the list and save its localized text into the new text list.
                            combinedLocalizedEnums.Add(enumStringTable[enumType]);
                        }

                        //Add combined items as a new individual item under the given key.
                        enumStringTable[key] = StringHelper.ListToCommaSeparatedString(combinedLocalizedEnums);
                    }
                }
            }
        }

        /// <summary>
        /// Takes a unique EnumType with an associated array of enums which
        /// will be registered as a new combined type, displayed as a comma delimited list
        /// of the individual enums in the list.
        /// </summary>
        /// <param name="enumValue">A unique identifier for this combination of enumtypes</param>
        /// <param name="combinedEnumList">the list of enums which are to be treated as one new enum type</param>
        public void AddCombinedValue(EnumType enumValue, EnumType[] combinedEnumList)
        {
            //Lock so because writing to additionalCombinedValues and so that
            //cultureToEnumTableLookup keys do not change.
            lock (this.syncObject)
            {
                if (!this.additionalCombinedValues.ContainsKey(enumValue))
                {
                    this.additionalCombinedValues.Add(enumValue, combinedEnumList);

                    //Add the additional items to all loaded cultures. Cultures loaded in the future
                    //will have the items added at load time.
                    foreach (CultureInfo culture in this.cultureToEnumTableLookup.Keys)
                    {
                        this.PopulateAdditionalCombinedValues(this.cultureToEnumTableLookup[culture]);
                    }
                }
            }
        }

        public string GetName(EnumType state)
        {
            CultureInfo requestCulture = Thread.CurrentThread.CurrentUICulture;
            return this.GetName(state, requestCulture);
        }

        /// <summary>
        /// Get the localized name for the given object type
        /// </summary>
        /// <param name="state">Enum value</param>
        /// <param name="requestCulture">CultureInfo for localization</param>
        /// <returns></returns>
        public string GetName(EnumType state, CultureInfo requestCulture)
        {
            Dictionary<EnumType, string> enumStringTable = null;

            if (!this.cultureToEnumTableLookup.ContainsKey(requestCulture))
            {
                this.LoadEnumValuesForCurrentCulture();
                AppAssert.Assert(this.cultureToEnumTableLookup.ContainsKey(requestCulture), "Could not find localized enum dictionary for the current culture after calling LoadEnumValuesForCurrentCulture");
            }

            enumStringTable = this.cultureToEnumTableLookup[requestCulture];

            AppAssert.Assert(enumStringTable.ContainsKey(state), "Request for an enum state was made without that state having been loaded.");
            return enumStringTable[state];
        }
    }

    public class TypeConverterEnumHelper<EnumType> : TypeConverter
        where EnumType : struct
    {
        private IEnumHelper<EnumType> enumHelper;

        protected TypeConverterEnumHelper(IEnumHelper<EnumType> enumHelper)
        {
            this.enumHelper = enumHelper;
        }

        #region TypeConverter overloads
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.Equals(typeof(string)) || sourceType.Equals(typeof(EnumType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.Equals(typeof(string)) || destinationType.Equals(typeof(EnumType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (!this.CanConvertFrom(context, value.GetType()))
            {
                throw new NotSupportedException();
            }

            if (value.GetType().Equals(typeof(string)))
            {
                return Enum.Parse(typeof(EnumType), (string)value, true);
            }
            else
            {
                return value;
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (!this.CanConvertTo(context, destinationType))
            {
                throw new NotSupportedException();
            }

            EnumType? enumValue = value as EnumType?;
            if (enumValue.HasValue && destinationType.Equals(typeof(string)))
            {
                return this.enumHelper.GetName(enumValue.Value);
            }
            else
            {
                return value;
            }
        }
        #endregion
    }

    public class InstallItemDisplayTitleEnumHelper : TypeConverterEnumHelper<InstallItem>
    {
        private static LocalizableEnumHelper<InstallItem> helper =
            new LocalizableEnumHelper<InstallItem>(CMP.Setup.Properties.Resources.ResourceManager);

        public InstallItemDisplayTitleEnumHelper()
            : base(helper)
        {
        }

        public static string GetName(InstallItem installItem)
        {
            return InstallItemDisplayTitleEnumHelper.helper.GetName(installItem);
        }

    }
}
