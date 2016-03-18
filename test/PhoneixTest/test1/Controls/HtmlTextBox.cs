// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlTextBox.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic actions for an Html TextBox.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;
    using Phoenix.Test.UI.Framework.Logging;

    public class HtmlTextBox : HtmlControl
    {
        public HtmlTextBox(Page pageOwner, By by) : base(pageOwner, by)
        {
        }
        public HtmlTextBox(Page page, IWebElement element)
            : base(page, element)
        {
        }

        public void Input(string input)
        {
            //Log.Information("Input: " + input);
            SendKeys(input);
        }

        public string Text
        {
            get { return this.Text; }
        }
    }
}
