using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Setup.Helpers
{
    public class InstallationSqlLoginQuery
    {
        public const string SqlLoginQueryDuringInstall =

                @"-- If exists, delete the previous sql login and create a new one
                USE MASTER
                IF EXISTS (SELECT loginname FROM master.dbo.syslogins WHERE name = '{0}')
                DROP LOGIN {0}
                CREATE LOGIN {0} WITH PASSWORD = '{1}'

                -- Add the new sql login as sysadmin
                EXEC master..sp_addsrvrolemember @loginame = '{0}', @rolename = N'sysadmin'

                -- If user for the same login exists, delete it - for master DB
                Use MASTER;
                IF EXISTS (SELECT * FROM sys.sysusers WHERE name = '{0}')
                DROP USER {0};

                -- Create the database user for the login and add as dbowner
                IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = '{0}')
                BEGIN
                    CREATE USER [{0}] FOR LOGIN [{0}]
                    EXEC sp_addrolemember N'db_owner', '{0}'
                END;

                -- If user for the same login exists, delete it - for the WAP store DB
                Use [Microsoft.MgmtSvc.Store];
                IF EXISTS (SELECT * FROM sys.sysusers WHERE name = '{0}')
                DROP USER {0};

                -- Create the database user for the login and add as dbowner
                IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = '{0}')
                BEGIN
                    CREATE USER [{0}] FOR LOGIN [{0}]
                    EXEC sp_addrolemember N'db_owner', '{0}'
                END;

                -- If user for the same login exists, delete it - for CMP WAP DB
                Use CMPWAP_DB;
                IF EXISTS (SELECT * FROM sys.sysusers WHERE name = '{0}')
                DROP USER {0};

                -- Create the database user for the login and add as dbowner
                IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = '{0}')
                BEGIN
                    CREATE USER [{0}] FOR LOGIN [{0}]
                    EXEC sp_addrolemember N'db_owner', '{0}'
                END;


                -- If user for the same login exists, delete it - for CMP DB
                Use CMP_DB;
                IF EXISTS (SELECT * FROM sys.sysusers WHERE name = '{0}')
                DROP USER {0};

                -- Create the database user for the login and add as dbowner
                IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = '{0}')
                BEGIN
                    CREATE USER [{0}] FOR LOGIN [{0}]
                    EXEC sp_addrolemember N'db_owner', '{0}'
                END;";
    }
}
