// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.IO;
using Microsoft.Win32;

namespace CMP.Setup
{
    using CMP.Setup;

    /// <summary>
    /// InputDefaults contains default values for all setup inputs
    /// </summary>
    public class InputDefaults
    {
        static InputDefaults()
        {
        }

        /// <summary>
        /// Get the default install location
        /// </summary>
        public static String InstallLocation
        {
            get
            {
                String defaultInstallLocation = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + Path.DirectorySeparatorChar + SetupConstants.DefaultInstallDirectory;
                return defaultInstallLocation;
            }
        }

        public static String DoubleSeparatorCharString = Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString();

        public const bool CreateNewSqlDatabase = true; // create new sql database

        public const bool WapCreateNewSqlDatabase = false; // create new WAP sql database

        public const bool RetainSqlDatabase = false; // retain sql database

        public const bool WapRetainSqlDatabase = true; // retain WAP sql database

        public const String SqlDatabaseName = SetupConstants.DefaultDBName;

        public const String WapSqlDatabaseName = SetupConstants.DefaultWapDBName;

        public const int SqlServerPort = 1433;

        public const int WapSqlServerPort = 1433;

        public const bool OnRemoteServer = false; // use local host

        public const bool RemoteDatabaseImpersonation = false; // use current user

        public const bool WapRemoteDatabaseImpersonation = false; // use current user

        public const bool CreateNewLibraryShare = true; // create new lib share

        public const bool CmpServiceLocalAccount = true;
    }
}
