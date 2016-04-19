using CMP.Setup.SetupFramework;
using Microsoft.SqlServer.Dac;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace CMP.Setup.Helpers
{
    class SetupDatabaseHelper
    {
        private const String PartialConnectionStringTemplate = "Integrated Security=SSPI;Application Name=CMP;Max Pool Size=500;Encrypt=true;TrustServerCertificate=true;Server={0};"; //connection string minus the database name
        private const String ConnectionStringTemplate = SetupDatabaseHelper.PartialConnectionStringTemplate + "Database={1};";
        private const String PartialWebsiteConnectionStringTemplate = "Persist Security Info=True;User ID={0};Password=;MultipleActiveResultSets=True;Data Source={1}.corp.microsoft.com;"; //connection string minus the database name

        internal const String LocalHost = "localhost";
        public const String SqlLocalHostString = "(local)";
        public const String MasterDatabaseName = "master";
        private const string VmmSetupUserName = @"VMMSetup";
        public const string DBConnectionStringFormat = "{0}database={1}";
        private const String DatabaseOwnerRole = "db_owner";
        private const string CMPDBResourceName = "CMP.Setup.Cmp_Db.dacpac";
        private const string CMPWAPDBResourceName = "CMP.Setup.CMPWAPDB.dacpac";
        private const string CreateDBUserCommandUsingBinarySid = @"DECLARE @user_Account AS VARCHAR(MAX)
                                                                   SELECT @user_Account = SUSER_SNAME({1})
                                                                   EXEC('create user [{0}] for login [' + @user_Account + ']')";
        private const string CreateDBUserCommand = @"create user [{0}] for login [{1}]";
        private const string CreateSqlLoginCommand = @"create login [{0}] from windows";
        private const string CreateSqlLoginCommandUsingBinarySid = @"DECLARE @user_Account AS VARCHAR(MAX)
                                                                     SELECT @user_Account = SUSER_SNAME({0})
                                                                     EXEC('create login [' + @user_Account + '] from windows')";
        private const string AddUserToDBRoleCommand = @"sp_addrolemember [{0}], [{1}]";
        private const String SQLSubKey = @"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL";
        private const String DatabaseNameQuery = "select name from sys.sysdatabases";
        private const string RemoveDBUserCommand = @"drop user [{0}]";
        private const string RevokeSqlLoginCommand = @"drop login [{0}]";
        private const string RevokeSqlLoginCommandUsingBinarySid = @"DECLARE @user_Account AS VARCHAR(MAX)
                                                                     SELECT @user_Account = SUSER_SNAME({0})
                                                                     EXEC('drop login [' + @user_Account + ']')";

        private const string CheckLoginServerRole = @"SELECT
                                                        r.name
                                                      FROM
                                                        sys.server_principals r INNER JOIN 
                                                        sys.server_role_members m 
                                                        ON r.principal_id = m.role_principal_id
                                                        INNER JOIN sys.server_principals p ON 
                                                        p.principal_id = m.member_principal_id
                                                      WHERE
                                                        p.Name = '{0}'
                                                      AND r.Type ='R'";
        private const String GetTableCountQuery = "select count(*) from sysobjects where xtype='U' AND Name LIKE '%WapSubscriptionData%'";


        private static string GetSqlServerUserName(bool isWap)
        {
            return isWap ? WapSqlServerUserName : SqlServerUserName;
        }
        private const string SqlServerUserName = @"CMPServer";
        private const string WapSqlServerUserName = @"CMPWapExtension";

        private const String CreateDatabaseWithDefaultLocation =
            @"
            CREATE DATABASE [{0}]
            COLLATE {1}
            ;";

        private const string CreateDatabaseCommand =
            @"
            CREATE DATABASE [{0}]
            ON
            (
                NAME=[{2}],
                FILENAME='{1}\{2}.mdf'
            )
            LOG ON
            (
                NAME=[{2}_log],
                FILENAME='{1}\{2}_log.ldf'
            )
            COLLATE {3}
            ;";
        private const string UseDatabaseCommand = "Use [{0}]";
        private const string DropDatabaseCommand = "DROP DATABASE [{0}]";

        private const string GetSqlDataLocationQuery = @"SELECT SUBSTRING(physical_name, 1, CHARINDEX(N'master.mdf', LOWER(physical_name)) - 1)
                                                    FROM master.sys.master_files
                                                    WHERE database_id = 1 AND file_id = 1";

        public const string SqlUsernameDuringInstall = "cmp_0";
        public static readonly string SqlDbUserPassword = null;

        static SetupDatabaseHelper()
        {
            // Create a random password. Sql DBs need a special character so adding that and a number just in case that's needed
            SqlDbUserPassword = "!1" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
        }

        private enum SpidColumns
        {
            Spid,
            COUNT
        }

        /// <summary>
        /// Constructs the full instance name for the SQL instance to use
        /// </summary>
        /// <returns></returns>
        private static string ConstructFullInstanceName(string machineName, string instanceName, int? port)
        {
            String fullInstanceName = !String.IsNullOrEmpty(machineName) ? machineName : SetupDatabaseHelper.SqlLocalHostString;
            if (!String.IsNullOrEmpty(instanceName))
            {
                fullInstanceName = String.Format("{0}\\{1}", fullInstanceName, instanceName);
            }

            if (port.HasValue && port.Value != InputDefaults.SqlServerPort)
            {
                fullInstanceName = String.Format("{0},{1}", fullInstanceName, port.Value);
            }

            return fullInstanceName;
        }

        /// <summary>
        /// Constructs the full instance name for the SQL instance to use
        /// </summary>
        /// <returns>MachineName\InstanceName for non-default</returns>
        public static string ConstructFullInstanceName(bool remote, string machineName, string instanceName, int port)
        {
            if (!remote)
            {
                machineName = null;
            }

            return ConstructFullInstanceName(machineName, instanceName, port);
        }

        public static bool SqlServerIsOnLocalComputer(String computerName)
        {
            if ((String.Compare(computerName, SetupDatabaseHelper.LocalHost, StringComparison.InvariantCultureIgnoreCase) == 0) ||
                (String.Compare(computerName, SetupDatabaseHelper.SqlLocalHostString, StringComparison.InvariantCultureIgnoreCase) == 0) ||
                (String.Compare(computerName, Environment.MachineName, StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Constructs the full connection string given the instance name
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public static string ConstructConnectionString(string instanceName)
        {
            AppAssert.Assert(null != instanceName, "Null instance name passed to ConstructConnectionString");
            string retstr = string.Format(SetupDatabaseHelper.PartialConnectionStringTemplate, instanceName);
            return retstr;
        }

        /// <summary>
        /// Constructs the full connection string given the instance name for use by the admin and tenant website
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public static string ConstructWebsiteConnectionString(string username, string instanceName)
        {
            AppAssert.Assert(null != instanceName, "Null instance name passed to ConstructConnectionString");
            string retstr = string.Format(SetupDatabaseHelper.PartialWebsiteConnectionStringTemplate, username, instanceName);
            return retstr;
        }

        public static void CheckDatabase(bool isWap)
        {
            CheckDatabase(InstallItemCustomDelegates.GetSQLServerInstanceNameStr(isWap), isWap ? SetupConstants.WapDBName : SetupConstants.DBName, isWap);
        }

        /// <summary>
        /// Constructs the connection string for the given instance name and database name
        /// </summary>
        /// <param name="instanceName"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static string GetConnectionStringToDatabase(string instanceName, string databaseName)
        {
            AppAssert.Assert(null != instanceName, "Null instance name passed");
            AppAssert.Assert(null != databaseName, "Null database name passed");
            string retstr = string.Format(SetupDatabaseHelper.ConnectionStringTemplate, instanceName, databaseName);
            return retstr;
        }

        /// <summary>
        /// Check to see whether the specified DB exists
        /// if yes, further check whether it's compatible
        /// </summary>
        public static void CheckDatabase(String server, String databaseName, bool isWap)
        {
            String masterConnectionString = SetupDatabaseHelper.GetConnectionStringToDatabase(
                server, SetupDatabaseHelper.MasterDatabaseName);
            bool databaseImpersonation = SetupInputs.Instance.FindItem(SetupInputTags.GetRemoteDatabaseImpersonationTag(isWap));
            bool DBExists = false;
            if (databaseImpersonation)
            {
                using (ImpersonationHelper impersonationHelper = new ImpersonationHelper())
                {
                    DBExists = SetupDatabaseHelper.CheckDBExistence(masterConnectionString, databaseName, isWap);
                }
            }
            else
            {
                DBExists = SetupDatabaseHelper.CheckDBExistence(masterConnectionString, databaseName, isWap);
            }

            bool toCreateNewDB = SetupInputs.Instance.FindItem(SetupInputTags.GetCreateNewSqlDatabaseTag(isWap));

            if (DBExists && toCreateNewDB)
            {
                throw new Exception(String.Format("The SQL database {0} already exists", databaseName));
            }

            if (!DBExists && !toCreateNewDB)
            {
                throw new Exception(String.Format("The SQL database {0} does not exist", databaseName));
            }

            if (DBExists)
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.SqlDatabaseDetected, "1");

                String connectionString = SetupDatabaseHelper.GetConnectionStringToDatabase(
                    server, databaseName);
                using (ImpersonationHelper impersonationHelper = new ImpersonationHelper())
                {
                    SetupDatabaseHelper.CheckDBEmpty(connectionString, isWap);
                }
            }
            else
            {
                PropertyBagDictionary.Instance.SafeRemove(PropertyBagConstants.SqlDatabaseVersion);
            }
        }

        /// <summary>
        /// Checks whether the db is present
        /// </summary>
        public static bool CheckDBExistence(String masterConnStr, string databaseName, bool isWap)
        {
            AppAssert.AssertNotNull(masterConnStr, "masterConnStr");
            AppAssert.AssertNotNull(databaseName, "databaseName");
            string sqlInstanceName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(isWap);

            bool dbExists = false;
            try
            {
                String dbSelectCommand = "select * from master.dbo.sysdatabases where name=\'" + databaseName + "\'";

                SqlConnection sqlConnection = new SqlConnection(masterConnStr);
                SqlCommand sqlCmd = new SqlCommand(dbSelectCommand, sqlConnection);

                sqlConnection.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();
                dbExists = reader.HasRows;
                sqlConnection.Close();

                if (dbExists)
                {
                    SetupLogger.LogInfo("CheckDBExistence: found db" + databaseName);
                }
            }
            catch (Exception)
            {
            }

            return dbExists;
        }

        public static void CheckDBEmpty(String connStr, bool isWap)
        {
            AppAssert.AssertNotNull(connStr, "connStr");
            string dbName = (String)SetupInputs.Instance.FindItem(SetupInputTags.GetSqlDatabaseNameTag(isWap));

            string sqlInstanceName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(isWap);

            bool dbIsEmpty = false;
            try
            {
                string uniqueTableName = isWap ? "WapSubscriptionData" : "ServiceProviderAccounts";

                String dbSelectCommand = String.Format("select count(*) from sysobjects where xtype='U' AND Name LIKE '%{0}%'", uniqueTableName);

                SqlConnection sqlConnection = new SqlConnection(connStr);
                SqlCommand sqlCmd = new SqlCommand(dbSelectCommand, sqlConnection);

                sqlConnection.Open();
                object tableCount = sqlCmd.ExecuteScalar();
                dbIsEmpty = false;
                if (tableCount != null)
                {
                    dbIsEmpty = (int)tableCount == 0;
                }
                sqlConnection.Close();

                if (dbIsEmpty)
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.GetSqlDatabaseEmpty(isWap), "1");
                }
                else
                {
                    PropertyBagDictionary.Instance.Remove(PropertyBagConstants.GetSqlDatabaseEmpty(isWap));
                }
            }
            catch (Exception)
            {
                throw new Exception(String.Format("The existing database {0} is not accessible", dbName));
            }
        }

        public static void CreateDB(bool isWap)
        {
            // Create the vNext database
            SetupLogger.LogInfo("Configuration : Create DB");

            string sqlInstanceName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(isWap);
            string partialConnectionString = SetupDatabaseHelper.ConstructConnectionString(sqlInstanceName);
            string dbName = isWap ? SetupInputs.Instance.FindItem(SetupInputTags.WapSqlDatabaseNameTag) : SetupInputs.Instance.FindItem(SetupInputTags.SqlDatabaseNameTag);

            string connectionString = String.Format(DBConnectionStringFormat, partialConnectionString, dbName);
            string masterDBConnectionString = String.Format(DBConnectionStringFormat, partialConnectionString, SetupDatabaseHelper.MasterDatabaseName);

            bool onRemoteServer = isWap ? SetupConstants.WapDBOnRemoteServer : SetupConstants.DBOnRemoteServer;

            using (ImpersonationHelper impersonationHelper = new ImpersonationHelper())
            {
                bool isNewDatabase = SetupInputs.Instance.FindItem(SetupInputTags.GetCreateNewSqlDatabaseTag(isWap));
                bool isEmptyDatabase = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.GetSqlDatabaseEmpty(isWap));

                if (isNewDatabase)
                {
                    SetupDatabaseHelper.CreateDB(onRemoteServer, connectionString, masterDBConnectionString, dbName, sqlInstanceName);
                }

                if (isNewDatabase || isEmptyDatabase)
                {
                    // Output the dacpac files to use them
                    string pathToDatabaseDacpac = isWap ? SetupDatabaseHelper.GetPathToOutputEmbeddedResource(SetupDatabaseHelper.CMPWAPDBResourceName, "dacpac") : SetupDatabaseHelper.GetPathToOutputEmbeddedResource(SetupDatabaseHelper.CMPDBResourceName, "dacpac");

                    var dp = DacPackage.Load(pathToDatabaseDacpac);
                    var dbDeployOptions = new DacDeployOptions
                    {
                        BlockOnPossibleDataLoss = false,
                        ScriptDatabaseOptions = false
                    };
                    var dbServices = new DacServices(connectionString);
                    dbServices.Deploy(dp, dbName, true, dbDeployOptions);
                }
            }

            SetupDatabaseHelper.GrantDBAccess(connectionString, masterDBConnectionString, isWap);
        }

        public static void DeployWAPDacpac()
        {
            // Create the vNext database
            SetupLogger.LogInfo("Configuration : Deploy WAP Dacpac");
            string dbName = SetupConstants.WapDBName;

            string sqlInstanceName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(true);
            string partialConnectionString = SetupDatabaseHelper.ConstructConnectionString(sqlInstanceName);

            string connectionString = String.Format(DBConnectionStringFormat, partialConnectionString, dbName);
            string masterDBConnectionString = String.Format(DBConnectionStringFormat, partialConnectionString, SetupDatabaseHelper.MasterDatabaseName);

            bool onRemoteServer = SetupConstants.WapDBOnRemoteServer;

            using (ImpersonationHelper impersonationHelper = new ImpersonationHelper())
            {
                bool isEmptyDatabase = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.GetSqlDatabaseEmpty(true));

                if (isEmptyDatabase)
                {
                    // Output the dacpac files to use them
                    string pathToCMPWAPDBDacpac = SetupDatabaseHelper.GetPathToOutputEmbeddedResource(SetupDatabaseHelper.CMPWAPDBResourceName, "dacpac");

                    var dp = DacPackage.Load(pathToCMPWAPDBDacpac);
                    var dbDeployOptions = new DacDeployOptions
                    {
                        BlockOnPossibleDataLoss = false,
                        ScriptDatabaseOptions = false
                    };
                    var dbServices = new DacServices(connectionString);
                    dbServices.Deploy(dp, dbName, true, dbDeployOptions);
                }
            }

            SetupDatabaseHelper.GrantDBAccess(connectionString, masterDBConnectionString, true);
        }

        internal static string GetPathToOutputEmbeddedResource(string resourceName, string extensionWithoutDot)
        {
            var embeddedResource = Assembly.GetAssembly(typeof(SetupDatabaseHelper)).GetManifestResourceStream(resourceName);
            string dacpacContent;

            string temporaryResourcePath = System.IO.Path.GetTempFileName();

            var output = File.Open(temporaryResourcePath, FileMode.OpenOrCreate);
            SetupDatabaseHelper.CopyStream(embeddedResource, output);
            embeddedResource.Dispose();
            output.Dispose();

            string temporaryTxtPath = temporaryResourcePath;
            string temporaryDacpacPath = Path.ChangeExtension(temporaryResourcePath, extensionWithoutDot);

            File.Move(temporaryTxtPath, temporaryDacpacPath);

            return temporaryDacpacPath;
        }

        internal static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                    return;
                output.Write(buffer, 0, read);
            }
        }

        internal static void CreateCarmineLoginAndDBUser(string connectionString, string masterDBConnectionString, string loginName, string userName, bool isWap)
        {
            // create login account for Carmine server service using DOMAIN-NAME\MACHINE-NAME$, or DOMAIN-NAME\USER-NAME
            SetupDatabaseHelper.CreateSqlLogin(masterDBConnectionString, loginName);

            // create a db user for Carmine server service
            SetupDatabaseHelper.CreateDBUserForLogin(connectionString, userName, loginName, isWap);

            // create a db user for Carmine server
            SetupDatabaseHelper.AddDBUserToRole(connectionString, userName, DatabaseOwnerRole);
        }

        public static void GrantDBAccess(string connectionString, string masterDBConnectionString, bool isWap)
        {
            String loginName = SetupDatabaseHelper.GetSqlLoginName(true, isWap);

            using (ImpersonationHelper impersonationHelper = new ImpersonationHelper())
            {
                try
                {
                    SetupDatabaseHelper.CreateCarmineLoginAndDBUser(connectionString, masterDBConnectionString, loginName, SetupDatabaseHelper.GetSqlServerUserName(isWap), isWap);
                }
                catch
                {
                    // On WAP database, the user may already have access.
                    if (!isWap)
                    {
                        throw;
                    }
                }
            }
        }

        private static String GetBinarySidString(String accountName)
        {
            NTAccount ntAccount = new NTAccount(accountName);
            SecurityIdentifier id = (SecurityIdentifier)ntAccount.Translate(typeof(SecurityIdentifier));
            byte[] binarySid = UserAccountHelper.GetBinarySid(id);
            return "0x" + BitConverter.ToString(binarySid).Replace("-", string.Empty);
        }

        public static void RevokeDBAccess(string connectionString, string masterDBConnectionString, bool isWap)
        {
            string loginName = SetupDatabaseHelper.GetSqlLoginName(false, isWap);
            string userName = SetupDatabaseHelper.GetSqlServerUserName(isWap);

            // drop db user Carmine server
            SetupDatabaseHelper.RemoveDBUser(connectionString, userName);

            // drop db login Carmine server
            SetupDatabaseHelper.RemoveSqlLogin(masterDBConnectionString, loginName);
        }

        private static void RemoveSqlLogin(string masterDBConnectionString, string loginName)
        {
            SetupLogger.LogInfo("Try removing sql login [{0}]", loginName);

            SetupLogger.LogInfo("Removing db user [{0}]", loginName);

            SqlConnection sqlConnection = new SqlConnection(masterDBConnectionString);
            String commandText = String.Format(CheckLoginServerRole, loginName);

            sqlConnection.Open();

            SqlCommand sqlCmd = new SqlCommand(commandText, sqlConnection);

            SqlDataReader reader = sqlCmd.ExecuteReader();

            if (!reader.Read())
            {
                commandText = SetupDatabaseHelper.GetUserLoginCommandText(RevokeSqlLoginCommand, RevokeSqlLoginCommandUsingBinarySid, loginName);
                sqlCmd = new SqlCommand(commandText, sqlConnection);
                sqlCmd.ExecuteNonQuery();
            }
        }

        private static void RemoveDBUser(string connectionString, string userName)
        {
            SetupLogger.LogInfo("Removing db user [{0}]", userName);

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            String commandText = String.Format(RemoveDBUserCommand, userName);

            SqlCommand sqlCmd = new SqlCommand(commandText, sqlConnection);

            sqlConnection.Open();
            sqlCmd.ExecuteNonQuery();

            sqlConnection.Close();
            SetupLogger.LogInfo("Successfully removed db user[{0}]", userName);
        }

        public static void RevokeSetupDBAccess(string connectionString, string masterDBConnectionString, bool isWap)
        {
            string loginName = SetupDatabaseHelper.GetSqlLoginName(false, isWap);
            String setupUser = SetupInputs.Instance.FindItem(SetupInputTags.SetupUserAccountTag);
            if (!String.Equals(loginName, setupUser, StringComparison.OrdinalIgnoreCase))
            {
                using (ImpersonationHelper impersonationHelper = new ImpersonationHelper())
                {

                    // drop db user Carmine server
                    SetupDatabaseHelper.RemoveDBUser(connectionString, SetupDatabaseHelper.VmmSetupUserName);

                    // drop db login Carmine server
                    SetupDatabaseHelper.RemoveSqlLogin(masterDBConnectionString, loginName);
                }
            }
        }

        public static void RemoveDB(bool isWap)
        {
            string sqlInstanceName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(isWap);
            string partialConnectionString = SetupDatabaseHelper.ConstructConnectionString(sqlInstanceName);
            string dbName = isWap ? SetupConstants.WapDBName : SetupConstants.DBName;

            string connectionString = String.Format(DBConnectionStringFormat, partialConnectionString, dbName);
            string masterDBConnectionString = String.Format(DBConnectionStringFormat, partialConnectionString, SetupDatabaseHelper.MasterDatabaseName);

            SetupDatabaseHelper.KillDatabaseConnections(dbName, masterDBConnectionString);
            bool dbExists = true;
            try
            {
                SqlConnection sqlConnection = new SqlConnection(masterDBConnectionString);
                sqlConnection.Open();

                //Check whether the DB is existing...
                string commandText = string.Format(SetupDatabaseHelper.UseDatabaseCommand, dbName);
                SqlCommand useDbCmd = new SqlCommand(commandText, sqlConnection);
                useDbCmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception)
            {
                dbExists = false;
            }

            if (dbExists)
            {
                SqlConnection dropSqlConnection = new SqlConnection(masterDBConnectionString);
                dropSqlConnection.Open();
                //Check whether the DB is existing...
                string commandText = string.Format(SetupDatabaseHelper.DropDatabaseCommand, dbName);
                SqlCommand dropDbCmd = new SqlCommand(commandText, dropSqlConnection);
                dropDbCmd.ExecuteNonQuery();
                dropSqlConnection.Close();
            }
        }

        private static void KillDatabaseConnections(string databaseName, string masterDbConnectionString)
        {
            string getSpidOfOpenConnectionsCommand = string.Format("select spid from dbo.sysprocesses where db_name(dbid) = '{0}'", databaseName);
            string killCommand = "kill ";

            SetupLogger.LogInfo("Killing database connections..");

            SqlConnection sqlConnection = new SqlConnection(masterDbConnectionString);
            sqlConnection.Open();
            //Check whether the DB is existing...
            SqlCommand useDbCmd = new SqlCommand(getSpidOfOpenConnectionsCommand, sqlConnection);
            SqlDataReader reader = useDbCmd.ExecuteReader();

            List<short> spidList = new List<short>();

            while (reader.Read())
            {
                object[] values = new object[(int)SpidColumns.COUNT];
                reader.GetSqlValues(values);

                short spid = ((System.Data.SqlTypes.SqlInt16)values[0]).Value;

                if (!spidList.Contains(spid))
                {
                    spidList.Add(spid);
                }
            }

            if (!reader.IsClosed)
            {
                reader.Close();
            }

            foreach (short spid in spidList)
            {
                string spidToKill = killCommand + spid.ToString();
                SqlCommand sqlKillCommand = new SqlCommand(spidToKill, sqlConnection);

                sqlKillCommand.ExecuteNonQuery();
            }

            sqlConnection.Close();
        }

        private static String GetSqlDataLocation(string instanceName)
        {
            String partialConnectionString = SetupDatabaseHelper.ConstructConnectionString(instanceName);

            String dbName = SetupDatabaseHelper.MasterDatabaseName;
            String dbConnectionString = String.Format(SetupDatabaseHelper.DBConnectionStringFormat, partialConnectionString, dbName);
            String sqlDataLocation = null;

            SqlConnection sqlConnection = new SqlConnection(dbConnectionString);
            SqlCommand sqlCmd = new SqlCommand(SetupDatabaseHelper.GetSqlDataLocationQuery, sqlConnection);

            sqlConnection.Open();
            sqlDataLocation = (string)sqlCmd.ExecuteScalar();

            sqlConnection.Close();

            return sqlDataLocation;
        }

        private static string GetCollationName()
        {
            string collationName = "SQL_Latin1_General_CP1_CI_AS";  // Default collation name
            switch (CultureInfo.CurrentUICulture.LCID)
            {
                case 0x0813:  // Dutch (Belgium)
                case 0x0413:  // Dutch (Netherlands)
                case 0x0c09:  // English (Australia)
                case 0x2809:  // English (Belize)
                case 0x1009:  // English (Canada)
                case 0x2409:  // English (Caribbean)
                case 0x4009:  // English (India)
                case 0x1809:  // English (Ireland)
                case 0x2009:  // English (Jamaica)
                case 0x4409:  // English (Malaysia)
                case 0x1409:  // English (New Zealand)
                case 0x3409:  // English (Philippines)
                case 0x4809:  // English (Singapore)
                case 0x1c09:  // English (South Africa)
                case 0x2c09:  // English (Trinidad and Tobago)
                case 0x0809:  // English (United Kingdom)
                case 0x3009:  // English (Zimbabwe)
                case 0x0409:  // English (United States)
                case 0x0c07:  // German (Austria)
                case 0x0407:  // German (Germany)
                case 0x1407:  // German (Liechtenstein)
                case 0x1007:  // German (Luxembourg)
                case 0x0807:  // German (Switzerland)
                case 0x10407: // German - Phone Book Sort (DIN)
                case 0x0410:  // Italian (Italy)
                case 0x0810:  // Italian (Switzerland)
                case 0x0816:  // Portuguese (Portugal) 
                case 0x0416:  // Portuguese (Brazil)
                case 0x0462:  // Frisian (Netherlands) 
                    collationName = "SQL_Latin1_General_CP1_CI_AS";
                    break;
                case 0x0419:  // Russian (Russia)
                    collationName = "Cyrillic_General_100_CI_AS";
                    break;
                case 0x0c04:   // Chinese (Hong Kong SAR, PRC)
                case 0x21404:  // Chinese (Macau)
                case 0x0404:   // Chinese (Taiwan)
                case 0x1404:   // Chinese (Macao SAR)
                case 0x30404:  // Chinese (Taiwan)
                    collationName = "Chinese_Traditional_Stroke_Count_100_CI_AS";
                    break;
                case 0x0804:  // Chinese (PRC)
                case 0x1004:  // Chinese (Singapore)
                case 0x20804:  // Chinese (PRC)
                case 0x21004:  // Chinese (Singapore)
                    collationName = "Chinese_Simplified_Pinyin_100_CI_AS";
                    break;
                case 0x0405:   // Czech (Czech Republic)
                    collationName = "Czech_100_CI_AS";
                    break;
                case 0x0406:   // Danish (Denmark)
                    collationName = "Danish_Greenlandic_100_CI_AS";
                    break;
                case 0x040b:  // Finnish (Finland)
                case 0x081d:  // Swedish (Finland)
                case 0x041d:  // Swedish (Sweden)
                    collationName = "Finnish_Swedish_100_CI_AS";
                    break;
                case 0x080c:  // French (Belgium)
                case 0x0c0c:  // French (Canada)
                case 0x040c:  // French (France)
                case 0x140c:  // French (Luxembourg)
                case 0x180c:  // French (Monaco)
                case 0x100c:  // French (Switzerland)
                    collationName = "French_100_CI_AS";
                    break;
                case 0x0408:  // Greek (Greece)
                    collationName = "Greek_100_CI_AS";
                    break;
                case 0x0411:  // Japanese (Japan XJIS)
                case 0x040411:  // Japanese (Japan)
                    collationName = "Japanese_XJIS_100_CI_AS";
                    break;
                case 0x540a:  // Spanish (United States)
                case 0x2c0a:  // Spanish (Argentina)
                case 0x400a:  // Spanish (Bolivia)
                case 0x340a:  // Spanish (Chile)
                case 0x240a:  // Spanish (Colombia)
                case 0x140a:  // Spanish (Costa Rica)
                case 0x1c0a:  // Spanish (Dominican Republic)
                case 0x300a:  // Spanish (Ecuador)
                case 0x440a:  // Spanish (El Salvador)
                case 0x100a:  // Spanish (Guatemala)
                case 0x480a:  // Spanish (Honduras)
                case 0x080a:  // Spanish (Mexico)
                case 0x4c0a:  // Spanish (Nicaragua)
                case 0x180a:  // Spanish (Panama) 
                case 0x3c0a:  // Spanish (Paraguay)
                case 0x280a:  // Spanish (Peru)
                case 0x500a:  // Spanish (Puerto Rico)
                case 0x0c0a:  // Spanish (Spain)
                case 0x380a:  // Spanish (Uruguay)
                case 0x200a:  // Spanish (Venezuela)
                case 0x040a:  // Spanish (Spain, Traditional Sort)
                    collationName = "Modern_Spanish_100_CI_AS";
                    break;
                case 0x0412:  // Korean (Korea Dictionary Sort)
                    collationName = "Korean_100_CI_AS";
                    break;
                case 0x0414:  // Norwegian (Bokmå l, Norway) 
                case 0x0814:  // Norwegian (Nynorsk, Norway)
                    collationName = "Norwegian_100_CI_AS";
                    break;
                case 0x0415:  // Polish (Poland)
                    collationName = "Polish_100_CI_AS";
                    break;
                case 0x041f:  // Turkish (Turkey)
                    collationName = "Turkish_100_CI_AS";
                    break;
                case 0x040e:  // Hungarian (Hungary)
                    collationName = "Hungarian_100_CI_AS";
                    break;
                default:
                    break;
            }

            return collationName;
        }

        /// <summary>
        /// Creates the Database with parameters specified in the constructor.
        /// </summary>
        /// <param name="onRemoteServer">whether we are create db on remote server</param>
        /// <param name="connectionString"></param>
        /// <param name="masterDBConnectionString"></param>
        /// <param name="dbName"></param>
        /// <param name="instanceName"></param>
        public static void CreateDB(bool onRemoteServer, string connectionString, string masterDBConnectionString, string dbName, string instanceName)
        {
            String commandText;
            String databaseLocation = GetSqlDataLocation(instanceName);

            if ((!onRemoteServer) && (!String.IsNullOrEmpty(databaseLocation)))
            {
                SetupDatabaseHelper.CreateDatabaseLocation(databaseLocation);

                commandText = String.Format(SetupDatabaseHelper.CreateDatabaseCommand,
                    dbName,
                    databaseLocation,
                    dbName,
                    GetCollationName()
                    );
            }
            else
            {
                commandText = String.Format(SetupDatabaseHelper.CreateDatabaseWithDefaultLocation, dbName, GetCollationName());
            }

            //create the Virtual Machine Manager DB
            try
            {
                SqlConnection sqlConnection = new SqlConnection(masterDBConnectionString);
                SqlCommand sqlCmd = new SqlCommand(commandText, sqlConnection);

                sqlConnection.Open();
                sqlCmd.ExecuteNonQuery();

                sqlConnection.Close();
            }
            catch (Exception e)
            {
                SetupLogger.LogInfo("Create DB failed");
                //SetupLogger.LogException(sqlException);
                throw new Exception("Create DB failed", e);
            }
        }

        private static void CreateDatabaseLocation(String databaseLocation)
        {
            AppAssert.AssertNotNull(databaseLocation, "databaseLocation");
            SetupLogger.LogInfo("DBConfigurator.CreateDatabaseLocation : Database location {0}", databaseLocation);

            try
            {
                if (!Directory.Exists(databaseLocation))
                {

                    SetupLogger.LogInfo("DBConfigurator.CreateDatabaseLocation : CreateDirectory {0}", databaseLocation);
                    Directory.CreateDirectory(databaseLocation);
                }
            }

            catch (IOException ioException)
            {
                SetupLogger.LogInfo("DBConfigurator.CreateDatabaseLocation : Exception : {0}", ioException.ToString());
                throw new Exception("Creating the database folder failed", ioException);
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                SetupLogger.LogInfo("DBConfigurator.CreateDatabaseLocation : Exception : {0}", unauthorizedAccessException.ToString());
                throw new Exception("Creating the database folder failed", unauthorizedAccessException);
            }
        }

        private static String GetSqlLoginName(bool isInstall, bool isWap)
        {
            String userAccountName = null;
            if (isInstall)
            {
                if (SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceLocalAccountTag))
                {
                    userAccountName = DnsHelper.GetLocalMachineAccount();

                    // the vmm service account is NT AUTHORITY\SYSTEM for a local SQL server
                    string sqlMachineName = DnsHelper.GetComputerNameFromFqdnOrNetBios((String)SetupInputs.Instance.FindItem(SetupInputTags.GetSqlMachineNameTag(isWap)));
                    bool isSqlServerLocal = String.Compare(sqlMachineName, Environment.MachineName, true) == 0 ||
                        String.Compare(sqlMachineName, SetupDatabaseHelper.LocalHost, StringComparison.InvariantCultureIgnoreCase) == 0;
                    if (isSqlServerLocal)
                    {
                        SecurityIdentifier localSystemId = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
                        userAccountName = UserAccountHelper.GetAccountNameFromSid(localSystemId);
                    }
                }
                else
                {
                    userAccountName = UserAccountHelper.GetVmmServiceDomainAccount();
                }
            }
            else
            {
                if (SetupConstants.VmmServiceRunningAsLocalSystem)
                {
                    userAccountName = DnsHelper.GetLocalMachineAccount();
                    // the vmm service account is NT AUTHORITY\SYSTEM for a local SQL server
                    // But setup is not to remove the login NT AUTHORITY\SYSTEM during uninstallation
                    // so there is no need to get this right account name as in installation. 
                }
                else
                {
                    userAccountName = SetupConstants.VmmServiceAccount;
                }
            }

            SetupLogger.LogInfo("GetSqlLoginName: TThe login name for the vmm server service is [{0}]", userAccountName);
            return userAccountName;
        }

        /// <summary>
        /// Create db login for carmine server service account, add it to sysadmin role
        /// </summary>
        public static void GrantSetupUserDBAccess(bool install, bool isWap)
        {
            String loginName = String.Empty;
            loginName = SetupDatabaseHelper.GetSqlLoginName(install, isWap);
            String setupUser = SetupInputs.Instance.FindItem(SetupInputTags.SetupUserAccountTag);
            if (!String.Equals(loginName, setupUser, StringComparison.OrdinalIgnoreCase))
            {
                using (ImpersonationHelper impersonationHelper = new ImpersonationHelper())
                {
                    SetupDatabaseHelper.CreateCarmineLoginAndDBUser(setupUser, VmmSetupUserName, isWap);
                }
            }
        }

        /// <summary>
        /// Create login account, and add DB user for Carmine Server
        /// </summary>
        static void CreateCarmineLoginAndDBUser(string loginName, string userName, bool isWap)
        {
            bool shouldAddRole = true;
            string serverName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(isWap);

            // create login account for Carmine server service using DOMAIN-NAME\MACHINE-NAME$, or DOMAIN-NAME\USER-NAME
            SetupDatabaseHelper.CreateSqlLogin(
                SetupDatabaseHelper.GetConnectionStringToDatabase(
                serverName, SetupDatabaseHelper.MasterDatabaseName),
                loginName);

            String connectionString = SetupDatabaseHelper.GetConnectionStringToDatabase(serverName, SetupConstants.DBName);

            // create a db user for Carmine server service
            SetupDatabaseHelper.CreateDBUserForLogin(connectionString, userName, loginName, isWap);

            if (shouldAddRole)
            {
                // create a db user for Carmine server
                SetupDatabaseHelper.AddDBUserToRole(connectionString, userName, DatabaseOwnerRole);
            }
        }

        /// <summary>
        /// add the user as a member of the db role
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="userName"></param>
        /// <param name="dbRole"></param>
        public static void AddDBUserToRole(string connectionString, string userName, string dbRole)
        {
            AppAssert.Assert(connectionString != null, "connectionString is Null");
            AppAssert.Assert(userName != null, "userName is Null");
            AppAssert.Assert(dbRole != null, "dbRole is Null");


            String commandText = String.Format(SetupDatabaseHelper.AddUserToDBRoleCommand, dbRole, userName);

            SetupLogger.LogInfo("AddUserToDBRoleCommand command: [{0}]", commandText);

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCmd = new SqlCommand(commandText, sqlConnection);

            sqlConnection.Open();
            sqlCmd.ExecuteNonQuery();

            sqlConnection.Close();

            SetupLogger.LogInfo("Successfully added db user[{0}] to db role[{1}]",
                userName, dbRole);
        }

        /// <summary>
        /// create a user for the login
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="userName"></param>
        /// <param name="loginName"></param>
        public static void CreateDBUserForLogin(string connectionString, string userName, string loginName, bool isWap)
        {
            AppAssert.Assert(connectionString != null, "connectionString is Null");
            AppAssert.Assert(userName != null, "userName is Null");
            AppAssert.Assert(loginName != null, "loginName is Null");

            string dbName = (String)SetupInputs.Instance.FindItem(SetupInputTags.GetSqlDatabaseNameTag(isWap));

            try
            {
                String commandText = String.Empty;
                try
                {
                    String binarySid = UserAccountHelper.GetBinarySidString(loginName);
                    if (!String.IsNullOrEmpty(binarySid))
                    {
                        commandText = String.Format(CreateDBUserCommandUsingBinarySid, userName, binarySid);
                    }
                }
                catch (Exception e)
                {
                    SetupLogger.LogError("Failed to get binary sid of account {0}, create the DB user using the account name.", loginName);
                }
                if (String.IsNullOrEmpty(commandText))
                {
                    commandText = String.Format(CreateDBUserCommand, userName, loginName);
                }

                SqlConnection sqlConnection = new SqlConnection(connectionString);
                SqlCommand sqlCmd = new SqlCommand(commandText, sqlConnection);

                sqlConnection.Open();
                sqlCmd.ExecuteNonQuery();

                sqlConnection.Close();
            }
            catch (SqlException exception)
            {
                if (exception.Number == 15023)
                {
                    SetupLogger.LogInfo("User {0} was already present in database {1}", userName, loginName);
                }
                else
                {
                    throw new Exception(String.Format("The existing database {0} is not accessible", dbName));
                }
            }

            SetupLogger.LogInfo("Successfully created db user[{0}] for login[{1}]", userName, loginName);
        }

        /// <summary>
        /// Create a SQL login
        /// </summary>
        public static void CreateSqlLogin(String connStr, string loginName)
        {
            AppAssert.AssertNotNull(connStr, "connStr");
            AppAssert.AssertNotNull(loginName, "loginName");

            SqlConnection sqlConnection = new SqlConnection(connStr);

            try
            {
                //create new login
                SetupLogger.LogInfo("Creating new Login: [{0}]", loginName);

                String commandText = SetupDatabaseHelper.GetUserLoginCommandText(CreateSqlLoginCommand, CreateSqlLoginCommandUsingBinarySid, loginName);

                SetupLogger.LogInfo("Creating new Login sql command: [{0}]", commandText);

                SqlCommand sqlCmd = new SqlCommand(commandText, sqlConnection);

                sqlConnection.Open();
                sqlCmd.ExecuteNonQuery();

                sqlConnection.Close();

                SetupLogger.LogInfo("Successfully created Login");
            }
            catch (SqlException sqlException)
            {
                // catch the login already exists error
                if (sqlException.Number == 15025)
                {
                    SetupLogger.LogError("Creating new Login sql command: login already exists!");
                }
                else
                {
                    throw;
                }
            }
        }

        private static String GetUserLoginCommandText(String commandStringUsingLoginAccountFormat, String commandStringUsingLoginSidFormat, String loginName)
        {
            String commandText = String.Empty;
            try
            {
                String binarySid = UserAccountHelper.GetBinarySidString(loginName);
                if (!String.IsNullOrEmpty(binarySid))
                {
                    commandText = String.Format(commandStringUsingLoginSidFormat, binarySid);
                }
            }
            catch (Exception e)
            {
                SetupLogger.LogInfo("Failed to get binary sid of account {0}, use the account name.", loginName);
            }
            if (String.IsNullOrEmpty(commandText))
            {
                commandText = String.Format(commandStringUsingLoginAccountFormat, loginName);
            }

            return commandText;
        }

        /// <summary>
        /// Get Sql server instance names on a specific machine
        /// </summary>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public static String[] GetSqlInstanceNames(string machineName)
        {
            try
            {
                RegistryKey baseKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName, RegistryView.Registry32);
                RegistryKey sqlKey = baseKey.OpenSubKey(SQLSubKey);
                if (sqlKey == null)
                {
                    baseKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName, RegistryView.Registry64);
                    sqlKey = baseKey.OpenSubKey(SQLSubKey);
                }
                return sqlKey.GetValueNames();
            }
            catch (System.IO.IOException)// e)
            {
            }
            catch (NullReferenceException)// e)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (System.Security.SecurityException)
            {
            }

            return null;
        }

        private static List<String> StandardDatabaNames = new List<String>(new String[] { "master", "tempdb", "model", "msdb" });

        /// <summary>
        /// Get the databases names of an sql instance
        /// </summary>
        /// <param name="isRemoteServer"></param>
        /// <param name="serverName"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public static String[] GetSqlDBNames(bool isRemoteServer, String serverName, String instanceName, int port, bool isWap)
        {
            String fullInstanceName = SetupDatabaseHelper.ConstructFullInstanceName(isRemoteServer, serverName, instanceName, port);
            String masterConnectionString = SetupDatabaseHelper.GetConnectionStringToDatabase(
                fullInstanceName, SetupDatabaseHelper.MasterDatabaseName);

            SetupLogger.LogInfo("Get SqlDatabase names from : server = {0} instance = {1}", serverName, instanceName);

            // Check connection
            try
            {
                using (SqlConnection conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();
                    conn.Close();
                }
            }
            catch (SqlException sqlException)
            {
                SetupLogger.LogError(
                    "GetSqlDBNames: Code is {0} and SqlException is: {1}",
                    sqlException.Number, sqlException.ToString());

                throw new Exception("Setup cannot connect to the specified SQL Server instance", sqlException);
            }

            // Retrieve the database names
            List<String> dbNames = new List<string>();

            string sqlInstanceName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(isWap);

            try
            {
                String dbSelectCommand = SetupDatabaseHelper.DatabaseNameQuery;

                SqlConnection sqlConnection = new SqlConnection(masterConnectionString);
                SqlCommand sqlCmd = new SqlCommand(dbSelectCommand, sqlConnection);

                sqlConnection.Open();
                SqlDataReader reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    object[] values = new object[reader.FieldCount];
                    reader.GetSqlValues(values);

                    String dbName = ((SqlString)values[0]).Value;
                    if (!StandardDatabaNames.Contains(dbName))
                    {
                        dbNames.Add(dbName);
                    }
                }

                sqlConnection.Close();
            }
            catch (Exception)
            {
                throw new Exception(String.Format("The existing database is not accessible"));
            }

            // Copy the database names from List to array
            String[] newDBNames = new String[dbNames.Count];
            for (int i = 0; i < dbNames.Count; i++)
            {
                newDBNames[i] = dbNames[i];
            }
            return newDBNames;
        }

        public static void CreateSqlLoginUser(string username, string password)
        {
            SetupLogger.LogInfo("Creating sql and db user [{0}]", username);

            string query = String.Format(InstallationSqlLoginQuery.SqlLoginQueryDuringInstall, username, password);

            string sqlInstanceName = InstallItemCustomDelegates.GetSQLServerInstanceNameStr(true);

            InstallItemCustomDelegates.EditSqlAdminUser(username);
            string partialConnectionString = SetupDatabaseHelper.ConstructConnectionString(sqlInstanceName);
            string masterDBConnectionString = String.Format(DBConnectionStringFormat, partialConnectionString, SetupDatabaseHelper.MasterDatabaseName);
            SqlConnection sqlConnection = new SqlConnection(masterDBConnectionString);
            String commandText = String.Format(query, username);

            SqlCommand sqlCmd = new SqlCommand(commandText, sqlConnection);

            sqlConnection.Open();
            sqlCmd.ExecuteNonQuery();

            sqlConnection.Close();
            SetupLogger.LogInfo("Successfully created sql and db user [{0}]", username);
        }
    }
}
