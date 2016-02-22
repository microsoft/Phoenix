//-----------------------------------------------------------------------
// <copyright file="EulaPage.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> vNext Manager Setup EULA Page
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
    using System.Windows.Forms;
    using System.Printing;
    using System.Windows.Xps;
    using CMP.Setup.SetupFramework;
    using Microsoft.Win32;

    #endregion Using Directives

    /// <summary>
    /// Interaction logic for EulaPage.xaml
    /// </summary>
    public partial class EulaPage : BasePageForWpfControls
    {
        AgreementType currentAgreementType;

        /// <summary>
        /// Initializes a new instance of the EulaPage class.
        /// </summary>
        public EulaPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the EulaPage class.
        /// </summary>
        /// <param name="page">Page</param>
        public EulaPage(CMP.Setup.SetupFramework.Page page)
            : base(page, WpfResources.WPFResourceDictionary.GettingStartedStepTitle, 1)
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// EnterPage
        /// </summary>
        public override void EnterPage()
        {
            base.EnterPage();
            this.checkBoxLicense.IsChecked = false;
            this.Page.Host.SetNextButtonState(true, checkBoxLicense.IsChecked.Value, null);

            bool showLicense = PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.VhdVersionConfiguration) || PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Server);
            this.currentAgreementType = AgreementType.Notice;
            this.licenseAgreementHeader.Text = WpfResources.WPFResourceDictionary.PlsReadNotice;
            this.acessTextLicense.Text = WpfResources.WPFResourceDictionary.AgreeWithNotice;
            this.LoadAgreementFile(SetupConstants.LicensePath);
        }

        /// <summary>
        /// ExitPage
        /// </summary>
        public override void ExitPage()
        {
            base.ExitPage();
        }

        /// <summary>
        /// CheckBoxLicenseChecked
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void CheckBoxLicenseChecked(object sender, RoutedEventArgs e)
        {
            this.Page.Host.SetNextButtonState(true, checkBoxLicense.IsChecked.Value, null);
        }

        /// <summary>
        /// CheckBoxLicenseUnchecked
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void CheckBoxLicenseUnchecked(object sender, RoutedEventArgs e)
        {
            this.Page.Host.SetNextButtonState(true, checkBoxLicense.IsChecked.Value, null);
        }


        /// <summary>
        /// Load RTF file into RichTextBox from a file specified by fileName
        /// </summary>
        /// <param name="fileName">The EULA file</param>
        private void LoadAgreementFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                this.richTextBoxEula.Multiline = true;
                this.richTextBoxEula.ScrollBars = RichTextBoxScrollBars.Vertical;
                this.richTextBoxEula.TabStop = true;
                this.richTextBoxEula.ReadOnly = true;
                this.richTextBoxEula.LoadFile(fileName, RichTextBoxStreamType.PlainText);
            }
        }

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FlowDocument flowDocumentCopy = new FlowDocument();
                TextRange copyDocumentRange = new TextRange(flowDocumentCopy.ContentStart, flowDocumentCopy.ContentEnd);
                String filePath = String.Empty;
                SetupFileValidation.FindAgreementFile(ref filePath, this.currentAgreementType);
                using (FileStream eulaStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    copyDocumentRange.Load(eulaStream, System.Windows.DataFormats.Rtf);
                    eulaStream.Close();
                }

                // Create a XpsDocumentWriter object, open a Windows common print dialog.
                // This methods returns a ref parameter that represents information about the dimensions of the printer media. 
                PrintDocumentImageableArea ia = null;
                XpsDocumentWriter docWriter = PrintQueue.CreateXpsDocumentWriter(ref ia);
                if (docWriter != null && ia != null)
                {
                    DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocumentCopy).DocumentPaginator;

                    // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
                    paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
                    Thickness pagePadding = flowDocumentCopy.PagePadding;
                    flowDocumentCopy.PagePadding = new Thickness(
                            Math.Max(ia.OriginWidth, pagePadding.Left),
                            Math.Max(ia.OriginHeight, pagePadding.Top),
                            Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), pagePadding.Right),
                            Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), pagePadding.Bottom));
                    flowDocumentCopy.ColumnWidth = double.PositiveInfinity;

                    // Send DocumentPaginator to the printer.
                    docWriter.Write(paginator);
                }
            }
            catch (Exception ex)
            {
                SetupHelpers.ShowError(ex.Message);
            }
        }

        private void wfHostControl1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }
    }
}
