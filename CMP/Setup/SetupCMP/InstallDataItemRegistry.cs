//-----------------------------------------------------------------------
// <copyright file="InstallDataItemRegistry.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> This class holds all of the active pages.
// </summary>
//-----------------------------------------------------------------------
namespace Microsoft.VirtualManager.SetupFramework
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;

    /// <summary>
    ///  This class acts as a registry for pages
    /// </summary>
    public class InstallDataItemRegistry
    {
        // This will hold the list of pages we can navigate to
        private Dictionary<String, InstallItemsInstallDataItem> installDataItems = new Dictionary<String, InstallItemsInstallDataItem>();

        private delegate bool InstallDataItemCallback();

        private static InstallDataItemRegistry instance;

        /// <summary>
        /// private static constructor
        /// </summary>
        private InstallDataItemRegistry()
        {
        }

        /// <summary>
        /// Returns an instance of the class.  If there
        /// is no instance, it creates one and returns it.
        /// </summary>
        public static InstallDataItemRegistry Instance
        {
            get
            {
                if (InstallDataItemRegistry.instance == null)
                {
                    InstallDataItemRegistry.instance = new InstallDataItemRegistry();
                }
                return InstallDataItemRegistry.instance;
            }
        }

        /// <summary>
        /// Gets the install data items.
        /// </summary>
        /// <value>The install data items.</value>
        public Dictionary<String, InstallItemsInstallDataItem> InstallDataItems
        {
            get
            {
                return installDataItems;
            }
        }

        /// <summary>
        /// Addes a Install data item to the install data item register
        /// </summary>
        /// <param name="pageToRegister">Page to add to the register</param>
        public void RegisterDataItem(InstallItemsInstallDataItem installDataItemToRegister)
        {
            this.installDataItems.Add(installDataItemToRegister.ControlTitle, installDataItemToRegister);
            if (installDataItemToRegister.InstallTypeEnumValue == InstallItemsInstallDataItem.InstallDataInputs.CustomAction)
            {
                InstallDataItemDelegateRegistry.Instance.RegisterCustomAction(
                    installDataItemToRegister.ControlTitle,
                    GetDelegate(
                        installDataItemToRegister.CustomAction.DelegateId,
                        installDataItemToRegister.CustomAction.Value));
            }
            if (!String.IsNullOrEmpty(installDataItemToRegister.PreProcessing.Value))
            {
                InstallDataItemDelegateRegistry.Instance.RegisterPreprocessDelegate(
                    installDataItemToRegister.ControlTitle, 
                    GetDelegate(
                        installDataItemToRegister.PreProcessing.DelegateId, 
                        installDataItemToRegister.PreProcessing.Value));
            }
            if (!String.IsNullOrEmpty(installDataItemToRegister.PostProcessing.Value))
            {
                InstallDataItemDelegateRegistry.Instance.RegisterPostProcessDelegate(
                    installDataItemToRegister.ControlTitle, 
                    GetDelegate(
                        installDataItemToRegister.PostProcessing.DelegateId, 
                        installDataItemToRegister.PostProcessing.Value));
            }
            if (!String.IsNullOrEmpty(installDataItemToRegister.Prereq.Value))
            {
                InstallDataItemDelegateRegistry.Instance.RegisterPrerequisiteDelegate(
                    installDataItemToRegister.ControlTitle, 
                    GetDelegate(
                        installDataItemToRegister.Prereq.DelegateId, 
                        installDataItemToRegister.Prereq.Value));
            }
        }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        private static Delegate GetDelegate(String className, String functionName)
        {
            Type preprocessingType = Type.GetType(className);
            MethodInfo preprocessingMethodInfo = preprocessingType.GetMethod(functionName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return Delegate.CreateDelegate(typeof(InstallDataItemCallback), preprocessingMethodInfo);
        }

        /// <summary>
        /// Used to get a page by it name
        /// </summary>
        /// <param name="pageId">String that identifies the page to return</param>
        /// <returns>Page matching the given string</returns>
        public InstallItemsInstallDataItem GetInstallDataItem(String controlTitle)
        {
            return this.installDataItems[controlTitle];
        }
    }
}
