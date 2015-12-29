//-----------------------------------------------------------------------
// <copyright file="ReadyToInstallPage.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> This is the page that is shown just before commiting the install.
//           List all the user input data for installation.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    #region Using directives 

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
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
    using System.Xml;

    using CMP.Setup.SetupFramework;
    using WpfResources;

    #endregion

    /// <summary>
    /// Interaction logic for EulaPage.xaml
    /// </summary>
    public partial class ReadyToInstallPage : BasePageForWpfControls
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadyToInstallPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public ReadyToInstallPage(CMP.Setup.SetupFramework.Page page)
            : base(page, WpfResources.WPFResourceDictionary.ConfigurationStepTitle, 3)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadyToInstallPage"/> class.
        /// </summary>
        public ReadyToInstallPage()
        {
            InitializeComponent();
        }

        #region Properties

        private Paragraph InstallationLocation
        {
            get
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.VhdVersionConfiguration))
                {
                    return null;
                }

                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server) ||
                    PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension))
                {
                    return this.GetSummaryItem(
                        WPFResourceDictionary.InstallationLocation,
                        SetupInputs.Instance.FindItem(SetupInputTags.BinaryInstallLocationTag));
                }

                return null;
            }
        }

        private Paragraph DatabaseInformation
        {
            get
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
                {
                    String databaseServerInfo = String.Empty;
                    String serverName = (String)SetupInputs.Instance.FindItem(SetupInputTags.SqlMachineNameTag);
                    String instanceName = (String)SetupInputs.Instance.FindItem(SetupInputTags.SqlInstanceNameTag);
                    String databaseName = (String)SetupInputs.Instance.FindItem(SetupInputTags.SqlDatabaseNameTag);

                    if (String.IsNullOrEmpty(instanceName))
                    {
                        databaseServerInfo = serverName;
                    }
                    else
                    {
                        databaseServerInfo = String.Format("{0}\\{1}", serverName, instanceName);
                    }

                    string databaseInfoFormatString = WPFResourceDictionary.DatabaseInformationValue;
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.SqlDatabaseVersion))
                    {
                        databaseInfoFormatString = WPFResourceDictionary.DatabaseUpdateInformationValue;
                    }

                    return this.GetSummaryItem(
                        WPFResourceDictionary.DatabaseInformation,
                        String.Format(databaseInfoFormatString, databaseName, databaseServerInfo));
                }

                return null;
            }
        }

        private Paragraph ServiceAccount
        {
            get
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
                {
                    String accountData = String.Empty;
                    if ((bool)SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceLocalAccountTag))
                    {
                        accountData = WPFResourceDictionary.LocalSystemAccount;
                    }
                    else
                    {
                        accountData = String.Format(
                                        "{0}\\{1}",
                                        (String)SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceDomainTag),
                                        (String)SetupInputs.Instance.FindItem(SetupInputTags.CmpServiceUserNameTag));
                    }
                    return this.GetSummaryItem(
                        WPFResourceDictionary.ServiceAccountInformation,
                        accountData);
                }

                return null;
            }
        }

        private Paragraph AddedFeatures
        {
            get
            {
                return this.GetSummaryItem(
                    WPFResourceDictionary.AddedFeatures,
                    this.GetFeatureList());
            }
        }

        private Paragraph RemovedFeatures
        {
            get
            {
                return this.GetSummaryItem(
                    WPFResourceDictionary.RemovedFeatures,
                    this.GetFeatureList());
            }
        }

        private Paragraph DatabaseOptions
        {
            get
            {
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
                {
                    if (SetupInputs.Instance.FindItem(SetupInputTags.RetainSqlDatabaseTag))
                    {
                        return this.GetSummaryItem(
                            WPFResourceDictionary.DatabaseOptions,
                            WPFResourceDictionary.RetainData);
                    }
                    else
                    {
                        return this.GetSummaryItem(
                            WPFResourceDictionary.DatabaseOptions,
                            WPFResourceDictionary.RemoveData);
                    }
                }

                return null;
            }
        }

        #endregion

        /// <summary>
        /// Enters the page.
        /// </summary>
        public override void EnterPage()
        {
            base.EnterPage();

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                this.Page.Host.SetNextButtonState(true, true, WpfResources.WPFResourceDictionary.UninstallButton);
            }
            else
            {
                this.Page.Host.SetNextButtonState(true, true, WpfResources.WPFResourceDictionary.InstallButton);
            }

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.VhdVersionConfiguration))
            {
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.Server, "1");
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.CMPServer, "1");
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.TenantExtension, "1");
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.AdminExtension, "1");
            }

            // Setup the data for the items to install
            PrepareInstallData.PrepareInstallDataItems();

            // Load the summary text box
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AddRemoveMode))
            {
                this.installationSummaryHeader.Text = WPFResourceDictionary.AddRemoveComponentTitleText;
                if (PropertyBagDictionary.Instance.GetProperty<int>(PropertyBagConstants.AddRemoveMode) == 
                    (int)CMP.Setup.AddRemovePage.AddRemoveProgramFilesModes.Add)
                {
                    this.installationSummaryDescription.Text = WPFResourceDictionary.InstallSummaryText;
                }
                else
                {
                    this.installationSummaryDescription.Text = WPFResourceDictionary.RemoveFeatureSummaryText;
                }
            }
            else
            {
                this.installationSummaryHeader.Text = WPFResourceDictionary.InstallTitleText;
                this.installationSummaryDescription.Text = WPFResourceDictionary.InstallSummaryText;
            }

            this.PopulateInstallationSummary();
        }

        /// <summary>
        /// Validates the page.
        /// The default implementation always returns true for 'validated'.
        /// </summary>
        /// <returns>true if valid</returns>
        public override bool ValidatePage()
        {
            if (PropertyBagDictionary.Instance.PropertyExists("GoingBackToPreviousPage"))
            {
                PropertyBagDictionary.Instance.Remove("GoingBackToPreviousPage");
                return true;
            }

            return true;
        }

        /// <summary>
        /// Exits the page.
        /// The default implementation writes the property bag.
        /// </summary>
        public override void ExitPage()
        {
            this.Page.Host.SetNextButtonState(true, true, WpfResources.WPFResourceDictionary.NextButtonText);
            base.ExitPage();
        }

        private void PopulateInstallationSummary()
        {
            // Create a FlowDocument to populate the rich text box
            FlowDocument flowDoc = new FlowDocument();

            // Check if we are on the uninstall path
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                if (this.RemovedFeatures != null)
                {
                    flowDoc.Blocks.Add(this.RemovedFeatures);
                }

                if (this.DatabaseOptions != null)
                {
                    flowDoc.Blocks.Add(this.DatabaseOptions);
                }
            }
            else
            {
                if (this.AddedFeatures != null)
                {
                    flowDoc.Blocks.Add(this.AddedFeatures);
                }

                if (this.InstallationLocation != null)
                {
                    flowDoc.Blocks.Add(this.InstallationLocation);
                }

                if (this.DatabaseInformation != null)
                {
                    flowDoc.Blocks.Add(this.DatabaseInformation);
                }

                if (this.ServiceAccount != null)
                {
                    flowDoc.Blocks.Add(this.ServiceAccount);
                }

            }

            this.richTextBoxInstallationSummary.Document = flowDoc;
        }

        private Paragraph GetSummaryItem(String label, params String[] textList)
        {
            Paragraph paragraph = new Paragraph();

            paragraph.Inlines.Add(new Run(label));
            foreach (String text in textList)
            {
                paragraph.Inlines.Add(new LineBreak());
                paragraph.Inlines.Add(new Bold(new Run(text)));
            }

            return paragraph;
        }

        private String[] GetFeatureList()
        {
            List<String> featureList = new List<string>();
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                featureList.Add(WPFResourceDictionary.ServerFeatureName);
            }

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon))
            {
                featureList.Add(WPFResourceDictionary.ExtensionCommonFeatureName);
            }

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension))
            {
                featureList.Add(WPFResourceDictionary.TenantExtensionFeatureName);
            }

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension))
            {
                featureList.Add(WPFResourceDictionary.AdminExtensionFeatureName);
            }

            return featureList.ToArray();
        }
    }
}
