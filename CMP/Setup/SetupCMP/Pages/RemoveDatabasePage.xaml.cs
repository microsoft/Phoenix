//-----------------------------------------------------------------------
// <copyright file="RemoveDatabasePage.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Next Manager Setup PrerequisitesProgress Page
//           This is the page that handles prequisites checks progress.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Threading;
    using System.Xml;
    using CMP.Setup.SetupFramework;
    using WpfResources;
    using CMP.Setup.Helpers;

    /// <summary>
    /// Interaction logic for PrerequisitesProgressPage.xaml
    /// </summary>
    public partial class RemoveDatabasePage : BasePageForWpfControls
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisitesProgressPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public RemoveDatabasePage(Page page)
            : base(page, WPFResourceDictionary.ConfigurationStepTitle, 1)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisitesProgressPage"/> class.
        /// </summary>
        public RemoveDatabasePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enters the page.
        /// </summary>
        public override void EnterPage()
        {
            base.EnterPage();

            this.Page.Host.SetNextButtonState(true, true);
        }

        /// <summary>
        /// Exits the page.
        /// </summary>
        public override void ExitPage()
        {
            SetupInputs.Instance.EditItem(
                SetupInputTags.RetainSqlDatabaseTag, 
                this.radioRetainDatabase.IsChecked.GetValueOrDefault(false));

            SetupInputs.Instance.EditItem(SetupInputTags.SqlMachineNameTag, SetupConstants.SqlMachineName);

            String sqlInstanceInRegistry = SetupConstants.SqlInstanceName;
            // Sql Instance can be in the form of server\instance,port
            // Parse it accordingly
            String[] instanceList = sqlInstanceInRegistry.Split('\\');
            String sqlInstance = String.Empty;
            if (!String.IsNullOrEmpty(sqlInstanceInRegistry))
            {
                sqlInstance = (instanceList.Length == 2) ? instanceList[1] : instanceList[0];
            }

            // Now we can have a port attached to instance
            String[] sqlInfoList = sqlInstance.Split(',');
            int port = 0;
            if (sqlInfoList.Length == 2)
            {
                sqlInstance = sqlInfoList[0];
                try
                {
                    int.TryParse(sqlInfoList[1], out port);
                }
                catch (FormatException)
                {
                }
            }

            SetupInputs.Instance.EditItem(SetupInputTags.SqlInstanceNameTag, sqlInstance);
            SetupInputs.Instance.EditItem(SetupInputTags.SqlServerPortTag, port);

            base.ExitPage();
        }

        public override void OnApplyTemplate()
        {
            this.radioRetainDatabase.IsChecked = true;
            this.SetCredentialBlock();

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Validates the inputs on this page
        /// </summary>
        /// <returns></returns>
        public override bool ValidatePage()
        {
            bool isPageValid = true;
            try
            {
                if (this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false))
                {
                    String userName = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminNameTag);
                    String domainName = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminDomainTag);
                    if (!UserAccountHelper.ValidateCredentials(userName, domainName, this.passwordBoxPassword.SecurePassword))
                    {
                        throw new Exception("Either the domain account or the password you entered are not valid.");
                    }
                }
            }
            catch (Exception backEndErrorException)
            {
                SetupLogger.LogException(backEndErrorException);
                SetupHelpers.ShowError(backEndErrorException.Message);

                isPageValid = false;
            }
            return isPageValid;
        }


        #region Private Methods

        private void SetCredentialBlock()
        {
            bool doUseNewCredential = this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false);
            SetupInputs.Instance.EditItem(SetupInputTags.RemoteDatabaseImpersonationTag, doUseNewCredential);
            this.labelUserName.IsEnabled = doUseNewCredential;
            this.textBoxUserName.IsEnabled = doUseNewCredential;
            this.labelUserNameFormat.IsEnabled = doUseNewCredential;
            this.labelPassword.IsEnabled = doUseNewCredential;
            this.passwordBoxPassword.IsEnabled = doUseNewCredential;
        }

        private void SetNextButtonState()
        {
            bool isNextButtonEnabled = true;

            if (this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false))
            {
                isNextButtonEnabled = !String.IsNullOrEmpty(this.textBoxUserName.Text) &&
                    (this.passwordBoxPassword.SecurePassword.Length != 0);
            }

            this.Page.Host.SetNextButtonState(true, isNextButtonEnabled, null);
        }

        #endregion

        #region Event Handlers

        private void checkBoxcheckBoxNewUserId_CheckedChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SetCredentialBlock();
            this.SetNextButtonState();
        }

        private void textBoxUserName_LostFocus(object sender, EventArgs e)
        {
            try
            {
                String fullUserName = this.textBoxUserName.Text;

                string[] nameSplits = fullUserName.Split(SetupConstants.AccountDomainUserSeparator);
                if (nameSplits.Length == 1)
                {
                    SetupInputs.Instance.EditItem(SetupInputTags.SqlDBAdminDomainTag, String.Empty);
                    SetupInputs.Instance.EditItem(SetupInputTags.SqlDBAdminNameTag, nameSplits[0]);
                }
                else if (nameSplits.Length == 2)
                {
                    SetupInputs.Instance.EditItem(SetupInputTags.SqlDBAdminDomainTag, nameSplits[0]);
                    SetupInputs.Instance.EditItem(SetupInputTags.SqlDBAdminNameTag, nameSplits[1]);
                }
                else
                {
                    SetupInputs.Instance.EditItem(SetupInputTags.SqlDBAdminDomainTag, String.Empty);
                    SetupInputs.Instance.EditItem(SetupInputTags.SqlDBAdminNameTag, String.Empty);
                }
            }
            catch (Exception backEndErrorException)
            {
                SetupLogger.LogException(backEndErrorException);
                SetupHelpers.ShowError(backEndErrorException.Message);
            }

            this.SetNextButtonState();
        }

        private void passwordBox_PasswordChanged(object sender, EventArgs e)
        {
            try
            {
                SetupInputs.Instance.EditItem(SetupInputTags.SqlDBAdminPasswordTag, this.passwordBoxPassword.SecurePassword);
            }
            catch (Exception backEndErrorException)
            {
                SetupLogger.LogException(backEndErrorException);
                SetupHelpers.ShowError(backEndErrorException.Message);
            }
            
            this.SetNextButtonState();
        }
        #endregion
    }
}
