//-----------------------------------------------------------------------
// <copyright file="ProgressPage.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Install Progress Page
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
   #region Using directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using System.Xml;
    using CMP.Setup.SetupFramework;
    using WpfResources;
    using CMP.Setup.Helpers;

   #endregion Using directives

    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class ProgressPage : BasePageForWpfControls, IDisposable
    {
        const String ParentNodeNameServer = "Server";
        const String ParentNodeNameTenantExtension = "TenantExtension";
        const String ParentNodeNameAdminExtension = "AdminExtension";
        const String ParentNodeNameExtensionCommon = "ExtensionCommon";

        /// <summary>
        /// Process installs background worker Thread
        /// </summary>
        private BackgroundWorker backgroundWorkerProcessInstalls;

        /// <summary>
        /// Initializes a new instance of the ProgressPage class.
        /// </summary>
        public ProgressPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the ProgressPage class.
        /// </summary>
        /// <param name="page">Page</param>
        public ProgressPage(CMP.Setup.SetupFramework.Page page)
            : base(page, WpfResources.WPFResourceDictionary.InstallingStepTitle, 4)
        {
            this.InitializeComponent();

            this.backgroundWorkerProcessInstalls = new BackgroundWorker();

            // Worker Thread Do Work Delegate
            this.backgroundWorkerProcessInstalls.DoWork += new DoWorkEventHandler(
                this.OnProcessInstallDoWork);

            // Worked Thread Completed Delegate
            this.backgroundWorkerProcessInstalls.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                this.OnProcessInstallRunWorkerCompleted);

            // Progress Change Delegate
            this.backgroundWorkerProcessInstalls.ProgressChanged += new ProgressChangedEventHandler(
                this.OnProcessInstallProgressChanged);
        }

        /// <summary>
        /// ProgressDelegateFunction
        /// </summary>
        private delegate void ProgressDelegateFunction();

        /// <summary>
        /// ExitPage
        /// </summary>
        public override void ExitPage()
        {
            PropertyBagDictionary.Instance.SafeAdd("installItemData", ((XmlDataProvider)this.Resources["installItemData"]).Document);
            
            base.ExitPage();
        }

        /// <summary>
        /// EnterPage
        /// </summary>
        public override void EnterPage()
        {
            // Ok... now we need to hook the action up to the progress bar
            base.EnterPage();
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {
                this.installationProgressHeader.Text = WpfResources.WPFResourceDictionary.UninstallationProgressText;

                PrepareInstallData.PrepareInstallDataItems();

                SetupFileValidation.ResetInstallItemFileLocations(
                    PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.LocationOfSetupFiles),
                    PropertyBagDictionary.Instance.GetProperty<string>(PropertyBagConstants.LocationOfSetupFiles));

                // Look for missing files
                if (SetupFileValidation.HaveAllNeededInstallItemFiles())
                {
                    SetupHelpers.LogPropertyBag();
                    // return true;
                }
            }
            else
            {
                this.installationProgressHeader.Text = WpfResources.WPFResourceDictionary.InstallationProgressText;
            }

            this.LayoutInstallItems();

            this.Page.Host.SetNextButtonState(true, false, null);

            this.backgroundWorkerProcessInstalls.RunWorkerAsync();
        }

        /// <summary>
        /// Notifies a base page that the user has clicked 'Close' or Alt-F4 in the main window
        /// The default implementation always returns true.
        /// Override to provide custom functionality when the main window is closed.
        /// </summary>
        /// <returns>
        /// True if it is possible to proceed with closing, false otherwise
        /// </returns>
        public override bool CanClose()
        {
            this.OnCancel();
            return false;
        }

        /// <summary>
        /// Called when the installation is being rolled back.
        /// </summary>
        public void OnRollback()
        {
            this.installationProgressHeader.Text = WPFResourceDictionary.RollbackProgressText;
        }

        /// <summary>
        /// Notifies a base page that the user has clicked 'Cancel' in the host page area
        /// The default implementation always returns true.
        /// Override to provide custom functionality when Cancel button is pressed.
        /// </summary>
        /// <returns>
        /// True if it is possible to proceed with cancel, false otherwise
        /// </returns>
        public override bool OnCancel()
        {
            // Are you sure?
            if (!PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
            {

                if (MessageBoxResult.Yes ==
                                    MessageBox.Show(
                                        WPFResourceDictionary.CancelDuringInstalProgressText,
                                        WPFResourceDictionary.SetupMessageBoxTitle,
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question))
                {
                    PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.UserCanceledInstall, "1");
                    this.installationProgressHeader.Text = WPFResourceDictionary.ProgressCancelling;
                }
            }
            else
            {
                // Bad user... You can not cancel this.
                MessageBox.Show(
                    WPFResourceDictionary.UninstallCanNotBeCanceled,
                    WPFResourceDictionary.SetupMessageBoxTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            return false;
        }
        /// <summary>
        /// Clean up any resources being used
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // Call a method which requests that ths system not call the
            // finalizer for this instance.
            GC.SuppressFinalize(this);

            return;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && (this.backgroundWorkerProcessInstalls != null))
            {
                this.backgroundWorkerProcessInstalls.Dispose();
            }

            return;
        }

        /// <summary>
        /// DeserializeXaml
        /// </summary>
        /// <param name="xaml">Xaml string</param>
        /// <returns>UIElement</returns>
        private static UIElement DeserializeXaml(string xaml)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(xaml);
                stream.Write(bytes, 0, bytes.Length);

                stream.Position = 0;

                return XamlReader.Load(stream) as UIElement;
            }
        }

        /// <summary>
        /// LayoutInstallItems
        /// </summary>
        private void LayoutInstallItems()
        {
            XmlDocument installItemDataXml = new XmlDocument();
            string xml = @"<?xml version='1.0' encoding='utf-8'?><Root></Root>";
            installItemDataXml.LoadXml(xml);

            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server))
            {
                this.InsertItemInfo(installItemDataXml, WPFResourceDictionary.ServerFeatureName, ParentNodeNameServer);
            }
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ExtensionCommon))
            {
                this.InsertItemInfo(installItemDataXml, WPFResourceDictionary.ExtensionCommonFeatureName, ParentNodeNameExtensionCommon);
            }
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.TenantExtension))
            {
                this.InsertItemInfo(installItemDataXml, WPFResourceDictionary.TenantExtensionFeatureName, ParentNodeNameTenantExtension);
            }
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.AdminExtension))
            {
                this.InsertItemInfo(installItemDataXml, WPFResourceDictionary.AdminExtensionFeatureName, ParentNodeNameAdminExtension);
            }

            PropertyBagDictionary.Instance.SafeAdd("installItemData", installItemDataXml);

            ((XmlDataProvider)this.Resources["installItemData"]).Document = installItemDataXml;
        }

        private void InsertItemInfo(XmlDocument installItemDataXml, String displayTitle, String parentName)
        {
            XmlNode rootNode = installItemDataXml.SelectSingleNode(SetupConstants.Root);
            XmlNode node = installItemDataXml.CreateNode(XmlNodeType.Element, SetupConstants.DisplayItem, null);
            XmlAttribute image = installItemDataXml.CreateAttribute(SetupConstants.Image);
            XmlAttribute displayText = installItemDataXml.CreateAttribute(SetupConstants.DisplayText);
            displayText.Value = displayTitle;
            XmlAttribute parent = installItemDataXml.CreateAttribute(SetupConstants.Parent);
            parent.Value = parentName;
            node.Attributes.Append(image);
            node.Attributes.Append(parent);
            node.Attributes.Append(displayText);
            rootNode.AppendChild(node);
        }

        #region Private Event Handlers
        /// <summary>
        /// OnProcessInstallDoWork
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">DoWorkEventArgs</param>
        private void OnProcessInstallDoWork(object sender, DoWorkEventArgs e)
        {
            InstallActionProcessor installs = new InstallActionProcessor();

            System.Diagnostics.Debug.WriteLine("ProcessInstalls Starting");

            installs.ProcessInstalls();

            System.Diagnostics.Debug.WriteLine("ProcessInstalls Done");
        }

        /// <summary>
        /// OnProcessInstallRunWorkerCompleted
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RunWorkerCompletedEventArgs</param>
        private void OnProcessInstallRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.Dispatcher.Thread == Thread.CurrentThread)
            {
                if (e.Error != null)
                {
                    SetupLogger.LogInfo("OnProcessInstallRunWorkerCompleted()", e.Error);
                }

                PageNavigation.Instance.MoveToNextPage();
            }
            else
            {
                this.Dispatcher.BeginInvoke(
                   DispatcherPriority.Normal,
                   new SendOrPostCallback(delegate { this.OnProcessInstallRunWorkerCompleted(sender,e); }),
                   null);
            }
        }

        /// <summary>
        /// OnProcessInstallRunWorkerCompleted
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RunWorkerCompletedEventArgs</param>
        private void OnProcessInstallProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        #endregion Private Event Handlers
    }
}
