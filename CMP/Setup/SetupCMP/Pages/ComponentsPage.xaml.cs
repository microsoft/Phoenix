//-----------------------------------------------------------------------
// <copyright file="ComponentsPage.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> vNext Manager Setup Components Page
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using System;
    using System.Windows;
    using System.Windows.Documents;
    using System.Diagnostics;
    using CMP.Setup.SetupFramework;
    using WpfResources;
    using CMP.Setup.Helpers;

    /// <summary>
    /// Interaction logic for ComponentsPage.xaml
    /// </summary>
    public partial class ComponentsPage : BasePageForWpfControls
    {
        private bool clientCheckBoxStatus = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentsPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public ComponentsPage(Page page)
            : base(page, WpfResources.WPFResourceDictionary.GettingStartedStepTitle, 1)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentsPage"/> class.
        /// </summary>
        public ComponentsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enters the page.
        /// </summary>
        public override void EnterPage()
        {
            base.EnterPage();

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                this.componentsPageHeader.Text = WpfResources.WPFResourceDictionary.SelectToRemove;
                this.checkBoxServer.IsEnabled = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ServerVersion);
                this.checkboxTenantExtension.IsEnabled = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension);
                this.checkboxAdminExtension.IsEnabled = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension);
                this.checkboxExtensionCommon.IsEnabled = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon);
            }
            else
            {
                this.checkBoxServer.IsChecked = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server);
                this.checkBoxServer.IsEnabled = !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ServerVersion);
                this.checkboxTenantExtension.IsChecked = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension);
                this.checkBoxServer.IsEnabled = !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtensionVersion);
                this.checkboxAdminExtension.IsEnabled = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension);
                this.checkboxExtensionCommon.IsEnabled = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon);
                this.checkBoxServer.IsEnabled = !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtensionVersion);

                if (!this.checkBoxServer.IsEnabled || !this.checkboxTenantExtension.IsEnabled || !this.checkboxAdminExtension.IsEnabled)
                {
                    this.componentsPageHeader.Text = WpfResources.WPFResourceDictionary.SelectToAdd;
                }
            }

            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ArchitectureIs64Check))
            {
                this.checkBoxServer.IsChecked = false;
                this.checkBoxServer.IsEnabled = false;
                this.serverDisableInfo.Visibility = Visibility.Visible;
                this.serverDisableInfo.Text = WPFResourceDictionary.OSBlockServer;
            }

            this.SetNextButtonState();
        }

        /// <summary>
        /// Exits the page.
        /// The default implementation writes the property bag.
        /// </summary>
        public override void ExitPage()
        {
            SetupHelpers.SetFeatureSwitches();
            // We always need the post install tasks so add them in
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.PostInstall, "1");
            SetupHelpers.RationalizeComponents();
            base.ExitPage();
        }

        /// <summary>
        /// Handles the Checked event of the CheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CheckBoxServerChanged(object sender, RoutedEventArgs e)
        {
            if (checkBoxServer.IsChecked.Value)
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.Server, "1");
            }
            else
            {
                PropertyBagDictionary.Instance.Remove(PropertyBagConstants.Server);
            }

            this.SetNextButtonState();
        }

        /// <summary>
        /// Handles the Checked and Unchecked events on the tenant extension check box
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void checkboxTenantExtensionChanged(object sender, RoutedEventArgs e)
        {
            if (checkboxTenantExtension.IsChecked.HasValue)
            {
                if (checkboxTenantExtension.IsChecked.Value)
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.TenantExtension, "1");
                }
                else
                {
                    PropertyBagDictionary.Instance.SafeRemove(PropertyBagConstants.TenantExtension);
                }
            }
            this.SetNextButtonState();
        }

        /// <summary>
        /// Handles the Checked and Unchecked events on the admin extension check box
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void checkboxAdminExtensionChanged(object sender, RoutedEventArgs e)
        {
            if (checkboxAdminExtension.IsChecked.HasValue)
            {
                if (checkboxAdminExtension.IsChecked.Value)
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AdminExtension, "1");
                }
                else
                {
                    PropertyBagDictionary.Instance.SafeRemove(PropertyBagConstants.AdminExtension);
                }
            }
            this.SetNextButtonState();
        }

        private void SetNextButtonState()
        {
            this.Page.Host.SetNextButtonState(true,
                checkBoxServer.IsChecked.Value || checkboxTenantExtension.IsChecked.Value || checkboxAdminExtension.IsChecked.Value || checkboxExtensionCommon.IsChecked.Value,
                null);
        }

        /// <summary>
        /// Handles the request navigate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleRequestNavigate(object sender, RoutedEventArgs e)
        {
            string navigateUri = ((Hyperlink)sender).NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }

        /// <summary>
        /// Preselects the switches.
        /// </summary>
        private void PreselectSwitches()
        {
            // Client
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtensionVersion))
            {
                this.checkboxTenantExtension.IsEnabled = false;
                this.checkboxTenantExtension.Content = String.Format(WpfResources.WPFResourceDictionary.AlreadyInstalled, this.checkboxTenantExtension.Content);
            }

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtensionVersion))
            {
                this.checkboxAdminExtension.IsEnabled = false;
                this.checkboxAdminExtension.Content = String.Format(WpfResources.WPFResourceDictionary.AlreadyInstalled, this.checkboxAdminExtension.Content);
            }

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommonVersion))
            {
                this.checkboxExtensionCommon.IsEnabled = false;
                this.checkboxExtensionCommon.Content = String.Format(WpfResources.WPFResourceDictionary.AlreadyInstalled, this.checkboxExtensionCommon.Content);
            }

            // Server
            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ServerVersion))
            {
                this.checkBoxServer.IsEnabled = false;
                this.checkBoxServer.Content = String.Format(WpfResources.WPFResourceDictionary.AlreadyInstalled, this.checkBoxServer.Content);
            }
        }
    }
}
