// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlTable.cs" company="Microsoft Corporation">
//   Microsoft Corporation. All rights reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
// <summary>
//  Defines the generic properties for an Html table structure like this: 
//
// --------------------------------------------------------------------------------------------------------------------

namespace Phoenix.Test.UI.Framework.Controls
{
    using OpenQA.Selenium;
    using Phoenix.Test.UI.Framework.WebPages;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the generic properties for an Html table.    
    /// </summary>
    public class HtmlTable : HtmlControl
    {
        private Page page;

        public Dictionary<string, string[]> RowValues = new Dictionary<string, string[]>();

        /// <summary>
        /// Construct HtmlTable found using By accessor.
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
        /// Construct HtmlTable by directly passing its element.
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