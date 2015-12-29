//-----------------------------------------------------------------------
// <copyright file="PageNavigation.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Class to control page navigation
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup.SetupFramework
{
    using System;
    using System.Threading;

    // Alias for a hash of Page delegates
    using CallBackRegistry = System.Collections.Generic.Dictionary<string, PageNavigation.PageCallback>;

    /// <summary>
    /// Singleton class to control page navigation
    /// </summary>
    public class PageNavigation
    {
        #region Private data

        private const int PageInitTimeOut = 100; // 100 milliseconds
        /// <summary>
        /// This is a special const used for the name of the NullPageHandler
        /// </summary>
        private const string NullPageHandler = "NullPageHandler";

        /// <summary>
        /// The unique instance of the singleton class
        /// </summary>
        private static PageNavigation instance;

        /// <summary>
        /// Backing store for CurrentPage property
        /// </summary>
        private Page currentPage;

        /// <summary>
        /// Backing store for Host property
        /// </summary>
        private IPageHost host;

        /// <summary>
        ///  Delegates to next pages, hashed by name
        /// </summary>
        private CallBackRegistry nextPageDelegates;

        /// <summary>
        ///  Delegates to previous pages, hashed by name
        /// </summary>
        private CallBackRegistry previousPageDelegates;

        #endregion // Private data

        /// <summary>
        /// Prevents a default instance of the <see cref="PageNavigation"/> class from being created.
        /// </summary>
        private PageNavigation()
        {
            this.nextPageDelegates = new CallBackRegistry();
            this.previousPageDelegates = new CallBackRegistry();
        }

        /// <summary>
        /// Delegate definition: 
        /// CONSIDER: Why current page is needed? 
        /// </summary>
        /// <param name="currentPage">the current page</param>
        /// <returns>The page to navigate to</returns>
        public delegate Page PageCallback(Page currentPage);

        /// <summary>
        /// Navigation type 
        /// </summary>
        private enum Navigation
        {
            /// <summary>
            /// Navigation to a Previous page
            /// </summary>
            Previous,

            /// <summary>
            /// Navigation to a Next page
            /// </summary>
            Next,

            /// <summary>
            /// Direct jump to a page 
            /// </summary>
            Jump,
        }

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static PageNavigation Instance
        {
            get
            {
                if (PageNavigation.instance == null)
                {
                    PageNavigation.instance = new PageNavigation();
                }

                return PageNavigation.instance;
            }
        }

        /// <summary>
        /// Gets or sets the host page interface.
        /// </summary>
        /// <value>The host.</value>
        public IPageHost Host
        {
            get { return this.host; }
            set { this.host = value; }
        }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <value>The current page.</value>
        public Page CurrentPage
        {
            get { return this.currentPage; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a next page.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has next page; otherwise, <c>false</c>.
        /// </value>
        public bool HasNextPage
        {
            get
            {
                return !this.currentPage.NextPageDelegateId.EndsWith(NullPageHandler, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a previous page.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has previous page; otherwise, <c>false</c>.
        /// </value>
        public bool HasPreviousPage
        {
            get
            {
                return !this.currentPage.NextPageDelegateId.EndsWith(NullPageHandler, StringComparison.OrdinalIgnoreCase);
            }
        }

        #endregion // Properties

        #region Public Methods

        /// <summary>
        /// Registers the page delegates.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <param name="nextPageDelegate">The next page delegate.</param>
        /// <param name="previousPageDelegate">The previous page delegate.</param>
        public void RegisterPageDelegates(
            string pageId, PageCallback nextPageDelegate, PageCallback previousPageDelegate)
        {
            if (this.currentPage == null)
            {
                // This is the first page we need to do some special stuff
                this.currentPage = PageRegistry.Instance.GetPage(pageId);

                // Initialize the page
                this.currentPage.PageUI.InitializePage();

                // Since this is the first page, we will enter it
                this.WaitEnterSet(this.currentPage);
            }

            // Add the delegate used to move to the next page
            this.nextPageDelegates.Add(pageId, nextPageDelegate);

            // Add the delegate used to move to the previous page
            this.previousPageDelegates.Add(pageId, previousPageDelegate);
        }

        /// <summary>
        /// Moves to next page, if current page validates.
        /// </summary>
        public void MoveToNextPage()
        {
            this.NavigateToPage(Navigation.Next, null, true);
        }

        /// <summary>
        /// Moves to previous page, validation not required.
        /// </summary>
        public void MoveToPreviousPage()
        {
            this.NavigateToPage(Navigation.Previous, null, false);
        }

        /// <summary>
        /// Moves to a given page, if the current page validates.
        /// </summary>
        /// <param name="pageName">Name of the page to move to.</param>
        public void MoveToPage(string pageName)
        {
            this.NavigateToPage(Navigation.Jump, pageName, true);
        }

        /// <summary>
        /// Moves to a given page with optional validation.
        /// </summary>
        /// <param name="pageName">Name of the page to move to.</param>
        /// <param name="shouldValidate">if set to <c>true</c>, move to the given page 
        /// only if the current page validates.</param>
        public void MoveToPage(string pageName, bool shouldValidate)
        {
            this.NavigateToPage(Navigation.Jump, pageName, shouldValidate);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Navigation helper 
        /// </summary>
        /// <param name="navigation">Navigation type</param>
        /// <param name="jumpPage">page name when applicable</param>
        private void NavigateToPage(Navigation navigation, string jumpPage, bool shouldValidate)
        {
            // Page must validate if it should
            if (shouldValidate &&
                (!this.currentPage.PageUI.ValidatePage()))
            {
                return;
            }

            // We are exiting the current page
            // Do the exit call before invoking Previous or Next Page delegates 
            this.currentPage.PageUI.ExitPage();
            Page output = null;
            switch (navigation)
            {
                case Navigation.Previous:
                    output = this.previousPageDelegates[this.currentPage.Id](this.currentPage);
                    break;
                case Navigation.Next:
                    output = this.nextPageDelegates[this.currentPage.Id](this.currentPage);
                    break;
                case Navigation.Jump:
                    output = PageRegistry.Instance.GetPage(jumpPage);
                    break;
                default:
                    // TODO: Debug Log something 
                    break;
            }

            if (output != null)
            {
                // Wait for the page to finish initialize, 
                // enter the page and sets the page as current
                this.WaitEnterSet(output);
            }
            else
            {
                // TODO: Debug Log something 
                this.host.Close();
            }
        }

        /// <summary>
        /// Wait for the page to finish initialize, Enter the page and sets the page as current
        /// </summary>
        /// <param name="page">target page </param>
        private void WaitEnterSet(Page page)
        {
            // Wait For End Init: Check to see if the page is initialized
            while (!page.PageUI.IsInitialized)
            {
                // Wait for the page to init.
                Thread.Sleep(PageInitTimeOut);
            }

            page.PageUI.EnterPage();

            // Set this page as the current page
            this.currentPage = page;
            this.host.SetPage(page.PageUI);
        }

        #endregion
    }
}
