//-----------------------------------------------------------------------
// <copyright file="SetupFileValidation.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Provides validation functions for use in the setup.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Text;
    using CMP.Setup.SetupFramework;
    using CMP.Setup.Helpers;

    /// <summary>
    /// Collection of validation functions for setup
    /// </summary>
    public class SetupFileValidation
    {
        /// <summary>
        /// Prevents a default instance of the SetupFileValidation class from being created.
        /// </summary>
        private SetupFileValidation()
        {
        }

        /// <summary>
        /// Checks to see if we have all needed install item files.
        /// </summary>
        /// <returns>true if they are all there, if not false (and asks for new path if not silent)</returns>
        public static bool HaveAllNeededInstallItemFiles()
        {
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                // we are doing an uninstall so we don't need anything from the CD
                return true;
            }
            else
            {
                bool haveAllNeededFiles = true;

                foreach (InstallItemsInstallDataItem itemToInstall in PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall))
                {
                    if (!string.IsNullOrEmpty(itemToInstall.FullPathToLaunch))
                    {
                        haveAllNeededFiles = haveAllNeededFiles & SetupFileValidation.ValidateFileExists(itemToInstall.FullPathToLaunch);
                    }
                }

                if (haveAllNeededFiles)
                {
                    return true;
                }

                // When we get here in the logic, we must return false...
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.Silent))
                {
                    // We don't have all the files and we are silent.... we must fail.
                    SetupLogger.LogError(
                        "HaveAllNeededInstallItemFiles:  We are missing some of the install files needed by the installitems.  Please check the path {0} to make sure it pointing to the root of our files.",
                        PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.LocationOfSetupFiles));
                }
                else
                {
                    // Open a browser on at my MyComputer and save the browsed location to the propertybag
                    string diskLocation = string.Empty;
                    SetupHelpers.BrowseFromLocation(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                        WpfResources.WPFResourceDictionary.PromptForSetupFiles,
                        out diskLocation);

                    if (string.IsNullOrEmpty(diskLocation) == false)
                    {
                        diskLocation = Path.Combine(diskLocation, SetupConstants.SetupFolder);
                        if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ArchitectureIs64Check))
                        {
                            diskLocation = Path.Combine(diskLocation, "amd64");
                        }
                        else
                        {
                            diskLocation = Path.Combine(diskLocation, "i386");
                        }

                        if (Directory.Exists(diskLocation))
                        {
                            ResetInstallItemFileLocations(
                                PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.LocationOfSetupFiles),
                                diskLocation);
                            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.LocationOfSetupFiles, diskLocation);
                        }
                        else
                        {
                            SetupLogger.LogInfo("Path {0} does not exist.", diskLocation);
                        }
                    }
                }
            }

            // We did not have all the files we needed.
            return false;
        }

        public static bool FindAgreementFile(ref String path, CMP.Setup.AgreementType agreementType)
        {
            bool returnValue = true;
            agreementType = AgreementType.Notice;
            String setupExePath = PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagDictionary.SetupExePath);

            path = Path.Combine(setupExePath, "Notice.rtf");
            returnValue = SetupFileValidation.ValidateFileExists(path);

            return returnValue;
        }

        /// <summary>
        /// Resets the install item file locations.
        /// </summary>
        /// <param name="oldLocation">The old location.</param>
        /// <param name="newLocation">The new location.</param>
        public static void ResetInstallItemFileLocations(string oldLocation, string newLocation)
        {
            SetupLogger.LogInfo("*****ResetInstallItemFileLocations********************************************");
            foreach (InstallItemsInstallDataItem itemToInstall in PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall))
            {
                if (!string.IsNullOrEmpty(itemToInstall.FullPathToLaunch))
                {
                    // if the old path is set, replace... otherwise, add
                    if (itemToInstall.FullPathToLaunch.Contains(oldLocation))
                    {
                        if (oldLocation.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                            !newLocation.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        {
                            // We need to add a \ to the new location
                            newLocation = newLocation + Path.DirectorySeparatorChar;
                        }

                        itemToInstall.FullPathToLaunch = itemToInstall.FullPathToLaunch.Replace(oldLocation, newLocation);
                    }
                    else
                    {
                        // get rid of the parent ".." symble in path 
                        String path = newLocation;
                        if (newLocation.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        {
                            path = newLocation.Substring(0, newLocation.LastIndexOf(Path.DirectorySeparatorChar));
                        }
                        while (itemToInstall.FullPathToLaunch.IndexOf("..") == 0)
                        {
                            itemToInstall.FullPathToLaunch = itemToInstall.FullPathToLaunch.Substring(3);
                            path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
                        }
                        
                        itemToInstall.FullPathToLaunch = Path.Combine(path, itemToInstall.FullPathToLaunch);
                    }

                    // get locale
                    if (itemToInstall.FullPathToLaunch.Contains(SetupConstants.UnknownLCID))
                    {
                        String fileFullPath = itemToInstall.FullPathToLaunch.Replace(SetupConstants.UnknownLCID, CultureInfo.CurrentUICulture.LCID.ToString());
                        if (File.Exists(fileFullPath))
                        {
                            itemToInstall.FullPathToLaunch = fileFullPath;
                        }
                        else
                        {
                            // the locale file does not exist, use the English one as default
                            itemToInstall.FullPathToLaunch = itemToInstall.FullPathToLaunch.Replace(SetupConstants.UnknownLCID, SetupConstants.BaseLanguageID);
                        }
                    }

                    SetupLogger.LogInfo("Item:        {0}", itemToInstall.DisplayTitle);
                    SetupLogger.LogInfo("path set to: {0}", itemToInstall.FullPathToLaunch);
                    SetupLogger.LogInfo("------------------------------------------------------------------------------");
                }
            }

            SetupLogger.LogInfo("******************************************************************************");
        }

        /// <summary>
        /// Validates EULA file exists on the media.
        /// </summary>
        /// <returns>true if successful</returns>
        public static bool ExistEULAFile()
        {
            bool returnValue = true;

            // Print a pretty header for the log...
            SetupLogger.LogInfo(string.Empty);
            SetupLogger.LogInfo("**************************************************");

            // Make the entry in the property bad if it is not there
            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.MissingFilesList))
            {
                StringBuilder missingFiles = new StringBuilder();
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.MissingFilesList, missingFiles);
            }

            // Print a pretty footer for the log.
            SetupLogger.LogInfo("**************************************************");

            if (!returnValue)
            {
                StringBuilder missingFiles = PropertyBagDictionary.Instance.GetProperty<StringBuilder>(PropertyBagConstants.MissingFilesList);
                SetupLogger.LogInfo(string.Format(CultureInfo.CurrentCulture, WpfResources.WPFResourceDictionary.MissingFileFormatString, missingFiles.ToString()));
                SetupHelpers.ShowError(string.Format(CultureInfo.CurrentCulture, WpfResources.WPFResourceDictionary.MissingFileFormatString, missingFiles.ToString()));
            }
            else
            {
                SetupLogger.LogInfo("All vital setup files found.");
            }

            // Cleanup the property bag
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.MissingFilesList);
            return returnValue;
        }


        /// <summary>
        /// Validates the file exists.
        /// </summary>
        /// <param name="fileToLookFor">The file to look for.</param>
        /// <returns>true if the file is found, false otherwise.</returns>
        public static bool ValidateFileExists(string fileToLookFor)
        {
            bool returnValue = false;
            SetupLogger.LogInfo("Looking for file: {0}.", fileToLookFor);
            if (!File.Exists(fileToLookFor))
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.MissingFilesList))
                {
                    // We are making a list to show the user.
                    StringBuilder missingFiles = PropertyBagDictionary.Instance.GetProperty<StringBuilder>(PropertyBagConstants.MissingFilesList);
                    missingFiles.AppendLine(fileToLookFor); // Name of the missing file
                    missingFiles.AppendLine(string.Empty); // Extra space to make the dialog more readable
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.MissingFilesList, missingFiles);
                }

                returnValue = false;
                SetupLogger.LogInfo("Missing {0}.", fileToLookFor);
            }
            else
            {
                SetupLogger.LogInfo("Found {0}.", fileToLookFor);
                returnValue = true;
            }

            return returnValue;
        }

        /// <summary>
        /// Checks for existing data base files.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="databaseFolder">The database folder.</param>
        /// <returns>true if successful, false otherwise</returns>
        public static bool CheckForExistingDataBaseFiles(string serverName, string databaseFolder)
        {
            Collection<string> existingFilesList = null;
            string conflictingFiles = string.Empty;
            if (!string.IsNullOrEmpty(serverName) &&
                !string.IsNullOrEmpty(databaseFolder))
            {
                existingFilesList = SetupFileValidation.ListOfFilesOnRemoteComputer(serverName, databaseFolder);
            }
            else
            {
                // could not do the check
                return false;
            }

            if (existingFilesList == null)
            {
                // There was an error. 
                return false;
            }

            if (existingFilesList.Count < 1)
            {
                // Folder was empty... this is ok.
                PropertyBagDictionary.Instance.SafeRemove(PropertyBagConstants.ExistingSqlFilesMessage);
                PropertyBagDictionary.Instance.SafeRemove(PropertyBagConstants.ExistingSqlFiles);
                return true;
            }
            else
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
                {
                    String dbName = SetupInputs.Instance.FindItem(SetupInputTags.SqlDatabaseNameTag);
                    if (existingFilesList.Contains(dbName.ToLowerInvariant()))
                    {
                        conflictingFiles = conflictingFiles + dbName + "\r\n";
                    }
                }

                if (string.IsNullOrEmpty(conflictingFiles))
                {
                    PropertyBagDictionary.Instance.SafeRemove(PropertyBagConstants.ExistingSqlFilesMessage);
                    PropertyBagDictionary.Instance.SafeRemove(PropertyBagConstants.ExistingSqlFiles);
                }
                else
                {
                    PropertyBagDictionary.Instance.SafeAdd(
                        PropertyBagConstants.ExistingSqlFiles,
                        conflictingFiles);
                    string message = 
                        string.Format(
                            WpfResources.WPFResourceDictionary.ExistingSqlFilesMessage,
                            conflictingFiles,
                            "\r\n");
                    PropertyBagDictionary.Instance.SafeAdd(
                        PropertyBagConstants.ExistingSqlFilesMessage,
                        message);
                }
            }

            // Life was good
            return true;
        }

        /// <summary>
        /// Validates the file exists on server.
        /// </summary>
        /// <param name="remoteComputer">The remote computer.</param>
        /// <param name="pathToCheck">The path to check.</param>
        /// <returns>
        /// true if there are files in the folder, false otherwise
        /// </returns>
        public static Collection<string> ListOfFilesOnRemoteComputer(string remoteComputer, string pathToCheck)
        {
            Collection<string> filesInFolder = null;

            if (string.IsNullOrEmpty(remoteComputer) || string.IsNullOrEmpty(pathToCheck))
            {
                SetupLogger.LogInfo("ValidateFileExistsOnServer: Either the computer name or the path to check was null.  We will return -1;");
                return null;
            }

            try
            {
                pathToCheck = pathToCheck.Replace("\\", "\\\\");
                string remoteFolder = "ASSOCIATORS OF " +
                    "{Win32_Directory=\"" + pathToCheck.TrimEnd(Path.DirectorySeparatorChar) + "\"} " +
                    "WHERE AssocClass=CIM_DirectoryContainsFile " +
                    "ResultClass=CIM_DataFile " +
                    "ResultRole=PartComponent " +
                    "Role=GroupComponent";

                ManagementObjectSearcher query = new ManagementObjectSearcher(remoteFolder);

                // Connect to the computer
                ConnectionOptions co = new ConnectionOptions();
                co.Impersonation = ImpersonationLevel.Impersonate;
                co.Timeout = new TimeSpan(0, 0, 30);

                ManagementScope ms = new ManagementScope(
                    "\\\\" + remoteComputer + "\\root\\cimv2", 
                    co);

                query.Scope = ms;
                ManagementObjectCollection queryCollection = query.Get();

                filesInFolder = new Collection<string>();

                foreach (ManagementObject mo in queryCollection)
                {
                    string fileExtension = (mo["Extension"].ToString());
                    if (fileExtension.Equals("mdf", StringComparison.OrdinalIgnoreCase) ||
                        fileExtension.Equals("ldf", StringComparison.OrdinalIgnoreCase))
                    {
                        filesInFolder.Add(mo["FileName"].ToString().ToLowerInvariant());
                    }
                }
            }
            catch (ArgumentException e)
            {
                SetupLogger.LogException(e);
            }
            catch (COMException e)
            {
                SetupLogger.LogException(e);
            }
            catch (UnauthorizedAccessException e)
            {
                SetupLogger.LogException(e);
            }
            catch (ManagementException e)
            {
                SetupLogger.LogException(e);
            }

            return filesInFolder;
        }
    }
}
