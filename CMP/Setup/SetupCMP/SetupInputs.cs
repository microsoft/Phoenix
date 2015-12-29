// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.VirtualManager.Setup
{
    using CMP.Setup.SetupLogger;
    using CMP.Setup.SetupFramework;

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
                    case SetupInputTags.CompanyNameTag:
                    {

                        PropertyBagDictionary.Instance.SafeAdd(parameter, new CompanyName());
                        break;
                    }
                    case SetupInputTags.ProductKeyTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new ProductKey());
                        break;
                    }
                    case SetupInputTags.BinaryInstallLocationTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new BinaryInstallLocation());
                        break;
                    }
                    case SetupInputTags.IntegratedInstallSourceTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new IntegratedInstallSource());
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
                    case SetupInputTags.ForceHAVMMUninstallTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new ForceHAVMMUninstall());
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
                    case SetupInputTags.IndigoTcpPortTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new IndigoTcpPort());
                        break;
                    }
                    case SetupInputTags.IndigoHTTPSPortTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new IndigoHTTPSPort());
                        break;
                    }
                    case SetupInputTags.IndigoNETTCPPortTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new IndigoNETTCPPort());
                        break;
                    }
                    case SetupInputTags.IndigoHTTPPortTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new IndigoHTTPPort());
                        break;
                    }
                    case SetupInputTags.WSManTcpPortTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new WSManTcpPort());
                        break;
                    }
                    case SetupInputTags.BitsTcpPortTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new BitsTcpPort());
                        break;
                    }
                    case SetupInputTags.VmmServerNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new VmmServerName());
                        break;
                    }
                    case SetupInputTags.VmmServiceLocalAccountTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new VmmServiceLocalAccount());
                        break;
                    }
                    case SetupInputTags.VmmServiceDomainTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new VmmServiceDomain());
                        break;
                    }
                    case SetupInputTags.VmmServiceUserNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new VmmServiceUserName());
                        break;
                    }
                    case SetupInputTags.VmmServiceUserPasswordTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new VmmServiceUserPassword());
                        break;
                    }
                    case SetupInputTags.CreateNewLibraryShareTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new CreateNewLibraryShare());
                        break;
                    }
                    case SetupInputTags.LibraryShareNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new LibraryShareName());
                        break;
                    }
                    case SetupInputTags.LibrarySharePathTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new LibrarySharePath());
                        break;
                    }
                    case SetupInputTags.LibraryShareDescriptionTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new LibraryShareDescription());
                        break;
                    }
                    case SetupInputTags.SQMOptInTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new SQMOptIn());
                        break;
                    }
                    case SetupInputTags.MUOptInTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new MUOptIn());
                        break;
                    }
                    case SetupInputTags.TopContainerNameTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new TopContainerName());
                        break;
                    }
                    case SetupInputTags.HighlyAvailableTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new HighlyAvailable());
                        break;
                    }
                    case SetupInputTags.HighlyAvailable2ndNodeTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new HighlyAvailable2ndNode());
                        break;
                    }
                    case SetupInputTags.UpgradeTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new Upgrade());
                        break;
                    }
                    case SetupInputTags.VMMStaticIPAddressTag:
                    {
                        PropertyBagDictionary.Instance.SafeAdd(parameter, new VMMStaticIPAddress());
                        break;
                    }
                    default:
                    {
                        AppAssert.Assert (false, "Unknown parameter", parameter);
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
            parameterList.Add(SetupInputTags.CompanyNameTag);
            parameterList.Add(SetupInputTags.ProductKeyTag);

            parameterList.Add(SetupInputTags.BinaryInstallLocationTag);
            parameterList.Add(SetupInputTags.IntegratedInstallSourceTag);

            parameterList.Add(SetupInputTags.CreateNewSqlDatabaseTag);
            parameterList.Add(SetupInputTags.RetainSqlDatabaseTag);
            parameterList.Add(SetupInputTags.ForceHAVMMUninstallTag);
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

            parameterList.Add(SetupInputTags.IndigoTcpPortTag);
            parameterList.Add(SetupInputTags.IndigoHTTPSPortTag);
            parameterList.Add(SetupInputTags.IndigoNETTCPPortTag);
            parameterList.Add(SetupInputTags.IndigoHTTPPortTag);
            parameterList.Add(SetupInputTags.WSManTcpPortTag);
            parameterList.Add(SetupInputTags.BitsTcpPortTag);

            parameterList.Add(SetupInputTags.VmmServerNameTag);

            parameterList.Add(SetupInputTags.CreateNewLibraryShareTag);
            parameterList.Add(SetupInputTags.LibraryShareNameTag);
            parameterList.Add(SetupInputTags.LibrarySharePathTag);
            parameterList.Add(SetupInputTags.LibraryShareDescriptionTag);
            parameterList.Add(SetupInputTags.SQMOptInTag);
            parameterList.Add(SetupInputTags.MUOptInTag);

            parameterList.Add(SetupInputTags.VmmServiceLocalAccountTag);
            parameterList.Add(SetupInputTags.VmmServiceDomainTag);
            parameterList.Add(SetupInputTags.VmmServiceUserNameTag);
            parameterList.Add(SetupInputTags.VmmServiceUserPasswordTag);

            parameterList.Add(SetupInputTags.TopContainerNameTag);

            parameterList.Add(SetupInputTags.HighlyAvailableTag);
            parameterList.Add(SetupInputTags.HighlyAvailable2ndNodeTag);
            parameterList.Add(SetupInputTags.VMMStaticIPAddressTag);
            parameterList.Add(SetupInputTags.UpgradeTag);

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
            
            Tracer.Trace.TraceMessage(CallSite.New(), TraceFlag.DbgNormal,  "Load inputs from file {0}", file);

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
                    Trc.Log(Microsoft.VirtualManager.SetupFramework.LogLevel.Always, parameter);
                    Trc.Log(Microsoft.VirtualManager.SetupFramework.LogLevel.Always, inputParameter.InputValue.ToString());
                }
            }
        }
    }
}
