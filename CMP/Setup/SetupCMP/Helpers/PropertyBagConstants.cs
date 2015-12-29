//-----------------------------------------------------------------------
// <copyright file="PropertyBagConstants.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>Provides property bag constants for use in the setup.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Contains Property bag constants to use in the setup
    /// </summary>
    public static class PropertyBagConstants
    {
        public const string ItemsToInstall = "ItemsToInstall";
        public const string AfterGrantSetupUserDBAccess = "AfterGrantSetupUserDBAccess";
        public const string WapAfterGrantSetupUserDBAccess = "WapAfterGrantSetupUserDBAccess";

        // Valid for switches on the command line
        public const string LocationOfSetupFiles = "LocationOfSetupFiles";
        public const string InstallationLocation = "InstallationLocation";
        public const string ExistingSqlFilesMessage = "existingSqlFilesMessage";
        public const string ExistingSqlFiles = "ExistingSqlFiles";
        public const string ExistingSqlDatabaseName = "ExistingSqlDatabaseName";

        public const string AddRemoveProgramFilesPath = "arppath";

        public const string ScreensLoaded = "ScreensLoaded";

        // SQL
        public const string SqlDatabaseDetected = "SqlDatabaseDetected";

        public static string GetSqlDatabaseEmpty(bool isWap)
        {
            return isWap ? WapSqlDatabaseEmpty : SqlDatabaseEmpty;
        }
        public const string SqlDatabaseEmpty = "SqlDatabaseEmpty";
        public const string WapSqlDatabaseEmpty = "WapSqlDatabaseEmpty";

        public const string SqlDatabaseVersion = "SqlDatabaseVersion";
        public const string BackupSqlDatabase = "BackupSqlDatabase";
        
        // SKU tracking...
        public const string SqlExpressSku = "SqlExpressSku";

        // Log constants
        public const string SendSetupLogsToWatson = "SendSetupLogsToWatson";
        public const string ErrorLogFile = "ErrorLogFile";
        public const string MachineStatusFile = "MachineStatusFile";
        public const string GeneralLogFile = "GeneralLogFile";
        public const string SetupStartTime = "SetupStartTime";

        public const string Upgrade = "upgrade";
        public const string VhdVersionPreparation = "prep";
        public const string VhdVersionConfiguration = "config";

        // CommandLine Switches
        public const string FullUninstall = "fulluninstall";
        public const string Uninstall = "uninstall";
        public const string Components = "Components";

        // Components - Use caps!
        public const string ComponentList = "SERVER,EXTENSIONCOMMON,TENANTEXTENSION,ADMINEXTENSION";
        public const string Server = "SERVER";
        public const string ExtensionCommon = "EXTENSIONCOMMON";
        public const string TenantExtension = "TENANTEXTENSION";
        public const string AdminExtension = "ADMINEXTENSION";

        public const string CMPServer = "CMPServer";

        // Certificate for encryption
        public const string CMPCertificateThumbprint = "CMPCertificateThumbprint";


        public const string MSDeploy = "MSDeploy";
        public const string SQLSysClrTypes = "SQLSysClrTypes";
        public const string SqlDom = "SqlDom";
        public const string SharedManagementObjects = "SharedManagementObjects";
        public const string DACFramework = "DACFramework";
        public const string TenantWAPExtension = "TenantWAPExtension";
        public const string WAPExtensionCommon = "WAPExtensionCommon";
        public const string AdminWAPExtension = "AdminWAPExtension";
        public const string PostInstall = "POSTINSTALL";
        public const string ListOfComponentsSelectedForInstalling = "LISTOFCOMPONENTSSELECTEDFORINSTALLING";

        public const string LoadedPrerequisiteXmlFile = "LoadedPrerequisiteXmlFile";
        public const string DefaultLogName = "DefaultLogName";

        /// <summary>
        /// sets the deault log path for logs
        /// </summary>
        public const string DefaultLogPath = "DefaultLogPath";

        /// <summary>
        /// Property to tell that list contains the items which contains log file with ending '*'
        /// </summary>
        public const string ItemsLogFileList = "ItemsLogFileList";

        /// <summary>
        /// Property to append the '*' to the log file name
        /// </summary>
        public const string AppendStarToLogFile = "AppendStarToLogFile";

        // Checks for installed components, etc...
        public const string VMMUnsupportedVersionInstalled = "VMMUNSUPPORTEDVERSIONINSTALLED";
        public const string VMMSupportedVersionInstalled = "VMMSUPPORTEDVERSIONINSTALLED";

        public const string ServerVersion = "SERVER-VERSION";
        public const string TenantExtensionVersion = "TENANTEXTENSION-VERSION";
        public const string AdminExtensionVersion = "ADMINEXTENSION-VERSION";
        public const string ExtensionCommonVersion = "EXTENSIONCOMMON-VERSION";

        public const string MissingFilesList = "MissingFilesList";

        public const string ReloadPrerequisitesPageChoice = "ReloadPrerequisitesPageChoice";

        public const string ComputerToSearchForSQL = "ComputerToSearchForSQL";
        public const string ArchitectureIs64Check = "ArchitectureIs64Check";
        public const string OperationSystemIs2k8 = "OperationSystemIs2k8";
        public const string OperationSystemIs2k8r2 = "OperationSystemIs2k8r2";

        public const string InvalidSQLReason = "InvalidSQLReason";

        public const string BlockReason = "BlockReason";

        public const string AddRemoveMode = "AddRemoveMode";
        public const string DataDispositionText = "DataDispositionText";

        public const string SelectedInstallablePrerequisites = "SelectedInstallablePrerequisites";

        /// <summary>
        /// Property contains a list of the items to rollback
        /// </summary>
        public const string RollbacksToProcess = "RollbacksToProcess";
        public const string UserCanceledInstall = "UserCanceledInstall";
        public const string ProcessingRollback = "ProcessingRollback";

        /// <summary>
        /// Properties for setup failure
        /// </summary>
        public const string FailureReason = "FailureReason";

        /// <summary>
        /// Properties for setup warning
        /// </summary>
        public const string WarningReason = "WarningReason";

        /// <summary>
        /// Property contains the full path to the SqlExpress Installer file
        /// Property only exists if path has been validated 
        /// </summary>
        public const string SqlExpressFullPathToExe = "SqlExpressFullPathToExe"; 

        /// <summary>
        /// Property contains the full path to the Windows Installer installer file
        /// Property only exists if path has been validated 
        /// </summary>
        public const string Msi45FullPathToExe = "Msi45FullPathToExe"; 

        /// <summary>
        /// Property contains the full path to the PowerShell 1.0 installer file
        /// Property only exists if path has been validated 
        /// </summary>
        public const string PowerShell10FullPathToExe = "PowerShell10FullPathToExe";

        /// <summary>
        /// Property exists (value == "1") when user rejects upgrade 
        /// </summary>
        public const string UserRejectUpgrade = "UserRejectUpgrade";

        /// <summary>
        /// Property exists (value == "1") when a reboot is required immediately 
        /// </summary>
        public const string RebootRequired = "RebootRequired";

        /// <summary>
        /// Property exists (value == "1") when reboot is confirmed by user
        /// </summary>
        public const string RebootNow = "RebootNow";

        /// <summary>
        /// Is it a first node of Installation of HAVMM 
        /// </summary>
        public const string IsAFirstHAVMMNode = "IsAFirstHAVMMNode";

        /// <summary>
        /// Is it a last node of uninstallation of HAVMM
        /// </summary>
        public const string IsLastHAVMMNode = "IsLastHAVMMNode";

        /// <summary>
        /// Did user cancel from upgrade, or at the beginning?
        /// </summary>
        // public const string ForceCancel = "ForceCancel";
    }
}
