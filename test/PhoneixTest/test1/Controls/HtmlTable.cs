// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlTable.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic properties for an Html tag structure like this: 
//
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using System.Collections.Generic;
    using System.Linq;
    using Phoenix.Test.UI.Framework.WebPages;
    using OpenQA.Selenium;
    using Phoenix.Test.UI.Framework.Logging;

    /// <summary>
    /// Defines the generic properties for an Html tag structure like this: .section > .section-title ~ .section-body > .section-item.
    /// </summary>
    public class HtmlTable : HtmlControl
    {
        private Page page;

        public Dictionary<string, string[]> RowValues = new Dictionary<string, string[]>();

        /// <summary>
        /// Construct HtmlSection found using By accessor.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="by">By accessor to find element.</param>
        public HtmlTable(Page page, By by)
            : base(page, by)
        {
            this.page = page;
            this.Headers = this.getHeaders().Select(e => e.Text).ToArray();
        }

        /// <summary>
        /// Construct HtmlSection by directly passing its element.
        /// </summary>
        /// <param name="page">Page where control is found.</param>
        /// <param name="element">IWebElement representing the control.</param>
        public HtmlTable(Page page, IWebElement element)
            : base(page, element)
        {
            this.page = page;
            this.Headers = this.getHeaders().Select(e => e.Text).ToArray();
        }

        /// <summary>
        /// Gets all section items as a Dictionary.
        /// </summary>
        public Dictionary<string, HtmlTableRow> Rows
        {
            get
            {
                var sectionItems = new Dictionary<string, HtmlTableRow>();
                var tableRows = this.Element.FindElements(By.XPath(".//tr[@role='row']")).Select(e => new HtmlTableRow(this.page, this, e));
                foreach (var row in tableRows)
                {
                    sectionItems.Add(row.Name, row);
                }
                return sectionItems;
            }

        }

        public string[] Headers { get; private set; }

        private IWebElement[] getHeaders()
        {
            return this.Element.FindElements(By.XPath(".//th[@role='columnheader']")).ToArray();
        }
    }
}