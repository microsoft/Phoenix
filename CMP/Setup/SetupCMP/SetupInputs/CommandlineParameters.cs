// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Security;
using CMP.Setup.Helpers;

namespace CMP.Setup
{
    /// <summary>
    /// Setup actions that can be specified from command line
    /// </summary>
    public enum SetupActions
    {
        /// <summary>
        /// Unattended install action
        /// </summary>
        UnattendedInstall,

        /// <summary>
        /// Unattended uninstall action
        /// </summary>
        UnattendedUninstall,

        /// <summary>
        /// User interface action
        /// </summary>
        UserInterface,
    };

    /// <summary>
    /// Delegate that parses a command line argument
    /// </summary>
    delegate bool ParseArgumentDelegate (CommandlineArgument commandlineArgument, String[] arguments, ref int index);

    /// <summary>
    /// Enumeration of all command line arguments
    /// </summary>
    public enum CommandlineParameterId
    {
        QuietMode,
        Action,
        LogFile,
        SetupLocation,
        IniFile,
        Spawned,
        Help,
        AcceptLicense,
        OemSetup,
        Configure,
        Feature,
        Options,
        OpsMgrConfig,
        SqlDBAdminDomain,
        SqlDBAdminName,
        SqlDBAdminPassword,
        CmpServiceDomain,
        CmpServiceUserName,
        CmpServiceUserPassword,
        CmpCertificateThumpbrint,
        Count
    }

    /// <summary>
    /// Represents one command line argument
    /// </summary>
    class CommandlineArgument
    {
        object parameter;
        bool valid;
        CommandlineParameterId commandlineParameterId;

        private ParseArgumentDelegate parseArgumentDelegate;
        public CommandlineArgument(CommandlineParameterId commandlineParameterId, ParseArgumentDelegate parseArgumentDelegate, object defaultValue)
        {
            this.parameter = defaultValue;
            this.valid = false;
            this.commandlineParameterId = commandlineParameterId;
            this.parseArgumentDelegate = parseArgumentDelegate;
        }

        #region Properties to access the fields

        public CommandlineParameterId ParameterId
        {
            get
            {
                return this.commandlineParameterId;
            }
        }

        public object Parameter
        {
            get
            {
                return this.parameter;
            }
            set
            {
                this.parameter = value;
                this.valid = true;
            }
        }

        public bool Valid
        {
            get
            {
                return this.valid;
            }
        }

        public ParseArgumentDelegate ParseArgumentDelegate
        {
            get
            {
                return this.parseArgumentDelegate; 
            }
        }
        #endregion
    }
    
    /// <summary>
    /// Stores the values specified in command line
    /// </summary>
    public class CommandlineParameters
    {
        /// <summary>
        /// Each argument starts with either of these markers
        /// </summary>
        private readonly static String[] CommandlineParameterMarkers = new String[]{"/", "-"};

        /// <summary>
        /// Table used to parse the command line arguments
        /// </summary>
        private CommandlineArgument[] CommandlineParameterTable = new CommandlineArgument[]{  
            new CommandlineArgument(CommandlineParameterId.QuietMode, new ParseArgumentDelegate (ParseQuiteModeArgument), false),
            new CommandlineArgument(CommandlineParameterId.Action, new ParseArgumentDelegate (ParseActionArgument), SetupActions.UserInterface),
            new CommandlineArgument(CommandlineParameterId.LogFile, new ParseArgumentDelegate (ParseLogFileArgument), null),
            new CommandlineArgument(CommandlineParameterId.SetupLocation, new ParseArgumentDelegate (ParseSetupLocationArgument), String.Empty), 
            new CommandlineArgument(CommandlineParameterId.IniFile, new ParseArgumentDelegate (ParseIniFileArgument), null),
            new CommandlineArgument(CommandlineParameterId.Spawned, new ParseArgumentDelegate (ParseSpawnedArgument), false),
            new CommandlineArgument(CommandlineParameterId.Help, new ParseArgumentDelegate (ParseHelpArgument), false),
            new CommandlineArgument(CommandlineParameterId.AcceptLicense, new ParseArgumentDelegate (ParseAcceptLicenseArgument), false),
            new CommandlineArgument(CommandlineParameterId.OemSetup, new ParseArgumentDelegate (ParseOemSetupArgument), false),
            new CommandlineArgument(CommandlineParameterId.Configure, new ParseArgumentDelegate (ParseConfigArgument), false),
            new CommandlineArgument(CommandlineParameterId.Feature, new ParseArgumentDelegate (ParseFeatureArgument), false),
            new CommandlineArgument(CommandlineParameterId.Options, new ParseArgumentDelegate (ParseOptionsArgument), null),
            new CommandlineArgument(CommandlineParameterId.OpsMgrConfig, new ParseArgumentDelegate (ParseOpsMgrConfigArgument), false),
            new CommandlineArgument(CommandlineParameterId.CmpCertificateThumpbrint, new ParseArgumentDelegate (ParseCertificateThumbprintArgument), null),
            new CommandlineArgument(CommandlineParameterId.SqlDBAdminDomain, CreateCredentialArgumentParser(CommandlineParameterId.SqlDBAdminDomain), null),
            new CommandlineArgument(CommandlineParameterId.SqlDBAdminName, CreateCredentialArgumentParser(CommandlineParameterId.SqlDBAdminName), null),
            new CommandlineArgument(CommandlineParameterId.SqlDBAdminPassword, CreateCredentialArgumentParser(CommandlineParameterId.SqlDBAdminPassword), null),
            new CommandlineArgument(CommandlineParameterId.CmpServiceDomain, CreateCredentialArgumentParser(CommandlineParameterId.CmpServiceDomain), null),
            new CommandlineArgument(CommandlineParameterId.CmpServiceUserName, CreateCredentialArgumentParser(CommandlineParameterId.CmpServiceUserName), null),
            new CommandlineArgument(CommandlineParameterId.CmpServiceUserPassword, CreateCredentialArgumentParser(CommandlineParameterId.CmpServiceUserPassword), null),
        };

        public CommandlineParameters()
        {
            // Sanity check
            for (CommandlineParameterId id = 0; id < CommandlineParameterId.Count; id ++)
            {
                AppAssert.Assert (this.CommandlineParameterTable[(int)id].ParameterId == id, "CommandlineParameterTable is incorrectly initialized", String.Format("{0} != {1}", this.CommandlineParameterTable[(int)id].ParameterId, id));
            }
        }

        /// <summary>
        /// Parses the command line. 
        /// In case of errors, shows the error dialog and then rethrows the exception.
        /// </summary>
        /// <param name="arguments"></param>
        public void ParseCommandline(String[] arguments)
        {
            int index = 1, iterator = 0;

            try
            {
                // Parse command line arguments
                while (index < arguments.Length)
                {
                    SetupLogger.LogInfo("Main : Parse argument {0}", arguments[index]);

                    if (IsArgument(arguments[index]))
                    {
                        for (iterator = 0; iterator < CommandlineParameterTable.Length; iterator ++)
                        {
                            SetupLogger.LogInfo("Main : Calling parse function for {0}",CommandlineParameterTable[iterator].ParameterId);  

                            if (CommandlineParameterTable[iterator].ParseArgumentDelegate(CommandlineParameterTable[iterator], arguments, ref index))
                            {
                                SetupLogger.LogInfo("Main : The argument was parsed, move to next one."); 
                                break;
                            }
                        }
                        if (iterator >= CommandlineParameterTable.Length)
                        {
                            throw new Exception(String.Format("The parameter {0} is not valid", arguments[index]));
                        }
                    }
                    else
                    {
                        throw new Exception(String.Format("The parameter {0} is not valid", arguments[index]));
                    }
                }
            }
            catch (Exception backEndErrorException)
            {
                SetupLogger.LogInfo("SpawnSetup : {0}", backEndErrorException.ToString());

                if ((bool)this.GetParameterValue(CommandlineParameterId.QuietMode) == false)
                {
                    SetupHelpers.ShowError(backEndErrorException.Message);
                }
                else
                {
                    SetupLogger.LogError("Quite mode error: {0}", backEndErrorException.Message);
                }
                throw;
            }
        }

        public object GetParameterValue(CommandlineParameterId commandlineParameterId)
        {
            return this.CommandlineParameterTable[(int)commandlineParameterId].Parameter;
        }
        public void SetParameterValue(CommandlineParameterId commandlineParameterId, object input)
        {
            this.CommandlineParameterTable[(int)commandlineParameterId].Parameter = input;
        }


        public bool IsParameterSpecified(CommandlineParameterId commandlineParameterId)
        {
            return this.CommandlineParameterTable[(int)commandlineParameterId].Valid;
        }

        #region Argument interpretation helpers

        private static bool IsArgument(String argument)
        {
            foreach (String marker in CommandlineParameterMarkers)
            {
                if (argument.StartsWith(marker))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ConsumeArgument(CommandlineArgument commandlineArgument, String argument, String parameter, bool throwOnDuplicate=true)
        {
            foreach (String marker in CommandlineParameterMarkers)
            {
                int comparison = String.Compare (argument, marker + parameter, true);
                if (comparison == 0)
                {
                    if (commandlineArgument.Valid == true)
                    {
                        SetupLogger.LogError( "Argument {0} already set", argument);
                        if (throwOnDuplicate)
                        {
                            throw new Exception(String.Format("The parameter {0} is already specified.", argument));
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private static bool FetchValue(CommandlineArgument commandlineArgument, String[] arguments, int index)
        {
            if (index + 1 < arguments.Length)
            {
                if (IsArgument(arguments[index + 1]))
                {
                    throw new Exception(String.Format("The parameter {0} does not have a value associated with it", arguments[index]));
                }

                index ++;
                commandlineArgument.Parameter = arguments[index];
         
                return true;
            }
            else
            {
                throw new Exception(String.Format("The parameter {0} does not have a value associated with it", arguments[index]));
            }
        }

        #endregion

        #region Parsing routines

        private static bool ParseActionArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            String[] parameters = { "i", "x", "runui"};
            SetupActions[] actions = { SetupActions.UnattendedInstall, SetupActions.UnattendedUninstall, SetupActions.UserInterface }; 

            for (int iterator = 0; iterator < parameters.Length; iterator ++)
            {
                if (ConsumeArgument(commandlineArgument, arguments[index], parameters[iterator]))
                {
                    commandlineArgument.Parameter = actions[iterator];
                    index ++;
                    return true;
                }
            }
            
            return false;
        }

        private static bool ParseLogFileArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            if (ConsumeArgument(commandlineArgument, arguments[index], "L"))
            {
                if (FetchValue(commandlineArgument, arguments, index))
                {
                    index ++;
                    commandlineArgument.Parameter = ValidatePath((String)commandlineArgument.Parameter, true);
                    
                    arguments[index] = (String)commandlineArgument.Parameter;

                    SetupLogger.LogInfo("file path {0}", commandlineArgument.Parameter);  
                    index ++;
                    return true;
                }
            }
           
            return false;
        }

        private static bool ParseSpawnedArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            if (ConsumeArgument(commandlineArgument, arguments[index], "spawned"))
            {
                commandlineArgument.Parameter = true;
                index ++;
                return true;
            }
           
            return false;
        }

        private static bool ParseQuiteModeArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            if (ConsumeArgument(commandlineArgument, arguments[index], "q"))
            {
                commandlineArgument.Parameter = true;
                index ++;
                return true;
            }

            return false;
        }

        private static bool ParseHelpArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            if (ConsumeArgument(commandlineArgument, arguments[index], "?"))
            {
                commandlineArgument.Parameter = true;
                index ++;
                return true;
            }

            return false;
        }

        private static bool ParseSetupLocationArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            if (ConsumeArgument(commandlineArgument, arguments[index], "SetupLocation"))
            {
                if (FetchValue(commandlineArgument, arguments, index))
                {
                    index ++;
                    commandlineArgument.Parameter = ValidateDirectoryPath((String)commandlineArgument.Parameter);
                    
                    arguments[index] = (String)commandlineArgument.Parameter;

                    SetupLogger.LogInfo("file path {0}", commandlineArgument.Parameter);  
                    index ++;
                    return true;
                }
            }            
            return false;
        }

        private static bool ParseCertificateThumbprintArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            if (ConsumeArgument(commandlineArgument, arguments[index], "CmpCertificateThumbprint"))
            {
                if (FetchValue(commandlineArgument, arguments, index))
                {
                    index++;
                    commandlineArgument.Parameter = (String)commandlineArgument.Parameter;

                    arguments[index] = (String)commandlineArgument.Parameter;
                    index++;
                    return true;
                }
            }
            return false;
        }

        private static bool ParseIniFileArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            if (ConsumeArgument(commandlineArgument, arguments[index], "f"))
            {
                if (FetchValue(commandlineArgument, arguments, index))
                {
                    index ++;
                    commandlineArgument.Parameter = ValidateFilePath((String)commandlineArgument.Parameter);
                    
                    arguments[index] = (String)commandlineArgument.Parameter;

                    SetupLogger.LogInfo("file path {0}", commandlineArgument.Parameter);  
                    index ++;
                    return true;
                }
            }       
            
            return false;
        }

        private static bool ParseAcceptLicenseArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            if (ConsumeArgument(commandlineArgument, arguments[index], "IACCEPTSCEULA"))
            {
                commandlineArgument.Parameter = true;
                index++;
                return true;
            }

            return false;
        }

        private static bool ParseOemSetupArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            if (ConsumeArgument(commandlineArgument, arguments[index], "prep"))
            {
                commandlineArgument.Parameter = true;
                index ++;
                return true;
            }

            return false;
        }

        private static bool ParseConfigArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            if (ConsumeArgument(commandlineArgument, arguments[index], "config"))
            {
                commandlineArgument.Parameter = true;
                index++;
                return true;
            }

            return false;
        }

        private static bool ParseOpsMgrConfigArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            if (ConsumeArgument(commandlineArgument, arguments[index], "opsmgr"))
            {
                commandlineArgument.Parameter = true;
                index++;
                return true;
            }

            return false;
        }


        private static bool ParseFeatureArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            String[] parameters = { "tenant", "admin", "server"};
            SetupFeatures[] features = { SetupFeatures.TenantExtension, SetupFeatures.AdminExtension, SetupFeatures.Server};

            for (int iterator = 0; iterator < parameters.Length; iterator++)
            {
                if (ConsumeArgument(commandlineArgument, arguments[index], parameters[iterator], throwOnDuplicate:false))
                {
                    if ((commandlineArgument.Parameter != null) && (commandlineArgument.Parameter is SetupFeatures))
                    {
                        commandlineArgument.Parameter = features[iterator] | (SetupFeatures)commandlineArgument.Parameter;
                    }
                    else
                    {
                        commandlineArgument.Parameter = features[iterator];
                    }
                    index++;
                    return true;
                }
            }

            return false;
        }

        private static bool ParseOptionsArgument(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
        {
            const string optionsParameter = @"options";
            
            if (ConsumeArgument(commandlineArgument, arguments[index], optionsParameter))
            {
                if (FetchValue(commandlineArgument, arguments, index))
                {
                    index++;
                    commandlineArgument.Parameter = ValidateFilePath((String)commandlineArgument.Parameter);

                    arguments[index] = (String)commandlineArgument.Parameter;

                    SetupLogger.LogInfo("MST file path {0}", commandlineArgument.Parameter);
                    index++;
                    return true;
                }
            }

            return false;
        }

        private static ParseArgumentDelegate CreateCredentialArgumentParser(CommandlineParameterId id)
        {            
            return delegate(CommandlineArgument commandlineArgument, String[] arguments, ref int index)
            {
                string parmeterName = Enum.GetName(typeof(CommandlineParameterId), id);
                if (ConsumeArgument(commandlineArgument, arguments[index], parmeterName))
                {
                    if (FetchValue(commandlineArgument, arguments, index))
                    {
                        index++;
                        arguments[index] = (String)commandlineArgument.Parameter;

                        SetupLogger.LogInfo("{0} received", parmeterName);
                        index++;
                        return true;
                    }
                }
                return false;
            };
        }
        #endregion

        #region Path validation routines

        private static String ValidateFilePath(String path)
        {
            path = ValidatePath(path, true);
            if (!File.Exists(path))
            {
                throw new Exception(String.Format("The specified path {0} is not valid", path));
            }

            return path;
        }

        private static String ValidateDirectoryPath(String path)
        {
            path = ValidatePath(path, false);
            if (!Directory.Exists(path))
            {
                throw new Exception(String.Format("The specified path {0} is not valid", path));
            }

            return path;
        }

        private static String ValidatePath(String path, bool filePathExpected)
        {
            if (filePathExpected == true 
                && path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                throw new Exception(String.Format("The specified path {0} is not valid", path));
            }

            try
            {
                return Path.GetFullPath(path);
            }
            catch (ArgumentException argumentException)
            {

                throw new Exception(String.Format("The specified path {0} is not valid", path));
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                throw new Exception(String.Format("The specified path {0} is not valid", path));
            }
            catch (SecurityException securityException)
            {
                throw new Exception(String.Format("The specified path {0} is not valid", path));
            }
            catch (PathTooLongException pathTooLongException)
            {
                throw new Exception(String.Format("The specified path {0} is not valid", path));
            }
            catch (IOException ioException)
            {
                throw new Exception(String.Format("The specified path {0} is not valid", path));
            }
            catch (NotSupportedException notSupportedException)
            {
                throw new Exception(String.Format("The specified path {0} is not valid", path));
            }
        }
    
        #endregion
    }
}
