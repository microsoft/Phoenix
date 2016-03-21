
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Page.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//   Defines the abstract base class that Web Page objects must inherit form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.WebPages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using Phoenix.Test.UI.Framework.Controls;
    using System.Collections.ObjectModel;
    public abstract class Page : ElementContainer
    {
        protected internal IWebDriver Browser;

        /// <summary>
        /// Gets the URL location.
        /// <para>If the URL is a path inner the test website, then you could just specify the Path part.</para>
        /// <para>If the URL is out of the test website, then you should specify its full path.</para>
        /// </summary>
        //abstract public string UrlLocation { get; }

        /// <summary>
        /// Gets the page element that verifies this web page is showing.
        /// </summary>
        abstract public HtmlControl VerifyPageElement { get; }



        ///// <summary>
        ///// Initializes a new instance of the <see cref="Page"/> class.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <param name="webDriver"></param>
        //protected Page(TestContext context, IWebDriver webDriver)
        //    : this(webDriver)
        //{ }

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
