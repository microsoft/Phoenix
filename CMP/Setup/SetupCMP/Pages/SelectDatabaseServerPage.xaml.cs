//-----------------------------------------------------------------------
// <copyright file="SelectDatabaseServerPage.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Select Database Server Page
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    #region Using directives

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.Diagnostics;
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

    using CMP.Setup.SetupFramework;
    using WpfResources;
    using CMP.Setup.Helpers;

    #endregion Using directives

    /// <summary>
    /// Select Database Server Page
    /// </summary>
    public partial class SelectDatabaseServerPage : BasePageForWpfControls
    {
        #region Members

        private String serverName = String.Empty;
        private int port = 0;
        private String selectedInstance = String.Empty;
        private String selectedDatabase = String.Empty;
        private static string _sqlMachineName = String.Empty;
        public static string sqlMachineName
        {
            get
            {
                return _sqlMachineName;
            }
            private set
            {
                _sqlMachineName = value;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the SelectDatabaseServerPage class.
        /// </summary>
        public SelectDatabaseServerPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the SelectDatabaseServerPage class.
        /// </summary>
        /// <param name="page">Page</param>
        public SelectDatabaseServerPage(CMP.Setup.SetupFramework.Page page)
            : base(page, WpfResources.WPFResourceDictionary.ConfigurationStepTitle, 3)
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// EnterPage
        /// </summary>
        public override void EnterPage()
        {
            base.EnterPage();
            //this.checkBoxNewUserId.IsVisible = false;
            this.SetCredentialBlock();
            this.SetNextButtonState();

            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                this.radioExistingDatabase.IsChecked = true;
                this.radioNewDatabase.IsEnabled = false;
            }
            else
            {
                this.radioNewDatabase.IsEnabled = true;
            }
        }

        /// <summary>
        /// ExitPage
        /// </summary>
        public override void ExitPage()
        {
            base.ExitPage();
        }

        public override void OnApplyTemplate()
        {
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.VMMSupportedVersionInstalled))
            {
                if (SetupConstants.DBOnRemoteServer)
                {
                    this.textBoxServer.Text = SetupConstants.SqlMachineName;
                }
                else
                {
                    this.textBoxServer.Text = Environment.MachineName;
                }

                this.comboBoxInstance.Text = SetupConstants.SqlInstanceName;
                this.resetInstanceName();

                this.radioExistingDatabase.IsChecked = true;
                this.comboBoxExistingDatabaseName.Text = SetupConstants.DBName;

                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.BackupSqlDatabase, true);

                this.DisableControls();
                // this.stackPanelBackupDatabase.Visibility = Visibility.Visible;
            }
            else
            {
                this.serverName = SetupInputs.Instance.FindItem(SetupInputTags.SqlMachineNameTag);
                this.textBoxServer.Text = this.serverName;

                this.port = (int)SetupInputs.Instance.FindItem(SetupInputTags.SqlServerPortTag);
                String sqlPortAsString = "1433";

                if (this.port != InputDefaults.SqlServerPort)
                {
                    sqlPortAsString = Convert.ToString(this.port);
                }
                this.textBoxPort.Text = sqlPortAsString;

                this.selectedDatabase = SetupInputs.Instance.FindItem(SetupInputTags.SqlDatabaseNameTag);

                if (!this.comboBoxExistingDatabaseName.Items.Contains(this.selectedDatabase))
                {
                    this.radioNewDatabase.IsChecked = true;
                    this.textBoxNewDatabaseName.Text = this.selectedDatabase;
                }
                else
                {
                    this.comboBoxExistingDatabaseName.Text = selectedDatabase;
                    this.radioExistingDatabase.IsChecked = true;
                }
            }

            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                this.radioExistingDatabase.IsChecked = true;
                this.radioNewDatabase.IsEnabled = true;
            }
            else
            {
                this.radioNewDatabase.IsEnabled = true;
            }

            base.OnApplyTemplate();

            this.SetNextButtonState();
        }


        private void SetPort()
        {
            this.textBoxPort.Text = (this.port != InputDefaults.SqlServerPort) ? this.port.ToString() : String.Empty;
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
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                SetupInputs.Instance.EditItem(SetupInputTags.SqlMachineNameTag, this.textBoxServer.Text);
                SetupInputs.Instance.EditItem(SetupInputTags.SqlInstanceNameTag,
                    IsDefaultInstance(this.comboBoxInstance.Text) ? String.Empty : this.comboBoxInstance.Text);

                this.port = String.IsNullOrEmpty(this.textBoxPort.Text) ?
                    InputDefaults.SqlServerPort : Convert.ToInt32(this.textBoxPort.Text);
                SetupInputs.Instance.EditItem(SetupInputTags.SqlServerPortTag, this.port);

                bool isNewDB = this.radioNewDatabase.IsChecked.GetValueOrDefault(false);

                SetupInputs.Instance.EditItem(SetupInputTags.CreateNewSqlDatabaseTag, isNewDB);

                if (isNewDB)
                {
                    SetupInputs.Instance.EditItem(SetupInputTags.SqlDatabaseNameTag, this.textBoxNewDatabaseName.Text);
                }
                else
                {
                    SetupInputs.Instance.EditItem(SetupInputTags.SqlDatabaseNameTag, this.comboBoxExistingDatabaseName.Text);
                }

                if (this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false))
                {
                    String userName = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminNameTag);
                    String domainName = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminDomainTag);
                    if (!UserAccountHelper.ValidateCredentials(userName, domainName, this.passwordBoxPassword.SecurePassword))
                    {
                        throw new Exception("Either the domain account or the password you entered are not valid.");
                    }
                }

                String fullInstanceName = SetupDatabaseHelper.ConstructFullInstanceName(
                    !SetupDatabaseHelper.SqlServerIsOnLocalComputer(this.serverName),
                    (String)SetupInputs.Instance.FindItem(SetupInputTags.SqlMachineNameTag),
                    (String)SetupInputs.Instance.FindItem(SetupInputTags.SqlInstanceNameTag),
                    (int)SetupInputs.Instance.FindItem(SetupInputTags.SqlServerPortTag));

                SetupDatabaseHelper.CheckDatabase(
                    fullInstanceName,
                    (String)SetupInputs.Instance.FindItem(SetupInputTags.GetSqlDatabaseNameTag(false)),
                    false);
            }
            catch (SqlException)
            {
                Exception exception = new Exception("Setup cannot connect to the specified SQL Server instance.");

                SetupLogger.LogException(exception);
                SetupHelpers.ShowError(exception.Message);

                isPageValid = false;
            }
            catch (Exception exception)
            {
                SetupLogger.LogException(exception);
                SetupHelpers.ShowError(exception.Message);

                isPageValid = false;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

            return isPageValid;
        }

        #region Event Handlers

        private void textBoxServer_LostFocus(Object sender, EventArgs e)
        {
            this.resetServerName();
            this.PopulateInstancesAsync();
        }

        private void comboBoxInstance_LostFocus(Object sender, EventArgs e)
        {
            this.resetInstanceName();
        }

        private void textBoxPort_LostFocus(Object sender, EventArgs e)
        {
            int enteredPort = String.IsNullOrEmpty(this.textBoxPort.Text) ?
                InputDefaults.SqlServerPort : Convert.ToInt32(this.textBoxPort.Text);

            if (this.port != enteredPort)
            {
                this.port = enteredPort;
            }
        }

        /// <summary>
        /// Texts the box user name text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void textBoxPort_Changed(
            object sender,
            TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox != null)
            {
                string port = textBox.Text;

                int index = 0;
                while (port.Length > index)
                {
                    if (!Char.IsNumber(port, index))
                    {
                        port = port.Remove(index, 1);
                    }
                    else
                    {
                        index++;
                    }
                }
                textBox.Text = port;
                textBox.Select(textBox.Text.Length, 0);
            }

            this.SetNextButtonState();
        }

        private void textBoxServer_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            this.SetNextButtonState();
        }

        private void textBoxNewDatabaseName_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            this.SetNextButtonState();
        }

        private void textBoxUserName_LostFocus(object sender, EventArgs e)
        {
            try
            {
                String fullUserName = String.Empty;
                try
                {
                    DnsHelper.CheckAndGetFullUserName(this.textBoxUserName.Text, out fullUserName);
                    fullUserName = this.textBoxUserName.Text;
                }
                catch
                {
                    // Ignore the exception
                }

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

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
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

        private void passwordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false) &&
                    !string.IsNullOrEmpty(this.textBoxUserName.Text.Trim()))
                {
                    String userName = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminNameTag);
                    String domainName = SetupInputs.Instance.FindItem(SetupInputTags.SqlDBAdminDomainTag);
                    if (!UserAccountHelper.ValidateCredentials(userName, domainName, this.passwordBoxPassword.SecurePassword))
                    {
                        throw new Exception("Either the domain account or the password you entered are not valid");
                    }
                }
            }
            catch (Exception backEndErrorException)
            {
                SetupLogger.LogException(backEndErrorException);
                SetupHelpers.ShowError(backEndErrorException.Message);
            }
        }

        private void radioNewDatabase_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.EnableDisableDatabaseOptionsControls();
            this.SetNextButtonState();
        }

        private void radioExistingDatabase_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.EnableDisableDatabaseOptionsControls();
            this.SetNextButtonState();
        }

        private void checkBoxcheckBoxNewUserId_CheckedChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SetupInputs.Instance.EditItem(SetupInputTags.RemoteDatabaseImpersonationTag, this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false));
            this.SetCredentialBlock();
            this.SetNextButtonState();
        }

        private void comboBoxInstance_DropDownOpened(object sender, EventArgs e)
        {
            this.DisableInputMode();
            String[] instanceArray = null;
            try
            {
                this.resetServerName();
                if (!String.IsNullOrEmpty(this.serverName))
                {
                    using (ImpersonationHelper impersonationHelper = new ImpersonationHelper())
                    {
                        //PropertyBagDictionary.Instance.SafeAdd("SQLMachineName", this.serverName);
                        instanceArray = SetupDatabaseHelper.GetSqlInstanceNames(this.serverName);
                    }
                }
                this.comboBoxInstance.Text = String.Empty;
                this.comboBoxInstance.Items.Clear();
                if (instanceArray != null && instanceArray.Length > 0)
                {
                    foreach (String instanceName in instanceArray)
                    {
                        this.comboBoxInstance.Items.Add(instanceName);
                    }

                    if (this.comboBoxInstance.Items.Contains(this.selectedInstance))
                    {
                        this.comboBoxInstance.Text = this.selectedInstance;
                    }
                    else
                    {
                        this.selectedInstance = instanceArray[0];
                        this.comboBoxInstance.Text = this.selectedInstance;
                    }
                }
            }
            catch (Exception backEndErrorException)
            {
                SetupLogger.LogException(backEndErrorException);
                SetupHelpers.ShowError(backEndErrorException.Message);
            }

            this.EnableInputMode();
        }

        private void comboBoxExistingDatabaseName_DropDownOpened(object sender, EventArgs e)
        {
            this.DisableInputMode();
            this.resetServerName();
            this.resetInstanceName();

            String[] databaseArray = null;
            try
            {
                if (!String.IsNullOrEmpty(this.serverName))
                {
                    using (ImpersonationHelper impersonationHelper = new ImpersonationHelper())
                    {
                        databaseArray = SetupDatabaseHelper.GetSqlDBNames(
                            !SetupDatabaseHelper.SqlServerIsOnLocalComputer(this.serverName),
                            this.serverName,
                            IsDefaultInstance(this.selectedInstance) ? String.Empty : this.selectedInstance,
                            this.port,
                            false);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                this.EnableInputMode();
                this.comboBoxExistingDatabaseName.Text = String.Empty;
                this.comboBoxExistingDatabaseName.Items.Clear();

                if (databaseArray != null && databaseArray.Length > 0)
                {
                    foreach (String databaseName in databaseArray)
                    {
                        this.comboBoxExistingDatabaseName.Items.Add(databaseName);
                    }

                    // First satisfy the below scenario:
                    // - User clicked on existing db radio button and selected a db
                    // - then clicked new db radio button 
                    // - and then clicked existing radio db again w/o changin server, instance, or port info
                    // Basically, check if the selected instance already exists in the list
                    // if yes, then choose it
                    // otherwise, this is a new population, select the first item
                    if (this.comboBoxExistingDatabaseName.Items.Contains(this.selectedDatabase))
                    {
                        this.comboBoxExistingDatabaseName.Text = selectedDatabase;
                    }
                    else
                    {
                        this.selectedDatabase = databaseArray[0];
                        this.comboBoxExistingDatabaseName.Text = this.selectedDatabase;
                    }
                }
            }
        }

        private void checkBoxBackupDatabase_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (this.checkBoxBackupDatabase.IsChecked.GetValueOrDefault(false))
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.BackupSqlDatabase, true);
            }
            else
            {
                PropertyBagDictionary.Instance.SafeRemove(PropertyBagConstants.BackupSqlDatabase);
            }
        }

        private void comboBoxExistingDatabaseName_Changed(Object sender, EventArgs e)
        {
            this.SetNextButtonState();
        }

        #endregion

        #region Private Methods

        private void SetCredentialBlock()
        {
            //commented for this release
            /*this.labelUserName.IsEnabled = this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false);
            this.textBoxUserName.IsEnabled = this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false);
            this.labelUserNameFormat.IsEnabled = this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false);
            this.labelPassword.IsEnabled = this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false);
            this.passwordBoxPassword.IsEnabled = this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false);
            if (!this.passwordBoxPassword.IsEnabled)
            {
                this.passwordBoxPassword.Clear();
            }
            */
            if (this.radioExistingDatabase.IsChecked.GetValueOrDefault())
            {
                this.checkBoxNewUserId.IsEnabled = true;
            }
               

            if (this.radioExistingDatabase.IsChecked.GetValueOrDefault() && this.checkBoxNewUserId.IsChecked.GetValueOrDefault())
            {
                this.comboBoxExistingDatabaseName.IsEnabled = true;
                this.labelUserName.IsEnabled = true;
                this.textBoxUserName.IsEnabled = true;
                this.labelUserNameFormat.IsEnabled = true;
                this.labelPassword.IsEnabled = true;
                this.passwordBoxPassword.IsEnabled = true;
            }
            else
            {
                this.comboBoxExistingDatabaseName.IsEnabled = false;
                this.labelUserName.IsEnabled = false;
                this.textBoxUserName.IsEnabled = false;
                this.labelUserNameFormat.IsEnabled = false;
                this.labelPassword.IsEnabled = false;
                this.passwordBoxPassword.IsEnabled = false;
            }
            

        }

        private void PopulateInstancesAsync()
        {
            this.DisableInputMode();
            BackgroundWorker backGroundWorker = new BackgroundWorker();
            backGroundWorker.DoWork += new DoWorkEventHandler(this.PopulateInstanceItemsWorker);
            backGroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(this.PopulateInstanceItemsWorkerCompleted);

            backGroundWorker.RunWorkerAsync();
        }

        private void PopulateDatabasesAsync()
        {
            this.DisableInputMode();
            BackgroundWorker backGroundWorker = new BackgroundWorker();
            backGroundWorker.DoWork += new DoWorkEventHandler(this.PopulateDatabaseNameItemsWorker);
            backGroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(this.PopulateDatabaseNameItemsWorkerCompleted);

            backGroundWorker.RunWorkerAsync();
        }

        private void DisableInputMode()
        {
            this.stackPanelCommunicatingSqlServer.Visibility = Visibility.Visible;
            this.progressBarCommunicatingSqlServer.IsIndeterminate = true;

            this.DisableControls();

            this.Page.Host.SetNextButtonState(true, false, null);
        }

        private void DisableControls()
        {
            this.textBoxServer.IsEnabled = false;
            this.textBoxPort.IsEnabled = false;
            this.comboBoxInstance.IsEnabled = false;
            this.textBoxNewDatabaseName.IsEnabled = false;
            this.comboBoxExistingDatabaseName.IsEnabled = false;

            this.radioNewDatabase.IsEnabled = false;
            this.radioExistingDatabase.IsEnabled = false;
        }

        private void EnableInputMode()
        {
            this.progressBarCommunicatingSqlServer.IsIndeterminate = false;
            this.stackPanelCommunicatingSqlServer.Visibility = Visibility.Hidden;

            this.EnableControls();

            this.SetNextButtonState();
        }

        private void EnableControls()
        {
            this.textBoxServer.IsEnabled = true;
            this.textBoxPort.IsEnabled = true;
            this.comboBoxInstance.IsEnabled = true;

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                this.radioNewDatabase.IsEnabled = true;
            }

            this.radioExistingDatabase.IsEnabled = true;
            this.EnableDisableDatabaseOptionsControls();
        }

        private void resetServerName()
        {
            string[] nameSplits = this.textBoxServer.Text.Split(SetupConstants.AccountDomainUserSeparator);
            if (nameSplits.Length == 2)
            {
                // Get the Instance name
                this.selectedInstance = nameSplits[1];

                // Remove instance name from Server  text box
                this.textBoxServer.Text = nameSplits[0];
            }

            if (!String.Equals(this.serverName, this.textBoxServer.Text, StringComparison.OrdinalIgnoreCase))
            {
                this.serverName = this.textBoxServer.Text;
            }
            sqlMachineName = this.serverName;

        }

        private void resetInstanceName()
        {
            // During upgrade, we set the instance value as INSTANCE,PORT
            // We need to split these two
            string originalText = this.comboBoxInstance.Text;

            if (originalText.Contains(","))
            {
                string[] splitValues = originalText.Split(',');
                this.comboBoxInstance.Text = splitValues[0];
                if (int.TryParse(splitValues[1], out this.port))
                {
                    this.SetPort();
                }
            }

            if (!String.Equals(this.selectedInstance, this.comboBoxInstance.Text, StringComparison.OrdinalIgnoreCase))
            {
                this.selectedInstance = this.comboBoxInstance.Text;
            }
        }

        private bool IsDefaultInstance(String instanceName)
        {
            int result = String.Compare(instanceName, SetupConstants.SqlServerDefaultInstanceName, StringComparison.InvariantCultureIgnoreCase);
            return (result == 0);
        }

        private void EnableDisableDatabaseOptionsControls()
        {
            this.textBoxNewDatabaseName.IsEnabled = this.radioNewDatabase.IsChecked.GetValueOrDefault(false);
            this.comboBoxExistingDatabaseName.IsEnabled = this.radioExistingDatabase.IsChecked.GetValueOrDefault(false);
            this.checkBoxNewUserId.IsEnabled = this.radioExistingDatabase.IsChecked.GetValueOrDefault(true);;
        }

        private void SetNextButtonState()
        {
            bool isNextButtonEnabled = true;

            if (String.IsNullOrEmpty(this.textBoxServer.Text))
            {
                isNextButtonEnabled = false;
            }

            if (this.radioNewDatabase.IsChecked.GetValueOrDefault(false))
            {
                if (String.IsNullOrEmpty(this.textBoxNewDatabaseName.Text))
                {
                    isNextButtonEnabled = false;
                }
            }
            else
            {
                if (String.IsNullOrEmpty(this.comboBoxExistingDatabaseName.Text))
                {
                    isNextButtonEnabled = false;
                }
            }

            if (isNextButtonEnabled && this.checkBoxNewUserId.IsChecked.GetValueOrDefault(false))
            {
                isNextButtonEnabled = !String.IsNullOrEmpty(this.textBoxUserName.Text) && (this.passwordBoxPassword.SecurePassword.Length != 0);
            }

            this.Page.Host.SetNextButtonState(true, isNextButtonEnabled, null);
            this.resetServerName();

        }

        #endregion

        #region Event handlers to populate SQL related data

        private void PopulateInstanceItemsWorker(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backgroundWorker = sender as BackgroundWorker;
            if (backgroundWorker == null)
            {
                return;
            }

            String[] instanceArray = null;

            if (!String.IsNullOrEmpty(this.serverName))
            {
                using (ImpersonationHelper impersonationHelper = new ImpersonationHelper())
                {
                    instanceArray = SetupDatabaseHelper.GetSqlInstanceNames(this.serverName);
                }
            }

            e.Result = instanceArray;
        }

        private void PopulateInstanceItemsWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.comboBoxInstance.Items.Clear();
            this.EnableInputMode();

            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                SetupLogger.LogError("Instance population failed");
            }
            else if (e.Cancelled)
            {
                SetupLogger.LogError("Instance population has been cancelled.");
            }
            else
            {
                // Handle the case where the operation succeeded.
                String[] instanceNames = e.Result as String[];
                if (instanceNames != null && instanceNames.Length > 0)
                {
                    this.DisableInputMode();

                    foreach (String instanceName in instanceNames)
                    {
                        this.comboBoxInstance.Items.Add(instanceName);
                    }

                    // If this.selectedInstance is not present in combo box,
                    // then set the first item as selected instance
                    if (string.IsNullOrEmpty(this.selectedInstance)
                        || !this.comboBoxInstance.Items.Contains(this.selectedInstance))
                    {
                        this.selectedInstance = instanceNames[0];
                    }

                    // Set the selected instance name as the combo box text
                    this.comboBoxInstance.Text = this.selectedInstance;

                    // Trigger the database population
                    this.PopulateDatabasesAsync();
                }
                else
                {
                    this.comboBoxExistingDatabaseName.Items.Clear();
                    this.comboBoxExistingDatabaseName.Text = String.Empty;
                }
            }
        }
        private void PopulateDatabaseNameItemsWorker(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backgroundWorker = sender as BackgroundWorker;
            if (backgroundWorker == null)
            {
                return;
            }

            String[] databaseArray = null;

            try
            {
                if (!String.IsNullOrEmpty(this.serverName))
                {
                    using (ImpersonationHelper impersonationHelper = new ImpersonationHelper())
                    {
                        databaseArray = SetupDatabaseHelper.GetSqlDBNames(
                            !SetupDatabaseHelper.SqlServerIsOnLocalComputer(this.serverName),
                            this.serverName,
                            IsDefaultInstance(this.selectedInstance) ? String.Empty : this.selectedInstance,
                            this.port,
                            false);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                e.Result = databaseArray;
            }
        }

        private void PopulateDatabaseNameItemsWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.comboBoxExistingDatabaseName.Text = String.Empty;
            this.comboBoxExistingDatabaseName.Items.Clear();

            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                SetupLogger.LogError("Database population failed");
                SetupHelpers.ShowError(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                SetupLogger.LogInfo("Database population has been cancelled.");
            }
            else
            {
                // Handle the case where the operation succeeded.
                String[] databaseNames = e.Result as String[];
                if (databaseNames != null && databaseNames.Length > 0)
                {
                    foreach (String databaseName in databaseNames)
                    {
                        this.comboBoxExistingDatabaseName.Items.Add(databaseName);
                    }

                    // First satisfy the below scenario:
                    // - User clicked on existing db radio button and selected a db
                    // - then clicked new db radio button 
                    // - and then clicked existing radio db again w/o changin server, instance, or port info
                    // Basically, check if the selected instance already exists in the list
                    // if yes, then choose it
                    // otherwise, this is a new population, select the first item
                    if (this.comboBoxExistingDatabaseName.Items.Contains(selectedDatabase))
                    {
                        this.comboBoxExistingDatabaseName.Text = selectedDatabase;
                        this.radioExistingDatabase.IsChecked = true;
                    }
                    else
                    {
                        this.selectedDatabase = databaseNames[0];
                        this.comboBoxExistingDatabaseName.Text = this.selectedDatabase;
                        this.radioNewDatabase.IsChecked = true;
                    }
                }
            }

            this.EnableInputMode();
        }

        #endregion

        private void comboBoxExistingDatabaseName_Changed(object sender, SelectionChangedEventArgs e)
        {
            this.SetNextButtonState();
        }
    }
}
