// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlDiv.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic actions for an Html Div.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using OpenQA.Selenium;
    using Phoenix.Test.UI.Framework.WebPages;

    public class HtmlDiv : HtmlControl
    {
        public HtmlDiv(Page page, By by)
            : base(page, by)
        {
        }

        public HtmlDiv(Page page, IWebElement element) : base(page, element)
        {
        }
    }
}
