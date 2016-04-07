// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlSection.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using OpenQA.Selenium;
    using Phoenix.Test.UI.Framework.Logging;
    using Phoenix.Test.UI.Framework.WebPages;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the generic properties for an Html tag structure like this: .section > .section-title ~ .section-body > .section-item.
    /// </summary>
    public class HtmlSection : HtmlControl
    {
        private Page page;

        /// <summary>
        /// Construct HtmlSection found using By accessor.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="by">By accessor to find element.</param>
        public HtmlSection(Page page, By by)
            : base(page, by)
        {
            this.page = page;
        }

        /// <summary>
        /// Construct HtmlSection by directly passing its element.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="element">IWebElement representing the control.</param>
        public HtmlSection(Page page, IWebElement element)
            : base(page, element)
        {
            this.page = page;
        }


        /// <summary>
        /// Gets all section items as a Dictionary.
        /// </summary>
        public Dictionary<string, HtmlSectionItem> Items
        {
            get
            {
                Dictionary<string, HtmlSectionItem> sectionItems = new Dictionary<string, HtmlSectionItem>();
             
                var sectionContainers = this.Element.FindElements(By.XPath(".//li"));

                foreach (var item in sectionContainers)
                {
                    if (item.Displayed)
                    {
                        HtmlSectionItem inlineSectionItem = new HtmlSectionItem(this.page, item);
                        string labelKey = inlineSectionItem.LabelText;
                        sectionItems.Add(labelKey, new HtmlSectionItem(this.page, item));
                    }
                }

                return sectionItems;
            }
        }

        public void SelectItem(string name)
        {
            Log.Information("Select item " + name);
            string a = Items[name].LabelText;
            Items[name].Select();
        }
    }
}