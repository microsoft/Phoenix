// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlCheckBox.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic actions for an Html CheckBox.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using Phoenix.Test.UI.Framework.WebPages;
    using Phoenix.Test.UI.Framework.Logging;
    using OpenQA.Selenium;

    public class HtmlCheckBox : HtmlControl
    {
        public HtmlCheckBox(Page page, By by)
            : base(page, by)
        {
        }

        public HtmlCheckBox(Page page, IWebElement element)
            : base(page, element)
        {
        }

        /// <summary>
        /// Check the checkbox if it is unchecked, with option to verify the blocker after the click (e.g. for ajax call).
        /// </summary>
        /// <param name="verifyBlocker">Whether should verify the blocker</param>
        public void Check(bool verifyBlocker = false)
        {
            Log.Information("Check checkbox: " + this.Text);
            if (!this.Selected)
            {
                this.Click(verifyBlocker);
            }
        }

        /// <summary>
        /// Uncheck the checkbox if it is checked, with option to verify the blocker after the click
        /// </summary>
        /// <param name="verifyBlocker">Whether should verify the blocker</param>
        public void UnCheck(bool verifyBlocker = false)
        {
            Log.Information("Uncheck checkbox: " + this.Text);
            if (this.Selected)
            {
                this.Click(verifyBlocker);
            }
        }

        public void SetCheckBox(bool checkIt, bool invokeVerifyBlocker = false)
        {
            if (checkIt)
            {
                Check(invokeVerifyBlocker);
            }
            else
            {
                UnCheck(invokeVerifyBlocker);
            }
        }
    }
}
