// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlButton.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic actions for an Html Button.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;


    public class HtmlButton : HtmlControl
    {
        public HtmlButton(Page page, By by) : base(page, by)
        {
        }

        public HtmlButton(Page page, IWebElement element)
            : base(page, element)
        {
        }
    }
}
