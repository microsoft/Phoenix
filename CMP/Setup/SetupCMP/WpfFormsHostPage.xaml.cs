//-----------------------------------------------------------------------
// <copyright file="WpfFormsHostPage.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> This is the base page that hosts all the other xaml pages.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using CMP.Setup.SetupFramework;
    using WpfResources;

    /// <summary>
    /// Interaction logic for the window hosting the wizard's pages 
    /// </summary>
    public partial class WpfFormsHostPage : Window, IPageHost
    {
        #region Public Members
        /// <summary>
        /// Initializes a new instance of the <see cref="WpfFormsHostPage"/> class.
        /// </summary>
        public WpfFormsHostPage()
        {
            InitializeComponent();
            this.setupPhaseProgressBar.Maximum = 5;
        }

        #region Button States Methods

        /// <summary>
        /// Sets the state of the previous button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        public void SetPreviousButtonState(bool visibleState, bool enabledState)
        {
            this.SetButtonState(this.buttonPrevious, visibleState, enabledState);
        }

        /// <summary>
        /// Sets the state of the previous button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        /// <param name="buttonText">The button text.</param>
        public void SetPreviousButtonState(bool visibleState, bool enabledState, string buttonText)
        {
            this.SetButtonState(this.buttonPrevious, visibleState, enabledState, buttonText);
        }

        /// <summary>
        /// Sets the state of the next button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        public void SetNextButtonState(bool visibleState, bool enabledState)
        {
            this.SetButtonState(this.buttonNext, visibleState, enabledState);
        }

        /// <summary>
        /// Sets the state of the next button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        /// <param name="buttonText">The button text.</param>
        public void SetNextButtonState(bool visibleState, bool enabledState, string buttonText)
        {
            this.SetButtonState(this.buttonNext, visibleState, enabledState, buttonText);
        }

        /// <summary>
        /// Sets the state of the cancel button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        public void SetCancelButtonState(bool visibleState, bool enabledState)
        {
            this.SetButtonState(this.buttonCancel, visibleState, enabledState);
        }

        /// <summary>
        /// Sets the state of the cancel button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        /// <param name="buttonText">The button text.</param>
        public void SetCancelButtonState(bool visibleState, bool enabledState, string buttonText)
        {
            this.SetButtonState(this.buttonCancel, visibleState, enabledState, buttonText);
        }

        #endregion Button States Methods

        #endregion Public Members

        #region IPageHost Members
        /// <summary>
        /// Sets the page.
        /// </summary>
        /// <param name="page">The page.</param>
        public void SetPage(IPageUI page)
        {
            Control control = page as Control;
            if (control != null)
            {
                this.border.Child = control;
                this.border.Background = null;
                control.Background = null;
                this.pageLabel.Text = page.Title;
                this.setupPhaseProgressBar.Value = page.ProgressPhase;
            }
        }

        /// <summary>
        /// Shows the help.
        /// </summary>
        public void ShowHelp()
        {
            Guid currentPageHelpGuid = PageNavigation.Instance.CurrentPage.HelpGuid;
            string helpString = string.Format(CultureInfo.InvariantCulture, WPFResourceDictionary.HelpFormatString, currentPageHelpGuid.ToString("D", CultureInfo.InvariantCulture));
            System.Windows.Forms.Help.ShowHelp(null, WPFResourceDictionary.HelpFile, helpString);
        }
        #endregion

        #region Protected Members

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.RebootRequired) ||
                PropertyBagDictionary.Instance.PropertyExists(PropertyBagConstants.UserRejectUpgrade))
            {
                // If a reboot is required,
                // or the user rejects upgrade, the user already had a confirmation message box
                // therefore we continue without further checks... 
            }
            else
            {
                // Make sure we are have a next page... if we don't
                // we don't need to warn about closing as that is what should happen
                if (PageNavigation.Instance.HasNextPage)
                {
                    // Cast because CanClose is not a member of IPageUI
                    BasePageForWpfControls currentPage =
                        PageNavigation.Instance.CurrentPage.PageUI as BasePageForWpfControls;
                    if (currentPage.CanClose())
                    {
                        // Make sure the user wants to cancel
                        // e.Cancel - if set to false, we will cancel, if set to true, the cancel will be aborted.            
                        e.Cancel = !VerifyUserClose();
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            } 

            base.OnClosing(e);
        }
        #endregion

        #region Statics
        /// <summary>
        /// Verifies the user close.
        /// </summary>
        /// <returns>true if the user selects the yes button, false otherwise.</returns>
        private static bool VerifyUserClose()
        {
            return
                MessageBoxResult.Yes == System.Windows.MessageBox.Show(
                    WPFResourceDictionary.CancelText,
                    WPFResourceDictionary.SetupMessageBoxTitle,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
        }

        #endregion  Statics

        #region Private Members

        private const String ReportAProblemLink = @"http://go.microsoft.com/fwlink/?LinkId=193489";

        #region Button State Private Helper Methods

        /// <summary>
        /// Sets the state of the given button.
        /// </summary>
        /// <param name="button">the button to set.</param>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        private void SetButtonState(Button button, bool visibleState, bool enabledState)
        {
            if (visibleState)
            {
                button.Visibility = Visibility.Visible;
            }
            else
            {
                button.Visibility = Visibility.Hidden;
            }

            button.IsEnabled = enabledState;
        }

        /// <summary>
        /// Sets the state of the given button.
        /// </summary>
        /// <param name="button">the button to set.</param>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        /// <param name="buttonText">The button text.</param>
        private void SetButtonState(Button button, bool visibleState, bool enabledState, string buttonText)
        {
            this.SetButtonState(button, visibleState, enabledState);
            if (!string.IsNullOrEmpty(buttonText))
            {
                button.Content = buttonText;
            }
        }

        #endregion // Button State Private Helper Methods

        /// <summary>
        /// Handles the Click event of the buttonPrevious control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ButtonPreviousClick(object sender, RoutedEventArgs e)
        {
            PageNavigation.Instance.MoveToPreviousPage();
        }

        /// <summary>
        /// Handles the Click event of the buttonNext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ButtonNextClick(object sender, RoutedEventArgs e)
        {
            PageNavigation.Instance.MoveToNextPage();
        }

        /// <summary>
        /// Handles the Click event of the buttonCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            // Cast because OnCancel is not a member of IPageUI
            BasePageForWpfControls currentPage = PageNavigation.Instance.CurrentPage.PageUI as BasePageForWpfControls;
            if (currentPage.OnCancel())
            {
                this.Close();
            }
        }

        /// <summary>
        /// Handle the click event on the ReportProblem button .
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ButtonReportProblemClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(ReportAProblemLink));
            e.Handled = true;
        }
        #endregion // Private
    }
}
