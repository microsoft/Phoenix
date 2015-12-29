//-----------------------------------------------------------------------
// <copyright file="FinishPage.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Setup Finish Page
// </summary>
// This UI is for temp usage. The real code will be checked in M3
// TODO: bug#47408
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Xml;

    using CMP.Setup.SetupFramework;
    using WpfResources;
    using CMP.Setup.Helpers;

    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class FinishPage : BasePageForWpfControls
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinishPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public FinishPage(CMP.Setup.SetupFramework.Page page)
            : base(page, WpfResources.WPFResourceDictionary.CompleteStepTitle, 5)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FinishPage"/> class.
        /// </summary>
        public FinishPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enters the page.
        /// </summary>
        public override void EnterPage()
        {
            // Ok... now we need to hook the action up to the progress bar
            base.EnterPage();

            // Need to disable the back and cancel button
            this.Page.Host.SetCancelButtonState(false, false, null);
            this.Page.Host.SetPreviousButtonState(false, false, null);

            // Set the text on the next button to indicate that we are done
            this.Page.Host.SetNextButtonState(true, true, WpfResources.WPFResourceDictionary.FinishScreenNextButtonText);
            this.LayoutPage();
        }

        /// <summary>
        /// Exits the page.
        /// The default implementation writes the property bag.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "We are calling our console")]
        public override void ExitPage()
        {
            base.ExitPage();
        }

        /// <summary>
        /// Layouts the page.
        /// </summary>
        private void LayoutPage()
        {
            // Ok... set the label text and button visibility
            this.richTextBoxMessage.Visibility = Visibility.Hidden;

            // Is the installation cancelled by the user?
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.UserCanceledInstall))
            {
                this.finishPageHeader.Text = WPFResourceDictionary.ProgressCanceled;
                //this.finishPageDescription.Visibility = Visibility.Collapsed;
            }
            // or has it failed?
            else if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagDictionary.VitalFailure))
            {
                this.finishPageHeader.Text = WPFResourceDictionary.SetupFailed;
                //this.finishPageDescription.Visibility = Visibility.Visible;
                this.PopulateException();
            }
            // or it succeeded
            else 
            {
                // Uninstall is successful
                if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.Uninstall))
                {
                    // Succeed with warning
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.WarningReason))
                    {
                        this.finishPageHeader.Text = WPFResourceDictionary.UninstallSucceededWithWarnings;
                        this.PopulateWarningMessage();
                    }
                    else
                    {
                        this.finishPageHeader.Text = WPFResourceDictionary.UninstallSuccessful;
                    }
                }
                // Installation is successful
                else
                {
                    // It may succeed with warning
                    if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.WarningReason))
                    {
                        this.finishPageHeader.Text = WPFResourceDictionary.SetupSucceededWithWarnings;
                        this.PopulateWarningMessage();
                    }
                    else
                    {
                        this.finishPageHeader.Text = WPFResourceDictionary.SetupSuccessful;
                        //this.finishPageDescription.Visibility = Visibility.Collapsed;
                    }
                }
            }

            // Set the datasource
            ((XmlDataProvider)this.Resources["finishInstallItemData"]).Document = PropertyBagDictionary.Instance.GetProperty<XmlDocument>("installItemData");
        }

        private void PopulateException()
        {
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.FailureReason))
            {
                Exception exception = PropertyBagDictionary.Instance.GetProperty<Exception>(PropertyBagConstants.FailureReason);
                
                if (exception != null)
                {
                    string exceptionMessage = exception.Message;

                    FlowDocument flowDoc = new FlowDocument();
                    Paragraph paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Run(exceptionMessage));
                    flowDoc.Blocks.Add(paragraph);
                    this.richTextBoxMessage.Document = flowDoc;

                    this.richTextBoxMessage.Visibility = Visibility.Visible;
                }
            }
        }

        private void PopulateWarningMessage()
        {
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.WarningReason))
            {
                String warningMessage = PropertyBagDictionary.Instance.GetProperty<String>(PropertyBagConstants.WarningReason);
                FlowDocument flowDoc = new FlowDocument();
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run(warningMessage));
                flowDoc.Blocks.Add(paragraph);
                this.richTextBoxMessage.Document = flowDoc;

                this.richTextBoxMessage.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Buttons the release notes click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ButtonReleaseNotesClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(SetupConstants.ReleaseNotesLink));
            e.Handled = true;
        }

        /// <summary>
        /// Handles the request navigate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleRequestNavigate(object sender, RoutedEventArgs e)
        {
            string navigateUri = ((Hyperlink)sender).NavigateUri.ToString();

            try
            {
                Process.Start(new ProcessStartInfo(navigateUri));
            }
            catch (InvalidOperationException exception)
            {
                SetupLogger.LogInfo("Unable to launch {0}.  Exception {1}", navigateUri, exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                SetupLogger.LogInfo("Unable to launch {0}.  Exception {1}", navigateUri, exception.Message);
            }
            catch (Win32Exception exception)
            {
                SetupLogger.LogInfo("Unable to launch {0}.  Exception {1}", navigateUri, exception.Message);
            }

            e.Handled = true;
        }
    }

    /// <summary>
    /// Converter class to read xaml
    /// </summary>
    public sealed class RunConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            string valueString = ((XmlAttribute)value).Value;
            TextBlock nodeTextBlock = (TextBlock)XamlReader.Load(XmlReader.Create(new StringReader(valueString)));
            foreach (Inline nodeInline in nodeTextBlock.Inlines)
            {
                if (nodeInline is Hyperlink)
                {
                    nodeInline.AddHandler(Hyperlink.RequestNavigateEvent, new RoutedEventHandler(this.HandleRequestNavigate));
                }
            }

            return nodeTextBlock;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
        /// <summary>
        /// Handles the request navigate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleRequestNavigate(object sender, RoutedEventArgs e)
        {
            string navigateUri = ((Hyperlink)sender).NavigateUri.ToString();

            try
            {
                navigateUri = SetupHelpers.GetLatestNavigationUri(navigateUri);

                Uri newNavigateUri = new Uri(navigateUri);
                string filePath = newNavigateUri.AbsolutePath;
                string fileExtension = Path.GetExtension(filePath);
                ProcessStartInfo startInfo = null;

                if (SetupHelpers.IsSupportedFileExtension(fileExtension))
                {
                    startInfo = new ProcessStartInfo(navigateUri);
                }
                else
                {
                    startInfo = new ProcessStartInfo();
                    startInfo.FileName = Environment.SystemDirectory + Path.DirectorySeparatorChar + "notepad.exe";
                    startInfo.Arguments = filePath;
                }

                Process.Start(startInfo);
            }
            catch (UriFormatException exception)
            {
                SetupLogger.LogInfo("Unable to launch {0}.  Exception {1}", navigateUri, exception.Message);
            }
            catch (InvalidOperationException exception)
            {
                SetupLogger.LogInfo("Unable to launch {0}.  Exception {1}", navigateUri, exception.Message);
            }
            catch (ArgumentException exception)
            {
                SetupLogger.LogInfo("Unable to launch {0}.  Exception {1}", navigateUri, exception.Message);
            }
            catch (Win32Exception exception)
            {
                SetupLogger.LogInfo("Unable to launch {0}.  Exception {1}", navigateUri, exception.Message);
            }            

            e.Handled = true;
        }
    }
}
