//-----------------------------------------------------------------------
// <copyright file="AccountConfigurationPage.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> This is the page that handles collecting and verifying the account information
//  </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    #region Using directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    using CMP.Setup.Helpers;
    using WpfResources;
    using CMP.Setup.SetupFramework;
    using System.Security.Cryptography.X509Certificates;

    #endregion

    /// <summary>
    /// Interaction logic for AccountConfigurationPage.xaml
    /// </summary>
    public partial class AccountConfigurationPage : BasePageForWpfControls
    {
        private String selectedCertificate = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountConfigurationPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public AccountConfigurationPage(CMP.Setup.SetupFramework.Page page)
            : base(page, WpfResources.WPFResourceDictionary.ConfigurationStepTitle, 3)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountConfigurationPage"/> class.
        /// </summary>
        public AccountConfigurationPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Exits the page.
        /// </summary>
        public override void ExitPage()
        {
            base.ExitPage();
        }

        /// <summary>
        /// Validates the page.
        /// The default implementation always returns true for 'validated'.
        /// </summary>
        /// <returns>true if valid</returns>
        public override bool ValidatePage()
        {
            bool isPageValid = true;

            try
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
                {
                    if (this.radioDomainAccount.IsChecked.GetValueOrDefault(false))
                    {
                        String fullUserName;

                        try
                        {
                            DnsHelper.CheckAndGetFullUserName(this.textBoxUserName.Text, out fullUserName);
                            fullUserName = this.textBoxUserName.Text;
                        }
                        catch
                        {
                            // Ignore failures
                            fullUserName = String.Empty;
                        }

                        string[] nameSplits = fullUserName.Split(SetupConstants.AccountDomainUserSeparator);
                        if (nameSplits.Length == 1)
                        {
                            SetupInputs.Instance.EditItem(SetupInputTags.CmpServiceDomainTag, String.Empty);
                            SetupInputs.Instance.EditItem(SetupInputTags.CmpServiceUserNameTag, nameSplits[0]);
                        }
                        else if (nameSplits.Length == 2)
                        {
                            SetupInputs.Instance.EditItem(SetupInputTags.CmpServiceDomainTag, nameSplits[0]);
                            SetupInputs.Instance.EditItem(SetupInputTags.CmpServiceUserNameTag, nameSplits[1]);
                        }
                        else
                        {
                            SetupInputs.Instance.EditItem(SetupInputTags.CmpServiceDomainTag, String.Empty);
                            SetupInputs.Instance.EditItem(SetupInputTags.CmpServiceUserNameTag, String.Empty);
                        }
                        SetupInputs.Instance.EditItem(
                            SetupInputTags.CmpServiceUserPasswordTag,
                            this.passwordBoxPassword.SecurePassword);

                        if (!this.TestCredential())
                        {
                            throw new Exception("Invalid service account");
                        }
                    }
                }

                // Only the certificate is selected for both server and extensions
                this.selectedCertificate = SetupInputs.Instance.FindItem(SetupInputTags.CmpCertificateThumbprintTag);
                this.certificateSelectionCombobox.Text = selectedCertificate;
            }
            catch (Exception backEndErrorException)
            {
                SetupLogger.LogException(backEndErrorException);
                SetupHelpers.ShowError(backEndErrorException.Message);

                isPageValid = false;
            }

            return isPageValid;
        }

        /// <summary>
        /// Enters the page
        /// </summary>
        public override void EnterPage()
        {
            SetupLogger.LogInfo("Enter Account configuration page.");

            base.EnterPage();

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.VMMSupportedVersionInstalled))
            {
                string serviceAccount = SetupConstants.VmmServiceAccount;
                if (String.Equals(serviceAccount, SetupConstants.LocalSystem, StringComparison.OrdinalIgnoreCase))
                {
                    this.radioLocalSystemAccount.IsChecked = true;
                }
                else
                {
                    this.radioDomainAccount.IsChecked = true;
                    this.textBoxUserName.Text = serviceAccount;

                    // If VMM service was already running under domain account
                    // don't let it be changed. We need the password though
                    this.radioLocalSystemAccount.IsEnabled = false;
                    this.textBoxUserName.IsEnabled = false;
                }
            }
            else
            {
                //if (!this.radioLocalSystemAccount.IsChecked.GetValueOrDefault(false))
                //{
                //    this.radioDomainAccount.IsChecked = true;
                //}
                this.radioLocalSystemAccount.IsChecked = true;
            }

            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                this.serviceAccountGrid.Height = 0;
            }
            else
            {
                this.serviceAccountGrid.Height = Double.NaN;
            }

            this.SetNextButtonState();
        }

        private void radioLocalSystemAccount_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HandleRadioButtonAccountChoiceChangeEvent();
            SetupInputs.Instance.EditItem(SetupInputTags.CmpServiceLocalAccountTag, true);
        }

        private void radioDomainAccount_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HandleRadioButtonAccountChoiceChangeEvent();
            SetupInputs.Instance.EditItem(SetupInputTags.CmpServiceLocalAccountTag, false);
        }

        private void buttonSelectADLocation_Click(object sender, RoutedEventArgs e)
        {
            //TODO: AD Location picker
        }

        private void ButtonTypeOfAccountClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(SetupConstants.TypeOfAccountLink));
            e.Handled = true;
        }

        private void HandleRadioButtonAccountChoiceChangeEvent()
        {
            if (this.radioDomainAccount.IsChecked.GetValueOrDefault(false))
            {
                this.textBoxUserName.IsEnabled = true;
                this.passwordBoxPassword.IsEnabled = true;
            }
            else
            {
                this.textBoxUserName.IsEnabled = false;
                this.passwordBoxPassword.IsEnabled = false;
            }

            this.SetNextButtonState();
        }

        private void SetNextButtonState()
        {
            bool isNextButtonEnabled = true;

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                // if domain account for service is selected, then username and password should be filled as well
                if (this.radioDomainAccount.IsChecked.GetValueOrDefault(false))
                {
                    if (String.IsNullOrEmpty(this.textBoxUserName.Text) ||
                        (this.passwordBoxPassword.SecurePassword.Length == 0))
                    {
                        isNextButtonEnabled = false;
                    }
                }
            }

            if (String.IsNullOrEmpty(this.certificateSelectionCombobox.Text))
            {
                isNextButtonEnabled = false;
            }

            this.Page.Host.SetNextButtonState(true, isNextButtonEnabled, null);
        }

        /// <summary>
        /// User name text is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void TextBox_UserNameChanged(
            object sender, 
            TextChangedEventArgs e)
        {
            this.passwordBoxPassword.Clear();
            this.SetNextButtonState();
        }

        /// <summary>
        /// Passwords is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PasswordBoxChanged(object sender, RoutedEventArgs e)
        {
            this.SetNextButtonState();
        }

        /// <summary>
        /// Tests the credential.
        /// </summary>
        /// <returns>true if valid credentials, false otherwise</returns>
        private bool TestCredential()
        {
            bool isValidUserAccount = true;

            if (this.radioDomainAccount.IsChecked.GetValueOrDefault(false))
            {
                isValidUserAccount = UserAccountHelper.ValidateServiceAccount();
            }

            return isValidUserAccount;
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


        private void certificateSelectionCombobox_DropDownOpened(object sender, EventArgs e)
        {
            Dictionary<string, string> certificateMap = new Dictionary<string, string>();
            try
            {
                // Get all certificates in the local certificate store
                var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                var certificates = store.Certificates;
                foreach (var certificate in certificates)
                {
                    string friendlyName = String.IsNullOrEmpty(certificate.FriendlyName) ? "No Name" : certificate.FriendlyName;
                    certificateMap[certificate.Thumbprint.ToString()] = String.Format("{0} ({1})", friendlyName, certificate.Thumbprint.ToString());
                }
                store.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                this.certificateSelectionCombobox.Text = String.Empty;
                this.certificateSelectionCombobox.Items.Clear();

                if (certificateMap.Keys.Count > 0)
                {
                    string firstCertificate = String.Empty;
                    foreach (KeyValuePair<string,string> certificate in certificateMap)
                    {
                        if (String.IsNullOrEmpty(firstCertificate))
                        {
                            firstCertificate = certificate.Key;
                        }
                        this.certificateSelectionCombobox.Items.Add(certificate.Value);
                    }

                    if (certificateMap.ContainsKey(this.selectedCertificate))
                    {
                        this.certificateSelectionCombobox.Text = certificateMap[selectedCertificate];
                    }
                    else
                    {
                        this.selectedCertificate = firstCertificate;
                        this.certificateSelectionCombobox.Text = certificateMap[this.selectedCertificate];
                    }
                }
            }

            this.SetNextButtonState();
        }

        private void certificateSelectionCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedCertificate = this.certificateSelectionCombobox.Text;
            string thumbprint = String.Empty;
            if (this.certificateSelectionCombobox.SelectedIndex != -1)
            {
                try
                {
                    // Get all certificates in the local certificate store
                    var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                    store.Open(OpenFlags.ReadOnly);
                    var certificates = store.Certificates;

                    if (certificates.Count > this.certificateSelectionCombobox.SelectedIndex)
                    {
                        thumbprint = certificates[this.certificateSelectionCombobox.SelectedIndex].Thumbprint.ToString();
                    }

                    store.Close();
                }
                catch (Exception)
                {
                }
            }
            if (!String.IsNullOrEmpty(thumbprint))
            {
                SetupInputs.Instance.EditItem(SetupInputTags.CmpCertificateThumbprintTag, thumbprint);
            }
            this.SetNextButtonState();
        }
    }
}
