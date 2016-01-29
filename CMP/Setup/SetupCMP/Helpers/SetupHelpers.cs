//-----------------------------------------------------------------------
// <copyright file="SetupHelpers.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Provides Helper functions for the setup.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Management;
    using System.Security;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Text;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Resources;
    using System.Xml;
    using CMP.Setup.SetupFramework;
    using Microsoft.Win32;
    using WpfResources;
    using SetupFramework = CMP.Setup.SetupFramework;
    using CMP.Setup.Helpers;
    using System.Reflection;

    public enum InstallMode
    {
        Install,
        Uninstall,
        Update
    }

    /// <summary>
    /// Setup Helper class
    /// </summary>
    public static class SetupHelpers
    {
        /// <summary>
        /// Delimiter for user login name
        /// </summary>
        private const string UserLoginDelimiter1 = "@";

        /// <summary>
        /// Delimiter for user login name
        /// </summary>
        private const string UserLoginDelimiter2 = "\\";

        /// <summary>
        /// Name of the setup.
        /// </summary>
        private static string SetupExe = Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        #region Public

        /// <summary>
        /// Gets the add remove programs cache path.
        /// </summary>
        /// <returns>the path of the ARP Cache</returns>
        public static string GetAddRemoveProgramsCachePath()
        {
            string addRemoveProgramsCachePath = string.Empty;

            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AddRemoveProgramFilesPath))
            {
                //if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                //{
                //    // get the registry string
                //    addRemoveProgramsCachePath = SetupHelpers.ReadRemoteRegistryEx(SystemInformation.ComputerName, string.Format(CultureInfo.InvariantCulture, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{0}", WpfResources.WPFResourceDictionary.ProductTitle), "UninstallString");
                //    if (string.Equals(addRemoveProgramsCachePath, "Error", StringComparison.OrdinalIgnoreCase))
                //    {
                //        return string.Empty;
                //    }

                //    // Strip out the exe name from the path
                //    addRemoveProgramsCachePath =
                //        addRemoveProgramsCachePath.Replace("\"", String.Empty);
                //    addRemoveProgramsCachePath =
                //        addRemoveProgramsCachePath.Remove(
                //            addRemoveProgramsCachePath.LastIndexOf(
                //                (SetupConstants.SetupFolder + Path.DirectorySeparatorChar + SetupExe),
                //                StringComparison.OrdinalIgnoreCase));
                //}
                //else
                //{
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.VhdVersionConfiguration))
                    {
                        addRemoveProgramsCachePath = SetupConstants.GetServerInstallPath();
                    }
                    else
                    {
                        addRemoveProgramsCachePath = SetupInputs.Instance.FindItem(SetupInputTags.BinaryInstallLocationTag);
                    }
                //}

                if (!addRemoveProgramsCachePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    addRemoveProgramsCachePath = addRemoveProgramsCachePath + Path.DirectorySeparatorChar;
                }

                addRemoveProgramsCachePath = Path.Combine(
                addRemoveProgramsCachePath,
                SetupConstants.SetupFolder);

                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AddRemoveProgramFilesPath, addRemoveProgramsCachePath);
            }
            else
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.VhdVersionConfiguration))
                {
                    addRemoveProgramsCachePath = SetupConstants.GetServerSetupPath();
                }
                else
                {
                    addRemoveProgramsCachePath = PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.AddRemoveProgramFilesPath);
                }
            }

            return addRemoveProgramsCachePath;
        }

        /// <summary>
        /// Sets the log path.
        /// </summary>       
        /// <returns>a string that is the FQN of the log path</returns>
        public static string SetLogFolderPath()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), SetupConstants.LogFolder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        /// <summary>
        /// Sets the log file path.
        /// </summary>
        /// <param name="logFilename">The log filename.</param>
        /// <returns>a string that is the FQN of the log file</returns>
        public static string SetLogFilePath(string logFilename)
        {
            return Path.Combine(SetupHelpers.SetLogFolderPath(), logFilename);
        }

        public static string GetPrereqResultFilePath()
        {
            return SetupHelpers.SetLogFilePath("FullPrereq.xml");
        }


        /// <summary>
        /// Removes the folder and files.
        /// </summary>
        /// <param name="folderToRemove">The folder to remove.</param>
        /// <param name="writeHeader">if set to <c>true</c> [write header].</param>
        public static void RemoveFolderAndFiles(string folderToRemove, bool writeHeader)
        {
            // Ok need to 
            // 1. Process all the subfolders
            // 2. Remove all the folders from the dir
            // 3. Remove all the files from the dir
            if (writeHeader)
            {
                SetupLogger.LogInfo("*****");
                SetupLogger.LogInfo("*****RemoveFolderAndFiles: Start delete status messages for {0}", folderToRemove);
            }

            bool couldNotDelete = false;

            if (string.IsNullOrEmpty(folderToRemove) ||
                !Directory.Exists(folderToRemove))
            {
                SetupLogger.LogInfo(
                    "Folder [{0}] is empty or missing.",
                    folderToRemove);
                return;
            }

            string[] filesToDelete = null;
            try
            {
                filesToDelete = Directory.GetFiles(folderToRemove, "*");
            }
            catch (IOException exception)
            {
                SetupLogger.LogInfo("Unable to read files in folder {0}. Exception message: {1}", folderToRemove, exception.Message);
            }
            catch (ArgumentException exception)
            {
                SetupLogger.LogInfo("Unable to read files in folder {0}. Exception message: {1}", folderToRemove, exception.Message);
            }
            catch (SecurityException exception)
            {
                SetupLogger.LogInfo("Unable to read files in folder {0}. Exception message: {1}", folderToRemove, exception.Message);
            }

            if (filesToDelete != null)
            {
                // Need to remove all the files we can.
                // Search for our log file pattern and remove any matches
                foreach (string fileToDelete in filesToDelete)
                {
                    try
                    {
                        File.SetAttributes(fileToDelete, FileAttributes.Archive);
                        File.Delete(fileToDelete);
                        SetupLogger.LogInfo(
                            "RemoveFolderAndFiles: File {0} has been deleted.",
                            fileToDelete);
                    }
                    catch (ArgumentException exception)
                    {
                        // Ignore
                        SetupLogger.LogInfo(
                            "RemoveFolderAndFiles: Unable to delete file {0}, Exception message: {1}",
                            fileToDelete,
                            exception.Message);
                        couldNotDelete = true;
                    }
                    catch (NotSupportedException exception)
                    {
                        // Ignore
                        SetupLogger.LogInfo(
                            "RemoveFolderAndFiles: Unable to delete file {0}, Exception message: {1}",
                            fileToDelete,
                            exception.Message);
                        couldNotDelete = true;
                    }
                    catch (UnauthorizedAccessException exception)
                    {
                        // Ignore
                        SetupLogger.LogInfo(
                            "RemoveFolderAndFiles: Unable to delete file {0}, Exception message: {1}",
                            fileToDelete,
                            exception.Message);
                        couldNotDelete = true;
                    }
                    catch (IOException exception)
                    {
                        // Ignore
                        SetupLogger.LogInfo(
                            "RemoveFolderAndFiles: Unable to delete file {0}, Exception message: {1}",
                            fileToDelete,
                            exception.Message);
                        couldNotDelete = true;
                    }

                    if (couldNotDelete)
                    {
                        SetupLogger.LogInfo(
                            "RemoveFolderAndFiles: Unable to delete file {0}.  We will attempt to remove it after reboot.",
                            fileToDelete);

                        // Add the file to the delete after reboot list.
                        if (!NativeMethods.MoveFileEx(
                            fileToDelete,
                            null,
                            NativeMethods.MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT))
                        {
                            SetupLogger.LogInfo(
                                "RemoveFolderAndFiles: Could not schedule the file to be removed after a reboot.");
                        }
                        else
                        {
                            SetupLogger.LogInfo(
                                "RemoveFolderAndFiles: scheduled the file {0} to be removed after a reboot.",
                                fileToDelete);
                            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.RebootRequired, "True"); // We will need a reboot for this to work.
                        }

                        // reset the switch
                        couldNotDelete = false;
                    }
                }
            }

            string[] subFoldersToDelete = null;
            try
            {
                subFoldersToDelete = Directory.GetDirectories(folderToRemove);
            }
            catch (IOException exception)
            {
                SetupLogger.LogInfo("Unable to read files in folder {0}. Exception message: {1}", folderToRemove, exception.Message);
            }
            catch (ArgumentException exception)
            {
                SetupLogger.LogInfo("Unable to read files in folder {0}. Exception message: {1}", folderToRemove, exception.Message);
            }
            catch (SecurityException exception)
            {
                SetupLogger.LogInfo("Unable to read files in folder {0}. Exception message: {1}", folderToRemove, exception.Message);
            }

            if (subFoldersToDelete != null)
            {
                // Remove any folders
                // Check the other subfolders for files that we may need to remove.
                foreach (string subFolderToDelete in subFoldersToDelete)
                {
                    // Delete any items in the folder that are on our list.
                    RemoveFolderAndFiles(subFolderToDelete, false);

                    try
                    {
                        // Delete the folder and all its contents
                        Directory.Delete(subFolderToDelete, true);
                        SetupLogger.LogInfo(
                            "RemoveFolderAndFiles: deleted folder {0} an all files/subfolders",
                            subFolderToDelete);
                    }
                    catch (IOException exception)
                    {
                        // Ignore
                        SetupLogger.LogInfo(
                            "RemoveFolderAndFiles: Unable to delete folder {0}, Exception message: {1}",
                            subFolderToDelete,
                            exception.Message);
                    }
                    catch (UnauthorizedAccessException exception)
                    {
                        // Ignore
                        SetupLogger.LogInfo(
                            "RemoveFolderAndFiles: Unable to delete folder {0}, Exception message: {1}",
                            subFolderToDelete,
                            exception.Message);
                    }
                    catch (ArgumentException exception)
                    {
                        // Ignore
                        SetupLogger.LogInfo(
                            "RemoveFolderAndFiles: Unable to delete folder {0}, Exception message: {1}",
                            subFolderToDelete,
                            exception.Message);
                    }
                }
            }

            // Ok... Now remove the base folder
            try
            {
                // Delete the folder and all its contents
                Directory.Delete(folderToRemove, true);
                SetupLogger.LogInfo("RemoveFolderAndFiles: deleted folder {0}", folderToRemove);
            }
            catch (IOException exception)
            {
                // Ignore
                SetupLogger.LogInfo(
                    "RemoveFolderAndFiles: Unable to delete folder {0}, Exception message: {1}",
                    folderToRemove,
                    exception.Message);
            }
            catch (UnauthorizedAccessException exception)
            {
                // Ignore
                SetupLogger.LogInfo(
                    "RemoveFolderAndFiles: Unable to delete folder {0}, Exception message: {1}",
                    folderToRemove,
                    exception.Message);
            }
            catch (ArgumentException exception)
            {
                // Ignore
                SetupLogger.LogInfo(
                    "RemoveFolderAndFiles: Unable to delete folder {0}, Exception message: {1}",
                    folderToRemove,
                    exception.Message);
            }

            if (writeHeader)
            {
                SetupLogger.LogInfo(
                    "*****RemoveFolderAndFiles: End delete status messages for {0}",
                    folderToRemove);
                SetupLogger.LogInfo("*****");
            }
        }

        /// <summary>
        /// Sets the feature switches.
        /// </summary>
        /// <returns>bool</returns>
        public static bool SetFeatureSwitches()
        {
            ArrayList componentsList = new ArrayList();

            // Ok so we need to fill the list based on the selections made by the user in the UI
            // We take the list of all possible components, then check to see which of these is already set.
            // if one is set then we add it to the list.
            string[] components = PropertyBagConstants.ComponentList.ToUpperInvariant().Split(new char[] { ',' });

            // See if we have something in the list
            if (components.Length > 0)
            {
                int count = 0;

                // Take a look at each of the items
                foreach (string component in components)
                {
                    if (PropertyBagDictionary.Instance.PropertyExists(component))
                    {
                        componentsList.Add(component);
                        count++;
                    }
                }

                // Same number of args as the split, everything was valid
                PropertyBagDictionary.Instance.SafeAdd(
                    PropertyBagConstants.ListOfComponentsSelectedForInstalling,
                    componentsList);
            }

            return true;
        }

        /// <summary>
        /// Shows the command line help.
        /// </summary>
        /// <param name="helpTopic">Help Topic</param>
        public static void ShowHelpFileTopic(string helpTopic)
        {
            string helpString = string.Format(
                CultureInfo.InvariantCulture,
                WpfResources.WPFResourceDictionary.HelpFormatString,
                helpTopic);
            Help.ShowHelp(null, WpfResources.WPFResourceDictionary.HelpFile, helpString);
        }

        /// <summary>
        /// BrowseFromLocation
        /// </summary>
        /// <param name="startingLocation">Location</param>
        /// <param name="titleText">Browse Dialog Text</param>
        /// <param name="selectedPath">Selected new path</param>
        /// <returns>path</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#")]
        public static DialogResult BrowseFromLocation(
            string startingLocation, 
            string titleText,
            out string selectedPath)
        {
            selectedPath = string.Empty;

            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = true;
            string folder = startingLocation;

            if (!string.IsNullOrEmpty(titleText))
            {
                folderBrowser.Description = titleText;
            }

            if (Directory.Exists(folder))
            {
                folderBrowser.SelectedPath = folder;
            }
            else
            {
                folderBrowser.SelectedPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFiles);
            }

            DialogResult dialogResult = folderBrowser.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                try
                {
                    selectedPath = folderBrowser.SelectedPath;
                }
                catch (NotSupportedException)
                {
                }
            }

            return dialogResult;
        }

        #endregion

        #region internal
        /// <summary>
        /// Rationalizes the components.
        /// </summary>
        /// <returns>returns true a valid set, false otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "We need to parse the commandline... spliting this into several functions does not make sense.")]
        public static bool RationalizeComponents()
        {
            // Ok so we need to reset the properties used by this function
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.CMPServer);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.WAPExtensionCommon);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.TenantWAPExtension);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.AdminWAPExtension);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.BlockReason);

            RationalizeUninstall();

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.CMPServer, "1");
            }

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon))
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.WAPExtensionCommon, "1");
            }

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension))
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.TenantWAPExtension, "1");
            }

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension))
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AdminWAPExtension, "1");
            }

            return true;
        }

        /// <summary>
        /// Shows the Error to the user using a messagebox.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public static void ShowError(string errorMessage)
        {
            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.Silent))
            {
                System.Windows.MessageBox.Show(errorMessage,
                    WpfResources.WPFResourceDictionary.SetupMessageBoxTitle,
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// ArchitectureIs64Check
        /// </summary>
        /// <returns>bool</returns>
        public static bool ArchitectureIs64Check()
        {
            object returnFromReg = Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment", "PROCESSOR_ARCHITECTURE", null);
            if (null == returnFromReg)
            {
                SetupLogger.LogInfo("ArchitectureIs64Check: Unable to read arch reg key.  We will return false");
            }
            else
            {
                // need to see if this is AMD64
                if (((string)returnFromReg).Equals("AMD64", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// LoadResource
        /// </summary>
        /// <typeparam name="T">Template</typeparam>
        /// <param name="path">path</param>
        /// <returns>T</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "GenericMethodsShouldProvideTypeParameter")]
        public static T LoadResource<T>(string path)
        {
            T c = default(T);
            StreamResourceInfo streamResource = System.Windows.Application.GetResourceStream(new Uri(path, UriKind.Relative));
            if (streamResource.ContentType == "application/xaml+xml")
            {
                c = (T)XamlReader.Load(streamResource.Stream);
            }
            else if (streamResource.ContentType.IndexOf("image", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = streamResource.Stream;
                bmp.EndInit();
                if (typeof(T) == typeof(ImageSource))
                {
                    c = (T)((object)bmp);
                }
                else if (typeof(T) == typeof(System.Windows.Controls.Image))
                {
                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    img.Source = bmp;
                    c = (T)((object)img);
                }
            }

            streamResource.Stream.Close();
            streamResource.Stream.Dispose();
            return c;
        }

        #endregion

        #region Public Static Methods
        public static ProcessStartInfo GetProcessStartInfoForConfigureSCP(InstallMode config)
        {
            DirectoryInfo installDir = new DirectoryInfo(SetupConstants.GetServerInstallPath() + SetupConstants.SetupFolder);
            string applicationName = installDir.FullName + @"\ConfigureSCPTool.exe";

            string haParameters = string.Empty;
            bool isFirstNode = false;

            bool isHASetup;

            ProcessStartInfo startInfo = null;
            switch (config)
            {
                case InstallMode.Install:
                    startInfo = new ProcessStartInfo(applicationName, "-install" + haParameters);
                    break;
                case InstallMode.Uninstall:
                    startInfo = new ProcessStartInfo(applicationName, "-uninstall" + haParameters);
                    break;
                case InstallMode.Update:
                    startInfo = new ProcessStartInfo(applicationName, "-update" + haParameters);
                    break;
                default:
                    startInfo = new ProcessStartInfo(applicationName, "-install" + haParameters);
                    break;
            }
            return startInfo;
        }

        public static void AppendWarning(Exception e)
        {
            String warningMessage = String.Empty;
            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.WarningReason))
            {
                warningMessage = e.Message;
            }
            else
            {
                warningMessage = PropertyBagDictionary.Instance.GetProperty<String>(PropertyBagConstants.WarningReason);
                warningMessage = String.Format("{0}{1}{2}",
                    warningMessage,
                    Environment.NewLine,
                    e.Message);
            }
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.WarningReason, warningMessage);
        }

        /// <summary>
        /// This method checks if 
        /// a. if the Navigation Uri contains the File format or just the file Name
        /// b. If it contaions the File Format, it find the latest file as per format  and
        ///    returns the file name
        /// </summary>
        /// <param name="navigateUri">Either File name or File Format</param>
        /// <returns>File Name</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        public static string GetLatestNavigationUri(string navigateUri)
        {
            if (string.IsNullOrEmpty(navigateUri))
            {
                throw new ArgumentNullException("navigateUri", "The navigateUri parameter must not be null.");
            }

            string latestNavigationUri = string.Empty;

            try
            {
                if (navigateUri.EndsWith("*"))
                {
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.DefaultLogPath))
                    {
                        string logfilePath = PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.DefaultLogPath);
                        DirectoryInfo directory = new DirectoryInfo(logfilePath);

                        int index = navigateUri.LastIndexOf(@"/");
                        latestNavigationUri = navigateUri.Substring(0, index + 1);
                        string pattern = navigateUri.Substring(index + 1);

                        FileInfo[] files = directory.GetFiles(pattern);

                        if (files != null)
                        {
                            FileInfo logfile = files[0];
                            foreach (FileInfo file in files)
                            {
                                if (logfile.CreationTime < file.CreationTime)
                                {
                                    logfile = file;
                                }
                            }

                            latestNavigationUri = latestNavigationUri + logfile.Name;
                        }
                    }
                }
                else
                {
                    latestNavigationUri = navigateUri;
                }
            }
            catch (ArgumentException ex)
            {
                SetupLogger.LogException(ex);
            }
            catch (SecurityException ex)
            {
                SetupLogger.LogException(ex);
            }
            catch (PathTooLongException ex)
            {
                SetupLogger.LogException(ex);
            }
            catch (NotSupportedException ex)
            {
                SetupLogger.LogException(ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                SetupLogger.LogException(ex);
            }

            return latestNavigationUri;
        }

        /// <summary>
        /// Is the given file extension is supported
        /// </summary>
        /// <param name="fileExtension">file extension</param>
        /// <returns>bool</returns>
        public static bool IsSupportedFileExtension(string fileExtension)
        { 
            List<string> fileExtensions = GetAllRegisteredFileExtensions();

            if (fileExtensions != null &&
                fileExtensions.Count > 0 &&
                fileExtensions.Contains(fileExtension))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets array containing known file extensions from HKEY_CLASSES_ROOT.
        /// </summary>
        /// <returns>String array containing extensions.</returns>
        private static List<string> GetAllRegisteredFileExtensions()
        {
            // generic list to hold all the subkey names
            List<string> subKeys = null;

            try
            {
                // get into the HKEY_CLASSES_ROOT
                Microsoft.Win32.RegistryKey root = Microsoft.Win32.Registry.ClassesRoot;
                subKeys = new List<string>();

                // IEnumerator for enumerating through the subkeys
                IEnumerator enums = root.GetSubKeyNames().GetEnumerator();

                // make sure we still have values
                while (enums.MoveNext())
                {
                    // all registered extensions start with a period (.) so
                    // we need to check for that
                    if (enums.Current.ToString().StartsWith("."))
                    {
                        // valid extension so add it
                        subKeys.Add(enums.Current.ToString());
                    }
                }
            }
            catch (SecurityException ex)
            {
                SetupLogger.LogException(ex);
            }
            catch (ObjectDisposedException ex)
            {
                SetupLogger.LogException(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                SetupLogger.LogException(ex);
            }
            catch (IOException ex)
            {
                SetupLogger.LogException(ex);
            }
            catch (InvalidOperationException ex)
            {
                SetupLogger.LogException(ex);
            }
            catch (ArgumentException ex)
            {
                SetupLogger.LogException(ex);
            }

            return subKeys;
        } 

        /// <summary>
        /// Reads the remote registry ex.
        /// </summary>
        /// <param name="searchMachine">The search machine.</param>
        /// <param name="searchPath">The search path.</param>
        /// <param name="searchValue">The search value.</param>
        /// <returns>value of the reg. key or "Error"</returns>
        public static string ReadRemoteRegistryEx(
            string searchMachine,
            string searchPath,
            string searchValue)
        {
            string returnValue = "Error";

            if (string.IsNullOrEmpty(searchMachine))
            {
                throw new ArgumentNullException("searchMachine", "The searchMachine parameter must not be null.");
            }

            if (string.IsNullOrEmpty(searchPath))
            {
                throw new ArgumentNullException("searchPath", "The searchPath parameter must not be null.");
            }

            if (string.IsNullOrEmpty(searchValue))
            {
                throw new ArgumentNullException("searchValue", "The searchValue parameter must not be null.");
            }

            try
            {
                ConnectionOptions connectionOptions = new ConnectionOptions();

                ManagementScope scope = new ManagementScope(@"//" + searchMachine + @"/root/default", connectionOptions);

                ManagementClass registry = new ManagementClass(
                    scope,
                    new ManagementPath("StdRegProv"),
                    null);

                // Returns a specific value for a specified key 
                ManagementBaseObject inputParams = registry.GetMethodParameters(
                    "GetStringValue");

                inputParams["sSubKeyName"] = searchPath;
                inputParams["sValueName"] = searchValue;

                ManagementBaseObject outParams = registry.InvokeMethod(
                    "GetStringValue",
                    inputParams,
                    null);

                if (outParams["sValue"] != null)
                {
                    returnValue = outParams["sValue"].ToString();
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                SetupLogger.LogException(uae);
            }
            catch (ManagementException me)
            {
                SetupLogger.LogException(me);
            }
            catch (System.Runtime.InteropServices.COMException comex)
            {
                SetupLogger.LogException(comex);
            }

            return returnValue;
        }

        /// <summary>
        /// Does all prerequisite checks.
        /// </summary>
        /// <returns>2 if failed, 1 if warn, 0 if all passed</returns>
        public static int DoAllPrerequisiteChecks()
        {
            int returnValue = 0;

            // TODO: Add checks for any prerequisites here.

            return returnValue;
        }


        public static void LogPropertyBag()
        {
            SetupLogger.LogInfo("Property Bag Values:");
            foreach (KeyValuePair<string, object> pair in PropertyBagDictionary.Instance)
            {
                if (PropertyBagDictionary.IsProtectedProperty(pair.Key))
                {
                    // We do not want to show the passwords.
                    SetupLogger.LogInfo("{0} = Due to data security, we will not print the value of the property bag key {0}", pair.Key);
                }
                else
                {
                    // if this can be enumerated then print all values.
                    if (pair.Value is string)
                    {
                        // string is also IEnumerable ! 
                        SetupLogger.LogInfo("{0} = {1}", pair.Key, pair.Value.ToString());
                    }
                    else if (pair.Value is XmlDocument)
                    {
                        // XmlDocument is also IEnumerable ! 
                        XmlDocument doc = pair.Value as XmlDocument;
                        SetupLogger.LogInfo(pair.Key);
                        SetupLogger.LogInfo(doc.InnerXml);
                    }
                    else if (pair.Value is IEnumerable)
                    {
                        IEnumerable enumerable = pair.Value as IEnumerable;
                        SetupLogger.LogInfo("Collection {0} ({1}):", pair.Key, enumerable.GetType().ToString());
                        int count = 0;
                        foreach (object value in enumerable)
                        {
                            if (!value.GetType().Equals(typeof(String)))
                            {
                                SetupLogger.LogInfo("\t[{0}] = {1} ({2}) ", count.ToString(), value.ToString(), value.GetType().ToString());
                            }
                            else
                            {
                                SetupLogger.LogInfo("\t[{0}] = {1} ", count.ToString(), value.ToString());
                            }

                            count++;
                        }
                    }
                    else if (pair.Value is InputParameter)
                    {
                        InputParameter inputParam = SetupInputs.Instance.FindItem(pair.Key);
                        String value = String.Empty;
                        if (inputParam is UserName ||
                            inputParam is BinaryInstallLocation ||
                            inputParam is SqlInstanceName ||
                            inputParam is SqlDatabaseName ||
                            inputParam is SqlMachineName ||
                            inputParam is SqlDBAdminName ||
                            inputParam is SqlDBAdminDomain ||
                            inputParam is WapSqlInstanceName ||
                            inputParam is WapSqlDatabaseName ||
                            inputParam is WapSqlMachineName ||
                            inputParam is WapSqlDBAdminName ||
                            inputParam is WapSqlDBAdminDomain ||
                            inputParam is CmpServerName ||
                            inputParam is CmpCertificateThumbprint ||
                            inputParam is VmmServiceDomain ||
                            inputParam is VmmServiceUserName)
                        {
                            value = inputParam;
                        }
                        if (inputParam is CreateNewSqlDatabase ||
                            inputParam is RemoteDatabaseImpersonation ||
                            inputParam is WapRemoteDatabaseImpersonation ||
                            inputParam is CmpServiceLocalAccount)
                        {
                            value = ((bool)inputParam).ToString();
                        }
                        if (inputParam is SqlServerPort)
                        {
                            value = ((int)inputParam).ToString();
                        }
                        if (inputParam is WapSqlServerPort)
                        {
                            value = ((int)inputParam).ToString();
                        }

                        SetupLogger.LogInfo("{0} = {1}", pair.Key, value);
                    }
                    else
                    {
                        SetupLogger.LogInfo("{0} = {1}", pair.Key, pair.Value.ToString());
                    }
                }
            }
            SetupLogger.LogInfo("End of list Property Bag Values.");
        }

        #endregion Public Static Methods

        #region Private Static Methods
        /// <summary>
        /// Rationalizes the uninstall.
        /// </summary>
        private static void RationalizeUninstall()
        {
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                // Check the whole list of things that are installed
                // make sure the requested state matches the installed state
                bool fullUninstall = true;

                // Server
                fullUninstall = fullUninstall &&
                    (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server) == PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ServerVersion));

                // Tenant Extension
                fullUninstall = fullUninstall &&
                    (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension) == PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtensionVersion));

                // Tenant Extension
                fullUninstall = fullUninstall &&
                    (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon) == PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommonVersion));

                // Admin Extension
                fullUninstall = fullUninstall &&
                    (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension) == PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtensionVersion));

                if (fullUninstall)
                {
                    // This is a full uninstall of all components on the machine.
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FullUninstall, "1");
                    SetupLogger.LogInfo("RationalizeComponents: We are doing a full uninstall of all components that are currently installed.");
                }
                else
                {
                    SetupLogger.LogInfo("RationalizeComponents: We are doing a partial uninstall of the components that are currently installed.");
                }
            }
        }

        /// <summary>
        /// Safes the copy folder.
        /// </summary>
        /// <param name="sourcefolderToCopy">The sourcefolder to copy.</param>
        /// <param name="destinationFolder">The destination folder.</param>
        /// <param name="patternToCopy">The pattern to copy.</param>
        /// <returns>returns true if there were no copy errors... false otherwise.</returns>
        private static bool SafeCopyFolder(string sourcefolderToCopy, string destinationFolder, string patternToCopy)
        {
            bool returnValue = true;
            SetupLogger.LogInfo("Copying files from {0} To {1} that match the pattern {2}", sourcefolderToCopy, destinationFolder, patternToCopy);

            // If the source and destination match, do nothing.
            if (string.Equals(sourcefolderToCopy, destinationFolder, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Copy the files
            string fullPathOfNewFile = string.Empty;
            string[] filesInFolder = null;
            try
            {
                filesInFolder = Directory.GetFiles(sourcefolderToCopy, patternToCopy);
            }
            catch (UnauthorizedAccessException ex)
            {
                SetupLogger.LogInfo("SafeCopyFolder: Error {0} Getting files from {1} with pattern {2}", ex.Message, sourcefolderToCopy, patternToCopy);
                return false;
            }
            catch (ArgumentException ex)
            {
                SetupLogger.LogInfo("SafeCopyFolder: Error {0} Getting files from {1} with pattern {2}", ex.Message, sourcefolderToCopy, patternToCopy);
                return false;
            }
            catch (PathTooLongException ex)
            {
                SetupLogger.LogInfo("SafeCopyFolder: Error {0} Getting files from {1} with pattern {2}", ex.Message, sourcefolderToCopy, patternToCopy);
                return false;
            }
            catch (DirectoryNotFoundException ex)
            {
                SetupLogger.LogInfo("SafeCopyFolder: Error {0} Getting files from {1} with pattern {2}", ex.Message, sourcefolderToCopy, patternToCopy);
                return false;
            }

            if (filesInFolder != null)
            {
                foreach (string fullPathOfFileToCopy in filesInFolder)
                {
                    try
                    {
                        fullPathOfNewFile = Path.Combine(destinationFolder, new FileInfo(fullPathOfFileToCopy).Name);
                        File.Copy(fullPathOfFileToCopy, fullPathOfNewFile, true);
                        SetupLogger.LogInfo("SafeCopyFolder: Copied {0} to {1}", fullPathOfFileToCopy, fullPathOfNewFile);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        SetupLogger.LogInfo("SafeCopyFolder: Error {0} Copying file: {1} to {2}", ex.Message, fullPathOfFileToCopy, fullPathOfNewFile);
                        returnValue = false;
                    }
                    catch (ArgumentException ex)
                    {
                        SetupLogger.LogInfo("SafeCopyFolder: Error {0} Copying file: {1} to {2}", ex.Message, fullPathOfFileToCopy, fullPathOfNewFile);
                        returnValue = false;
                    }
                    catch (PathTooLongException ex)
                    {
                        SetupLogger.LogInfo("SafeCopyFolder: Error {0} Copying file: {1} to {2}", ex.Message, fullPathOfFileToCopy, fullPathOfNewFile);
                        returnValue = false;
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        SetupLogger.LogInfo("SafeCopyFolder: Error {0} Copying file: {1} to {2}", ex.Message, fullPathOfFileToCopy, fullPathOfNewFile);
                        returnValue = false;
                    }
                    catch (FileNotFoundException ex)
                    {
                        SetupLogger.LogInfo("SafeCopyFolder: Error {0} Copying file: {1} to {2}", ex.Message, fullPathOfFileToCopy, fullPathOfNewFile);
                        returnValue = false;
                    }
                    catch (IOException ex)
                    {
                        SetupLogger.LogInfo("SafeCopyFolder: Error {0} Copying file: {1} to {2}", ex.Message, fullPathOfFileToCopy, fullPathOfNewFile);
                        returnValue = false;
                    }
                    catch (NotSupportedException ex)
                    {
                        SetupLogger.LogInfo("SafeCopyFolder: Error {0} Copying file: {1} to {2}", ex.Message, fullPathOfFileToCopy, fullPathOfNewFile);
                        returnValue = false;
                    }
                }
            }

            // Copy the folders
            string fullPathOfNewFolder = string.Empty;
            string[] foldersInFolder = null;
            try
            {
                foldersInFolder = Directory.GetDirectories(sourcefolderToCopy);
            }
            catch (UnauthorizedAccessException ex)
            {
                SetupLogger.LogInfo("SafeCopyFolder: Error {0} Getting folders from {1} with pattern {2}", ex.Message, sourcefolderToCopy, patternToCopy);
                return false;
            }
            catch (ArgumentException ex)
            {
                SetupLogger.LogInfo("SafeCopyFolder: Error {0} Getting folders from {1} with pattern {2}", ex.Message, sourcefolderToCopy, patternToCopy);
                return false;
            }
            catch (PathTooLongException ex)
            {
                SetupLogger.LogInfo("SafeCopyFolder: Error {0} Getting folders from {1} with pattern {2}", ex.Message, sourcefolderToCopy, patternToCopy);
                return false;
            }
            catch (DirectoryNotFoundException ex)
            {
                SetupLogger.LogInfo("SafeCopyFolder: Error {0} Getting folders from {1} with pattern {2}", ex.Message, sourcefolderToCopy, patternToCopy);
                return false;
            }

            if (foldersInFolder != null)
            {
                string newSourcefolderToCopy = null;
                string folderName = null;
                foreach (string fullPathOfFolderToCopy in foldersInFolder)
                {
                    try
                    {
                        folderName = new DirectoryInfo(fullPathOfFolderToCopy).Name;
                        newSourcefolderToCopy = Path.Combine(sourcefolderToCopy, folderName);
                        fullPathOfNewFolder = Path.Combine(destinationFolder, folderName);
                        // Make the folder if it does not exist
                        if (!Directory.Exists(fullPathOfNewFolder))
                        {
                            Directory.CreateDirectory(fullPathOfNewFolder);
                            SetupLogger.LogInfo("SafeCopyFolder: Created folder: {0}.", fullPathOfNewFolder);
                        }
                        SafeCopyFolder(newSourcefolderToCopy, fullPathOfNewFolder, patternToCopy);
                    }
                    catch (IOException e)
                    {
                        SetupLogger.LogException(e);
                        returnValue = false;
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        SetupLogger.LogException(e);
                        returnValue = false;
                    }

                }
            }

            return returnValue;
        }

        /// <summary>
        /// Determines whether [is itemName] [the specified].
        /// </summary>
        /// <param name="itemName">The itemName.</param>
        /// <returns>
        ///     <c>true</c> if [is itemName ] [the specified in ItemsLogFileList]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsItemNameExists(string itemName)
        {
            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ItemsLogFileList))
            {
                return false;
            }

            ArrayList itemsLogFileList = PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsLogFileList);

            if (itemsLogFileList.Contains(itemName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds the ARP entry.
        /// </summary>
        public static void AddUninstallChangeEntry()
        {
            RegistryKey addRemoveProgramsRegistryTree = null;
            string installLocation = string.Empty;

            // Verify that we have an valid existing install location
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                installLocation = SetupInputs.Instance.FindItem(SetupInputTags.BinaryInstallLocationTag);
            }
            else
            {
                installLocation = Environment.ExpandEnvironmentVariables(@"%SYSTEMDRIVE%\inetpub\MgmtSvc-CmpWapExtension");
            }

            if (!Directory.Exists(installLocation))
            {
                SetupLogger.LogInfo(string.Format(CultureInfo.InvariantCulture, "AddARPEntry: Install folder {0} does not exist.", installLocation));
                throw new ArgumentException(installLocation);
            }

            string arpCachePath = SetupHelpers.GetAddRemoveProgramsCachePath();

            string uninstallCommand = ARPFileCache();

            if (string.IsNullOrEmpty(uninstallCommand))
            {
                SetupLogger.LogInfo(@"AddUninstallChangeEntry: Unable to create/copy the ARPCache files to the install location");
                SetupLogger.LogInfo("AddUninstallChangeEntry: uninstallCommand does not exist.");
                throw new ArgumentException(WpfResources.WPFResourceDictionary.InvalidArgument);
            }

            string productCode = String.Empty;
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                productCode = SetupConstants.ServerProductCode;
            }

            if (!String.IsNullOrEmpty(productCode))
            {
                try
                {
                    // Allow access to Uninstall key only to administrators and System account
                    RegistrySecurity regSecurity = new RegistrySecurity();

                    // Creating Built-in Admin rule.
                    RegistryAccessRule adminRule = new RegistryAccessRule(
                        new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                        RegistryRights.FullControl | RegistryRights.ReadPermissions,
                        InheritanceFlags.ContainerInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow);

                    // Creating Local system rule...
                    RegistryAccessRule systemRule = new RegistryAccessRule(
                        new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
                        RegistryRights.FullControl | RegistryRights.ReadPermissions,
                        InheritanceFlags.ContainerInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow);

                    // Creating EVERYONE rule...
                    RegistryAccessRule everyoneRule = new RegistryAccessRule(
                        new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                        RegistryRights.ReadPermissions | (RegistryRights.QueryValues | RegistryRights.EnumerateSubKeys | RegistryRights.Notify | RegistryRights.CreateLink | RegistryRights.ReadKey),
                        InheritanceFlags.ContainerInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow);

                    // Adding Access Rules to the Security object...
                    regSecurity.SetAccessRule(adminRule);
                    regSecurity.SetAccessRule(systemRule);
                    regSecurity.SetAccessRule(everyoneRule);

                    // Make the security protected, so it won't inherit from parent
                    regSecurity.SetAccessRuleProtection(true, false);

                    addRemoveProgramsRegistryTree = Registry.LocalMachine.CreateSubKey(
                        string.Format(CultureInfo.InvariantCulture,
                                        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{0}",
                                        productCode),
                        RegistryKeyPermissionCheck.ReadWriteSubTree,
                        regSecurity);
                }
                catch (SecurityException e)
                {
                    SetupLogger.LogInfo("SetupHelpers.AddUninstallChangeEntry()", e);
                }
                catch (UnauthorizedAccessException e)
                {
                    SetupLogger.LogInfo("SetupHelpers.AddUninstallChangeEntry()", e);
                }

                if (addRemoveProgramsRegistryTree != null)
                {
                    addRemoveProgramsRegistryTree.SetValue("DisplayName", WpfResources.WPFResourceDictionary.ProductTitle, RegistryValueKind.String);
                    addRemoveProgramsRegistryTree.SetValue("DisplayIcon", Path.Combine(arpCachePath, SetupExe));
                    addRemoveProgramsRegistryTree.SetValue("HelpLink", WpfResources.WPFResourceDictionary.AddRemoveProgramsHelpLink, RegistryValueKind.String);
                    addRemoveProgramsRegistryTree.SetValue("Publisher", WpfResources.WPFResourceDictionary.AddRemoveProgramsPublisher, RegistryValueKind.String);
                    // TODO: remove the feature option /server when /runui 
                    addRemoveProgramsRegistryTree.SetValue("UninstallString", string.Format(CultureInfo.InvariantCulture, "\"{0}\" /server /runui", uninstallCommand), RegistryValueKind.String);

                    string setupVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
                    addRemoveProgramsRegistryTree.SetValue("ProductVersion", setupVersion, RegistryValueKind.String);
                    addRemoveProgramsRegistryTree.SetValue("DisplayVersion", Assembly.GetEntryAssembly().GetName().Version.ToString(), RegistryValueKind.String);
                    addRemoveProgramsRegistryTree.SetValue("WindowsInstaller", 0, RegistryValueKind.DWord);
                }
                else
                {
                    SetupLogger.LogInfo("AddUninstallChangeEntry: Unable to create the ARP Hive entry");
                }
            }

            return;
        }

        /// <summary>
        /// Removes the uninstall change entry.
        /// </summary>
        public static void RemoveUninstallChangeEntry()
        {
            SetupLogger.LogInfo("RemoveUninstallChangeEntry: Starting ARPEntry removal.");

            string cachePath = SetupHelpers.GetAddRemoveProgramsCachePath();

            // Remove the ARP entries.
            try
            {
                Registry.LocalMachine.DeleteSubKeyTree(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{0}",
                        SetupConstants.ServerProductCode));
            }
            catch (ArgumentException e)
            {
                SetupLogger.LogException(e);
                SetupLogger.LogInfo("This is not fatal we will continue.");
            }
            catch (SecurityException e)
            {
                SetupLogger.LogException(e);
                SetupLogger.LogInfo("This is not fatal we will continue.");
            }
            catch (UnauthorizedAccessException e)
            {
                SetupLogger.LogException(e);
                SetupLogger.LogInfo("This is not fatal we will continue.");
            }

            // Delete the msi files under setup\msi
            RemoveFolderAndFiles(Path.Combine(cachePath, SetupConstants.MsiFolder), true);

            // Delete the license files under setup\Licenses
            RemoveFolderAndFiles(Path.Combine(cachePath, SetupConstants.LicensesFolder), true);

            // Delete the leftover folders
            try
            {
                // VMM directory, cachePath is the setup directory which always has VMM directory as its parent.
                DirectoryInfo vmmDirInfo = Directory.GetParent(cachePath);

                // move up the application's current working directory so that various parent directories can be deleted if they are empty.
                Environment.CurrentDirectory = vmmDirInfo.Parent != null ? Directory.GetParent(vmmDirInfo.FullName).FullName : vmmDirInfo.FullName;

                Directory.Delete(cachePath, true);  // Delete the setup directory. joaldaba
                if (vmmDirInfo.Parent != null)
                {
                    Directory.Delete(vmmDirInfo.FullName, true);  // Delete the VMM directory if is empty
                }
            }
            catch (Exception e)
            {
                SetupLogger.LogException(e);
                SetupLogger.LogInfo("This is not fatal we will continue.");
            }
        }

        internal static void CreateEventSources()
        {
            try
            {
                var EL = new EventLog("Application");
                EL.Source = CmpCommon.Constants.CmpAzureServiceWebRole_EventlogSourceName;
                EL.WriteEntry("Successfully created '" + CmpCommon.Constants.CmpAzureServiceWebRole_EventlogSourceName + "' event source.", EventLogEntryType.Information, 0, 0);
                SetupLogger.LogInfo("Successfully created '" + CmpCommon.Constants.CmpAzureServiceWebRole_EventlogSourceName + "' event source.");
            }
            catch (Exception ex)
            {
                SetupLogger.LogException(ex);
            }

            try
            {
                var EL2 = new EventLog("Application");
                EL2.Source = CmpCommon.Constants.CmpAzureServiceWorkerRole_EventlogSourceName;
                EL2.WriteEntry("Successfully created '" + CmpCommon.Constants.CmpAzureServiceWorkerRole_EventlogSourceName + "' event source.", EventLogEntryType.Information, 0, 0);
                SetupLogger.LogInfo("Successfully created '" + CmpCommon.Constants.CmpAzureServiceWorkerRole_EventlogSourceName + "' event source.");
            }
            catch (Exception ex)
            {
                SetupLogger.LogException(ex);
            }

            try
            {
                var el3 = new EventLog("Application");
                el3.Source = CmpCommon.Constants.CmpWapConnector_EventlogSourceName;
                el3.WriteEntry("Successfully created '" + CmpCommon.Constants.CmpWapConnector_EventlogSourceName + "' event source.", EventLogEntryType.Information, 0, 0);
                SetupLogger.LogInfo("Successfully created '" + CmpCommon.Constants.CmpWapConnector_EventlogSourceName + "' event source.");
            }
            catch (Exception ex)
            {
                SetupLogger.LogException(ex);
            }
        }

        internal static void DeleteEventSources()
        {
            try
            {
                EventLog.DeleteEventSource(CmpCommon.Constants.CmpAzureServiceWebRole_EventlogSourceName);
                SetupLogger.LogInfo("Successfully deleted '" + CmpCommon.Constants.CmpAzureServiceWebRole_EventlogSourceName + "' event source.");
            }
            catch (Exception ex)
            {
                SetupLogger.LogException(ex);
            }

            try
            {
                EventLog.DeleteEventSource(CmpCommon.Constants.CmpAzureServiceWorkerRole_EventlogSourceName);
                SetupLogger.LogInfo("Successfully deleted '" + CmpCommon.Constants.CmpAzureServiceWorkerRole_EventlogSourceName + "' event source.");
            }
            catch (Exception ex)
            {
                SetupLogger.LogException(ex);
            }

            try
            {
                EventLog.DeleteEventSource(CmpCommon.Constants.CmpWapConnector_EventlogSourceName);
                SetupLogger.LogInfo("Successfully deleted '" + CmpCommon.Constants.CmpWapConnector_EventlogSourceName + "' event source.");
            }
            catch (Exception ex)
            {
                SetupLogger.LogException(ex);
            }
        }

        /// <summary>
        /// ARPs the file cache.
        /// </summary>
        /// <returns>returns the name of the arp executable</returns>
        private static string ARPFileCache()
        {
            string installLocation = String.Empty;
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                installLocation = SetupInputs.Instance.FindItem(SetupInputTags.BinaryInstallLocationTag);
            }
            else
            {
                installLocation = Environment.ExpandEnvironmentVariables(@"%SYSTEMDRIVE%\inetpub\MgmtSvc-CmpWapExtension");
            }

            if (!Directory.Exists(installLocation))
            {
                SetupLogger.LogInfo(string.Format(CultureInfo.InvariantCulture, "AddARPEntry: Install folder {0} does not exist.", installLocation));
                throw new ArgumentException(WpfResources.WPFResourceDictionary.InvalidArgument);
            }

            string arpCachePath = SetupHelpers.GetAddRemoveProgramsCachePath();
            // Make the folder if it does not exist
            if (!Directory.Exists(arpCachePath))
            {
                try
                {
                    Directory.CreateDirectory(arpCachePath);
                }
                catch (IOException e)
                {
                    SetupLogger.LogException(e);

                    return String.Empty;
                }
                catch (UnauthorizedAccessException e)
                {
                    SetupLogger.LogException(e);

                    return String.Empty;
                }

                SetupLogger.LogInfo("ARPFileCache: Created folder: {0}.", arpCachePath);
            }

            String msiFolder = Path.Combine(arpCachePath, SetupConstants.MsiFolder);
            // if it is a new installation, the msi folder is not created
            if (!Directory.Exists(msiFolder))
            {
                try
                {
                    Directory.CreateDirectory(msiFolder);
                }
                catch (IOException e)
                {
                    SetupLogger.LogException(e);

                    return String.Empty;
                }
                catch (UnauthorizedAccessException e)
                {
                    SetupLogger.LogException(e);

                    return String.Empty;
                }

                SetupLogger.LogInfo("ARPFileCache: Created folder: {0}.", msiFolder);

                String sourceFolder = PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagDictionary.SetupExePath);
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.LocationOfSetupFiles))
                {
                    sourceFolder = PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.LocationOfSetupFiles);
                }
                // Copy all the executable files from the installation source folder to the target location msi folder
                if (!SafeCopyFolder(sourceFolder, arpCachePath, "*.exe"))
                {
                    SetupLogger.LogInfo(
                        "ARPFileCache: Could not copy all the files from {0} to {1}.  We will have to rollback.",
                        sourceFolder,
                        arpCachePath);
                    return string.Empty;
                }
                // Copy all the msi files from the installation source folder to the target location msi folder
                if (!SafeCopyFolder(sourceFolder, msiFolder, "*.msi"))
                {
                    SetupLogger.LogInfo(
                        "ARPFileCache: Could not copy all the files from {0} to {1}.  We will have to rollback.",
                        sourceFolder,
                        msiFolder);
                    return string.Empty;
                }
                string licenseFilePath = Path.Combine(arpCachePath, SetupConstants.LicensesFolder);
                // Copy all the license files from the installation source folder to the target location licenses folder
                if (!SafeCopyFolder(sourceFolder, licenseFilePath, "*.rtf"))
                {
                    SetupLogger.LogInfo(
                        "ARPFileCache: Could not copy all the files from {0} to {1}.  We will have to rollback.",
                        sourceFolder,
                        licenseFilePath);
                    return string.Empty;
                }
            }

            return Path.Combine(arpCachePath, SetupExe);
        }

        #endregion Private Static Methods
    }
}