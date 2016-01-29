//-----------------------------------------------------------------------
// <copyright file="InstallActionProcessor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Provides processing for install actions.
// </summary>
//-----------------------------------------------------------------------
#region Using directives

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Win32;
using System.Security.Permissions;
using CMP.Setup.SetupFramework;

#endregion

namespace CMP.Setup
{
    using CMP.Setup.Helpers;
    using System.Reflection;
    /// <summary>
    /// Processing class for install actions
    /// </summary>
    public class InstallActionProcessor
    {
        private ProgressData progressData;
        NativeMethods.InstallUIHandler MsiProgressCallback;

        // Constants used for install states
        const int INSTALLSTATE_ABSENT = 2;
        const int INSTALLSTATE_LOCAL = 3;

        internal const string FinishPageWarning = "finishPageWarning";

        const double SecondsWaitForMutexMSIExecute = 60*60;  // 1 hour

        /// <summary>
        /// Constructor
        /// </summary>
        public InstallActionProcessor()
        {
            this.progressData = ProgressData.Instance;
            // Create delegate for MSI progress message callback
            this.MsiProgressCallback = new NativeMethods.InstallUIHandler(MicrosoftInstallerUIHandler);
        }

        private delegate void iconForProgressDelegateFunction();

        /// <summary>
        /// Processes the current install actions
        /// </summary>
        /// 
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.ControlThread)]
        public SetupReturnValues ProcessInstalls()
        {
            SetupReturnValues returnValue = SetupReturnValues.Successful;

            bool continueWithInstall;
            foreach (InstallItemsInstallDataItem itemToInstall
                in PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall))
            {
                continueWithInstall = true;
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.Rollback))
                {
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                    {
                        SetupLogger.LogInfo("ProcessInstalls: Rollback is set but we are doing an uninstall so we will clear the rollback switch and continue");
                        // Clear the rollback switch
                        PropertyBagDictionary.Instance.Remove(PropertyBagDictionary.Rollback);
                    }
                    else
                    {
                        SetupLogger.LogInfo("ProcessInstalls: Rollback is set and we are not doing an uninstall so we will stop processing installs");
                        break; // Get out of the loop.
                    }
                }

                // Add our current install item to the property
                PropertyBagDictionary.Instance.SafeAdd("currentInstallItem", itemToInstall);
                
                // Set the installing procress screen for this object
                SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.Installing | InstallItemsInstallDataItem.InstallDataInputs.InitializeProgress);

                // Do the pre install task
                SetupLogger.LogInfo("Doing Preinstall task for {0}", itemToInstall.ControlTitle);
                continueWithInstall = DoPreInstallTask();

                if (continueWithInstall)
                {
                    // Add any transforms needed to the commandline of the full install
                    //itemToInstall.Arguments = string.Concat(itemToInstall.Arguments, " ", quickFixEngineeringHandler.TransformListToApply(itemToInstall.ControlTitle));

                    // Do the install task
                    SetupLogger.LogInfo("Doing Install task for {0}", itemToInstall.ControlTitle);
                    continueWithInstall = DoInstallTask(itemToInstall);

                    if (!continueWithInstall)
                    {
                        itemToInstall.InstallSuccessful = false;
                        returnValue = SetupReturnValues.Failed;
                        PropertyBagDictionary.Instance.SafeAdd("currentInstallItem", itemToInstall);
                        SetupLogger.LogInfo("ProcessInstalls: Install Item {0} failed to install.  We did not launch the post process delegate.", itemToInstall.DisplayTitle);
                        SetRollbackDependents(itemToInstall, false);
                    }
                    else
                    {
                        SetupLogger.LogInfo("Doing Postinstall task for {0}", itemToInstall.ControlTitle);
                        continueWithInstall = DoPostInstallTask();
                        if (!continueWithInstall)
                        {
                            returnValue = SetupReturnValues.Failed;
                            SetRollbackDependents(itemToInstall, true);
                        }
                    }
                }
                else
                {
                    returnValue = SetupReturnValues.Failed;
                    SetRollbackDependents(itemToInstall, false);
                }

                // if user canceled, we need to undo this task and all the other ones we have done
                // if we were installing the component and it failed, we need to roll back the VMM client also
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.UserCanceledInstall))
                {
                    SetupLogger.LogInfo("***********************************************************");
                    SetupLogger.LogInfo("User decided to cancel the install.");
                    SetupLogger.LogInfo("***********************************************************");
                    SetRollbackDependents(itemToInstall, true);
                    // Ok... now set progress and stop processing installs.
                    SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.Installing | InstallItemsInstallDataItem.InstallDataInputs.FinalizeProgress);
                    break;
                }

                SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.Installing | InstallItemsInstallDataItem.InstallDataInputs.FinalizeProgress);
            }

            ProcessRollback();

            return returnValue;
        }

        /// <summary>
        /// Sets the rollback dependents.
        /// </summary>
        /// <param name="itemToInstall">The item to install.</param>
        private static void SetRollbackDependents(InstallItemsInstallDataItem itemToInstall, bool includeCurrentItem)
        {
            ArrayList installList = PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall);

            // if we are doing upgrade, and if we are past removing existing products
            // then, we cannot roll back past that point
            int rollbackStopPoint = -1;
            int indx = 0;
            InstallItemsInstallDataItem item = (InstallItemsInstallDataItem)installList[indx];
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.VMMSupportedVersionInstalled))
            {
                while (!String.Equals(item.ControlTitle, itemToInstall.ControlTitle, StringComparison.OrdinalIgnoreCase))
                {
                    indx++;
                    item = (InstallItemsInstallDataItem)installList[indx];
                }
            }

            indx = 0;
            item = (InstallItemsInstallDataItem)installList[indx];
            while (!string.Equals(item.ControlTitle, itemToInstall.ControlTitle, StringComparison.OrdinalIgnoreCase))
            {
                if (indx > rollbackStopPoint)
                {
                    AddItemToUninstallList(item);
                }
                item = (InstallItemsInstallDataItem)installList[++indx];
            }

            if (includeCurrentItem)
            {
                if (indx > rollbackStopPoint)
                {
                    AddItemToUninstallList(itemToInstall);
                }
            }
        }

        private void ProcessRollback()
        {
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ProcessingRollback, "1");
            // Process Uninstalls if needed
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.RollbacksToProcess))
            {
                // if this in not an uninstall.. we need to roll back
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall) ||
                    PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.UserCanceledInstall))
                {
                    SetupLogger.LogInfo("****************************************************************");
                    SetupLogger.LogInfo("****Starting*RollBack*******************************************");
                    SetupLogger.LogInfo("****************************************************************");

                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.Uninstall, true);

                    bool continueWithInstall;
                    ArrayList rollbackToProcess = PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.RollbacksToProcess);

                    // Reverse the list so we take stuff out in FILO order
                    rollbackToProcess.Reverse();

                    // UI
                    if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.Silent))
                    {
                        NotifyProgressPageOfRollback();
                    }

                    // Ok... process the rollbacks
                    foreach (InstallItemsInstallDataItem itemToUninstall
                        in rollbackToProcess)
                    {
                        continueWithInstall = true;

                        // Add our current install item to the property
                        PropertyBagDictionary.Instance.SafeAdd("currentInstallItem", itemToUninstall);

                        // Set the installing procress screen for this object
                        SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.Installing | InstallItemsInstallDataItem.InstallDataInputs.InitializeProgress);

                        // Do the pre install task
                        continueWithInstall = DoPreInstallTask();

                        if (continueWithInstall)
                        {
                            // Do the install task
                            itemToUninstall.InstallSuccessful = DoInstallTask(itemToUninstall);
                            continueWithInstall = itemToUninstall.InstallSuccessful;

                            if (!continueWithInstall)
                            {
                                itemToUninstall.InstallSuccessful = false;
                                SetupLogger.LogInfo("ProcessInstalls: Install Item {0} failed to install.  We did not launch the post process delegate.", itemToUninstall.DisplayTitle);
                            }
                            else
                            {
                                continueWithInstall = DoPostInstallTask();
                            }
                        }

                        itemToUninstall.InstallSuccessful = false;
                        SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.Installing | InstallItemsInstallDataItem.InstallDataInputs.FinalizeProgress);
                    }

                    SetupLogger.LogInfo("****************************************************************");
                    SetupLogger.LogInfo("****Ended*RollBack**********************************************");
                    SetupLogger.LogInfo("****************************************************************");
                }
            }
        }

        /// <summary>
        /// Does the pre install task.
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        private bool DoPreInstallTask()
        {
            InstallItemsInstallDataItem itemToInstall = PropertyBagDictionary.Instance.GetProperty<InstallItemsInstallDataItem>("currentInstallItem");

            // Run the preprocess command if we have one
            if (!string.IsNullOrEmpty(itemToInstall.PreProcessing.Value))
            {
                SetupLogger.LogInfo("ProcessInstalls: Install Item {0} has a Preprocessing delegate of {1}.  Launching it now.", itemToInstall.DisplayTitle, itemToInstall.PreProcessing.Value);
                try
                {
                    // Ok... so we could return false if there is a problem.  In that case, we should not continue with Install
                    itemToInstall.InstallSuccessful = (bool)(InstallDataItemDelegateRegistry.Instance.GetPreprocessDelegate(itemToInstall.ControlTitle)).DynamicInvoke();
                }
                catch (ArgumentException e)
                {
                    SetupLogger.LogInfo("ProcessInstalls: Running the PreprocessDelegate for {0} threw the following exception:  {1}", itemToInstall.ControlTitle, e.Message);
                    itemToInstall.InstallSuccessful = false;
                    SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.Installing | InstallItemsInstallDataItem.InstallDataInputs.FinalizeProgress);
                    PropertyBagDictionary.Instance.SafeAdd("currentInstallItem", itemToInstall);
                    if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                    {
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason, e);
                    }
                    return false;
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    itemToInstall.InstallSuccessful = false;
                    if (e.InnerException != null)
                    {
                        if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                        {
                            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason, e.InnerException);
                        }
                        SetupLogger.LogInfo("ProcessInstalls: Running the PreprocessDelegate for {0} threw the following exception:  {1}", itemToInstall.ControlTitle, e.InnerException.Message);
                    }

                    SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.Installing | InstallItemsInstallDataItem.InstallDataInputs.FinalizeProgress);
                    return false;
                }
                catch (Exception e)
                {
                    itemToInstall.InstallSuccessful = false;
                    PropertyBagDictionary.Instance.SafeAdd("currentInstallItem", itemToInstall);
                    if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                    {
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason, e);
                    }
                    SetupLogger.LogInfo("ProcessInstalls: Running the PreprocessDelegate for {0} threw the following exception:  {1}", itemToInstall.ControlTitle, e.Message);
                    SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.Installing | InstallItemsInstallDataItem.InstallDataInputs.FinalizeProgress);
                    throw;
                }

                if (!itemToInstall.InstallSuccessful)
                {
                    if (0 != (itemToInstall.ItemWeAreInstallingEnumValue & InstallItemsInstallDataItem.InstallDataInputs.ItemNotFatal))
                    {
                        SetupLogger.LogInfo("ProcessInstalls: Running the PreprocessDelegate for {0} failed.... but this item is not fatal so we will process the other items.", itemToInstall.ControlTitle);
                    }
                    else
                    {
                        SetupLogger.LogInfo("ProcessInstalls: Running the PreprocessDelegate for {0} failed.... This is a fatal item.  Setting rollback.", itemToInstall.ControlTitle);
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, true);
                    }
                }
            }
            else
            {
                itemToInstall.InstallSuccessful = true;
            }
            // Save the property state
            PropertyBagDictionary.Instance.SafeAdd("currentInstallItem", itemToInstall);
            return itemToInstall.InstallSuccessful;
        }

        /// <summary>
        /// Does the post install task.
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        private bool DoPostInstallTask()
        {
            InstallItemsInstallDataItem itemToInstall = PropertyBagDictionary.Instance.GetProperty<InstallItemsInstallDataItem>("currentInstallItem");

            // Run the postprocess command if we have one and the install did not fail.
            if (!string.IsNullOrEmpty(itemToInstall.PostProcessing.Value))
            {
                SetupLogger.LogInfo("ProcessInstalls: Install Item {0} was successful.  We will launch the post process delegate.", itemToInstall.DisplayTitle);
                try
                {
                    itemToInstall.InstallSuccessful = (bool)(InstallDataItemDelegateRegistry.Instance.GetPostProcessDelegate(itemToInstall.ControlTitle)).DynamicInvoke();
                }
                catch (ArgumentException argumentException)
                {
                    SetupLogger.LogInfo("ProcessInstalls: Running the PostProcessDelegate threw the following ArgumentException {0}", argumentException.Message);

                    // Save the property state
                    PropertyBagDictionary.Instance.SafeAdd("currentInstallItem", itemToInstall);
                    return false;
                }
                catch (TargetInvocationException argumentException)
                {
                    SetupLogger.LogInfo("ProcessInstalls: Running the PostProcessDelegate threw the following TargetInvocationException {0}", argumentException.Message);

                    // Save the property state
                    PropertyBagDictionary.Instance.SafeAdd("currentInstallItem", itemToInstall);
                    return false;
                }

                if (!itemToInstall.InstallSuccessful)
                {
                    SetupLogger.LogInfo("ProcessInstalls: Running the PostProcessDelegate returned false.");

                    //AddItemToUninstallList(itemToInstall);

                    if (0 != (itemToInstall.ItemWeAreInstallingEnumValue & InstallItemsInstallDataItem.InstallDataInputs.ItemNotFatal))
                    {
                        SetupLogger.LogInfo("ProcessInstalls: Running the PostProcessDelegate for {0} failed.... but this item is not fatal so we will process the other items.", itemToInstall.ControlTitle);
                    }
                    else
                    {
                        SetupLogger.LogInfo("ProcessInstalls: Running the PostProcessDelegate for {0} failed.... This is a fatal item.  Setting rollback.", itemToInstall.ControlTitle);
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, true);
                    }
                }
            }
            else
            {
                itemToInstall.InstallSuccessful = true;
            }

            // Save the property state
            PropertyBagDictionary.Instance.SafeAdd("currentInstallItem", itemToInstall);

            return itemToInstall.InstallSuccessful;

        }

        /// <summary>
        /// Adds the item to uninstall list.
        /// </summary>
        /// <param name="itemToUninstall">The item to uninstall.</param>
        private static void AddItemToUninstallList(InstallItemsInstallDataItem itemToUninstall)
        {
            ArrayList itemsToUninstall;
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.RollbacksToProcess))
            {
                itemsToUninstall = PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.RollbacksToProcess);
            }
            else
            {
                itemsToUninstall = new ArrayList();
            }

            // Make sure we don't already have the item in our list.
            if (-1 == itemsToUninstall.IndexOf(itemToUninstall))
            {
                itemsToUninstall.Add(itemToUninstall);
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.RollbacksToProcess, itemsToUninstall);
            }
        }

        /// <summary>
        /// Does the install task.
        /// </summary>
        /// <param name="itemToInstall">The item to install.</param>
        /// <returns></returns>
        /// 
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.ControlThread)]
        private bool DoInstallTask(InstallItemsInstallDataItem itemToInstall)
        {

            int installReturn = 0;
            if (0 != (itemToInstall.InstallTypeEnumValue & InstallItemsInstallDataItem.InstallDataInputs.ExecutableInstall))
            {
                SetupLogger.LogInfo(itemToInstall.Arguments);
                installReturn = LaunchExeSetup(itemToInstall.FullPathToLaunch, //Exe to launch
                                itemToInstall.Arguments, //Arguments
                                itemToInstall.ItemWeAreInstallingEnumValue, //Item we are installing/uninstalling
                                itemToInstall.Time, // time the install will take
                                itemToInstall.SuccessValue); //success value for install
            }
            else if (0 != (itemToInstall.InstallTypeEnumValue & InstallItemsInstallDataItem.InstallDataInputs.MicrosoftInstaller))
            {
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.VhdVersionConfiguration))
                {
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                    {
                        itemToInstall.ProductCode = GetProductCode(itemToInstall.ControlTitle);
                    }
                    installReturn = LaunchMsi(itemToInstall.FullPathToLaunch, //Full path to MSI to launch
                                                itemToInstall.Arguments, //Arguments
                                                itemToInstall.LogFile, //Name of the log file
                                                itemToInstall.ProductCode); //product code - this causes uninstall

                    // http://msdn.microsoft.com/en-us/library/Aa368542
                    // The error codes ERROR_SUCCESS, ERROR_SUCCESS_REBOOT_INITIATED, and ERROR_SUCCESS_REBOOT_REQUIRED 
                    // are indicative of success. If ERROR_SUCCESS_REBOOT_REQUIRED is returned, the installation completed successfully but a reboot is required to complete the installation operation.

                    // interpret reboot required and reboot initiated as success. LaunchMSI already added 
                    // PropertyBagDictionary.RebootRequired if this is the case
                    if (((int)NativeMethods.InstallErrorLevel.Error_Success_Reboot_Initiated == installReturn) ||
                        ((int)NativeMethods.InstallErrorLevel.Error_Success_Reboot_Required == installReturn))
                    {
                        SetupLogger.LogInfo("DoInstallTask: LaunchMSI returned {0}, setup will interpret this as Success.",
                            installReturn.ToString());
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.RebootRequired, "1");
                        installReturn = (int)NativeMethods.InstallErrorLevel.Error_Success;
                    }
                }
            }
            else if (0 != (itemToInstall.InstallTypeEnumValue & InstallItemsInstallDataItem.InstallDataInputs.CustomAction))
            {
                itemToInstall.LogFile = PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.DefaultLogName);
                if (!this.ProcessCustomAction())
                {
                    // PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, "1");
                    installReturn = 1;
                }
            }
            else if (0 != (itemToInstall.InstallTypeEnumValue & InstallItemsInstallDataItem.InstallDataInputs.PostInstallItem))
            {
                itemToInstall.LogFile = PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.DefaultLogName);
                if (!PostInstallProcessor())
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, "1");
                }
            }
            else
            {
                SetupLogger.LogError("itemToInstall: {0}: Argument InstallData.InstallDataInput was invalid.", itemToInstall.DisplayTitle);
                throw new ArgumentNullException("itemToInstall", WpfResources.WPFResourceDictionary.InvalidArgument);
            }
            CheckPointPassed(itemToInstall.LogFile, itemToInstall);

            return (installReturn == 0);
        }

        static private String GetProductCode(string controlTitle)
        {
            SetupFeatures feature = SetupFeatures.Server;
            if (controlTitle.Equals(PropertyBagConstants.CMPServer))
            {
                feature = SetupFeatures.Server;
            }
            if (controlTitle.Equals(PropertyBagConstants.WAPExtensionCommon))
            {
                feature = SetupFeatures.ExtensionCommon;
            }
            if (controlTitle.Equals(PropertyBagConstants.TenantWAPExtension))
            {
                feature = SetupFeatures.TenantExtension;
            }
            if (controlTitle.Equals(PropertyBagConstants.AdminWAPExtension))
            {
                feature = SetupFeatures.AdminExtension;
            }

            if ((feature == SetupFeatures.Server) || (feature == SetupFeatures.TenantExtension) || (feature == SetupFeatures.AdminExtension) || (feature == SetupFeatures.ExtensionCommon))
            {
                Version versionFound = new Version();
                List<string> productCodes = SystemStateDetection.CheckProductByUpgradeCode(SetupConstants.GetUpgradeCode(feature), ref versionFound);

                if (productCodes != null && productCodes.Count == 1)
                {
                    return productCodes[0];
                }
                else
                {
                    return String.Empty;
                }
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Checks the point passed.
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="itemToCheckPoint">The item to check point.</param>
        /// <returns></returns>
        static private bool CheckPointPassed(string logFile, InstallItemsInstallDataItem itemToCheckPoint)
        {
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.Rollback))
            {
                // Ok... if this is an uninstall then clear the rollback switch.
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    PropertyBagDictionary.Instance.Remove(PropertyBagDictionary.Rollback);
                }

                // Add the log to the list for watson.
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ErrorLogFile, logFile);

                // Am I in the list?  If yes, Add new property to append * to log file Name
                if (SetupHelpers.IsItemNameExists(itemToCheckPoint.ControlTitle))
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AppendStarToLogFile, true);
                }

                // set the error
                SetErrorType(itemToCheckPoint.ItemWeAreInstallingEnumValue);
            }
            return true;
        }

        /// <summary>
        /// Set the Error type for the install item
        /// </summary>
        /// <param name="itemToCheckPoint">InstallDataInputs</param>
        static void SetErrorType(InstallItemsInstallDataItem.InstallDataInputs itemToCheckPoint)
        {
            // If the item is not fatal then we will never fail.
            if (0 != (itemToCheckPoint & InstallItemsInstallDataItem.InstallDataInputs.ItemNotFatal))
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.NonVitalFailure,
                                                        PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.NonVitalFailure)
                                                                ? (PropertyBagDictionary.Instance.GetProperty<InstallItemsInstallDataItem.InstallDataInputs>(PropertyBagDictionary.NonVitalFailure) | itemToCheckPoint)
                                                                : itemToCheckPoint);

                InstallItemsInstallDataItem currentItem = PropertyBagDictionary.Instance.GetProperty<InstallItemsInstallDataItem>("currentInstallItem");
                // add to rollbacktoprocess list
                if (currentItem.ControlTitle.Equals("VM"))
                {
                    AddItemToUninstallList(currentItem);
                }

                PropertyBagDictionary.Instance.Remove(PropertyBagDictionary.Rollback);
            }
            else
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.VitalFailure,
                                                        (PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.VitalFailure)
                                                                ? (PropertyBagDictionary.Instance.GetProperty<InstallItemsInstallDataItem.InstallDataInputs>(PropertyBagDictionary.VitalFailure) | itemToCheckPoint)
                                                                : itemToCheckPoint));
            }
        }
        
        #region Install Functions
        /// <summary>
        /// Launches an Exe Setup process
        /// </summary>
        /// <param name="fullPathToExe">The full path to exe.</param>
        /// <param name="argumentsToUse">The arguments to use.</param>
        /// <param name="exeWeAreInstalling">The exe we are installing.</param>
        /// <param name="estimatedTimeInSeconds">The estimated time in seconds.</param>
        /// <param name="successValue">The success value.</param>
        /// <returns></returns>
        /// 
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.ControlThread)]
        private int LaunchExeSetup(string fullPathToExe, string argumentsToUse, InstallItemsInstallDataItem.InstallDataInputs exeWeAreInstalling, double estimatedTimeInSeconds, int successValue)
        {
            System.Diagnostics.ProcessStartInfo installProcessInfo = new System.Diagnostics.ProcessStartInfo(fullPathToExe, argumentsToUse.ToString());
            installProcessInfo.UseShellExecute = false;
            System.Diagnostics.Process installProcess;
            SetupLogger.LogInfo("LaunchExeSetup: Launching {0} with arguments: {1}", fullPathToExe, argumentsToUse.ToString());
            try
            {
                installProcess = System.Diagnostics.Process.Start(installProcessInfo);
            }
            catch (FileNotFoundException e)
            {
                SetupLogger.LogException(e);
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, true); //We may need to roll back other installs
                return -1;
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                SetupLogger.LogException(e);
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, true); //We may need to roll back other installs
                return -1;
            }
            
            //Take care of setting up the progress information
            progressData.TickSize = 1;
            progressData.Ticks = estimatedTimeInSeconds;
            progressData.ProgressValue = 0;
            //Remove the done flag 
            PropertyBagDictionary.Instance.Remove(PropertyBagDictionary.ExecutableInstallDone);
            this.SetProgressScreen(exeWeAreInstalling);

            while (!installProcess.HasExited)
            {
                installProcess.WaitForExit(1000); // Wait 1 Second
                this.SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.ProgressOnly | exeWeAreInstalling);
            }
            //Make the last little move on the progress bar
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.ExecutableInstallDone, true);
            this.SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.ProgressOnly | exeWeAreInstalling);
            Thread.Sleep(250); //Sleep long enough for the user to see the full progressbar (.25 of a second)

            if (installProcess.ExitCode != successValue)
            {
                //We failed
                //Message and rollback
                SetupLogger.LogInfo("LaunchExeSetup: Install failed, return code was {0}", installProcess.ExitCode.ToString(CultureInfo.InvariantCulture));
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, true); //We may need to roll back other installs
            }
            else
            {
                SetupLogger.LogInfo("LaunchExeSetup: Install return value was: {0}", installProcess.ExitCode.ToString(CultureInfo.InvariantCulture));
            }

            return installProcess.ExitCode;
        }

        private bool ProcessCustomAction()
        {
            InstallItemsInstallDataItem itemToInstall = PropertyBagDictionary.Instance.GetProperty<InstallItemsInstallDataItem>("currentInstallItem");

            // Run the postprocess command if we have one and the install did not fail.
            if (!string.IsNullOrEmpty(itemToInstall.ControlTitle))
            {
                SetupLogger.LogInfo("ProcessCustomAction: Installing Item {0}.", itemToInstall.DisplayTitle);
                try
                {
                    itemToInstall.InstallSuccessful = (bool)(InstallDataItemDelegateRegistry.Instance.GetCustomAction(itemToInstall.ControlTitle)).DynamicInvoke();
                }
                catch (ArgumentException argumentException)
                {
                    SetupLogger.LogInfo("ProcessCustomAction: Running the ProcessCustomAction threw the following exception {0}", argumentException.Message);

                    // Save the property state
                    PropertyBagDictionary.Instance.SafeAdd("currentInstallItem", itemToInstall);
                    return false;
                }

                if (!itemToInstall.InstallSuccessful)
                {
                    SetupLogger.LogInfo("ProcessCustomAction: Running the ProcessCustomAction for {0} failed.... This is a fatal item.  Setting rollback.", itemToInstall.ControlTitle);
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, true);
                }
            }
            else
            {
                itemToInstall.InstallSuccessful = true;
            }

            // Save the property state
            PropertyBagDictionary.Instance.SafeAdd("currentInstallItem", itemToInstall);

            return itemToInstall.InstallSuccessful;
        }

        /// <summary>
        /// Adds the warning to current node.
        /// </summary>
        /// <param name="informationToDisplay">The information to display.</param>
        private void AddWarningToCurrentNode(string informationToDisplay)
        {
            InstallItemsInstallDataItem itemToInstall = (InstallItemsInstallDataItem)PropertyBagDictionary.Instance.GetProperty<InstallItemsInstallDataItem>("currentInstallItem");

            XmlDocument InstallItemDataXml = PropertyBagDictionary.Instance.GetProperty<XmlDocument>("installItemData");
            string nodeToFind = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}[@{2}='{3}']/@{4}", SetupConstants.Root, SetupConstants.DisplayItem, SetupConstants.Parent, itemToInstall.ParentItem, SetupConstants.Image);
            XPathNavigator currentNode = InstallItemDataXml.CreateNavigator().SelectSingleNode(nodeToFind);

            if (currentNode != null)
            {
                currentNode.SetValue("/SetupCMP;component/Images/yieldIcon.png");
            }

            nodeToFind = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}[@{2}='{3}']", SetupConstants.Root, SetupConstants.DisplayItem, SetupConstants.Parent, itemToInstall.ParentItem);
            XPathNavigator warningNode = InstallItemDataXml.CreateNavigator().SelectSingleNode(nodeToFind);

            // Ok... so we need to add an attribute for the failure if
            // we don't already have an attribute with the same name
            if (string.IsNullOrEmpty(warningNode.GetAttribute("ErrorInformationText", "")))
            {
                string errorLogFile = PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.DefaultLogName);

                try
                {
                    // Append the Star to log file if we have added the 'AppendStarToLogFile' in CheckPointPassed
                    // Remove the the property once Star is appened to log file
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AppendStarToLogFile))
                    {
                        errorLogFile = string.Format("{0}*", errorLogFile);
                        PropertyBagDictionary.Instance.Remove(PropertyBagConstants.AppendStarToLogFile);
                    }                  

                    warningNode.CreateAttribute(
                        "",
                        "ErrorInformationText",
                        "",
                        string.Format(
                            WpfResources.WPFResourceDictionary.FinishWarningFormat,
                            itemToInstall.DisplayTitle,
                            informationToDisplay,
                            errorLogFile));
                }
                catch (ArgumentException)
                {
                }
                catch (FormatException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (NotSupportedException)
                {
                }

            }

            return;
        }

        /// <summary>
        /// Adds the error to current node.
        /// </summary>
        private void AddErrorToCurrentNode()
        {
            InstallItemsInstallDataItem itemToInstall = (InstallItemsInstallDataItem)PropertyBagDictionary.Instance.GetProperty<InstallItemsInstallDataItem>("currentInstallItem");

            XmlDocument InstallItemDataXml = PropertyBagDictionary.Instance.GetProperty<XmlDocument>("installItemData");
            string nodeToFind = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}[@{2}='{3}']/@{4}", SetupConstants.Root, SetupConstants.DisplayItem, SetupConstants.Parent, itemToInstall.ParentItem, SetupConstants.Image);
            XPathNavigator currentNode = InstallItemDataXml.CreateNavigator().SelectSingleNode(nodeToFind);

            if (currentNode != null)
            {
                currentNode.SetValue("/SetupCMP;component/Images/smallError.png");
            }

            return;
        }
        /// <summary>
        /// Posts the install processor.
        /// </summary>
        /// <returns>true if successful</returns>
        private bool PostInstallProcessor()
        {
            bool returnValue = true;
            // Take care of setting up the progress information
            progressData.TickSize = 1;
            progressData.Ticks = 2; // Need one ticks for general items
            progressData.ProgressValue = 0;

            // Remove the done flag 
            PropertyBagDictionary.Instance.Remove(PropertyBagDictionary.ExecutableInstallDone);
            this.SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.ExecutableInstall);

            // Were we successful?
            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.VitalFailure))
            {
                // Was this an install?
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    // Make sure at least one thing succeeded in the setup.
                    returnValue = AtLeastOneInstallSucceeded();
                    if (!returnValue)
                    {
                        SetupLogger.LogInfo("No components were successfully installed.");
                    }
                    else
                    {
                        // Need to add the ARP entry
                        try
                        {
                            SetupHelpers.AddUninstallChangeEntry();
                        }
                        catch (ArgumentException exception)
                        {
                            SetupLogger.LogException(exception);
                            returnValue = false;
                        }
                    }

                    this.SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.ProgressOnly | InstallItemsInstallDataItem.InstallDataInputs.ExecutableInstall);
                }
                else // This was an uninstall.... we must remove the ARP
                {
                    // We are doing an uninstall
                    // Was this a full uninstall?
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FullUninstall))
                    {
                        // Need to remove the ARP entry
                        SetupHelpers.RemoveUninstallChangeEntry();

                        // Need to remove the event sources
                        SetupHelpers.DeleteEventSources();
                    }
                    this.SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.ProgressOnly | InstallItemsInstallDataItem.InstallDataInputs.ExecutableInstall);
                }
            }

            // Make the last little move on the progress bar
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.ExecutableInstallDone, true);
            this.SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.ProgressOnly | InstallItemsInstallDataItem.InstallDataInputs.ExecutableInstall);
            Thread.Sleep(250); // Sleep long enough for the user to see the full progressbar (.25 of a second)
            
            return returnValue;
        }

        /// <summary>
        /// Adds to warning.
        /// </summary>
        /// <param name="warningTextToAdd">The warning text to add.</param>
        private static void AddToWarning(string warningTextToAdd)
        {
            string warning = string.Empty;
            try
            {
                if (PropertyBagDictionary.Instance.PropertyExists(FinishPageWarning))
                {
                    warning = PropertyBagDictionary.Instance.GetProperty<string>(FinishPageWarning);
                    warning = warning + "&#xA;";
                }

                PropertyBagDictionary.Instance.SafeAdd(
                FinishPageWarning,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}",
                    warning,
                    warningTextToAdd));
            }
            catch (ArgumentException)
            {
            }
            catch (FormatException)
            {
            }

            return;
        }

        /// <summary>
        /// Ats the least one install succeeded.
        /// </summary>
        /// <returns>Return true if as least one thing succeeded</returns>
        private static bool AtLeastOneInstallSucceeded()
        {
            bool returnValue = false;

            // Make sure at least one thing succeeded.
            foreach (InstallItemsInstallDataItem itemToInstall
                in PropertyBagDictionary.Instance.GetProperty<ArrayList>(PropertyBagConstants.ItemsToInstall))
            {
                if (itemToInstall.InstallSuccessful &&
                    !string.Equals(itemToInstall.ControlTitle, "POSTINSTALL", StringComparison.OrdinalIgnoreCase))
                {
                    returnValue = true;
                    break;
                }
            }

            return returnValue;
        }
        
        /// <summary>
        /// Launches a Msi
        /// </summary>
        /// <param name="msiPath">The msi path.</param>
        /// <param name="commandLineArguments">The arguments to use.</param>
        /// <param name="installerLogFile">The installer log file.</param>
        /// <param name="productCode">The product code.</param>
        /// <returns></returns>
        public int LaunchMsi(string msiPath, string commandLineArguments , string installerLogFile, string productCode)
        {
            progressData.TickSize = 1;
            progressData.ProgressValue = 1;

            StringBuilder argumentsToUse = new StringBuilder(commandLineArguments);
           
            this.SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.MicrosoftInstaller);
            PropertyBagDictionary.Instance.Remove(PropertyBagDictionary.MicrosoftInstallerInstallDone);
            int installerReturnValue = (int)NativeMethods.InstallErrorLevel.Error_Install_Failed;
            try
            {
                bool msiEngineMutexInUse = true; //We will use this to warn the user if the mutex is in use.
                while (msiEngineMutexInUse)
                {
                    //Check the mutex.  If it is in use write that to the log and then ask the user what they want to do
                    Mutex msiEngineMutex = new Mutex(false, "_MSIExecute");
                    try
                    {
                        if (msiEngineMutex.WaitOne(TimeSpan.FromSeconds(SecondsWaitForMutexMSIExecute), false))
                        {
                            SetupLogger.LogInfo("LaunchMsi: Msi mutex is not in use.");
                            msiEngineMutexInUse = false;
                            msiEngineMutex.ReleaseMutex();
                        }
                        else
                        {
                            SetupLogger.LogInfo("LaunchMsi: Msi mutex is in use.  We can not run our msi because the mutex is not avaiable.");
                            msiEngineMutexInUse = true;
                            PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, true); //We may need to roll back other installs
                            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.Silent))
                            {                                
                                //DialogResult result = this.wizardScreenData.LogSetupWarning(setup.mutexInUse, MessageBoxButtons.RetryCancel);
                                //if (DialogResult.Cancel == result)
                                //{
                                    return (int)NativeMethods.InstallErrorLevel.Error_Install_Failed;
                                //}
                            }
                            else
                            {
                                SetupLogger.LogInfo("LaunchMsi: Msi mutex is in use and we are in silent mode.  We must fail setup.  Check the event log for details about what the msi engine was doing.");
                                return (int)NativeMethods.InstallErrorLevel.Error_Install_Failed;
                            }
                        }
                    }
                    catch (AbandonedMutexException)
                    {
                        SetupLogger.LogInfo("LaunchMsi: Msi mutex is Abandoned.  This is very bad.  We must fail.");
                        return (int)NativeMethods.InstallErrorLevel.Error_Install_Failed;
                    }
                }

                SetupLogger.LogInfo("LaunchMsi: MSI to launch is: {0}.", msiPath);

                //Ok now we need to:
                // 1.   Turn on the UI mode
                // 2.   Turn on logging
                // 3.   Launch the MSI.
                IntPtr nullPointer = IntPtr.Zero;
                NativeMethods.MsiSetInternalUI((int)NativeMethods.InstallUiLevel.None, ref nullPointer); // Set the UI to full
                SetupLogger.LogInfo("LaunchMsi: Turning off the internal UI for {0}.", msiPath);

                int logReturn = NativeMethods.MsiEnableLog((int)(NativeMethods.InstallLogModes.ActionData
                                                                                        | NativeMethods.InstallLogModes.ActionStart
                                                                                        | NativeMethods.InstallLogModes.Error
                                                                                        | NativeMethods.InstallLogModes.FatalExit
                                                                                        | NativeMethods.InstallLogModes.Info
                                                                                        | NativeMethods.InstallLogModes.OutOfDiskSpace
                                                                                        | NativeMethods.InstallLogModes.PropertyDump
                                                                                        | NativeMethods.InstallLogModes.User
                                                                                        | NativeMethods.InstallLogModes.Warning
                                                                                      //| NativeMethods.InstallLogModes.LogOnError
                                                                                        | NativeMethods.InstallLogModes.Verbose),
                                                                                        installerLogFile,
                                                                                        (int)NativeMethods.InstallLogAttributes.Append);
                if (logReturn != 0)
                {
                    SetupLogger.LogInfo("LaunchMSI:  Unable to enable logging for the MSI {0}.", msiPath);
                    //TODO:  So we can't log... may want to fail.
                }
                else
                {
                    SetupLogger.LogInfo("LaunchMSI:  Enable logging for the MSI at {0}.", installerLogFile);
                }

                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.Silent))
                {
                    SetupLogger.LogInfo("LaunchMsi: MSI {0} is not in silent mode.  Setting the external UI.", msiPath);
                    NativeMethods.MsiSetExternalUI(
                        MsiProgressCallback
                        , (int)(NativeMethods.InstallLogModes.ActionData
                        | NativeMethods.InstallLogModes.ActionStart
                        | NativeMethods.InstallLogModes.FilesInUse
                        | NativeMethods.InstallLogModes.Error
                        | NativeMethods.InstallLogModes.FatalExit
                        | NativeMethods.InstallLogModes.Warning
                        | NativeMethods.InstallLogModes.Progress)
                        , (IntPtr)0);
                }

                //Ok... So if we are doing an Uninstall then we will have a value for the GUID...
                //if not, then we are doing an install

                // Add a supress to the commandline so that we never reboot here
                argumentsToUse.Append(" REBOOT=ReallySuppress");

                if (string.IsNullOrEmpty(productCode))
                {
                    //Check to make sure that the MSI exists
                    if (!File.Exists(msiPath))
                    {
                        SetupLogger.LogInfo("LaunchMsi: MSI not found at: {0}", msiPath);
                        SetupLogger.LogInfo("LaunchMSI: Setting rollback to true");
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, true); //We may need to roll back other installs
                        return (int)NativeMethods.InstallErrorLevel.Error_Install_Failed;
                    }
                    else
                    {
                        // Use the full path format
                        msiPath = Path.GetFullPath(msiPath);
                    }

                    SetupLogger.LogInfo("LaunchMsi: Launching {0} with arguments '{1}'", msiPath, argumentsToUse.ToString());
                    installerReturnValue = NativeMethods.MsiInstallProduct(msiPath, argumentsToUse.ToString());
                    SetupLogger.LogInfo("MsiInstallProduct finished for msi {0}.", msiPath);
                }
                else
                {
                    SetupLogger.LogInfo("LaunchMsi: Launching MSI with product code {0} using arguments '{1}'", productCode, argumentsToUse.ToString());

                    installerReturnValue = NativeMethods.MsiConfigureProductEx(productCode, 100, INSTALLSTATE_ABSENT, argumentsToUse.ToString());
                    SetupLogger.LogInfo("MsiConfigureProductEx finished for {0}.", productCode);
                }
            }
            catch (Exception ex)
            {
                SetupLogger.LogInfo("LaunchMsi: Exception: {0}. TargetSite: {1}. StackTrace: {2}", ex.Message, ex.TargetSite.ToString(), ex.StackTrace.ToString());
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason, ex);
                }
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, true); //We may need to roll back other installs
                throw;
            }

            PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.MicrosoftInstallerInstallDone, true);
            //See if the install failed for any reason.
            //except:
            //1.    User Canceled
            //2.    We needed a reboot
            if (((int)NativeMethods.InstallErrorLevel.Error_Success != installerReturnValue)
                && ((int)NativeMethods.InstallErrorLevel.Error_Success_Reboot_Initiated != installerReturnValue)
                && ((int)NativeMethods.InstallErrorLevel.Error_Success_Reboot_Required != installerReturnValue))
            {
                SetupLogger.LogInfo("LaunchMSI: Setting rollback to true");
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.Rollback, true); //We may need to roll back other installs
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.LastFailureLog))
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.LastFailureLog, installerLogFile);
                }
                SetupLogger.LogInfo("LaunchMSI: MSI {0} returned error {1}", msiPath, installerReturnValue.ToString(CultureInfo.InvariantCulture));
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
                {
                    if ((int)NativeMethods.InstallErrorLevel.Error_CannotOpenInstallationPackage == installerReturnValue)
                    {
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason,
                            new Exception(String.Format("Installing {0} failed with Windows Installer error {1}",
                                msiPath, installerReturnValue.ToString())));
                    }
                    else
                    {
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.FailureReason,
                            new Exception(String.Format("Installing {0} failed with Windows Installer error {1}", msiPath, installerReturnValue.ToString())));
                    }
                }
            }
            else
            {
                //Do we just need to do a reboot?
                if (((int)NativeMethods.InstallErrorLevel.Error_Success_Reboot_Initiated == installerReturnValue)
                    || ((int)NativeMethods.InstallErrorLevel.Error_Success_Reboot_Required == installerReturnValue))
                {
                    SetupLogger.LogInfo("LaunchMSI: MSI {0} requires a reboot.", msiPath);
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.RebootRequired, true); //We need a reboot
                }
                SetupLogger.LogInfo("LaunchMSI: MSI {0} succeeded.", msiPath);

                return installerReturnValue;
            } //end if

            //Clear the minor step message
            return installerReturnValue;

        } //end of function LaunchMsi

        #endregion

        #region Progress Related Functions
        private delegate void ProgressCallback(); //This is used to give us a type to cast the anonymous method to
        
        public int MicrosoftInstallerUIHandler(IntPtr context, Int32 messageType, string message)
        {
            //SetupLogger.LogInfo("context=" + context);
            //SetupLogger.LogInfo("type=" + messageType);
            //SetupLogger.LogInfo("message=" + message);

            int installMessageTypeFlag = (int)(0xFF000000 & messageType);

            switch ((int)installMessageTypeFlag)
            {
                case NativeMethods.mtActionData:
                case NativeMethods.mtActionStart:
                    //case (int)NativeMethods.InstallLogModes.ActionStart:
                    string[] actionFields = ParseCommonData(message);
                    if (null == actionFields || null == actionFields[0])
                    {
                        return (int)MessageBoxResult.OK;
                    }

                    if (null != actionFields[0])
                    {
                        this.progressData.DoMicrosoftInstallerProgress = true;
                        this.progressData.MinorStep = actionFields[0].ToString();
                        this.SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.ProgressOnly);
                    }
                    System.Diagnostics.Debug.WriteLine("Some Kind of action message " + message);
                    return (int)MessageBoxResult.OK;
                case NativeMethods.mtProgress:
                    //case (int)NativeMethods.InstallLogModes.Progress:
                    {
                        //Process the message
                        string[] fields = ParseProgressString(message);
                        if (null == fields)
                        {
                            return (int)MessageBoxResult.OK;
                        }
                        else if (null == fields[0])
                        {
                            return (int)MessageBoxResult.OK;
                        }

                        switch (fields[0][0])
                        {
                            case '0': //   reset progress bar
                                //   1 = total, 2 = direction , 3 = state
                                this.progressData.DoMicrosoftInstallerProgress = true;
                                this.progressData.Ticks = Convert.ToInt32(fields[1].ToString(), CultureInfo.InvariantCulture);
                                this.progressData.ProgressValue = 0;
                                this.SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.ProgressOnly);
                                return (int)MessageBoxResult.OK;

                            case '1': //   action info
                                //   1 = # ticks for the step size, 2 = actuall step it
                                return (int)MessageBoxResult.OK;

                            case '2': //   progress
                                //   1 = how far the progress bar moved,
                                //   forward / backward, based on case '0'
                                this.progressData.DoMicrosoftInstallerProgress = true;
                                this.progressData.Ticks = 0;
                                //SetupLogger.LogInfo("Reset Ticks to 0");
                                this.progressData.TickSize = Convert.ToInt32(fields[1].ToString(), CultureInfo.InvariantCulture);
                                this.progressData.DoTick = 1;
                                this.SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs.ProgressOnly);
                                return (int)MessageBoxResult.OK;

                            default:
                                System.Diagnostics.Debug.WriteLine("Not understood inside " + message);
                                return (int)MessageBoxResult.OK;
                        }
                    }
                //Premature termination
                case NativeMethods.mtFatalExit:
                    /* Get fatal error message here and display it*/
                    //wizardScreenData.LogSetupError(message);
                    return (int)MessageBoxResult.Cancel;

                case NativeMethods.mtError:
                    /* Get error message here and display it*/
                    //wizardScreenData.LogSetupError(message);
                    return (int)MessageBoxResult.Cancel;

                case NativeMethods.mtWarning:
                    /* Get warning message here and display it */
                    //wizardScreenData.LogSetupWarning(message);
                    break;

                case NativeMethods.mtFilesInUse:
                    // This will display Files In Use dialog.
                    //wizardScreenData.LogSetupWarning(message);
                    // TODO: Right way is to call MsiProcessMessage
                    //return (int)DialogResult.Retry;
                    return (int)MessageBoxResult.OK;

                default:
                    System.Diagnostics.Debug.WriteLine("Not understood outside " + message);
                    break;
                }

            return 0;
        }

        static public string[] ParseProgressString(string dataToParse)
        {
            string[] parse = new string[4];
            Regex regex = new Regex(@"\d:\s\d+\s");
            int count = 0;

            foreach (Match match in regex.Matches(dataToParse))
            {
                if (count > 4) return null;

                parse[count++] = match.Value.Substring(match.Value.IndexOf(":", StringComparison.OrdinalIgnoreCase) + 2).Trim();
            }

            return parse;
        }

        static public string[] ParseCommonData(string dataToParse)
        {
            string[] res = new string[3];
            Regex regex = new Regex(@"[.]\s.*");
            int count = 0;

            foreach (Match match in regex.Matches(dataToParse))
            {
                if (count > 3) return null;

                res[count++] = match.Value.Substring(match.Value.IndexOf(".", StringComparison.OrdinalIgnoreCase) + 1).Trim();
            }

            return res;
        }

        /// <summary>
        /// Calls the progress page to update the header text to indicate that we are rolling back features.
        /// </summary>
        private void NotifyProgressPageOfRollback()
        {
            ProgressPage progressPage = PageRegistry.Instance.GetPage("ProgressPage").PageUI as ProgressPage;

            ProgressCallback statusUpdate = delegate
            {
                progressPage.OnRollback();                
            };
            progressPage.progressBarInstallation.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, statusUpdate);            
        }

        /// <summary>
        /// Processes the current install actions
        /// </summary>        
        public void SetProgressScreen(InstallItemsInstallDataItem.InstallDataInputs currentProgressItem)
        {
            //Check to see if we are silent
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.Silent))
                return; //We are silent... no need for progress screen

            //We are not silent... need to setup the progress 
            InstallItemsInstallDataItem itemToInstall = (InstallItemsInstallDataItem)PropertyBagDictionary.Instance.GetProperty<InstallItemsInstallDataItem>("currentInstallItem");
            if (itemToInstall == null)
            {
                return;
            }
                
            XmlDocument InstallItemDataXml = PropertyBagDictionary.Instance.GetProperty<XmlDocument>("installItemData");
            string nodeToFind = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}[@{2}='{3}']/@{4}", SetupConstants.Root, SetupConstants.DisplayItem, SetupConstants.Parent, itemToInstall.ParentItem, SetupConstants.Image);
            XPathNavigator currentNode = InstallItemDataXml.CreateNavigator().SelectSingleNode(nodeToFind);
            
            ProgressData progressDataInfo = this.progressData;
            ProgressPage progressPage = PageRegistry.Instance.GetPage("ProgressPage").PageUI as ProgressPage;
            if (0 != (currentProgressItem & InstallItemsInstallDataItem.InstallDataInputs.ProgressOnly))
            {
                //Special case for exe installs due to progress bar issues.
                if (0 != (currentProgressItem & InstallItemsInstallDataItem.InstallDataInputs.ExecutableInstall))
                {
                    //Since setup.exe's do not provide progress, we have to guess how
                    //much progress to display.  We do not want to display the last
                    //10% of the bar incase the machine is running slower than we
                    //estimated.
                    if (progressDataInfo.ProgressValue < (.9 * progressDataInfo.Ticks))
                    {
                        progressDataInfo.ProgressValue += progressDataInfo.TickSize;
                        SetupLogger.LogInfo(
                            "SetProgressScreen: Setting progress to: {0}",
                            progressDataInfo.ProgressValue.ToString(CultureInfo.InvariantCulture));
                    }

                    //If the setup is all done, move the bar to 100%
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.ExecutableInstallDone))
                    {
                        progressDataInfo.ProgressValue = progressDataInfo.Ticks;
                    }
                }
            }
            else
            {
                if (0 != (currentProgressItem & InstallItemsInstallDataItem.InstallDataInputs.MicrosoftInstaller))
                {
                    progressDataInfo.ResetBarValue = true;
                    SetupLogger.LogInfo("SetProgressScreen: Init MSI Install progress.");
                }
                if (0 != (currentProgressItem & InstallItemsInstallDataItem.InstallDataInputs.ExecutableInstall))
                {
                    progressDataInfo.ResetBarValue = true;
                    SetupLogger.LogInfo("SetProgressScreen: Init Exe Install progress.");
                }

                if (0 != (currentProgressItem & InstallItemsInstallDataItem.InstallDataInputs.InitializeProgress))
                {
                    progressDataInfo.StartMinorStep = true;
                    SetupLogger.LogInfo("SetProgressScreen: StartMinorStep.");
                }

                if (0 != (currentProgressItem & InstallItemsInstallDataItem.InstallDataInputs.FinalizeProgress))
                {
                    progressDataInfo.FinishMinorStep = true;
                    SetupLogger.LogInfo("SetProgressScreen: FinishMinorStep.");
                }

            }

            ProgressCallback statusUpdate = delegate
            {                
                if (progressDataInfo.ResetBarValue)
                {
                    progressPage.progressBarInstallation.Value = 0;
                    progressPage.progressBarInstallation.Maximum = progressDataInfo.Ticks;
                    progressDataInfo.ResetBarValue = false;
                    progressDataInfo.MinorStep = "";
                    progressPage.textBlockDetailedProgress.Text = itemToInstall.DisplayTitle;
                    //SetupLogger.LogInfo("statusUpdate: Resetting progress bar.");
                }
                if (progressDataInfo.StartMinorStep)
                {
                    // Commenting out because we should fix the wait icon to show up at every node instead of just a single spot
                    //if (currentNode != null)
                    //{
                    //    currentNode.SetValue("/SetupCMP;component/Images/smallWait.png");
                    //}
                    progressDataInfo.StartMinorStep = false;
                }
                if (progressDataInfo.FinishMinorStep)
                {
                    if (itemToInstall.InstallSuccessful)
                    {
                        if (PropertyBagDictionary.Instance.PropertyExists(FinishPageWarning))
                        {
                            AddWarningToCurrentNode(PropertyBagDictionary.Instance.GetProperty<string>(FinishPageWarning));
                            PropertyBagDictionary.Instance.SafeRemove(FinishPageWarning);
                        }
                        else
                        {
                            // Commenting out because we should fix the check icon to show up at every node instead of just a single spot
                            //if (currentNode != null)
                            //{
                            //    currentNode.SetValue("/SetupCMP;component/Images/smallGreenCheck.png");
                            //}
                        }
                    }
                    else
                    {
                        AddErrorToCurrentNode();
                    }

                    progressDataInfo.FinishMinorStep = false;
                }

                //Should we step the list in the tree view?
                if (progressDataInfo.StepTreeView)
                {
                    if (currentNode != null)
                    {
                        currentNode.SetValue(itemToInstall.InstallSuccessful ? "/SetupCMP;component/Images/smallGreenCheck.png" : "/SetupCMP;component/Images/smallError.png");
                    }
                    progressDataInfo.StepTreeView = false;
                }

                if (progressDataInfo.ProgressValue > 0)
                {
                    if (progressDataInfo.ProgressValue <= progressPage.progressBarInstallation.Maximum)
                    {
                        progressPage.progressBarInstallation.Value = progressDataInfo.ProgressValue;
                    }
                }
                if (progressDataInfo.DoMicrosoftInstallerProgress)
                {
                    //SetupLogger.LogInfo("We are doing a MSI progress message");
                    progressDataInfo.DoMicrosoftInstallerProgress = false;
                    if (0 != progressDataInfo.Ticks)
                    {
                        progressPage.progressBarInstallation.Value = 0;
                        progressPage.progressBarInstallation.Maximum = progressDataInfo.Ticks;
                        SetupLogger.LogInfo("statusUpdate: Resetting progress bar for MSI: MaxValue {0}", progressDataInfo.Ticks.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        progressPage.progressBarInstallation.Value += progressData.TickSize;
                    }
                }
            };
            progressPage.progressBarInstallation.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, statusUpdate);
        }
        #endregion
    }

    public class ProgressData
    {
        private bool doMicrosoftInstallerProgress;
        private bool resetBarValue = true;
        private bool stepTreeView;
        private bool startMinorStep;
        private bool finishMinorStep;
        private double ticks;
        private double doTick;
        private double tickSize;
        private double progressValue;
        private string majorStep;
        private string minorStep;
        private static ProgressData instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressData"/> class.
        /// </summary>
        private ProgressData()
        {
            //Do Nothing
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ProgressData Instance
        {
            get
            {
                if (ProgressData.instance == null)
                {
                    ProgressData.instance = new ProgressData();
                }
                return ProgressData.instance;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [do microsoft installer progress].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [do microsoft installer progress]; otherwise, <c>false</c>.
        /// </value>
        public bool DoMicrosoftInstallerProgress
        {
            get { return this.doMicrosoftInstallerProgress; }
            set { this.doMicrosoftInstallerProgress = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [reset bar value].
        /// </summary>
        /// <value><c>true</c> if [reset bar value]; otherwise, <c>false</c>.</value>
        public bool ResetBarValue
        {
            get { return this.resetBarValue; }
            set { this.resetBarValue = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [step tree view].
        /// </summary>
        /// <value><c>true</c> if [step tree view]; otherwise, <c>false</c>.</value>
        public bool StepTreeView
        {
            get { return this.stepTreeView; }
            set { this.stepTreeView = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [start minor step].
        /// </summary>
        /// <value><c>true</c> if [start minor step]; otherwise, <c>false</c>.</value>
        public bool StartMinorStep
        {
            get { return this.startMinorStep; }
            set { this.startMinorStep = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [finish minor step].
        /// </summary>
        /// <value><c>true</c> if [finish minor step]; otherwise, <c>false</c>.</value>
        public bool FinishMinorStep
        {
            get { return this.finishMinorStep; }
            set { this.finishMinorStep = value; }
        }

        /// <summary>
        /// Gets or sets the ticks.
        /// </summary>
        /// <value>The ticks.</value>
        public double Ticks
        {
            get { return this.ticks; }
            set { this.ticks = value; }
        }

        /// <summary>
        /// Gets or sets the do tick.
        /// </summary>
        /// <value>The do tick.</value>
        public double DoTick
        {
            get { return this.doTick; }
            set { this.doTick = value; }
        }

        /// <summary>
        /// Gets or sets the size of the tick.
        /// </summary>
        /// <value>The size of the tick.</value>
        public double TickSize
        {
            get { return this.tickSize; }
            set { this.tickSize = value; }
        }

        /// <summary>
        /// Gets or sets the progress value.
        /// </summary>
        /// <value>The progress value.</value>
        public double ProgressValue
        {
            get { return this.progressValue; }
            set { this.progressValue = value; }
        }

        /// <summary>
        /// Gets or sets the major step.
        /// </summary>
        /// <value>The major step.</value>
        public string MajorStep
        {
            get { return this.majorStep; }
            set { this.majorStep = value; }
        }

        /// <summary>
        /// Gets or sets the minor step.
        /// </summary>
        /// <value>The minor step.</value>
        public string MinorStep
        {
            get { return this.minorStep; }
            set { this.minorStep = value; }
        }
    }
}
