//-----------------------------------------------------------------------
// <copyright file="BasePageForWpfControls.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Base class for all wizard custom pages 
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Controls;
    using WpfResources;
    using CMP.Setup.SetupFramework;
    using CMP.Setup.Helpers;

    /// <summary>
    /// Base class for all wizard custom pages 
    /// A BasePageForWpfControls is a User Control, compliant to IPageUI 
    /// </summary>
    public partial class BasePageForWpfControls : UserControl, IPageUI
    {
        /// <summary>
        /// Backing store for the corresponding property 
        /// </summary>
        private bool isBasePageInitialized; 
        /// <summary>
        /// Backing store for the corresponding IPageUI property 
        /// </summary>
        private readonly CMP.Setup.SetupFramework.Page page;
        /// <summary>
        /// Backing store for the corresponding IPageUI property 
        /// </summary>
        private string pageTitle;
        /// <summary>
        /// Backing store for the corresponding IPageUI property 
        /// </summary>
        private readonly double progressPhase;
        /// <summary>
        /// Initializes a new instance of the <see cref="WpfBasePage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="title">The page title.</param>
        /// <param name="progressPhase">The progress phase.</param>
        public BasePageForWpfControls(CMP.Setup.SetupFramework.Page page, string title, double progressPhase)
        {
            this.page = page;
            this.pageTitle = title;
            this.progressPhase = progressPhase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePageForWpfControls"/> class.
        /// </summary>
        public BasePageForWpfControls()
        {
        } 
        /// <summary>
        /// Writes the property bag. 
        /// The default implementation does nothing. 
        /// </summary>
        /// <param name="propertyBag">The property bag.</param>
        public virtual void WritePropertyBag(PropertyBagDictionary propertyBag)
        {
        }
        /// <summary>
        /// Returns the underlying Page instance 
        /// </summary>
        public CMP.Setup.SetupFramework.Page Page
        {
            get { return page; }
        }

        #region IPageUI Members
        /// <summary>
        /// Validates the page.
        /// The default implementation always returns true for 'validated'. 
        /// </summary>
        /// <returns>true if valid</returns>
        public virtual bool ValidatePage()
        {
            return true;
        }
        /// <summary>
        /// Initializes the page.
        /// The default implementation always mark the page as initialized. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces", Justification = "Invalid FXCOP error. Interface not private"), STAThread]
        public virtual void InitializePage()
        {
            isBasePageInitialized = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsBasePageInitialized
        {
            get { return isBasePageInitialized; }
            protected set { isBasePageInitialized = value; }
        }

        /// <summary>
        /// Gets the page's title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return pageTitle; }
        }

        /// <summary>
        /// The progress phase the page belong to
        /// </summary>
        public double ProgressPhase
        {
            get { return progressPhase; }
        }

        /// <summary>
        /// Enters the page.
        /// </summary>
        public virtual void EnterPage()
        {
            SetupLogger.LogInfo(String.Format("Enter {0}", this.ToString()));
            //CMP.Setup.SetupLogger.LogInfo(String.Format("Enter {0}", this.ToString()));

            // Need to get to the host page so we can access it's public functions
            IPageHost host = this.page.Host;        
            // Set the previous page button to false if there is no previous button
            host.SetPreviousButtonState(true, this.page.PreviousPageArgument != null);
            // Set the next page button to "Finish" if there is no next page
            host.SetNextButtonState(true, true, 
                this.page.NextPageArgument == null ? 
                    WPFResourceDictionary.FinishButtonText : 
                    // Set to 'Next' when there are other pages to move to.
                    WPFResourceDictionary.NextButtonText ) ;
            // Reset the cancel button to enabled
            host.SetCancelButtonState(true, true);
        }

        /// <summary>
        /// Exits the page.
        /// The default implementation writes the property bag. 
        /// </summary>
        public virtual void ExitPage()
        {
            WritePropertyBag(PropertyBagDictionary.Instance);
            SetupLogger.LogInfo(String.Format("Exit {0}", this.ToString()));
        }
        #endregion

        #region Other virtuals
        /// <summary>
        /// Notifies a base page that the user has clicked 'Cancel' in the host page area 
        /// The default implementation always returns true. 
        /// Override to provide custom functionality when Cancel button is pressed.
        /// </summary>
        /// <returns>True if it is possible to proceed with cancel, false otherwise</returns>
        public virtual bool OnCancel()
        {
            return true; 
        }

        /// <summary>
        /// Notifies a base page that the user has clicked 'Close' or Alt-F4 in the main window
        /// The default implementation always returns true. 
        /// Override to provide custom functionality when the main window is closed.
        /// </summary>
        /// <returns>True if it is possible to proceed with closing, false otherwise</returns>
        public virtual bool CanClose()
        {
            return true; 
        }
        #endregion // Other protected virtuals
    }
}
