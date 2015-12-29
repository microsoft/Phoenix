//-----------------------------------------------------------------------
// <copyright file="PrerequisitesProgressPage.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Next Manager Setup PrerequisitesProgress Page
//           This is the page that handles prequisites checks progress.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    #region Using directives

    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using sw = System.Windows;
    using System.Windows.Forms;
    using System.Windows.Threading;
    using System.Xml;
    using Microsoft.Win32;

    using CMP.Setup;
    using CMP.Setup.SetupFramework;
    using WpfResources;
    using CMP.Setup.Helpers;

    #endregion

    /// <summary>
    /// Interaction logic for PrerequisitesProgressPage.xaml
    /// </summary>
    public partial class InstallationLocationPage : BasePageForWpfControls
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisitesProgressPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public InstallationLocationPage(Page page)
            : base(page, WPFResourceDictionary.GettingStartedStepTitle, 1)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisitesProgressPage"/> class.
        /// </summary>
        public InstallationLocationPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enters the page.
        /// </summary>
        public override void EnterPage()
        {
            base.EnterPage();

            this.textBoxInstallationLocation.Text = SetupInputs.Instance.FindItem(SetupInputTags.BinaryInstallLocationTag);
            this.ShowFreeSpace();

            this.Page.Host.SetNextButtonState(true, !String.IsNullOrEmpty(this.textBoxInstallationLocation.Text), null);
        }

        /// <summary>
        /// Exits the page.
        /// </summary>
        public override void ExitPage()
        {
            base.ExitPage();
        }

        public override bool ValidatePage()
        {
            try
            {
                String selectedLocation = this.textBoxInstallationLocation.Text;
                if (!selectedLocation.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    selectedLocation = selectedLocation + Path.DirectorySeparatorChar.ToString();
                }

                // If this is an upgrade, we cannot let the user choose the same location as 
                // the previous installation
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.VMMSupportedVersionInstalled))
                {
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.ServerVersion) &&
                        selectedLocation.Equals(SetupConstants.GetServerInstallPath(), StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception("An installation already exists at this location");
                    }
                }

                SetupInputs.Instance.EditItem(SetupInputTags.BinaryInstallLocationTag, selectedLocation);
                
                // Need to save the installation location in property bag for prerequisite check
                PropertyBagDictionary.Instance.SafeAdd(PropertyBagConstants.InstallationLocation, this.textBoxInstallationLocation.Text);
            }
            catch (Exception backEndErrorException)
            {
                SetupLogger.LogException(backEndErrorException);
                SetupHelpers.ShowError(backEndErrorException.Message);

                return false;
            }

            return true;
        }

        private void buttonBrowse_Click(object sender, sw.RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = this.textBoxInstallationLocation.Text;
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBoxInstallationLocation.Text = folderDialog.SelectedPath;
            }
        }

        private void textBoxInstallationLocation_LostFocus(Object sender, EventArgs e)
        {
            this.ShowFreeSpace();
        }

        private void ShowFreeSpace()
        {
            try
            {
                ulong OneMB = 1024 * 1024;
                UInt64 freeSpace = 0;
                String path = this.textBoxInstallationLocation.Text;

                SetupInputs.Instance.EditItem(SetupInputTags.BinaryInstallLocationTag, path);

                freeSpace = InstallLocationValidation.Instance.GetFreeDiskSpace(path);
                ulong spaceInByte = (ulong)freeSpace * OneMB;
                SizeFormat space = new SizeFormat((long)spaceInByte);
                String freeSpaceAsText = space.FormatSizeWithLabel((long)spaceInByte);

                this.textBlockFreeSpace.Text = String.Format(WpfResources.WPFResourceDictionary.FreeSpace, freeSpaceAsText);
            }
            catch (Exception backEndErrorException)
            {
                SetupLogger.LogException(backEndErrorException);
                SetupHelpers.ShowError(backEndErrorException.Message);
            }
        }

        private void textBoxInstallationLocation_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.Page.Host.SetNextButtonState(true, !String.IsNullOrEmpty(this.textBoxInstallationLocation.Text), null);
        }
    }
}
