//-----------------------------------------------------------------------
// <copyright file="AddRemovePage.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> This is the page that provides add/remove choice
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using System;
    using System.Windows;
    using CMP.Setup.SetupFramework;

    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class AddRemovePage : BasePageForWpfControls
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddRemovePage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public AddRemovePage(Page page)
            : base(page, WpfResources.WPFResourceDictionary.GettingStartedStepTitle, 1)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddRemovePage"/> class.
        /// </summary>
        public AddRemovePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enum for ARP modes
        /// </summary>
        public enum AddRemoveProgramFilesModes
        {
            /// <summary>
            /// Adding component
            /// </summary>
            Add = 0,

            /// <summary>
            /// Removing component
            /// </summary>
            Remove = 1,
        }

        /// <summary>
        /// Enters the page.
        /// </summary>
        public override void EnterPage()
        {
            base.EnterPage();
            this.IntializeScreenState();
            this.Page.Host.SetNextButtonState(true, false, null);
        }

        /// <summary>
        /// Exits the page.
        /// </summary>
        public override void ExitPage()
        {
            base.ExitPage();
        }

        /// <summary>
        /// Intializes the state of the screen.
        /// </summary>
        private void IntializeScreenState()
        {
            bool canAddServer = !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ServerVersion) &&
                PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ArchitectureIs64Check);
            addComponent.IsEnabled = canAddServer ||
                !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtensionVersion) ||
                !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommonVersion) ||
                !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtensionVersion);
            removeComponent.IsEnabled = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ServerVersion) ||
                PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtensionVersion) ||
                PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommonVersion) ||
                PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtensionVersion);
            if (removeComponent.IsEnabled)
                message.Text = "Some CMP features are already installed on this machine.";
            else
                message.Text = "";
        }

        /// <summary>
        /// Clears all switches.
        /// </summary>
        private void ClearAllSwitches()
        {
            // Ok user launched but had stuff already installed. We will clear any install state switches
            // they passed or that we may have already set
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.Components);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.Server);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.CMPServer);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.MSDeploy);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.SQLSysClrTypes);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.SqlDom);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.SharedManagementObjects);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.DACFramework);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.TenantExtension);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.AdminExtension);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.ExtensionCommon);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.TenantWAPExtension);
            PropertyBagDictionary.Instance.Remove(PropertyBagConstants.AdminWAPExtension);
        }

        /// <summary>
        /// Handles the Click event of the addComponent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddComponent_Click(object sender, RoutedEventArgs e)
        {
            this.ClearAllSwitches();
            this.PreselectSwitches();
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AddRemoveMode, (int)AddRemoveProgramFilesModes.Add);
            PageNavigation.Instance.MoveToNextPage();
        }

        /// <summary>
        /// Handles the Click event of the removeComponent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveComponent_Click(object sender, RoutedEventArgs e)
        {
            this.ClearAllSwitches();
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.Uninstall, "1");
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AddRemoveMode, (int)AddRemoveProgramFilesModes.Remove);
            PageNavigation.Instance.MoveToNextPage();
        }

        /// <summary>
        /// Preselects the switches.
        /// </summary>
        private void PreselectSwitches()
        {
        }
    }
}
