// Copyright (c) Microsoft Corporation.  All rights reserved.
using System.Windows;
using System;
using CMP.Setup.SetupFramework;
using CMP.Setup;

namespace WpfResources
{
    internal class WPFResourceDictionary
    {
        private static ResourceDictionary resources;
        static WPFResourceDictionary()
        {
            resources = ResourcesHelper.LoadInternal("WPFResourceDictionary.xaml");
        }

        public static string ServerFeatureName
        {
            get
            {
                return (string)resources["serverFeatureName"];
            }
        }

        public static string ExtensionCommonFeatureName
        {
            get
            {
                return (string)resources["extensionCommonFeatureName"];
            }
        }

        public static string TenantExtensionFeatureName
        {
            get
            {
                return (string)resources["tenantExtensionFeatureName"];
            }
        }

        public static string AdminExtensionFeatureName
        {
            get
            {
                return (string)resources["adminExtensionFeatureName"];
            }
        }
        

        public static string PortalFeatureName
        {
            get
            {
                return (string)resources["portalFeatureName"];
            }
        }
        
        public static string WindowsUpdateContentFolderDefault
        {
            get
            {
                return (string)resources["windowsUpdateContentFolderDefault"];
            }
        }

        public static string VirtualMachineManagerLibrarySharePathDefault
        {
            get
            {
                return (string)resources["virtualMachineManagerLibrarySharePathDefault"];
            }
        }

        public static string VirtualMachineManagerLibraryFiles
        {
            get
            {
                return (string)resources["virtualMachineManagerLibraryFiles"];
            }
        }

        public static string ResultLabelWarning
        {
            get
            {
                return (string)resources["resultLabelWarning"];
            }
        }

        public static string AnotherSetupRunningOnThisMachineMessage
        {
            get
            {
                return (string)resources["AnotherSetupRunningOnThisMachineMessage"];
            }
        }

        public static string ResultLabelSuccess
        {
            get
            {
                return (string)resources["resultLabelSuccess"];
            }
        }

        public static string ResultLabelError
        {
            get
            {
                return (string)resources["resultLabelError"];
            }
        }

        public static string ReportingImagesFolder
        {
            get
            {
                return (string)resources["reportingImagesFolder"];
            }
        }

        public static string ProductTitle
        {
            get
            {
                return (string)resources["productTitle"];
            }
        }

        public static string WizardTitle
        {
            get
            {
                return (string)resources["wizardTitle"];
            }
        }

        public static string PrerequisitesCheckWarning
        {
            get
            {
                return (string)resources["PrerequisitesCheckWarning"];
            }
        }

        public static string PrerequisitesCheckPassed
        {
            get
            {
                return (string)resources["PrerequisitesCheckPassed"];
            }
        }

        public static string PrerequisitesCheckFailed
        {
            get
            {
                return (string)resources["PrerequisitesCheckFailed"];
            }
        }

        public static string NextButtonText
        {
            get
            {
                return (string)resources["nextButtonText"];
            }
        }

        public static string MissingFileFormatString
        {
            get
            {
                return (string)resources["missingFileFormatString"];
            }
        }

        public static string LibraryShareDescription
        {
            get
            {
                return (string)resources["LibraryShareDescription"];
            }
        }

        public static string InvalidArgument
        {
            get
            {
                return (string)resources["invalidArgument"];
            }
        }

        public static string DifferentVersionInstalled
        {
            get
            {
                return (string)resources["DifferentVersionInstalled"];
            }
        }

        public static string InstallButton
        {
            get
            {
                return (string)resources["installButton"];
            }
        }
        public static string UninstallButton
        {
            get
            {
                return (string)resources["uninstallButton"];
            }
        }
        public static string Uninstall
        {
            get
            {
                return (string)resources["uninstall"];
            }
        }
        public static string UninstallResults
        {
            get
            {
                return (string)resources["uninstallresults"];
            }
        }
        public static string InstallTitleText
        {
            get
            {
                return (string)resources["InstallTitleText"];
            }
        }
        public static string AddRemoveComponentTitleText
        {
            get
            {
                return (string)resources["AddRemoveComponentTitleText"];
            }
        }
        public static string InstallSummaryText
        {
            get
            {
                return (string)resources["InstallSummaryText"];
            }
        }
        public static string RemoveFeatureSummaryText
        {
            get
            {
                return (string)resources["RemoveFeatureSummaryText"];
            }
        }

        public static string HelpUsage
        {
            get
            {
                return (string)resources["helpUsage"];
            }
        }

        public static string HelpTitle
        {
            get
            {
                return (string)resources["helpTitle"];
            }
        }

        public static string HelpFormatString
        {
            get
            {
                return (string)resources["helpFormatString"];
            }
        }

        public static string HelpFile
        {
            get
            {
                return (string)resources["helpFile"];
            }
        }

        public static string HelperObjectsLocation
        {
            get
            {
                return (string)resources["helperObjectsLocation"];
            }
        }

        public static string HelperCachePath
        {
            get
            {
                return (string)resources["helperCachePath"];
            }
        }

        public static string FinishScreenNextButtonText
        {
            get
            {
                return (string)resources["finishScreenNextButtonText"];
            }
        }

        public static string FinishButtonText
        {
            get
            {
                return (string)resources["finishButtonText"];
            }
        }

        public static string CancelButtonText
        {
            get
            {
                return (string)resources["cancelButtonText"];
            }
        }

        public static string DescripOnANewCluster
        {
            get
            {
                return (string)resources["DescripOnANewCluster"];
            }
        }

        public static string DescripOnHASecondNode
        {
            get
            {
                return (string)resources["DescripOnHASecondNode"];
            }
        }

        public static string DescripOnLastHANode
        {
            get
            {
                return (string)resources["DescripOnLastHANode"];
            }
        }

        public static string DescripOneHANode
        {
            get
            {
                return (string)resources["DescripOneHANode"];
            }
        }

        public static string CannotDetectHANodeCount
        {
            get
            {
                return (string)resources["CannotDetectHANodeCount"];
            }
        }

        public static string NotLastHANode
        {
            get
            {
                return (string)resources["NotLastHANode"];
            }
        }

        public static string UseExistingHAMMDB
        {
            get
            {
                return (string)resources["UseExistingHAMMDB"];
            }
        }

        public static string DescriptionRemoveServerWithClient
        {
            get
            {
                return (string)resources["DescriptionRemoveServerWithClient"];
            }
        }

        public static string SetupMessageBoxTitle
        {
            get
            {
                return (string)resources["SetupMessageBoxTitle"];
            }
        }

        public static string CancelText
        {
            get
            {
                return (string)resources["cancelText"];
            }
        }

        public static string UninstallCanNotBeCanceled
        {
            get
            {
                return (string)resources["UninstallCanNotBeCanceled"];
            }
        }
        public static string ProgressCancelling
        {
            get
            {
                return (string)resources["ProgressCancelling"];
            }
        }
        
        public static string ProgressCanceled
        {
            get
            {
                return (string)resources["ProgressCanceled"];
            }
        }

        public static string CancelDuringInstalProgressText
        {
            get
            {
                return (string)resources["CancelDuringInstalProgressText"];
            }
        }

        public static string SetupSuccessful
        {
            get
            {
                return (string)resources["SetupSuccessful"];
            }
        }

        public static string SetupSucceededWithWarnings
        {
            get
            {
                return (string)resources["SetupSucceededWithWarnings"];
            }
        }

        public static string SetupFailed
        {
            get
            {
                return (string)resources["SetupFailed"];
            }
        }

        public static string UninstallSuccessful
        {
            get
            {
                return (string)resources["UninstallSuccessful"];
            }
        }

        public static string UninstallSucceededWithWarnings
        {
            get
            {
                return (string)resources["UninstallSucceededWithWarnings"];
            }
        }

        public static string ArchTypeFolderx86
        {
            get
            {
                return (string)resources["archTypeFolderx86"];
            }
        }

        public static string ArchTypeFolderi386
        {
            get
            {
                return (string)resources["archTypeFolderi386"];
            }
        }

        public static string ArchTypeFolder64
        {
            get
            {
                return (string)resources["archTypeFolder64"];
            }
        }

        public static string AgentManagementPath
        {
            get
            {
                return (string)resources["agentManagementPath"];
            }
        }

        public static string AgentManagementFolder
        {
            get
            {
                return (string)resources["agentManagementFolder"];
            }
        }

        public static string AddRemoveProgramsPublisher
        {
            get
            {
                return (string)resources["addRemoveProgramsPublisher"];
            }
        }

        public static string AddRemoveProgramsHelpLink
        {
            get
            {
                return (string)resources["addRemoveProgramsHelpLink"];
            }
        }

        public static string SizeInGBFormatString
        {
            get
            {
                return (string)resources["sizeInGBFormatString"];
            }
        }

        public static string CredentialsInvalid
        {
            get
            {
                return (string)resources["credentialsInvalid"];
            }
        }

        public static string CanNotReadVersion
        {
            get
            {
                return (string)resources["CanNotReadVersion"];
            }
        }
        public static string InvalidSqlType
        {
            get
            {
                return (string)resources["InvalidSqlType"];
            }
        }
        public static string InvalidSqlCombination
        {
            get
            {
                return (string)resources["InvalidSqlCombination"];
            }
        }
        public static string InvalidSqlVersion
        {
            get
            {
                return (string)resources["InvalidSqlVersion"];
            }
        }
        public static string DatabaseAlreadyExists
        {
            get
            {
                return (string)resources["DatabaseAlreadyExists"];
            }
        }
        public static string UnsupportedOSForSQL
        {
            get
            {
                return (string)resources["UnsupportedOSForSQL"];
            }
        }
        public static string InvalidSQLInstance
        {
            get
            {
                return (string)resources["InvalidSQLInstance"];
            }
        }
        public static string CannotCreateDatabaseOnSqlInstance
        {
            get
            {
                return (string)resources["CannotCreateDatabaseOnSqlInstance"];
            }
        }
        public static string ValidSQLInstance
        {
            get
            {
                return (string)resources["ValidSQLInstance"];
            }
        }
        public static string ValidSQLReportingInstance
        {
            get
            {
                return (string)resources["ValidSQLReportingInstance"];
            }
        }
        public static string InvalidSQLReportingInstance
        {
            get
            {
                return (string)resources["InvalidSQLReportingInstance"];
            }
        }
        public static string InvalidSQLReportingInstanceBindings
        {
            get
            {
                return (string)resources["InvalidSQLReportingInstanceBindings"];
            }
        }
        public static string InvalidSQLReportingInstanceLoginName
        {
            get
            {
                return (string)resources["InvalidSQLReportingInstanceLoginName"];
            }
        }
        public static string NoInstancesFound
        {
            get
            {
                return (string)resources["NoInstancesFound"];
            }
        }
        public static string SamePortNumberCannotBeUsed
        {
            get
            {
                return (string)resources["SamePortNumberCannotBeUsed"];
            }
        }
        public static string NoSQLInstancesFoundDescription
        {
            get
            {
                return (string)resources["NoSQLInstancesFoundDescription"];
            }
        }
        public static string NoReportingInstancesFoundDescription
        {
            get
            {
                return (string)resources["NoReportingInstancesFoundDescription"];
            }
        }
        public static string PreviousVersionCannotBeUsed
        {
            get
            {
                return (string)resources["PreviousVersionCannotBeUsed"];
            }
        }

        public static string CommandlineUsage
        {
            get
            {
                return (string)resources["CommandlineUsage"];
            }
        }
        public static string BlankDisk1Name
        {
            get
            {
                return (string)resources["BlankDisk1Name"];
            }
        }
        public static string BlankDisk1Description
        {
            get
            {
                return (string)resources["BlankDisk1Description"];
            }
        }
        public static string BlankDisk2Name
        {
            get
            {
                return (string)resources["BlankDisk2Name"];
            }
        }
        public static string BlankDisk2Description
        {
            get
            {
                return (string)resources["BlankDisk2Description"];
            }
        }

        public static string Change
        {
            get
            {
                return (string)resources["Change"];
            }
        }

        public static string ManagementServer
        {
            get
            {
                return (string)resources["ManagementServer"];
            }
        }
        public static string ManagementConsole
        {
            get
            {
                return (string)resources["ManagementConsole"];
            }
        }
        public static string WebPortal
        {
            get
            {
                return (string)resources["WebPortal"];
            }
        }
        public static string VirtualizationManagement
        {
            get
            {
                return (string)resources["VirtualizationManagement"];
            }
        }

        public static string ProgramFilesLocation
        {
            get
            {
                return (string)resources["ProgramFilesLocation"];
            }
        }
        public static string UpdateFilesLocation
        {
            get
            {
                return (string)resources["UpdateFilesLocation"];
            }
        }
        public static string SqlServerDatabaseInstance
        {
            get
            {
                return (string)resources["SqlServerDatabaseInstance"];
            }
        }
        public static string AdministratorsAccount
        {
            get
            {
                return (string)resources["AdministratorsAccount"];
            }
        }
        public static string SqlServerReportingServicesInstance
        {
            get
            {
                return (string)resources["SqlServerReportingServicesInstance"];
            }
        }
        public static string VmFilesLocation
        {
            get
            {
                return (string)resources["VmFilesLocation"];
            }
        }
        public static string DownloadUpdatesIndividually
        {
            get
            {
                return (string)resources["DownloadUpdatesIndividually"];
            }
        }

        public static string RemoveData
        {
            get
            {
                return (string)resources["RemoveData"];
            }
        }
        public static string RetainData
        {
            get
            {
                return (string)resources["RetainData"];
            }
        }
        public static string DataDisposition
        {
            get
            {
                return (string)resources["DataDisposition"];
            }
        }
        public static string InstallationProgressText
        {
            get
            {
                return (string)resources["InstallationProgressText"];
            }
        }
        public static string UninstallationProgressText
        {
            get
            {
                return (string)resources["UninstallationProgressText"];
            }
        }
        public static string RollbackProgressText
        {
            get
            {
                return (string)resources["RollbackProgressText"];
            }
        }
        public static string FinishErrorFormat
        {
            get
            {
                return (string)resources["FinishErrorFormat"];
            }
        }
        public static string FinishWarningFormat
        {
            get
            {
                return (string)resources["FinishWarningFormat"];
            }
        }
        public static string CanNotStartServiceMessage
        {
            get
            {
                return (string)resources["CanNotStartServiceMessage"];
            }
        }
        public static string CanNotLoadManagementPacks
        {
            get
            {
                return (string)resources["CanNotLoadManagementPacks"];
            }
        }
        public static string PromptForSetupFiles
        {
            get
            {
                return (string)resources["PromptForSetupFiles"];
            }
        }
        public static string ThreeLetterLanguageCode
        {
            get
            {
                return (string)resources["ThreeLetterLanguageCode"];
            }
        }

        public static string OutOfSpaceOnSqlDrive
        {
            get
            {
                return (string)resources["OutOfSpaceOnSqlDrive"];
            }
        }

        public static string ExistingSqlFilesMessage
        {
            get
            {
                return (string)resources["ExistingSqlFilesMessage"];
            }
        }
        public static string ExistingFilesErrorText
        {
            get
            {
                return (string)resources["ExistingFilesErrorText"];
            }
        }
        public static string ExistingDatabaseErrorText
        {
            get
            {
                return (string)resources["ExistingDatabaseErrorText"];
            }
        }
        public static string UnableToReadDataErrorText
        {
            get
            {
                return (string)resources["UnableToReadDataErrorText"];
            }
        }
        public static string OutOfDiskSpaceError
        {
            get
            {
                return (string)resources["OutOfDiskSpaceError"];
            }
        }
        public static string InvalidSqlServiceState
        {
            get
            {
                return (string)resources["InvalidSqlServiceState"];
            }
        }

        public static string CouldNotReadData
        {
            get
            {
                return (string)resources["CouldNotReadData"];
            }
        }

        #region PageTitles

        public static string GettingStartedStepTitle
        {
            get
            {
                return (string)resources["GettingStartedStepTitle"];
            }
        }
        public static string PrerequisitesStepTitle
        {
            get
            {
                return (string)resources["PrerequisitesStepTitle"];
            }
        }
        public static string ConfigurationStepTitle
        {
            get
            {
                return (string)resources["ConfigurationStepTitle"];
            }
        }
        public static string InstallingStepTitle
        {
            get
            {
                return (string)resources["InstallingStepTitle"];
            }
        }
        public static string CompleteStepTitle
        {
            get
            {
                return (string)resources["CompleteStepTitle"];
            }
        }

        #endregion //Page Titles

        #region Localized strings for ComponentsPage

        public static string SelectToRemove
        {
            get
            {
                return (string)resources["SelectToRemove"];
            }
        }

        public static string SelectToAdd
        {
            get
            {
                return (string)resources["SelectToAdd"];
            }
        }

        public static string AgentBlockServer
        {
            get
            {
                return (string)resources["AgentBlockServer"];
            }
        }
        public static string OSBlockServer
        {
            get
            {
                return (string)resources["OSBlockServer"];
            }
        }

        public static string UpgradeInfo
        {
            get
            {
                return (string)resources["UpgradeInfo"];
            }
        }

        #endregion // Localized strings for ComponentsPage

        #region Localized strings for ClusterConfigPage
        public static string StaticIPConfig
        {
            get
            {
                return (string)resources["StaticIPConfig"];
            }
        }
        public static string AutoIPConfig
        {
            get
            {
                return (string)resources["AutoIPConfig"];
            }
        }
        #endregion // Localized strings for ClusterConfigPage

        #region Localized strings for InstallationLocationPage
        public static string FreeSpace
        {
            get
            {
                return (string)resources["FreeSpace"];
            }
        }
        #endregion // Localized strings for InstallationLocationPage

        #region Localized strings for the Installable Prereq page

        public static string InstallPrereqCount
        {
            get
            {
                return (string)resources["InstallPrereqCount"];
            }
        }

        public static string InstallPrereqDownloading
        {
            get
            {
                return (string)resources["InstallPrereqDownloading"];
            }
        }

        public static string InstallPrereqCancelling
        {
            get
            {
                return (string)resources["InstallPrereqCancelling"];
            }
        }

        public static string InstallPrereqSucceeded
        {
            get
            {
                return (string)resources["InstallPrereqSucceeded"];
            }
        }

        public static string InstallPrereqFailed
        {
            get
            {
                return (string)resources["InstallPrereqFailed"];
            }
        }

        public static string InstallPrereqCancelled
        {
            get
            {
                return (string)resources["InstallPrereqCancelled"];
            }
        }

        public static string InstallPrereqCannotContinue
        {
            get
            {
                return (string)resources["InstallPrereqCannotContinue"];
            }
        }

        public static string InstallPrereqTroubleshoot
        {
            get
            {
                return (string)resources["InstallPrereqTroubleshoot"];
            }
        }

        public static string InstallPrereqAllInstalled
        {
            get
            {
                return (string)resources["InstallPrereqAllInstalled"];
            }
        }

        public static string InstallPrereqClickNext
        {
            get
            {
                return (string)resources["InstallPrereqClickNext"];
            }
        }

        public static string InstallPrereqInstalling
        {
            get
            {
                return (string)resources["InstallPrereqInstalling"];
            }
        }

        public static string InstallPrereqInstallingDetails
        {
            get
            {
                return (string)resources["InstallPrereqInstallingDetails"];
            }
        }

        public static string InstallPrereqAbortText
        {
            get
            {
                return (string)resources["InstallPrereqAbortText"];
            }
        }
        #endregion // Localized strings for the Installable Prereq page

        #region Strings for the SQL Express Install

        public static string SqlExpressInstallFolder
        {
            get
            {
                return (string)resources["SqlExpressInstallFolder"];
            }
        }

        public static string SqlExpressInstanceName
        {
            get
            {
                return (string)resources["SqlExpressInstanceName"];
            }
        }        

        public static string SqlExpressWarningType1
        {
            get
            {
                return (string)resources["SqlExpressWarningType1"];
            }
        }

        public static string SqlExpressWarningType2
        {
            get
            {
                return (string)resources["SqlExpressWarningType2"];
            }
        }

        public static string SqlExpressWarningType3
        {
            get
            {
                return (string)resources["SqlExpressWarningType3"];
            }
        }

        public static string SqlExpressWarningType4
        {
            get
            {
                return (string)resources["SqlExpressWarningType4"];
            }
        }

        public static string SqlExpressWarningType5
        {
            get
            {
                return (string)resources["SqlExpressWarningType5"];
            }
        }

        public static string SqlServerCheck
        {
            get
            {
                return (string)resources["SqlServerCheck"];
            }
        }

        public static string SqlReportingCheck
        {
            get
            {
                return (string)resources["SqlReportingCheck"];
            }
        }      

        public static string Msi45Name
        {
            get
            {
                return (string)resources["Msi45Name"];
            }
        }

        public static string Msi45Title
        {
            get
            {
                return (string)resources["Msi45Title"];
            }
        }

        public static string PowerShell10Name
        {
            get
            {
                return (string)resources["PowerShell10Name"];
            }
        }

        public static string PowerShell10Title
        {
            get
            {
                return (string)resources["PowerShell10Title"];
            }
        }

        #endregion // Strings for the SQL Express Install

        #region Strings for IIS Install

        public static string CantInstallIis
        {
            get
            {
                return (string)resources["CantInstallIis"];
            }
        }

        #endregion // Strings for IIS Install

        #region Strings for the WSUS Prerequisite Checks

        public static string CheckWasPassed
        {
            get
            {
                return (string)resources["CheckWasPassed"];
            }
        }

        public static string CheckWasNotPassed
        {
            get
            {
                return (string)resources["CheckWasNotPassed"];
            }
        }

        #endregion // Strings for the WSUS Prerequisite Checks

        #region Strings for the Eula Page

        public static string PlsReadLicense
        {
            get
            {
                return (string)resources["PlsReadLicense"];
            }
        }
        public static string PlsReadNotice
        {
            get
            {
                return (string)resources["PlsReadNotice"];
            }
        }

        public static string AgreeWithLicense
        {
            get
            {
                return (string)resources["AgreeWithLicense"];
            }
        }

        public static string AgreeWithNotice
        {
            get
            {
                return (string)resources["AgreeWithNotice"];
            }
        }

        #endregion // Strings for the EulaPage

        #region Strings for the Components Page

        public static string AlreadyInstalled
        {
            get
            {
                return (string)resources["AlreadyInstalled"];
            }
        }

        #endregion // Strings for the AccountConfigurationPage

        #region Strings for the SelectDatabaseServer Page

        #endregion // Strings for the SelectDatabaseServer Page

        #region Strings for the AccountConfiguration Page

        public static string TopContainerNameAlreadyInDatabase
        {
            get
            {
                return (string)resources["TopContainerNameAlreadyInDatabase"];
            }
        }

        #endregion // Strings for the AccountConfiguration Page

        #region Strings for the Virtualization Management Files Location

        public static string VirtualizationFreeSpaceErrorMessage
        {
            get
            {
                return (string)resources["VirtualizationFreeSpaceErrorMessage"];
            }
        }

        public static string VirtualizationRootVolumeErrorMessage
        {
            get
            {
                return (string)resources["VirtualizationRootVolumeErrorMessage"];
            }
        }

        #endregion // Strings for the Virtualization Management Files Location

        #region Strings for the Upgrade Information Page

        public static string WindowsServer2008R2Detected
        {
            get
            {
                return (string)resources["WindowsServer2008R2Detected"];
            }
        }

        #endregion

        #region Strings for the ReadyToInstall Page
        public static string InstallationLocation
        {
            get
            {
                return (string)resources["InstallationLocation"];
            }
        }

        public static string DatabaseInformation
        {
            get
            {
                return (string)resources["DatabaseInformation"];
            }
        }

        public static string DatabaseInformationValue
        {
            get
            {
                return (string)resources["DatabaseInformationValue"];
            }
        }

        public static string DatabaseUpgradeInformationValue
        {
            get
            {
                return (string)resources["DatabaseUpgradeInformationValue"];
            }
        }

        public static string DatabaseUpdateInformationValue
        {
            get
            {
                return (string)resources["DatabaseUpdateInformationValue"];
            }
        }

        public static string ServiceAccountInformation
        {
            get
            {
                return (string)resources["ServiceAccountInformation"];
            }
        }

        public static string LocalSystemAccount
        {
            get
            {
                return (string)resources["LocalSystemAccount"];
            }
        }

        public static string CommunicationPorts
        {
            get
            {
                return (string)resources["CommunicationPorts"];
            }
        }

        public static string WcfPortDescription
        {
            get
            {
                return (string)resources["WcfPortDescription"];
            }
        }

        public static string WsmanPortDescription
        {
            get
            {
                return (string)resources["WsmanPortDescription"];
            }
        }

        public static string BitsPortDescription
        {
            get
            {
                return (string)resources["BitsPortDescription"];
            }
        }

        public static string IndigoNETTCPPortDescription
        {
            get
            {
                return (string)resources["IndigoNETTCPPortDescription"];
            }
        }

        public static string IndigoHTTPsPortDescription
        {
            get
            {
                return (string)resources["IndigoHTTPsPortDescription"];
            }
        }

        public static string IndigoHTTPPortDescription
        {
            get
            {
                return (string)resources["IndigoHTTPPortDescription"];
            }
        }

        public static string HttpPortDescription
        {
            get
            {
                return (string)resources["HttpPortDescription"];
            }
        }

        public static string LibraryShareInformation
        {
            get
            {
                return (string)resources["LibraryShareInformation"];
            }
        }

        public static string OptInMUDescription
        {
            get
            {
                return (string)resources["OptInMUDescription"];
            }
        }

        public static string OptInMUYes
        {
            get
            {
                return (string)resources["OptInMUYes"];
            }
        }

        public static string OptInMUNo
        {
            get
            {
                return (string)resources["OptInMUNo"];
            }
        }

        public static string AddedFeatures
        {
            get
            {
                return (string)resources["AddedFeatures"];
            }
        }

        public static string RemovedFeatures
        {
            get
            {
                return (string)resources["RemovedFeatures"];
            }
        }

        public static string DatabaseOptions
        {
            get
            {
                return (string)resources["DatabaseOptions"];
            }
        }

        #endregion
    }
}