// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Security;
using System.Collections.Specialized;
using CMP.Setup.Helpers;

namespace CMP.Setup
{
    public class SetupUserAccount : StringParameter
    {
        public SetupUserAccount()
            : base(SetupInputTags.SetupUserAccountTag, false, String.Empty)
        {
        }

        public override void Validate(object newInputValue)
        {
            String newUserName = (String)newInputValue;
            newUserName = newUserName.Trim();
            if (newUserName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(
                    String.Format("{0} exceeds {1} characters", SetupInputTags.UserNameTag, SetupInputsConstants.MaxInputNameLength.ToString()));
            }

        }

        public override bool CanLogToFile
        {
            get
            {
                return false;
            }
        }
    }

    public class UserName : StringParameter
    {
        public UserName() : base(SetupInputTags.UserNameTag, false, String.Empty)
        {
        }

        public override void Validate(object newInputValue)
        {
            String newUserName = (String)newInputValue;
            newUserName = newUserName.Trim();
            if (newUserName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} exceeds {1} characters", SetupInputTags.UserNameTag, SetupInputsConstants.MaxInputNameLength.ToString()));
            }

        }

        public override bool CanLogToFile
        {
            get
            {
                return false;
            }
        }
    }

    public class BinaryInstallLocation : StringParameter
    {
        public BinaryInstallLocation() : base(SetupInputTags.BinaryInstallLocationTag, false, InputDefaults.InstallLocation)
        {
        }

        public override void Validate(object newInputValue)
        {
            String newBinaryInstallLocation = (String)newInputValue;
            if (newBinaryInstallLocation.IndexOfAny(Path.GetInvalidPathChars()) >= 0 ||
                newBinaryInstallLocation.Contains(InputDefaults.DoubleSeparatorCharString) ||
                newBinaryInstallLocation.Length == 0)
            {
                throw new Exception(
                    String.Format("The specified path {0} contains invalid characters", newBinaryInstallLocation));
            }

            if (newBinaryInstallLocation.Length > SetupInputsConstants.MaxInputPathLength)
            {
                throw new Exception(String.Format("The path {0} exceeds {1} characters", newBinaryInstallLocation, SetupInputsConstants.MaxInputPathLength.ToString()));
            }
            
            InstallLocationValidation.Instance.CheckForRemovableMedia(newBinaryInstallLocation);
            try
            {
                InstallLocationValidation.Instance.CheckForDirectoryAttributes(newBinaryInstallLocation);
            }
            catch (System.ArgumentException)
            {
                throw new Exception(
                    String.Format("The specified path {0} contains invalid characters", newBinaryInstallLocation));
            }
            catch (System.NotSupportedException)
            {
                throw new Exception(
                    String.Format("The specified path {0} contains invalid characters", newBinaryInstallLocation));
            }
        }
    }

    public class CreateNewSqlDatabase : BoolParameter
    {
        public CreateNewSqlDatabase()
            : base(SetupInputTags.CreateNewSqlDatabaseTag, false, InputDefaults.CreateNewSqlDatabase)
        {
        }

        public override void Validate(object newInputValue)
        {
            return;
        }
    }

    public class WapCreateNewSqlDatabase : BoolParameter
    {
        public WapCreateNewSqlDatabase()
            : base(SetupInputTags.WapCreateNewSqlDatabaseTag, false, InputDefaults.WapCreateNewSqlDatabase)
        {
        }

        public override void Validate(object newInputValue)
        {
            return;
        }
    }

    public class RetainSqlDatabase : BoolParameter
    {
        public RetainSqlDatabase()
            : base(SetupInputTags.RetainSqlDatabaseTag, false, InputDefaults.RetainSqlDatabase)
        {
        }

        public override void Validate(object newInputValue)
        {
            return;
        }
    }

    public class WapRetainSqlDatabase : BoolParameter
    {
        public WapRetainSqlDatabase()
            : base(SetupInputTags.WapRetainSqlDatabaseTag, false, InputDefaults.WapRetainSqlDatabase)
        {
        }

        public override void Validate(object newInputValue)
        {
            return;
        }
    }

    public class RemoteDatabaseImpersonation : BoolParameter
    {
        public RemoteDatabaseImpersonation()
            : base(SetupInputTags.RemoteDatabaseImpersonationTag, false, InputDefaults.RemoteDatabaseImpersonation)
        {
        }

        public override void Validate(object newInputValue)
        {
            return;
        }
    }

    public class WapRemoteDatabaseImpersonation : BoolParameter
    {
        public WapRemoteDatabaseImpersonation()
            : base(SetupInputTags.WapRemoteDatabaseImpersonationTag, false, InputDefaults.WapRemoteDatabaseImpersonation)
        {
        }

        public override void Validate(object newInputValue)
        {
            return;
        }
    }

    public class SqlServerPort : IntParameter
    {
        public SqlServerPort()
            : base(SetupInputTags.SqlServerPortTag, false, InputDefaults.SqlServerPort)
        {
        }

        public override void Validate(object newInputValue)
        {
            int port = (int)newInputValue;
            if (port < 0 || port > 65535)
            {
                throw new Exception(String.Format("{0} must be an integer between 0 and 65535", SetupInputTags.SqlServerPortTag));
            }
        }
    }

    public class WapSqlServerPort : IntParameter
    {
        public WapSqlServerPort()
            : base(SetupInputTags.WapSqlServerPortTag, false, InputDefaults.WapSqlServerPort)
        {
        }

        public override void Validate(object newInputValue)
        {
            int port = (int)newInputValue;
            if (port < 0 || port > 65535)
            {
                throw new Exception(String.Format("{0} must be an integer between 0 and 65535", SetupInputTags.WapSqlServerPortTag));
            }
        }
    }

    public class SqlInstanceName : StringParameter
    {
        public SqlInstanceName()
            : base(SetupInputTags.SqlInstanceNameTag, false, String.Empty)
        {
        }

        protected override object LoadInputValueFromFile(String file)
        {
            String instanceName = (String)base.LoadInputValueFromFile(file);
            int result = String.Compare(instanceName, SetupConstants.SqlServerDefaultInstanceName, StringComparison.InvariantCultureIgnoreCase);
            return ((result == 0) ? String.Empty : instanceName);
        }

        public override void Validate(object newInputValue)
        {
            String sqlInstanceName = (String)newInputValue;
            sqlInstanceName = sqlInstanceName.Trim();
            if (sqlInstanceName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} exceeds {1} characters", SetupInputTags.SqlInstanceNameTag, SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }
    }

    public class WapSqlInstanceName : StringParameter
    {
        public WapSqlInstanceName()
            : base(SetupInputTags.WapSqlInstanceNameTag, false, String.Empty)
        {
        }

        protected override object LoadInputValueFromFile(String file)
        {
            String instanceName = (String)base.LoadInputValueFromFile(file);
            int result = String.Compare(instanceName, SetupConstants.WapSqlServerDefaultInstanceName, StringComparison.InvariantCultureIgnoreCase);
            return ((result == 0) ? String.Empty : instanceName);
        }

        public override void Validate(object newInputValue)
        {
            String sqlInstanceName = (String)newInputValue;
            sqlInstanceName = sqlInstanceName.Trim();
            if (sqlInstanceName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} exceeds {1} characters", SetupInputTags.WapSqlInstanceNameTag, SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }
    }

    public class SqlDatabaseName : StringParameter
    {
        public SqlDatabaseName()
            : base(SetupInputTags.SqlDatabaseNameTag, false, InputDefaults.SqlDatabaseName)
        {
        }

        public override void Validate(object newInputValue)
        {
            String sqlDatabaseName = (String)newInputValue;
            sqlDatabaseName = sqlDatabaseName.Trim();
            if (sqlDatabaseName.Length == 0 ||
                sqlDatabaseName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters", SetupInputTags.SqlDatabaseNameTag, SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }
    }

    public class WapSqlDatabaseName : StringParameter
    {
        public WapSqlDatabaseName()
            : base(SetupInputTags.WapSqlDatabaseNameTag, false, InputDefaults.WapSqlDatabaseName)
        {
        }

        public override void Validate(object newInputValue)
        {
            String sqlDatabaseName = (String)newInputValue;
            sqlDatabaseName = sqlDatabaseName.Trim();
            if (sqlDatabaseName.Length == 0 ||
                sqlDatabaseName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters", SetupInputTags.WapSqlDatabaseNameTag, SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }
    }

    public class SqlMachineName : StringParameter
    {
        public SqlMachineName()
            : base(SetupInputTags.SqlMachineNameTag, false, Environment.MachineName)
        {
        }

        public override void Validate(object newInputValue)
        {
            String sqlMachineName = (String)newInputValue;
            if (sqlMachineName != null)
            {
                sqlMachineName = sqlMachineName.Trim();
            }

            if (sqlMachineName == null ||
                sqlMachineName.Length == 0 ||
                sqlMachineName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters",
                    SetupInputTags.SqlMachineNameTag,
                    SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }
    }

    public class WapSqlMachineName : StringParameter
    {
        public WapSqlMachineName()
            : base(SetupInputTags.WapSqlMachineNameTag, false, Environment.MachineName)
        {
        }

        public override void Validate(object newInputValue)
        {
            String sqlMachineName = (String)newInputValue;
            if (sqlMachineName != null)
            {
                sqlMachineName = sqlMachineName.Trim();
            }
            if (sqlMachineName == null ||
                sqlMachineName.Length == 0 ||
                sqlMachineName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters",
                    SetupInputTags.WapSqlMachineNameTag,
                    SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }
    }

    public class SqlDBAdminName : StringParameter
    {
        public SqlDBAdminName()
            : base(SetupInputTags.SqlDBAdminNameTag, false, null)
        {
        }

        public override void Validate(object newInputValue)
        {
            String sqlDBAdminName = (String)newInputValue;
            if (sqlDBAdminName != null)
            {
                sqlDBAdminName = sqlDBAdminName.Trim();
            }
            if (sqlDBAdminName == null ||
                sqlDBAdminName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters",
                    SetupInputTags.SqlDBAdminNameTag,
                    SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }

        public override bool CanLogToFile
        {
            get
            {
                return false;
            }
        }
    }

    public class WapSqlDBAdminName : StringParameter
    {
        public WapSqlDBAdminName()
            : base(SetupInputTags.WapSqlDBAdminNameTag, false, null)
        {
        }

        public override void Validate(object newInputValue)
        {
            String sqlDBAdminName = (String)newInputValue;
            if (sqlDBAdminName != null)
            {
                sqlDBAdminName = sqlDBAdminName.Trim();
            }
            if (sqlDBAdminName == null ||
                sqlDBAdminName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters",
                    SetupInputTags.WapSqlDBAdminNameTag,
                    SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }

        public override bool CanLogToFile
        {
            get
            {
                return false;
            }
        }
    }

    public class SqlDBAdminPassword : SecureStringParameter
    {
        public SqlDBAdminPassword()
            : base(SetupInputTags.SqlDBAdminPasswordTag, false, null)
        {
        }

        public override void Validate(object newInputValue)
        {
            SecureString sqlDBAdminPassword;
            if (newInputValue is SecureString)
            {
                // If this is passed in during the wizard running, it will already be a SecureString
                sqlDBAdminPassword = (SecureString)newInputValue;
            }
            else
            {
                char[] newInputValueChars = ((string)newInputValue).ToCharArray();
                sqlDBAdminPassword = new SecureString();
                foreach (char inputValueChar in newInputValueChars)
                {
                    sqlDBAdminPassword.AppendChar(inputValueChar);
                }
            }

            if (sqlDBAdminPassword.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters",
                    SetupInputTags.SqlDBAdminPasswordTag,
                    SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }

        public override bool CanLogToFile
        {
            get
            {
                return false;
            }
        }
    }

    public class WapSqlDBAdminPassword : SecureStringParameter
    {
        public WapSqlDBAdminPassword()
            : base(SetupInputTags.WapSqlDBAdminPasswordTag, false, null)
        {
        }

        public override void Validate(object newInputValue)
        {
            SecureString sqlDBAdminPassword;
            if (newInputValue is SecureString)
            {
                // If this is passed in during the wizard running, it will already be a SecureString
                sqlDBAdminPassword = (SecureString)newInputValue;
            }
            else
            {
                char[] newInputValueChars = ((string)newInputValue).ToCharArray();
                sqlDBAdminPassword = new SecureString();
                foreach (char inputValueChar in newInputValueChars)
                {
                    sqlDBAdminPassword.AppendChar(inputValueChar);
                }
            }

            if (sqlDBAdminPassword.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters",
                    SetupInputTags.WapSqlDBAdminPasswordTag,
                    SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }

        public override bool CanLogToFile
        {
            get
            {
                return false;
            }
        }
    }

    public class SqlDBAdminDomain : StringParameter
    {
        public SqlDBAdminDomain()
            : base(SetupInputTags.SqlDBAdminDomainTag, false, null)
        {
        }
        
        public override void Validate(object newInputValue)
        {
            String sqlDBAdminDomain = (String)newInputValue;
            if (sqlDBAdminDomain != null)
            {
                sqlDBAdminDomain = sqlDBAdminDomain.Trim();
            }
            if (sqlDBAdminDomain == null ||
                sqlDBAdminDomain.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters",
                    SetupInputTags.SqlDBAdminDomainTag,
                    SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }

        public override bool CanLogToFile
        {
            get
            {
                return false;
            }
        }
    }

    public class WapSqlDBAdminDomain : StringParameter
    {
        public WapSqlDBAdminDomain()
            : base(SetupInputTags.WapSqlDBAdminDomainTag, false, null)
        {
        }

        public override void Validate(object newInputValue)
        {
            String sqlDBAdminDomain = (String)newInputValue;
            if (sqlDBAdminDomain != null)
            {
                sqlDBAdminDomain = sqlDBAdminDomain.Trim();
            }
            if (sqlDBAdminDomain == null ||
                sqlDBAdminDomain.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters",
                    SetupInputTags.WapSqlDBAdminDomainTag,
                    SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }

        public override bool CanLogToFile
        {
            get
            {
                return false;
            }
        }
    }

    public class SqlFileLocation : StringParameter
    {
        public SqlFileLocation()
            : base(SetupInputTags.SqlDataFileLocationTag, false, null)
        {
        }
        
        public override void Validate(object newInputValue)
        {
            String path = (String)newInputValue;
            if (String.IsNullOrEmpty(path))
            {
                InstallLocationValidation.Instance.ThrowInvalidLocationException(path);
            }

            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0 ||
                path.Length == 0)
            {
                InstallLocationValidation.Instance.ThrowInvalidLocationException(path);
            }

            if (path.Length > SetupInputsConstants.MaxInputPathLength)
            {
                throw new Exception(String.Format("The path {0} exceeds {1} characters", path, SetupInputsConstants.MaxInputPathLength.ToString()));
            }

            Uri pathUri = null;
            Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out pathUri);
            if (pathUri != null && pathUri.IsUnc)
            {
                InstallLocationValidation.Instance.ThrowInvalidLocationException(path);
            }
        }

        public override bool CanLogToFile
        {
            get
            {
                return false;
            }
        }
    }

    public class CmpServerName : StringParameter
    {
        public CmpServerName()
            : base(SetupInputTags.CmpServerNameTag, false, null)
        {
        }

        public override void Validate(object newInputValue)
        {
            String serverName = (String)newInputValue;
            if (serverName != null)
            {
                serverName = serverName.Trim();
            }
            if (serverName == null ||
                serverName.Length == 0 ||
                serverName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters", SetupInputTags.CmpServerNameTag, SetupInputsConstants.MaxInputNameLength.ToString()));
            }

            String netBIOSName = serverName;
            int idx = serverName.IndexOf('.');
            if (idx >= 0)
            {
                netBIOSName = serverName.Substring(0, idx);
            }
            if (netBIOSName.Length == 0 ||
                netBIOSName.Length > SetupInputsConstants.MaxServiceNetBIOSNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters", SetupInputTags.CmpServerNameTag, SetupInputsConstants.MaxServiceNetBIOSNameLength.ToString()));
            }
        }
    }

    public class CmpCertificateThumbprint : StringParameter
    {
        public CmpCertificateThumbprint()
            : base(SetupInputTags.CmpCertificateThumbprintTag, false, null)
        {
        }

        public override void Validate(object newInputValue)
        {
            if (string.IsNullOrEmpty(newInputValue as string))
            {
                throw new Exception(String.Format("{0} is empty", SetupInputTags.CmpCertificateThumbprintTag));
            }
        }
    }

    public class CmpServiceLocalAccount : BoolParameter
    {
        public CmpServiceLocalAccount()
            : base(SetupInputTags.CmpServiceLocalAccountTag, false, InputDefaults.CmpServiceLocalAccount)
        {
        }

        public override void Validate(object newInputValue)
        {
            return;
        }

        public override bool CanLogToFile
        {
            get
            {
                return true;
            }
        }
    }

    public class VmmServiceDomain : StringParameter
    {
        public VmmServiceDomain()
            : base(SetupInputTags.CmpServiceDomainTag, false, null)
        {
        }

        public override void Validate(object newInputValue)
        {
            String vmmServiceDomain = (String)newInputValue;
            if (vmmServiceDomain != null)
            {
                vmmServiceDomain = vmmServiceDomain.Trim();
            }
            if (vmmServiceDomain == null ||
                vmmServiceDomain.Length == 0 ||
                vmmServiceDomain.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters", SetupInputTags.CmpServiceDomainTag, SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }

        public override bool CanLogToFile
        {
            get
            {
                return true;
            }
        }
    }

    public class VmmServiceUserName : StringParameter
    {
        public VmmServiceUserName()
            : base(SetupInputTags.CmpServiceUserNameTag, false, null)
        {
        }

        public override void Validate(object newInputValue)
        {
            String vmmServiceUserName = (String)newInputValue;
            if (vmmServiceUserName != null)
            {
                vmmServiceUserName = vmmServiceUserName.Trim();
            }
            if (vmmServiceUserName == null ||
                vmmServiceUserName.Length == 0 ||
                vmmServiceUserName.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters", SetupInputTags.CmpServiceUserNameTag, SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }

        public override bool CanLogToFile
        {
            get
            {
                return true;
            }
        }
    }

    public class VmmServiceUserPassword : SecureStringParameter
    {
        public VmmServiceUserPassword()
            : base(SetupInputTags.CmpServiceUserPasswordTag, false, null)
        {
        }

        public override void Validate(object newInputValue)
        {
            SecureString vmmServiceUserPassword;

            if (newInputValue is SecureString)
            {
                // If this is passed in during the wizard running, it will already be a SecureString
                vmmServiceUserPassword = (SecureString) newInputValue;
            }
            else
            {
                char[] newInputValueChars = ((string)newInputValue).ToCharArray();
                vmmServiceUserPassword = new SecureString();
                foreach (char inputValueChar in newInputValueChars)
                {
                    vmmServiceUserPassword.AppendChar(inputValueChar);
                }
            }

            if (vmmServiceUserPassword.Length == 0 ||
                vmmServiceUserPassword.Length > SetupInputsConstants.MaxInputNameLength)
            {
                throw new Exception(String.Format("{0} is empty or it exceeds {1} characters", SetupInputTags.CmpServiceUserPasswordTag, SetupInputsConstants.MaxInputNameLength.ToString()));
            }
        }

        public override bool CanLogToFile
        {
            get
            {
                return false;
            }
        }
    }
}
