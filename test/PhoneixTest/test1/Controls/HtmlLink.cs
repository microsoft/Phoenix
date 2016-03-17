// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlLink.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic actions for an Html Link.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;

    public class HtmlLink : HtmlControl
    {

        /// <summary>
        /// Construct HtmlLink found using By accessor.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="by">By accessor to find element.</param>
        public HtmlLink(Page page, By by)
            : base(page, by)
        {
        }

        /// <summary>
        /// Construct HtmlLink by directly passing its element.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="element">IWebElement representing the control.</param>
        public HtmlLink(Page page, IWebElement element)
            : base(page, element)
        {
        }
    }
}
