// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementContainer.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//   Defines the abstract base class that Web Page objects must inherit form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.WebPages
{
    using OpenQA.Selenium;
    using Phoenix.Test.UI.Framework.Controls;
    public abstract class Page : ElementContainer
    {
        protected internal IWebDriver Browser;

        /// <summary>
        /// Gets the page element that verifies this web page is showing.
        /// </summary>
        abstract public HtmlControl VerifyPageElement { get; }

        /// <summary>
        /// Creates new instance of Page
        /// </summary>
        /// <param name="browser">browser</param>
        protected Page(IWebDriver browser) : base(browser)
        {
            Browser = browser;
            InitElements(this);
        }

        public int GetHeight()
        {
            return Browser.Manage().Window.Size.Height;
        }
    }
}
