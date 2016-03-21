// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlSectionItem.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using Phoenix.Test.UI.Framework.Logging;
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;

    /// <summary>
    /// Defines the generic properties for an Html tag structure like this: .section-item > .section-item-label ~ .section-item-editor.
    /// </summary>
    public class HtmlScrollBarMenuItem
    {
        private Page page;


        /// <summary>
        /// A div.section-item html element which contains current section items.
        /// </summary>
        internal IWebElement itemContainer;

        /// <summary>
        /// Construct HtmlSectionItem by a container element.
        /// </summary>
        /// <param name="itemContainer">The each section items for the container elemnt to get.</param>
        public HtmlScrollBarMenuItem(Page page, IWebElement itemContainer)
        {
            this.page = page;
            this.itemContainer = itemContainer;
        }

        public void Select()
        {
            Log.Information("Select item: " + this.itemContainer.Text);
            new HtmlButton(page, this.itemContainer.FindElement(By.XPath(".//a"))).Click();
            //this.itemContainer.Click();
        }

    }

}
