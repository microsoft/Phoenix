//-----------------------------------------------------------------------
// <copyright file="PageRegistry.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> This class holds all of the active pages.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup.SetupFramework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///  This class acts as a registry for pages
    /// </summary>
    public class PageRegistry
    {
        /// <summary>
        /// The single instance of PageRegistry 
        /// </summary>
        private static PageRegistry instance;

        /// <summary>
        /// Holds the list of pages we can navigate to 
        /// </summary>
        private Dictionary<string, Page> pages;

        #region Singleton Implementation

        /// <summary>
        /// Prevents a default instance of the PageRegistry class from being created
        /// </summary>
        private PageRegistry()
        {
            this.pages = new Dictionary<string, Page>();
        }

        /// <summary>
        /// Gets an instance of the class.  If there
        /// is no instance, it creates one and returns it.
        /// </summary>
        public static PageRegistry Instance
        {
            get
            {
                if (PageRegistry.instance == null)
                {
                    PageRegistry.instance = new PageRegistry();
                }

                return PageRegistry.instance;
            }
        }

        #endregion // Singleton Implementation

        /// <summary>
        /// Adds a page to the page registry
        /// </summary>
        /// <param name="pageToRegister">Page to add to the registry</param>
        public void RegisterPage(Page pageToRegister)
        {
            this.pages.Add(pageToRegister.Id, pageToRegister);

            PageNavigation.Instance.RegisterPageDelegates(
                pageToRegister.Id,
                DelegateRegistry.GetDelegate(pageToRegister.NextPageDelegateId),
                DelegateRegistry.GetDelegate(pageToRegister.PreviousPageDelegateId));
        }

        /// <summary>
        /// Gets a page by it name
        /// </summary>
        /// <param name="pageId">String that identifies the page to return</param>
        /// <returns>Page matching the given string</returns>
        public Page GetPage(string pageId)
        {
            return this.pages[pageId];
        }
    }
}