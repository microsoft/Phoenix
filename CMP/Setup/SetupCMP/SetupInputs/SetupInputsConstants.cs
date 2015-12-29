// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;

namespace CMP.Setup
{
    /// <summary>
    /// SetupInputsConstants contains the ini file tags for the inputs
    /// </summary>
    public class SetupInputTags
    {
        private SetupInputTags()
        {
        }

        #region user input data tags

        public const String SectionTag = "OPTIONS";

        public const String SetupUserAccountTag = "SetupUserAccount";

        public const String UserNameTag = "UserName";

        public const String BinaryInstallLocationTag = "ProgramFiles";

        public static string GetCreateNewSqlDatabaseTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + CreateNewSqlDatabaseTag;
        }

        public const String CreateNewSqlDatabaseTag = "CreateNewSqlDatabase"; // 1 or 0

        public const String WapCreateNewSqlDatabaseTag = "WapCreateNewSqlDatabase"; // 1 or 0

        public static string GetRetainSqlDatabaseTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + RetainSqlDatabaseTag;
        }

        public const String RetainSqlDatabaseTag = "RetainSqlDatabase"; // 1 or 0

        public const String WapRetainSqlDatabaseTag = "WapRetainSqlDatabase"; // 1 or 0

        public static string GetSqlServerPortTagTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + SqlServerPortTag;
        }

        public const String SqlServerPortTag = "SqlServerPort";

        public const String WapSqlServerPortTag = "WapSqlServerPort";

        public static string GetSqlInstanceNameTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + SqlInstanceNameTag;
        }

        public const String SqlInstanceNameTag = "SqlInstanceName";

        public const String WapSqlInstanceNameTag = "WapSqlInstanceName";

        public static string GetSqlDatabaseNameTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + SqlDatabaseNameTag;
        }

        public const String SqlDatabaseNameTag = "SqlDatabaseName";

        public const String WapSqlDatabaseNameTag = "WapSqlDatabaseName";

        public static string GetRemoteDatabaseImpersonationTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + RemoteDatabaseImpersonationTag;
        }

        public const String RemoteDatabaseImpersonationTag = "RemoteDatabaseImpersonation"; // 1 or 0

        public const String WapRemoteDatabaseImpersonationTag = "WapRemoteDatabaseImpersonation"; // 1 or 0

        public static string GetSqlMachineNameTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + SqlMachineNameTag;
        }

        public const String SqlMachineNameTag = "SqlMachineName";

        public const String WapSqlMachineNameTag = "WapSqlMachineName";

        public static string GetSqlDBAdminNameTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + SqlDBAdminNameTag;
        }

        public const String SqlDBAdminNameTag = "SqlDBAdminName";

        public const String WapSqlDBAdminNameTag = "WapSqlDBAdminName";

        public static string GetSqlDBAdminPasswordTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + SqlDBAdminPasswordTag;
        }

        public const String SqlDBAdminPasswordTag = "SqlDBAdminPassword";

        public const String WapSqlDBAdminPasswordTag = "WapSqlDBAdminPassword";

        public static string GetSqlDataFileLocationTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + SqlDataFileLocationTag;
        }

        public const String SqlDataFileLocationTag = "SqlDataFileLocation";

        public const String WapSqlDataFileLocationTag = "WapSqlDataFileLocation";

        public static string GetSqlLogFileLocationTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + SqlLogFileLocationTag;
        }

        public const String SqlLogFileLocationTag = "SqlLogFileLocation";

        public const String WapSqlLogFileLocationTag = "WapSqlLogFileLocation";

        public static string GetSqlDBAdminDomainTag(bool isWap)
        {
            return (isWap ? "Wap" : String.Empty) + SqlDBAdminDomainTag;
        }

        public const String SqlDBAdminDomainTag = "SqlDBAdminDomain";

        public const String WapSqlDBAdminDomainTag = "WapSqlDBAdminDomain";


        public const String CmpServerNameTag = "CmpServerName";

        public const String CmpServiceLocalAccountTag = "CmpServiceLocalAccount";  // 1 or 0

        public const String CmpServiceDomainTag = "CmpServiceDomain";

        public const String CmpServiceUserNameTag = "CmpServiceUserName";

        public const String CmpServiceUserPasswordTag = "CmpServiceUserPassword";

        public const String OldCmpServiceUserNameTag = "OldCmpServiceUserName";

        public const String CmpCertificateThumbprintTag = "CmpCertificateThumbprint";


        #endregion
    }

    /// <summary>
    /// SetupInputsConstants contains some constants
    /// </summary>
    public class SetupInputsConstants
    {
        private SetupInputsConstants()
        {
        }

        #region Other constants
        /// <summary>
        /// Path of registry key that contains the user name and company name
        /// of registered owner
        /// </summary>
        public const String WindowsNTRegistryPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";

        /// <summary>
        /// Registered owner key
        /// </summary>
        public const String RegisteredOwnerKey = "RegisteredOwner";

        /// <summary>
        /// Registered organization key
        /// </summary>
        public const String RegisteredOrganizationKey = "RegisteredOrganization";

        public const int MaxServiceNetBIOSNameLength = 15;

        public const int MaxInputNameLength = 255;

        public const int MaxInputPathLength = 160;

        public const int MaxStringLength = 4096;

        #endregion
    }
}
