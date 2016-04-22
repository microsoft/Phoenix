//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Drives the setup.exe Program.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using CMP.Setup.Helpers;
    using CMP.Setup.SetupFramework;
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Xml.Serialization;
    using WpfResources;

    /// <summary>
    /// List of values that the setup can return
    /// </summary>
    public enum SetupReturnValues
    {
        /// <summary>
        /// There is already another setup running on this computer
        /// </summary>
        AnotherSetupRunningOnThisMachine = 18,

        /// <summary>
        /// Can not install on this machine
        /// </summary>
        InvalidToInstallOnThisMachine = 17,

        /// <summary>
        /// CD Image is not valid
        /// </summary>
        InvalidInstallImage = 16,

        /// <summary>
        /// Command Line is not valid
        /// </summary>
        InvalidCommandLine = 15,

        /// <summary>
        /// Failed prereq checks
        /// </summary>
        FailedPrerequisiteChecks = 14,

        /// <summary>
        /// The installed vNext feature's version does not match setup version
        /// </summary>
        DifferentVersionInstalled = 13,

        /// <summary>
        /// Setup failed
        /// </summary>
        Failed = 1,

        /// <summary>
        /// setup was successful
        /// </summary>
        Successful = 0,

        /// <summary>
        /// Setup was successful and we need a reboot
        /// </summary>
        SuccessfulNeedReboot = 3010,
    }

    /// <summary>
    /// This is the main class that control the setup.exe
    /// </summary>
    public static class Program
    {
        private const string SetupWizardLogFileName = "SetupWizard.log";
        // Event name triggered by Setup Wizard just before loading, Launch screen waits for this event
        private const String SetupWizardEventName = "VMSetup1CC42CD0-8402-426a-9F03-3243C7BC367A";
        private static SplashPage splashPage = null;

        private static StringCollection parameterList = SetupInputs.Instance.GetUserInputParameterList();

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <returns>int a value from the SetupReturnValues</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Threading.Mutex"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Setup should never crash on the user.  By catching the general exception at this level, we keep that from happening.")]
        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        [STAThread]
        public static int Main()
        {
            SetupReturnValues returnValue = SetupReturnValues.Failed;

            // This is needed to load the WPF stuff so
            // that we can get at the resources.
            Application apptry = Application.Current;
            Application.Equals(apptry, null);

            SetDefaultPropertyValues();

            SetupLogger.Initialize(PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.DefaultLogName));
            SetupLogger.LogInfo("Application Started");
            bool createdNew;
            Mutex mSetupwizard = new Mutex(true, @"Global\Phoenix Setup", out createdNew);
            if (!createdNew)
            {
                SetupLogger.LogInfo("There is already another setup wizard running on this computer.");
                SetupLogger.LogInfo("Please wait until that program is finished.");
                returnValue = SetupReturnValues.AnotherSetupRunningOnThisMachine;
                MessageBox.Show(WPFResourceDictionary.AnotherSetupRunningOnThisMachineMessage, WPFResourceDictionary.SetupMessageBoxTitle, MessageBoxButton.OK);
                goto AllDone;
            }

            try
            {
                // Set CurrentDirectory
                Environment.CurrentDirectory = PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagDictionary.SetupExePath);

                try
                {
                    SetupLogger.Initialize(SetupHelpers.SetLogFilePath("SetupWizardAdditional.log"), LogLevel.Verbose, true);
                }
                catch (Exception exception)
                {
                    // Since the exception is thrown before the logger is initialized, 
                    // just display the message in a message box
                    MessageBox.Show(exception.Message, WPFResourceDictionary.SetupMessageBoxTitle, MessageBoxButton.OK);
                    goto AllDone;
                }

                if (!ParseCommandLine())
                {
                    // Could not parse the command line
                    returnValue = SetupReturnValues.InvalidCommandLine;
                    goto AllDone;
                }

                // Get the state of the components on; this machine
                SystemStateDetection.CheckInstalledComponents();

                // If not silent, show splash screen here
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.Silent))
                {
                    Program.splashPage = new SplashPage();
                    Program.splashPage.Show();
                }
                else
                {
                    // Silent setup
                    // Check command line against the existing components
                    if (!CheckCommandLine())
                    {
                        returnValue = SetupReturnValues.InvalidCommandLine;
                        goto AllDone;
                    }
                }

                // If we don't have a location for the setup files, assume current location.
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.LocationOfSetupFiles))
                {
                    PropertyBagDictionary.Instance.SafeAdd(
                        PropertyBagConstants.LocationOfSetupFiles,
                        PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagDictionary.SetupExePath));
                }

                // Check to see if the path is valid for our install
                if (!SetupValidationHelpers.IsValidPathForInstall(SetupInputs.Instance.FindItem(SetupInputTags.BinaryInstallLocationTag)))
                {
                    SetupLogger.LogInfo("Invalid path passed to the setup. {0}", SetupInputs.Instance.FindItem(SetupInputTags.BinaryInstallLocationTag));
                    returnValue = SetupReturnValues.InvalidCommandLine;
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.Silent))
                    {
                        goto AllDone;
                    }
                }

                // What we have selected is valid given the machine state, etc...
                if (!SetupHelpers.RationalizeComponents())
                {
                    SetupLogger.LogInfo("We could not rationalize the component choices.  We must fail.");
                    returnValue = SetupReturnValues.InvalidCommandLine;

                    goto AllDone;
                }

                // Need to load the install data items
                XmlSerializer dataItemSerializer = new XmlSerializer(typeof(InstallItems));
                using (MemoryStream stream = new MemoryStream())
                {
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(Properties.Resources.InstallItems);
                    writer.Flush();
                    stream.Position = 0;
                    InstallItems inputInstallItems = (InstallItems)dataItemSerializer.Deserialize(stream);
                    SetupLogger.LogInfo("Start adding DataItems");
                    foreach (InstallItemsInstallDataItem installDataItem in inputInstallItems.InstallDataItem)
                    {
                        SetupLogger.LogInfo(installDataItem.DisplayTitle);

                        // localize the display title
                        installDataItem.DisplayTitle = InstallItemDisplayTitleEnumHelper.GetName((InstallItem)Enum.Parse(typeof(InstallItem), installDataItem.DisplayTitle));
                        // Add this Item to our data items
                        InstallDataItemRegistry.Instance.RegisterDataItem(installDataItem);
                    }
                    SetupLogger.LogInfo("Done adding DataItems");
                }

                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.Silent))
                {
                    returnValue = Program.SilentRun();
                }
                else
                {
                    Program.UiRun();
                    // Set the application return value: Always success in UI mode
                    returnValue = SetupReturnValues.Successful;
                }

                // Create the username/password in the DBs that the service will use to access the DB
                SetupDatabaseHelper.CreateSqlLoginUser(SetupDatabaseHelper.SqlUsernameDuringInstall, SetupDatabaseHelper.SqlDbUserPassword);
            }
            catch (Exception exception)
            {
                SetupLogger.LogInfo("Uncaught Exception", exception);
            }
            finally
            {
            }


            // All done with the install.
        AllDone:
            // Reboot if we have to
            RebootIfNeeded();

            //release the setup wizard mutex
            mSetupwizard.ReleaseMutex();

            return (int)returnValue;
        }

        /// <summary>
        /// Parses the command line.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        /// <returns>true if no errors</returns>
        private static bool ParseCommandLine()
        {
            // Get command line arguments and decide if setup is to be Unattended
            String[] arguments = Environment.GetCommandLineArgs();
            CommandlineParameters commandlineParameters = new CommandlineParameters();

            // Parse command line arguments
            commandlineParameters.ParseCommandline(arguments);

            if ((bool)commandlineParameters.GetParameterValue(CommandlineParameterId.Help) == true)
            {
                MessageBox.Show(WPFResourceDictionary.CommandlineUsage, WPFResourceDictionary.SetupMessageBoxTitle, MessageBoxButton.OK);
                return false;
            }

            SetupLogger.LogInfo("Main : Current directory = {0}", Environment.CurrentDirectory);

            if (commandlineParameters.IsParameterSpecified(CommandlineParameterId.Action) == false)
            {
                commandlineParameters.SetParameterValue(CommandlineParameterId.Action, SetupActions.UserInterface);
            }

            if ((SetupActions)commandlineParameters.GetParameterValue(CommandlineParameterId.Action) == SetupActions.UnattendedInstall &&
                (bool)commandlineParameters.GetParameterValue(CommandlineParameterId.AcceptLicense) == false &&
                (bool)commandlineParameters.GetParameterValue(CommandlineParameterId.OemSetup) == false)
            {
                SetupLogger.LogInfo("Have NOT acknowledge acceptance of the license terms in unattended install.");
                return false;
            }

            String iniFile = (String)commandlineParameters.GetParameterValue(CommandlineParameterId.IniFile);
            try
            {
                if (iniFile == null)
                {
                    SetupInputs.Instance.LoadParameterList(parameterList);
                }
                else
                {
                    SetupInputs.Instance.LoadFrom(iniFile, parameterList);
                }
            }
            catch (Exception ex)
            {
                SetupLogger.LogInfo("ParseCommandLine() ", ex);
                return false;
            }

            // Need to save the installation location in property bag for prerequisite check
            String installationLocation = SetupInputs.Instance.FindItem(SetupInputTags.BinaryInstallLocationTag);
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.InstallationLocation, installationLocation);

            String installSource = (String)commandlineParameters.GetParameterValue(CommandlineParameterId.SetupLocation);
            if (!String.IsNullOrEmpty(installSource))
            {
                installSource = Path.Combine(installSource, SetupConstants.SetupFolder);
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.LocationOfSetupFiles, installSource);
            }
            else
            {
                installSource = PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagDictionary.SetupExePath);
            }
            String certificateThumbprint = SetupInputs.Instance.FindItem(SetupInputTags.CmpCertificateThumbprintTag);
            if (!String.IsNullOrEmpty(certificateThumbprint))
            {
                certificateThumbprint = "LocalMachine,My," + certificateThumbprint;
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.CMPCertificateThumbprint, certificateThumbprint);
            }

            SetupLogger.LogInfo("OriginalInstallSource: {0}", installSource);
            SetupActions setupAction = (SetupActions)commandlineParameters.GetParameterValue(CommandlineParameterId.Action);
            if (setupAction == SetupActions.UnattendedUninstall)
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.Uninstall, "1");
            }
            if (setupAction != SetupActions.UserInterface)
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Silent, true);

                SetupFeatures setupFeaturesFlags = (SetupFeatures)commandlineParameters.GetParameterValue(CommandlineParameterId.Feature);
                if ((setupFeaturesFlags & SetupFeatures.TenantExtension) == SetupFeatures.TenantExtension)
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.TenantExtension, "1");
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ExtensionCommon, "1");
                }
                if ((setupFeaturesFlags & SetupFeatures.AdminExtension) == SetupFeatures.AdminExtension)
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AdminExtension, "1");
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ExtensionCommon, "1");
                }
                if ((setupFeaturesFlags & SetupFeatures.Server) == SetupFeatures.Server)
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.Server, "1");
                }
            }

            bool isLocalSystemAccount = SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceLocalAccountTag);
            if (!isLocalSystemAccount)
            {
                SetupInputs.Instance.EditItem(SetupInputTags.CmpServiceDomainTag, commandlineParameters.GetParameterValue(CommandlineParameterId.CmpServiceDomain));
                SetupInputs.Instance.EditItem(SetupInputTags.CmpServiceUserNameTag, commandlineParameters.GetParameterValue(CommandlineParameterId.CmpServiceUserName));
                SetupInputs.Instance.EditItem(SetupInputTags.CmpServiceUserPasswordTag, commandlineParameters.GetParameterValue(CommandlineParameterId.CmpServiceUserPassword));
            }

            bool isDatabaseImpersonation = SetupInputs.Instance.FindItem(SetupInputTags.RemoteDatabaseImpersonationTag);
            if (isDatabaseImpersonation)
            {
                try
                {
                    SetupInputs.Instance.EditItem(SetupInputTags.SqlDBAdminDomainTag, commandlineParameters.GetParameterValue(CommandlineParameterId.SqlDBAdminDomain));
                    SetupInputs.Instance.EditItem(SetupInputTags.SqlDBAdminNameTag, commandlineParameters.GetParameterValue(CommandlineParameterId.SqlDBAdminName));
                    SetupInputs.Instance.EditItem(SetupInputTags.SqlDBAdminPasswordTag, commandlineParameters.GetParameterValue(CommandlineParameterId.SqlDBAdminPassword));
                }
                catch (Exception ex)
                {
                    SetupLogger.LogInfo("ParseCommandLine(), SqlDBAdminDomain or SqlDBAdminName or SqlDBAdminPassword is null  or invalid", ex);
                    return false;
                }
            }

            if ((bool)commandlineParameters.GetParameterValue(CommandlineParameterId.OemSetup))
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.VhdVersionPreparation, true);
            }

            if ((bool)commandlineParameters.GetParameterValue(CommandlineParameterId.Configure))
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.VhdVersionConfiguration, true);
            }


            return true;
        }

        /// <summary>
        /// Check command line against the existing components
        /// </summary>
        private static bool CheckCommandLine()
        {
            // Uninstall
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server) &&
                    !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ServerVersion))
                {
                    SetupLogger.LogInfo("CheckCommandLine - command line error: Uninstall server, but server does NOT exist in the computer.");
                    return false;
                }
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension) &&
                    !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtensionVersion))
                {
                    // Uninstall tenant extension, but tenant extension does NOT exist in the computer.
                    SetupLogger.LogInfo("CheckCommandLine - command line error: Uninstall tenant extension, but tenant extension does NOT exist in the computer.");
                    return false;
                }
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon) &&
                    !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommonVersion))
                {
                    // Uninstall extension common components, but tenant extension does NOT exist in the computer.
                    SetupLogger.LogInfo("CheckCommandLine - command line error: Uninstall extension common components, but extension common components does NOT exist in the computer.");
                    return false;
                }
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension) &&
                    !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtensionVersion))
                {
                    // Uninstall admin extension, but admin extension does NOT exist in the computer.
                    SetupLogger.LogInfo("CheckCommandLine - command line error: Uninstall administrator extension, but v extension does NOT exist in the computer.");
                    return false;
                }
            }
            // Install
            else
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server) &&
                    PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ServerVersion))
                {
                    // Install server, but server already exists in the computer.
                    SetupLogger.LogInfo("CheckCommandLine -command line error: Install server, but server already exists in the computer.");
                    return false;
                }
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension) &&
                    PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtensionVersion))
                {
                    // Install tenant extension, but tenant extension already exists in the computer.
                    SetupLogger.LogInfo("CheckCommandLine -command line error: Install tenant extension, but tenant extension already exists in the computer.");
                    return false;
                }
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon) &&
                    PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommonVersion))
                {
                    // Install extension common components, but tenant extension common components already exists in the computer.
                    SetupLogger.LogInfo("CheckCommandLine -command line error: Install extension common components, but common components already exists in the computer.");
                    return false;
                }
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension) &&
                    PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtensionVersion))
                {
                    // Install admin extension, but admin extension already exists in the computer.
                    SetupLogger.LogInfo("CheckCommandLine -command line error: Install administrator extension, but administrator extension already exists in the computer.");
                    return false;
                }

                if ((PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server) ||
                    PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon)) &&
                    !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.CMPCertificateThumbprint))
                {
                    SetupLogger.LogInfo("CheckCommandLine -command line error: Install requires an imported certificate thumbprint for encryption but none was specified.");
                    return false;
                }
            }

            return true;
        }

        private static void SetDefaultPropertyValues()
        {
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.SetupStartTime, DateTime.Now);
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.PostInstall, "1");
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.DefaultLogPath, SetupHelpers.SetLogFolderPath());
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.DefaultLogName, SetupHelpers.SetLogFilePath(SetupWizardLogFileName));

            // Add the current path to the propertybag
            PropertyBagDictionary.Instance.SafeAdd(
                PropertyBagDictionary.SetupExePath,
                (new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)).DirectoryName);

            // Check running on 64bit machine
            if (SetupHelpers.ArchitectureIs64Check())
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ArchitectureIs64Check, true);
            }
        }

        /// <summary>
        /// Reboot if needed
        /// </summary>
        private static void RebootIfNeeded()
        {
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.RebootNow))
            {
                NativeMethods.ExitWindows(NativeMethods.EWX_REBOOT | NativeMethods.EWX_FORCEIFHUNG);
            }
        }

        /// <summary>
        /// Run Setup in UI mode.
        /// </summary>
        private static void UiRun()
        {
            App app = new App();
            app.Startup += new StartupEventHandler(app_Startup);
            app.Run();
        }

        static void app_Startup(object sender, StartupEventArgs e)
        {
            // Load and initializes all pages 
            IPageHost host = LoadUiPages();

            // Jump off the landing page to the correct page track.
            PageNavigation.Instance.MoveToNextPage();

            // In the page track, jump to the correct first page.
            PageNavigation.Instance.MoveToNextPage();

            // Close the splash page.
            if (Program.splashPage != null)
            {
                Program.splashPage.Close();
            }

            // Launch the UI 
            host.Closed += new EventHandler(Host_Closed);
            host.Show();
        }

        /// <summary>
        /// Loads and initializes all UI pages.
        /// </summary>
        /// <returns>the newly created IPageHost interface object</returns>
        private static IPageHost LoadUiPages()
        {
            // Creates a page factory, a host page and set up page navigation 
            Factory factory = new WpfFactory();
            IPageHost host = factory.CreateHost();
            PageNavigation.Instance.Host = host;

            // Add wait screen here if needed.
            XmlSerializer serializer = new XmlSerializer(typeof(Pages));
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(Properties.Resources.Pages);
                    writer.Flush();
                    stream.Position = 0;
                    Pages inputPages = (Pages)serializer.Deserialize(stream);

                    foreach (PagesPage page in inputPages.Page)
                    {
                        SetupLogger.LogInfo("Adding Page {0}", page.Id);
                        Page workingPage = new Page(factory, host, page);

                        // Add this page to our pages
                        PageRegistry.Instance.RegisterPage(workingPage);
                    }
                }
            }

            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ScreensLoaded, "1");
            return host;
        }

        /// <summary>
        /// Launch Setup in Silent Mode 
        /// </summary>
        /// <returns>a SetupReturnValues reflecting success or possible failure causes</returns>
        private static SetupReturnValues SilentRun()
        {
            // Setup the data for the items to install
            PrepareInstallData.PrepareInstallDataItems();

            // Check to see that we have all the install files we need.
            // Make sure the file locations are set for the install files.
            SetupFileValidation.ResetInstallItemFileLocations(
                    PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.LocationOfSetupFiles),
                    PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.LocationOfSetupFiles));

            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                if (!SetupFileValidation.HaveAllNeededInstallItemFiles())
                {
                    return SetupReturnValues.InvalidInstallImage;
                }

                SetupHelpers.SetFeatureSwitches();
                // If we are not uninstalling, Do the prereq check
                if (2 == SetupHelpers.DoAllPrerequisiteChecks())
                {
                    // We failed prereq tests so we will fail the install
                    SetupLogger.LogError("We failed the prerequisite checks.");
                    return SetupReturnValues.FailedPrerequisiteChecks;
                }

                // If this is a server installation, 
                // - check if there is an existing database, 
                // and if so, check if upgrade is supported from that version 
                // - make sure client is also installed with server. 
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
                {
                    // Error conditions
                    // An unsupported database found
                    // A supported database is found, but user didnt explicitly specify upgrade

                    // CheckDatabase will throw an exception if DB version is 
                    // incompatible
                    string sqlMachineName = (String)SetupInputs.Instance.FindItem(SetupInputTags.SqlMachineNameTag);

                    String fullInstanceName = SetupDatabaseHelper.ConstructFullInstanceName(
                        !SetupDatabaseHelper.SqlServerIsOnLocalComputer(sqlMachineName),
                        sqlMachineName,
                        (String)SetupInputs.Instance.FindItem(SetupInputTags.SqlInstanceNameTag),
                        (int)SetupInputs.Instance.FindItem(SetupInputTags.SqlServerPortTag));
                }
            }

            // Do the install using the passed information
            InstallActionProcessor installs = new InstallActionProcessor();
            SetupLogger.LogInfo("Silent ProcessInstalls Starting");

            SetupReturnValues rturn = installs.ProcessInstalls();

            SetupLogger.LogInfo("Silent ProcessInstalls Done");

            return rturn;
        }

        /// <summary>
        /// Handles the Closed event of the Host control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void Host_Closed(object sender, EventArgs e)
        {
            // Application.Exit();
        }
    }
}
