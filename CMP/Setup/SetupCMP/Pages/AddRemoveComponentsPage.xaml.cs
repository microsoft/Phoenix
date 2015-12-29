//-----------------------------------------------------------------------
// <copyright file="AdditionalPrerequisitesPage.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> This is the page that handles add/remove components.
// </summary>
//-----------------------------------------------------------------------
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CMP.Setup.SetupFramework;
using WpfResources;

#endregion

namespace CMP.Setup
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class AddRemoveComponentsPage : BasePageForWpfControls
    {
        #region Members

        private bool tenantExtensionCheckBoxStatus = false;
        private bool adminExtensionCheckBoxStatus = false;
        private bool extensionCommonCheckBoxStatus = false;
        private bool serverCheckBoxStatus = false;

        private bool isServerCheckBoxAccessible = false;
        private bool isTenantExtensionCheckBoxAccessible = false;
        private bool isAdminExtensionCheckBoxAccessible = false;
        private bool isExtensionCommonCheckBoxAccessible = false;

        #endregion

        public AddRemoveComponentsPage(CMP.Setup.SetupFramework.Page page)
            : base(page, WpfResources.WPFResourceDictionary.GettingStartedStepTitle, 1)
        {
            InitializeComponent();
        }

        public AddRemoveComponentsPage()
        {
            InitializeComponent();
        }

        public override void EnterPage()
        {
            base.EnterPage();
            SetNextButtonState();
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                addComponents.Visibility = Visibility.Hidden;
                removeComponents.Visibility = Visibility.Visible;

                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ServerVersion))
                {
                    this.stackPanelServer.Visibility = Visibility.Visible;
                    this.isServerCheckBoxAccessible = true;
                }
                else
                {
                    this.stackPanelServer.Visibility = Visibility.Collapsed;
                    this.isServerCheckBoxAccessible = false;
                }

                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtensionVersion))
                {
                    this.stackPanelTenantExtension.Visibility = Visibility.Visible;
                    this.isTenantExtensionCheckBoxAccessible = true;
                }
                else
                {
                    this.stackPanelTenantExtension.Visibility = Visibility.Collapsed;
                    this.isTenantExtensionCheckBoxAccessible = false;
                }

                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtensionVersion))
                {
                    this.stackPanelAdminExtension.Visibility = Visibility.Visible;
                    this.isAdminExtensionCheckBoxAccessible = true;
                }
                else
                {
                    this.stackPanelAdminExtension.Visibility = Visibility.Collapsed;
                    this.isAdminExtensionCheckBoxAccessible = false;
                }

                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommonVersion))
                {
                    this.stackPanelExtensionCommon.Visibility = Visibility.Visible;
                    this.isExtensionCommonCheckBoxAccessible = true;
                }
                else
                {
                    this.stackPanelExtensionCommon.Visibility = Visibility.Collapsed;
                    this.isExtensionCommonCheckBoxAccessible = false;
                }

            }
            else // install path
            {
                addComponents.Visibility = Visibility.Visible;
                removeComponents.Visibility = Visibility.Hidden;

                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ServerVersion))
                {
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ArchitectureIs64Check))
                    {
                        this.isServerCheckBoxAccessible = true;
                    }
                    else
                    {
                        this.stackPanelServer.IsEnabled = false;
                        this.isServerCheckBoxAccessible = false;
                        this.serverDisableInfo.Visibility = Visibility.Visible;
                        if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ArchitectureIs64Check))
                        {
                            this.serverDisableInfo.Text = WPFResourceDictionary.AgentBlockServer;
                        }
                        else
                        {
                            this.serverDisableInfo.Text = WPFResourceDictionary.OSBlockServer;
                        }
                    }
                }
                else
                {
                    this.stackPanelServer.Visibility = Visibility.Collapsed;
                    this.isServerCheckBoxAccessible = false;
                }

                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtensionVersion))
                {
                    this.stackPanelTenantExtension.Visibility = Visibility.Visible;
                    this.isTenantExtensionCheckBoxAccessible = true;
                }
                else
                {
                    this.stackPanelTenantExtension.Visibility = Visibility.Collapsed;
                    this.isTenantExtensionCheckBoxAccessible = false;
                }

                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtensionVersion))
                {
                    this.stackPanelAdminExtension.Visibility = Visibility.Visible;
                    this.isAdminExtensionCheckBoxAccessible = true;
                }
                else
                {
                    this.stackPanelAdminExtension.Visibility = Visibility.Collapsed;
                    this.isAdminExtensionCheckBoxAccessible = false;
                }

                if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommonVersion))
                {
                    this.stackPanelExtensionCommon.Visibility = Visibility.Visible;
                    this.isExtensionCommonCheckBoxAccessible = true;
                }
                else
                {
                    this.stackPanelExtensionCommon.Visibility = Visibility.Collapsed;
                    this.isExtensionCommonCheckBoxAccessible = false;
                }
            }
        }

        public override void ExitPage()
        {
            SetupHelpers.SetFeatureSwitches();
            // We always need the post install tasks so add them in
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.PostInstall, "1");
            SetupHelpers.RationalizeComponents();

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
                {
                    PropertyBagDictionary.Instance.SafeAdd("ARPComponentsNextPage", "0");
                }
                else
                {
                    PropertyBagDictionary.Instance.SafeAdd("ARPComponentsNextPage", "1");
                }
            }
            else
            {
                PropertyBagDictionary.Instance.SafeAdd("ARPComponentsNextPage", "2");
            }

            base.ExitPage();
        }

        private void SetNextButtonState()
        {
            if (checkBoxServer.IsChecked.Value || checkboxTenantExtension.IsChecked.Value || checkboxAdminExtension.IsChecked.Value)
            {
                this.Page.Host.SetNextButtonState(true, true, null);
            }
            else
            {
                this.Page.Host.SetNextButtonState(true, false, null);
            }
        }

        private void CheckBoxServerChanged(object sender, RoutedEventArgs e)
        {
            if (checkBoxServer.IsChecked.Value)
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.Server, "1");

            }
            else
            {
                PropertyBagDictionary.Instance.Remove(PropertyBagConstants.Server);
                // if this is an installation, reset the client checkbox to its last know status
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    this.serverCheckBoxStatus = false;
                }
            }

            SetNextButtonState();
        }

        private void checkboxTenantExtensionChanged(object sender, RoutedEventArgs e)
        {
            if (checkboxTenantExtension.IsChecked.Value)
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.TenantExtension, "1");

                // Check common components if uninstalling, and the other extension is being uninstalled
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension) ||
                        !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtensionVersion))
                    {
                        checkboxExtensionCommon.IsChecked = true;

                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ExtensionCommon, "1");
                    }
                }
                else
                {
                    checkboxExtensionCommon.IsChecked = true;

                    if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommonVersion))
                    {
                        // Install the common components if they are not already installed
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ExtensionCommon, "1");
                    }
                }
            }
            else
            {
                PropertyBagDictionary.Instance.Remove(PropertyBagConstants.TenantExtension);

                // Uncheck common components if uninstalling, and the other extension is being uninstalled
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    // Since the extension will be left, the common components must be left
                    checkboxExtensionCommon.IsChecked = false;
                    PropertyBagDictionary.Instance.Remove(PropertyBagConstants.ExtensionCommon);
                }
                else if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension))
                {
                    // Do not install the common components when neither extension type is going to be installed
                    checkboxExtensionCommon.IsChecked = false;
                    PropertyBagDictionary.Instance.Remove(PropertyBagConstants.ExtensionCommon);
                }
            }

            SetNextButtonState();
        }

        private void checkboxAdminExtensionChanged(object sender, RoutedEventArgs e)
        {
            if (checkboxAdminExtension.IsChecked.Value)
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AdminExtension, "1");

                // Check common components if uninstalling, and the other extension is being uninstalled
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension) ||
                        !PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtensionVersion))
                    {
                        checkboxExtensionCommon.IsChecked = true;
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ExtensionCommon, "1");
                    }
                }
                else
                {
                    checkboxExtensionCommon.IsChecked = true;

                    if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommonVersion))
                    {
                        // Install the common components if they are not already installed
                        PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.ExtensionCommon, "1");
                    }
                }
            }
            else
            {
                PropertyBagDictionary.Instance.Remove(PropertyBagConstants.AdminExtension);

                // Uncheck common components if uninstalling, and the other extension is being uninstalled
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    // Since the extension will be left, the common components must be left
                    checkboxExtensionCommon.IsChecked = false;
                    PropertyBagDictionary.Instance.Remove(PropertyBagConstants.ExtensionCommon);
                }
                else if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension))
                {
                    // Do not install the common components when neither extension type is going to be installed
                    checkboxExtensionCommon.IsChecked = false;
                    PropertyBagDictionary.Instance.Remove(PropertyBagConstants.ExtensionCommon);
                }
            }

            SetNextButtonState();
        }

        private void HandleRequestNavigate(object sender, RoutedEventArgs e)
        {
            string navigateUri = ((Hyperlink)sender).NavigateUri.ToString();
            SetupHelpers.ShowHelpFileTopic(navigateUri);
            e.Handled = true;
        }

    }
}
