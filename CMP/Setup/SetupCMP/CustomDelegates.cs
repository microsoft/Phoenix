//-----------------------------------------------------------------------
// <copyright file="CustomDelegates.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Provides Custom Movement Delegates.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using CMP.Setup.Helpers;
    using CMP.Setup.SetupFramework;
    using System;
    using System.Diagnostics;
    using System.Xml;

    /// <summary>
    /// Custom Page Navigation Delegates
    /// </summary>
    public static class CustomDelegates
    {
        #region Default Navigation Logic
        /// <summary>
        /// Null page handler.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>always null</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "currentPage", Justification = "Must match expected function parameters")]
        public static Page NullPageHandler(Page currentPage)
        {
            return null;
        }

        /// <summary>
        /// Returns the default next page.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page DefaultNextPage(Page currentPage)
        {
            return DefaultPage(currentPage.NextPageArgument);
        }

        /// <summary>
        /// Returns the default previous page.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page DefaultPreviousPage(Page currentPage)
        {
            return DefaultPage(currentPage.PreviousPageArgument);
        }
        #endregion Default Navigation Logic

        #region Generic forward/backward handlers

        /// <summary>
        /// Forwards to page based on property value handler.
        /// Ok so this fuction takes a string that looks like this
        /// [nameofproperty],[screenfor0],[screeenfor1],[screenfor2],...,[screenforN]
        /// it parses and looks for the property in the property bag.
        /// if it finds it, it returns the screen from the input that matches the value
        /// of the property bag entry.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page ForwardToPageBasedOnPropertyValueHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            int pageToMoveTo = 0;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            // Check to see if the length is correct
            if (pageChoices.Length < 2)
            {
                SetupLogger.LogError("ForwardToPageBasedOnPropertyValueHandler: Invalid XML description for Page");
                throw new ArgumentException(currentPage.ToString());
            }

            if (!PropertyBagDictionary.Instance.PropertyExists(pageChoices[0]))
            {
                // Page not specified               
                SetupLogger.LogError("ForwardToPageBasedOnPropertyValueHandler: Missing property bag entry for {0}.  This entry is vital to select the next page.", pageChoices[0]);
                throw new ArgumentException(currentPage.ToString());
            }
            else
            {
                pageToMoveTo = 1 + PropertyBagDictionary.Instance.GetProperty<int>(pageChoices[0]);
                SetupLogger.LogInfo("ForwardToPageBasedOnPropertyValueHandler: Property bag entry for {0} is {1}.  This means that we will move to a next page id {2}.", pageChoices[0], pageToMoveTo.ToString(), pageChoices[pageToMoveTo]);
            }

            // We will go to the page specifed in the pageChoice
            return PageRegistry.Instance.GetPage(pageChoices[pageToMoveTo]);
        }

        /// <summary>
        /// Backwards to page based on property value handler.
        /// Ok so this fuction takes a string that looks like this
        /// [nameofproperty],[screenfor0],[screeenfor1],[screenfor2],...,[screenforN]
        /// it parses and looks for the property in the property bag.
        /// if it finds it, it returns the screen from the input that matches the value
        /// of the property bag entry.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page BackwardToPageBasedOnPropertyValueHandler(Page currentPage)
        {
            string prevPageChoices = currentPage.PreviousPageArgument;
            string[] pageChoices = prevPageChoices.Split(new char[] { ',' });

            // Check to see if the length is correct
            if (pageChoices.Length < 2)
            {
                SetupLogger.LogError("BackwardToPageBasedOnPropertyValueHandler: Invalid XML description for Page");
                throw new ArgumentException(currentPage.ToString());
            }

            if (!PropertyBagDictionary.Instance.PropertyExists(pageChoices[0]))
            {
                // Page not specified
                SetupLogger.LogError("BackwardToPageBasedOnPropertyValueHandler: Missing property bag entry for {0}.  This entry is vital to select the next page.", pageChoices[0]);
                throw new ArgumentException(currentPage.ToString());
            }

            // We will go to the page specifed in the pageChoice
            int pageToMoveTo = 1 + PropertyBagDictionary.Instance.GetProperty<int>(pageChoices[0]);
            SetupLogger.LogInfo("BackwardToPageBasedOnPropertyValueHandler: Property bag entry for {0} is {1}.  This means that we will move to a next page id {2}.", pageChoices[0], pageToMoveTo.ToString(), pageChoices[pageToMoveTo]);
            return PageRegistry.Instance.GetPage(pageChoices[pageToMoveTo]);
        }

        #endregion

        #region Navigation logic for the Start Page

        /// <summary>
        /// Forward from switch page to start page handler.
        /// 0 - ARPStartPageSwitch (Add/Remove)
        /// 1 - InitialInstallStartPageSwitch (Initial install)
        /// 2 - ConfigInitialInstallStartPageSwitch (VHD Configuration - Setup/Config scenario)
        /// 3 - UpgradeStartPageSwitch (Upgrade)
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page ForwardFromSwitchPageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            // default to the upgrade page.
            Page pageToReturn = PageRegistry.Instance.GetPage(pageChoices[0]);

            SetupLogger.LogInfo("ForwardFromSwitchPageHandler: Moving to page {0}", pageToReturn.Id);
            return pageToReturn;
        }

        /// <summary>
        /// Forwards from start page handler.
        /// It can go to:
        /// 0 - ComponentsPage, if everything is fine
        /// 1 - BlockPage, if an error occured, like initial requirements (.NET) are not there
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page ForwardFromStartPageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            // default to go to the ComponentsPage.
            Page pageToReturn = PageRegistry.Instance.GetPage(pageChoices[0]);

            // There is a reason to block
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.BlockReason))
            {
                SetupLogger.LogError("ForwardFromStartPageHandler: We need to block");
                pageToReturn = PageRegistry.Instance.GetPage(pageChoices[1]);
            }

            return pageToReturn;
        }

        #endregion

        #region Navigation logic for EulaPage

        /// <summary>
        /// Backwards from Eula page.
        /// It can go to:
        /// 0 - Components page
        /// 1 - Upgrade Components Page, if we are in upgrade path
        /// 2 - ARPAddRemoveComponentsPage, if we are in add/remove
        /// 3 - RegistrationPage, if server feature is selected.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page BackwardFromEulaPageHandler(Page currentPage)
        {
            string previousPageChoices = currentPage.PreviousPageArgument;
            string[] pageChoices = previousPageChoices.Split(new char[] { ',' });

            // default to the Components Page
            Page pageToReturn = PageRegistry.Instance.GetPage(pageChoices[0]);

            // If we are in the add/remove path
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AddRemoveMode))
            {
                SetupLogger.LogInfo("BackwardFromEulaPageHandler: Move back to addremove components page");
                pageToReturn = PageRegistry.Instance.GetPage(pageChoices[1]);
            }

            return pageToReturn;
        }

        /// <summary>
        /// Forwards from Eula page.
        /// It can move to:
        /// 0 - InstallationLocationPage, if server component is selected
        /// 1 - SelectWapDatabaseServerPage, if above is not true
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page ForwardFromEulaPageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            // default to just go to the InstallationLocationPage
            Page pageToReturn = PageRegistry.Instance.GetPage(pageChoices[0]);

            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon))
                {
                    pageToReturn = PageRegistry.Instance.GetPage(pageChoices[1]);
                }
                else
                {
                    // No customization if the common components are already installed
                    pageToReturn = PageRegistry.Instance.GetPage(pageChoices[2]);
                }

            }

            return pageToReturn;
        }

        #endregion

        #region Navigation logic for ComponentsPage

        /// <summary>
        /// Forwards from components page.
        /// It can move to:
        /// 0 - RegistrationPage, if server component is selected
        /// 1 - EulaPage, if above is not true
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page ForwardFromComponentsPageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            // default to just go to the EulaPage
            Page pageToReturn = PageRegistry.Instance.GetPage(pageChoices[0]);

            return pageToReturn;
        }

        #endregion

        #region Navigation logic for InstallationLocationPage

        /// <summary>
        /// Backwards from InstallationLocationPage.
        /// It can go to:
        /// EulaPage,MicrosoftUpdatePage,,CeipPageForAdminConsole,CeipPage
        /// 0 - EulaPage, default
        /// 1 - MicrosoftUpdatePage, if Microsoft Update option can be changed
        /// 2 - CeipPageForAdminConsole, if the above is not true, and client component is selected
        /// 3 - CeipPage, if above is not true and server component is selected
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page BackwardFromInstallationLocationPageHandler(Page currentPage)
        {
            string previousPageChoices = currentPage.PreviousPageArgument;
            string[] pageChoices = previousPageChoices.Split(new char[] { ',' });

            // default to go to the EulaPage
            Page pageToReturn = PageRegistry.Instance.GetPage(pageChoices[0]);

            return pageToReturn;
        }

        #endregion

        #region Navigation logic for ProgressPage

        /// <summary>
        /// Forwards from progress page handler.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page ForwardFromProgressPageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            return PageRegistry.Instance.GetPage(pageChoices[0]);
        }

        #endregion

        #region Navigation Logic for the Prereq View Page

        /// <summary>
        /// Prerequisite Page Handler for forward Navigation
        /// Forwards from PrerequisitesProgressPage.
        /// It can go to:
        /// 0 - PrerequisitesProgressPage
        /// 1 - AdditionalPrerequisitesPage
        /// 2 - ClusterConfigPage
        /// 3 - SelectDatabaseServerPage
        /// 4 - PortConfigurationPage
        /// 5 - WebPortalConfigurationPage
        /// </summary>
        /// <param name="currentPage">The current page</param>
        /// <returns>The page to navigate to</returns>
        public static Page ForwardFromPrereqsPageHandler(Page currentPage)
        {
            string[] pageChoices = currentPage.NextPageArgument.Split(new char[] { ',' });

            //TODO: Determine the next valid page based on the prerequisite check output

            return PageRegistry.Instance.GetPage(pageChoices[0]);
        }
        #endregion // Navigation Logic for the Prereq View Page

        #region Navigation Logic for the Database Configuration Page

        /// <summary>
        /// Select Database Server Page Handler for backward Navigation
        /// Backwards from SelectDatabaseServerPage.
        /// It can go to:
        /// 0 - InstallationLocationPage
        /// 1 - EulaPage
        /// </summary>
        /// <param name="currentPage">The current page</param>
        /// <returns>The page to navigate to</returns>
        public static Page BackwardFromSelectDatabaseServerPageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.PreviousPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            string InstallationLocationPage = pageChoices[0];
            string EulaPage = pageChoices[1];
            string targetPage = InstallationLocationPage;

            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                targetPage = EulaPage;
            }

            // Set default next page to InstallationLocationPage
            Page pageToReturn = PageRegistry.Instance.GetPage(targetPage);

            return pageToReturn;
        }

        /// <summary>
        /// Select Database Server Page Handler for backward Navigation
        /// Backwards from SelectDatabaseServerPage.
        /// It can go to:
        /// 0 - InstallationLocationPage
        /// 1 - MicrosoftUpdatePage
        /// 2 - CeipPage
        /// </summary>
        /// <param name="currentPage">The current page</param>
        /// <returns>The page to navigate to</returns>
        public static Page BackwardFromSelectWapDatabaseServerPageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.PreviousPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            // Default to going back to the eula
            String targetPage = pageChoices[0];

            // Set default next page to InstallationLocationPage
            Page pageToReturn = PageRegistry.Instance.GetPage(targetPage);

            return pageToReturn;
        }

        /// <summary>
        /// Database page Handler for forward Navigation
        /// forward from SelectDatabaseServerPage.
        /// </summary>
        /// <param name="currentPage">The current page</param>
        /// <returns>The page to navigate to</returns>
        public static Page ForwardFromSelectDatabaseServerPageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            String SelectWapDatabaseServerPage = pageChoices[0];
            String AccountConfigurationPage = pageChoices[1];

            // Set default next page to AccountConfigurationPage
            Page pageToReturn = PageRegistry.Instance.GetPage(AccountConfigurationPage);

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension) ||
                PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension))
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon))
                {
                    // Choose the WAP db only if the common components are being installed
                    pageToReturn = PageRegistry.Instance.GetPage(SelectWapDatabaseServerPage);
                }
            }

            return pageToReturn;
        }

        public static Page ForwardFromInstallationLocationPage(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            Page pageToReturn = PageRegistry.Instance.GetPage(pageChoices[0]);

            return pageToReturn;
        }

        /// <summary>
        /// Database page Handler for forward Navigation
        /// forward from SelectDatabaseServerPage.
        /// It can go to:
        /// 0 - AccountConfigurationPage
        /// </summary>
        /// <param name="currentPage">The current page</param>
        /// <returns>The page to navigate to</returns>
        public static Page ForwardFromSelectWapDatabaseServerPageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            // Set default next page to AccountConfigurationPage
            Page pageToReturn = PageRegistry.Instance.GetPage(pageChoices[0]);

            return pageToReturn;
        }
        #endregion

        #region Navigation Logic for the AccountConfigurationPage

        /// <summary>
        /// Moves back from Account Configuration page.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page BackwardFromAccountConfigurationPageHandler(Page currentPage)
        {
            string previousPageChoices = currentPage.PreviousPageArgument;
            string[] pageChoices = previousPageChoices.Split(new char[] { ',' });

            string SelectWapDatabaseServerPage = pageChoices[0];
            string SelectDatabaseServerPage = pageChoices[1];

            // Set default next page to SelectDatabaseServerPage
            Page pageToReturn = PageRegistry.Instance.GetPage(SelectDatabaseServerPage);

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon))
            {
                pageToReturn = PageRegistry.Instance.GetPage(SelectWapDatabaseServerPage);
            }

            return pageToReturn;
        }

        /// <summary>
        /// Forwards from Account Configuration page.
        /// It can go to:
        /// 0 - PortConfigurationPage, if we are installing Server or Administrator Console
        /// 1 - ReadyToInstallPage, if above is not true
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page ForwardAccountConfigurationPageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            Page pageToReturn = PageRegistry.Instance.GetPage(pageChoices[0]);

            return pageToReturn;
        }

        #endregion

        #region Navigation for the ReadyToInstall Page

        /// <summary>
        /// Moves back from ReadyToInstall page.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page BackwardFromReadyToInstallPage(Page currentPage)
        {
            string previousPageChoices = currentPage.PreviousPageArgument;
            string[] pageChoices = previousPageChoices.Split(new char[] { ',' });

            String AccountConfigurationPage = pageChoices[0];
            String ARPAddRemoveComponentsPage = pageChoices[1];
            String ARPRemoveDatabasePage = pageChoices[2];
            String ARPRemoveWAPDatabasePage = pageChoices[3];
            String EulaPage = pageChoices[4];

            // Set default next page to PortConfigurationPage
            Page pageToReturn = PageRegistry.Instance.GetPage(AccountConfigurationPage);

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                // If we are at the uninstall path
                // then we need to check whether we are uninstalling a standalone server
                // or the last node of HA VMM server
                // if that's the case, then we need to move back to Retain DB option page
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
                {
                    SetupLogger.LogInfo("BackwardFromReadyToInstallPage: Move to Remove Database Page");
                    pageToReturn = PageRegistry.Instance.GetPage(ARPRemoveDatabasePage);
                }
                else
                {
                    SetupLogger.LogInfo("BackwardFromReadyToInstallPage: Move to Add/Remove Components Page");
                    pageToReturn = PageRegistry.Instance.GetPage(ARPAddRemoveComponentsPage);
                }

                // Override the server database options when the extension is getting removed
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon))
                {
                    SetupLogger.LogInfo("BackwardFromReadyToInstallPage: Move to Remove WAP Database Page");
                    pageToReturn = PageRegistry.Instance.GetPage(ARPRemoveWAPDatabasePage);
                }
            }
            else
            {
                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
                {
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon))
                    {
                        pageToReturn = PageRegistry.Instance.GetPage(AccountConfigurationPage);
                    }
                    else
                    {
                        // No customization if the installation doesn't involve the WAP common components
                        pageToReturn = PageRegistry.Instance.GetPage(EulaPage);
                    }
                }
            }

            return pageToReturn;
        }

        #endregion

        #region Navigation logic for ARPAddRemoveComponentsPage

        /// <summary>
        /// Forwards from AddRemoveComponents page.
        /// It can go to:
        /// 0 - EulaPage, if we are on the install path
        /// 1 - ARPRemoveDatabasePage, if we are on the uninstall path and we are uninstalling server
        /// 2 - ARPRemoveWAPDatabasePage, if we are on the uninstall path and we are uninstalling extension
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page ForwardFromAddRemoveComponentsPageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            // Check to see if the length is correct
            if (pageChoices.Length != 3)
            {
                SetupLogger.LogError("ForwardFromAddRemoveComponentsPageHandler: Invalid XML description for Page");
                SetupLogger.LogError("XML Was {0}:", nextPageChoices);
                SetupLogger.LogError("XML should have been like: FirstPage,SecondPage,ThirdPage");
                throw new ArgumentException(currentPage.ToString());
            }
            String EulaPage = pageChoices[0];
            String ARPRemoveDatabasePage = pageChoices[1];
            String ARPRemoveWAPDatabasePage = pageChoices[2];

            // default to go to the EulaPage
            Page pageToReturn = PageRegistry.Instance.GetPage(EulaPage);

            // Check if we are on the uninstall path
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
                {
                    SetupLogger.LogInfo("ForwardFromAddRemoveComponentsPageHandler: Move to Remove Database Page");
                    pageToReturn = PageRegistry.Instance.GetPage(ARPRemoveDatabasePage);
                }
                else
                {
                    SetupLogger.LogInfo("ForwardFromAddRemoveComponentsPageHandler: Move to Remove WAP database Page");
                    pageToReturn = PageRegistry.Instance.GetPage(ARPRemoveWAPDatabasePage);
                }
            }
            else
            {
                SetupLogger.LogInfo("ForwardFromAddRemoveComponentsPageHandler: Move to Eula Page");
                pageToReturn = PageRegistry.Instance.GetPage(EulaPage);
            }

            return pageToReturn;
        }

        /// <summary>
        /// Forwards from RemoveDatabasePage page.
        /// It can go to:
        /// 0 - ReadyToInstallPage
        /// 1 - RemoveWAPDatabasePage, if we are on the uninstall path and we are uninstalling extensions
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page ForwardFromRemoveDatabasePageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.NextPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            // Check to see if the length is correct
            if (pageChoices.Length != 2)
            {
                SetupLogger.LogError("ForwardFromAddRemoveComponentsPageHandler: Invalid XML description for Page");
                SetupLogger.LogError("XML Was {0}:", nextPageChoices);
                SetupLogger.LogError("XML should have been like: FirstPage,SecondPage");
                throw new ArgumentException(currentPage.ToString());
            }
            String ReadyToInstallPage = pageChoices[0];
            String ARPRemoveWAPDatabasePage = pageChoices[1];

            // default to go to the ReadyToInstallPage
            Page pageToReturn = PageRegistry.Instance.GetPage(ReadyToInstallPage);

            // Check if we are on the uninstall path
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall) &&
                PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon))
            {
                SetupLogger.LogInfo("ForwardFromAddRemoveComponentsPageHandler: Move to Remove WAP Database Page");
                pageToReturn = PageRegistry.Instance.GetPage(ARPRemoveWAPDatabasePage);
            }

            return pageToReturn;
        }

        /// <summary>
        /// Moves backward from RemoveWAPDatabasePage page.
        /// It can go to:
        /// 0 - AddRemoveComponentsPage
        /// 1 - RemoveDatabasePage, if we are on the uninstall path and we are uninstalling server
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns>Page to move to</returns>
        public static Page BackwardFromRemoveWAPDatabasePageHandler(Page currentPage)
        {
            string nextPageChoices = currentPage.PreviousPageArgument;
            string[] pageChoices = nextPageChoices.Split(new char[] { ',' });

            // Check to see if the length is correct
            if (pageChoices.Length != 2)
            {
                SetupLogger.LogError("ForwardFromAddRemoveComponentsPageHandler: Invalid XML description for Page");
                SetupLogger.LogError("XML Was {0}:", nextPageChoices);
                SetupLogger.LogError("XML should have been like: FirstPage,SecondPage");
                throw new ArgumentException(currentPage.ToString());
            }
            String AddRemoveComponentsPage = pageChoices[0];
            String RemoveDatabasePage = pageChoices[1];

            // default to go to the ReadyToInstallPage
            Page pageToReturn = PageRegistry.Instance.GetPage(AddRemoveComponentsPage);

            // Check if we are on the uninstall path
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall) &&
                PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                SetupLogger.LogInfo("ForwardFromAddRemoveComponentsPageHandler: Move to Remove WAP Database Page");
                pageToReturn = PageRegistry.Instance.GetPage(RemoveDatabasePage);
            }

            return pageToReturn;
        }

        #endregion

        #region Private Helpers
        /// <summary>
        /// Returns the page in the registry indexed by the provided string or null.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <returns>
        /// the page in the registry indexed by the provided string.
        /// </returns>
        private static Page DefaultPage(string pageId)
        {
            return
                string.IsNullOrEmpty(pageId) ?
                    null : PageRegistry.Instance.GetPage(pageId);
        }

        /// <summary>
        /// Page Skipper for Forward or Backward Navigation
        /// </summary>
        /// <param name="currentPage">The current page</param>
        /// <param name="isForward">if set to <c>true</c>, looks up the forward page arguments</param>
        /// <returns>The page to navigate to</returns>
        private static Page SkipPageHandler(Page currentPage, bool isForward)
        {
            // Get the page choices 
            string pageArgument =
                isForward ? currentPage.NextPageArgument : currentPage.PreviousPageArgument;
            string[] pageChoices = pageArgument.Split(new char[] { ',' });

            // Check to see if the length is reasonable
            // This is our best guess at a reasonable length.  It the
            // code becomes more complex than this it will be unreadable.
            if (pageChoices.Length > 10)
            {
                SetupLogger.LogError("SkipPageHandler: Invalid XML: Exceeded arbitrary length of 10 choices.  Fix length or adjust hardcode in the function");
                throw new ArgumentException(currentPage.ToString());
            }

            // Browse the provided list of pages until we find one for which there is no 
            // SkipXXXXX property existing in the bag
            Page foundPage = null;
            for (int pageIndex = 0; pageIndex < pageChoices.Length; ++pageIndex)
            {
                Page page = PageRegistry.Instance.GetPage(pageChoices[pageIndex]);
                string pageImplName = page.PageUI.GetType().Name;
                string propertyName = string.Format("Skip{0}", pageImplName);
                if (!PropertyBagDictionary.Instance.PropertyExists(propertyName))
                {
                    foundPage = page;
                    break;
                }
            }

            // No page found: Trace and throw 
            if (null == foundPage)
            {
                SetupLogger.LogError("SkipPageHandler: Cant find a page to navigate to");
                throw new ArgumentException(currentPage.ToString());
            }

            return foundPage;
        }

        /// <summary>
        /// Gets the page choice.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="truePage">The page that will be returned if the property exists.</param>
        /// <param name="falsePage">The page that will be returned if the property does not exists.</param>
        /// <returns>Page to move to</returns>
        private static Page GetPageChoice(string property, string truePage, string falsePage)
        {
            Page page = null;
            if (PropertyBagDictionary.Instance.PropertyExists(property))
            {
                page = PageRegistry.Instance.GetPage(truePage);
            }
            else
            {
                page = PageRegistry.Instance.GetPage(falsePage);
            }

            return page;
        }

        private static Page DecideNextPageFromPrerequisitesPage(
            Page selectDatabaseServerPage,
            Page portSelectionPage,
            Page webPortalConfigurationPage)
        {
            Page returnPage = null;

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                returnPage = selectDatabaseServerPage;
            }
            else if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension))
            {
                returnPage = portSelectionPage;
            }
            else if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension))
            {
                returnPage = webPortalConfigurationPage;
            }

            return returnPage;
        }

        #endregion // Private Helpers
    }
}
