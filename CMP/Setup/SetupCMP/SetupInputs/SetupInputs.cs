// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CMP.Setup
{
    using CMP.Setup.SetupFramework;
    using CMP.Setup.Helpers;

    /// <summary>
    /// SetupInputs stores all setup inputs
    /// </summary>
    public class SetupInputs
    {
        private StringCollection parameterList;

        private SetupInputs()
        {
        }

        private static SetupInputs setupInputs = new SetupInputs();

        public static SetupInputs Instance
        {
            get
            {
                return setupInputs;
            }
        }

        /// <summary>
        /// Loads the paramneters into wizard inputs
        /// </summary>
        /// <param name="parameterList"></param>
        public void LoadParameterList(StringCollection parameterList)
        {
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }

            this.parameterList = parameterList;

            InputParameter inputParameter = null;
            foreach (String parameter in parameterList)
            {
                switch(parameter)
                {
                    case SetupInputTags.SetupUserAccountTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new SetupUserAccount());
                        break;
                    }
                    case SetupInputTags.UserNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new UserName());
                        break;
                    }
                    case SetupInputTags.BinaryInstallLocationTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new BinaryInstallLocation());
                        break;
                    }
                    case SetupInputTags.CreateNewSqlDatabaseTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new CreateNewSqlDatabase());
                        break;
                    }
                    case SetupInputTags.RetainSqlDatabaseTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new RetainSqlDatabase());
                        break;
                    }
                    case SetupInputTags.SqlInstanceNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new SqlInstanceName());
                        break;
                    }
                    case SetupInputTags.SqlDatabaseNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new SqlDatabaseName());
                        break;
                    }
                    case SetupInputTags.RemoteDatabaseImpersonationTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new RemoteDatabaseImpersonation());
                        break;
                    }
                    case SetupInputTags.SqlMachineNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new SqlMachineName());
                        break;
                    }
                    case SetupInputTags.SqlDBAdminNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new SqlDBAdminName());
                        break;
                    }
                    case SetupInputTags.SqlDBAdminPasswordTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new SqlDBAdminPassword());
                        break;
                    }
                    case SetupInputTags.SqlDBAdminDomainTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new SqlDBAdminDomain());
                        break;
                    }
                    case SetupInputTags.SqlServerPortTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new SqlServerPort());
                        break;
                    }
                    case SetupInputTags.WapCreateNewSqlDatabaseTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new WapCreateNewSqlDatabase());
                        break;
                    }
                    case SetupInputTags.WapRetainSqlDatabaseTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new WapRetainSqlDatabase());
                        break;
                    }
                    case SetupInputTags.WapSqlInstanceNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new WapSqlInstanceName());
                        break;
                    }
                    case SetupInputTags.WapSqlDatabaseNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new WapSqlDatabaseName());
                        break;
                    }
                    case SetupInputTags.WapRemoteDatabaseImpersonationTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new WapRemoteDatabaseImpersonation());
                        break;
                    }
                    case SetupInputTags.WapSqlMachineNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new WapSqlMachineName());
                        break;
                    }
                    case SetupInputTags.WapSqlDBAdminNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new WapSqlDBAdminName());
                        break;
                    }
                    case SetupInputTags.WapSqlDBAdminPasswordTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new WapSqlDBAdminPassword());
                        break;
                    }
                    case SetupInputTags.WapSqlDBAdminDomainTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new WapSqlDBAdminDomain());
                        break;
                    }
                    case SetupInputTags.WapSqlServerPortTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new WapSqlServerPort());
                        break;
                    }
                    case SetupInputTags.SqlDataFileLocationTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new SqlFileLocation());
                        break;
                    }
                    case SetupInputTags.SqlLogFileLocationTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new SqlFileLocation());
                        break;
                    }
                    case SetupInputTags.CmpServerNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new CmpServerName());
                        break;
                    }
                    case SetupInputTags.CmpCertificateThumbprintTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new CmpCertificateThumbprint());
                        break;
                    }
                    case SetupInputTags.CmpServiceLocalAccountTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new CmpServiceLocalAccount());
                        break;
                    }
                    case SetupInputTags.CmpServiceDomainTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new VmmServiceDomain());
                        break;
                    }
                    case SetupInputTags.CmpServiceUserNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new VmmServiceUserName());
                        break;
                    }
                    case SetupInputTags.CmpServiceUserPasswordTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new VmmServiceUserPassword());
                        break;
                    }
                    default:
                    {
                        throw new ArgumentException("Unknown parameter");
                        break;
                    }
                }

                // Initialize all the parameters to defaults
                inputParameter = this.FindItem(parameter);
                inputParameter.ResetToDefault();
            }
        }

        #region Parameter List definitions

        public StringCollection GetUserInputParameterList()
        {
            StringCollection parameterList = new StringCollection();
            parameterList.Add(SetupInputTags.SetupUserAccountTag);
            parameterList.Add(SetupInputTags.UserNameTag);

            parameterList.Add(SetupInputTags.BinaryInstallLocationTag);

            parameterList.Add(SetupInputTags.CreateNewSqlDatabaseTag);
            parameterList.Add(SetupInputTags.RetainSqlDatabaseTag);
            parameterList.Add(SetupInputTags.WapCreateNewSqlDatabaseTag);
            parameterList.Add(SetupInputTags.WapRetainSqlDatabaseTag);
            parameterList.Add(SetupInputTags.WapSqlServerPortTag);
            parameterList.Add(SetupInputTags.WapSqlInstanceNameTag);
            parameterList.Add(SetupInputTags.WapSqlDatabaseNameTag);
            parameterList.Add(SetupInputTags.WapRemoteDatabaseImpersonationTag);
            parameterList.Add(SetupInputTags.WapSqlMachineNameTag);
            parameterList.Add(SetupInputTags.WapSqlDBAdminNameTag);
            parameterList.Add(SetupInputTags.WapSqlDBAdminPasswordTag);
            parameterList.Add(SetupInputTags.WapSqlDBAdminDomainTag);


            parameterList.Add(SetupInputTags.SqlServerPortTag);
            parameterList.Add(SetupInputTags.SqlInstanceNameTag);
            parameterList.Add(SetupInputTags.SqlDatabaseNameTag);
            parameterList.Add(SetupInputTags.RemoteDatabaseImpersonationTag);
            parameterList.Add(SetupInputTags.SqlMachineNameTag);
            parameterList.Add(SetupInputTags.SqlDBAdminNameTag);
            parameterList.Add(SetupInputTags.SqlDBAdminPasswordTag);
            parameterList.Add(SetupInputTags.SqlDBAdminDomainTag);
            parameterList.Add(SetupInputTags.SqlDataFileLocationTag);
            parameterList.Add(SetupInputTags.SqlLogFileLocationTag);

            parameterList.Add(SetupInputTags.CmpServerNameTag);

            parameterList.Add(SetupInputTags.CmpCertificateThumbprintTag);

            parameterList.Add(SetupInputTags.CmpServiceLocalAccountTag);
            parameterList.Add(SetupInputTags.CmpServiceDomainTag);
            parameterList.Add(SetupInputTags.CmpServiceUserNameTag);
            parameterList.Add(SetupInputTags.CmpServiceUserPasswordTag);

            return parameterList;
        }
        
        #endregion

        /// <summary>
        /// Loads input values from a file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="parameterList"></param>
        public void LoadFrom(String file, StringCollection parameterList)
        {
            AppAssert.AssertNotNull (file, "file");
            AppAssert.AssertNotNull (parameterList, "parameterList");

            this.LoadParameterList(parameterList);

            InputParameter inputParameter = null;
            foreach (String parameter in parameterList)
            {
                inputParameter = this.FindItem(parameter);
                inputParameter.LoadInputValue(file);
            }
        }

        /// <summary>
        /// Loads the input values from the command line
        /// </summary>
        /// <param name="cmdLineParameters"></param>
        public void LoadCmdLineParameters(Dictionary<string, object> cmdLineParameters)
        {
            Dictionary<string, object>.KeyCollection keyCollection = cmdLineParameters.Keys;

            foreach (String parameter in keyCollection)
            {   
                this.EditItem(parameter, cmdLineParameters[parameter]);
            }
            
        }

        /// <summary>
        /// Edits the value of a parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        virtual public void EditItem(String parameter, object value)
        {
            AppAssert.AssertNotNull (parameter, "parameter");
            AppAssert.Assert (this.parameterList.Contains(parameter), "Parameter List does not contain this parameter", parameter);
            AppAssert.AssertNotNull (value, "value");
            
            InputParameter inputParameter = PropertyBagDictionary.Instance.GetProperty<InputParameter>(parameter);
            inputParameter.SetInputValue(value);
        }

        /// <summary>
        /// Returns a parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        virtual public InputParameter FindItem(String parameter)
        {
            AppAssert.AssertNotNull (parameter, "parameter");

            return PropertyBagDictionary.Instance.GetProperty<InputParameter>(parameter);
        }

        /// <summary>
        /// Logs all inputs to the log file
        /// </summary>
        public void LogInputs()
        {
            InputParameter inputParameter = null;
            foreach (String parameter in parameterList)
            {
                inputParameter = this.FindItem(parameter);
                if (inputParameter.Valid && inputParameter.CanLogToFile)
                {
                    SetupLogger.LogInfo(parameter);
                    SetupLogger.LogInfo(inputParameter.InputValue.ToString());
                }
            }
        }
    }
}
